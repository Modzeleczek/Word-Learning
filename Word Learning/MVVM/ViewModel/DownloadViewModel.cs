using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Word_Learning.Core;
using Word_Learning.MVVM.View;
using static Word_Learning.MVVM.Model.Downloader;

namespace Word_Learning.MVVM.ViewModel
{
    public class DownloadViewModel : ObservableObject
    {
        private BackgroundWorker worker;
        private double progress;
        public double Progress
        {
            get { return progress; }
            set { progress = value; OnPropertyChanged(nameof(Progress)); }
        }
        public Model.Status Status { get; private set; }
        public RelayCommand Close { get; }
        public event EventHandler OnRequestClose;

        public DownloadViewModel()
        {
            Close = new RelayCommand(e => worker.CancelAsync());
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private class BackgroundWorkerError : Exception
        {
            public BackgroundWorkerError(string message) : base(message) { }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            LinkedList<string> words;
            var randomEx = new BackgroundWorkerError("Random words could not be downloaded.");
            try { words = GetRandomWords(50); }
            catch (AggregateException ex) { throw randomEx; }
            if (words == null)
            { // błąd podczas pobierania losowych słów
                if (!worker.CancellationPending) throw randomEx;
                else { e.Cancel = true; return; }
            }
            LinkedList<Word> detailedWords = new LinkedList<Word>();
            int progress = 0;
            foreach (string word in words)
            {
                if (worker.CancellationPending) { e.Cancel = true; return; }
                try
                {
                    Word detailedWord = null;
                    try { detailedWord = GetFirstHomonym(word); }
                    catch (AggregateException) { } // jeżeli nie udało się pobrać słowa, to je pomijamy
                    if (detailedWord != null) // jeżeli słowo nie spełnia warunków aplikacji, to je pomijamy
                        detailedWords.AddLast(detailedWord);
                }
                catch (JsonSerializationException) { } // jeżeli nie udało się pobrać słowa, to je pomijamy
                worker.ReportProgress(((++progress) * 100) / words.Count);
            }
            // if (detailedWords.Count < DOWNLOADED_WORDS / 4)
            if (detailedWords.Count == 0)
            {
                if (!worker.CancellationPending)
                    throw new BackgroundWorkerError("Word details could not be downloaded.");
                else { e.Cancel = true; return; }
            }
            if (worker.CancellationPending) { e.Cancel = true; return; }
            AddToUser(detailedWords);
            e.Result = new Model.Status(0, $"{detailedWords.Count} new words downloaded.");
        }

        // wywoływane co wywołanie ReportProgress
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        // wywoływane po returnie z DoWork, nawet jeżeli użytkownik anulował
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null) Status = new Model.Status(1, e.Error.Message); // wystąpił błąd
            else if (e.Cancelled) Status = new Model.Status(0, ""); // użytkownik anulował
            else Status = (Model.Status)e.Result; // zakończono powodzeniem
            OnRequestClose(this, new EventArgs());
        }

        private void AddToUser(LinkedList<Word> detailedWords)
        {
            var user = Model.User.Instance;
            var userWordSet = new HashSet<string>();
            foreach (var userWord in user.Words)
            {
                var content = userWord.Content;
                if (userWordSet.Contains(content))
                    throw new Exception($"User has duplicated word: {content}.");
                userWordSet.Add(content);
            }
            var userSynonymDict = new Dictionary<string, int>(); // (synonim, indeks)
            for (int i = 0; i < user.Synonyms.Count; ++i)
            {
                var content = user.Synonyms[i];
                if (userSynonymDict.ContainsKey(content))
                    throw new Exception($"User has duplicated synonym: {content}.");
                userSynonymDict.Add(content, i);
            }
            foreach (Word dw in detailedWords)
            {
                string word = dw.word; // zapisujemy treść słowa
                Meaning m = dw.meanings[0]; // bierzemy tylko pierwsze znaczenie
                string partOfSpeech = m.partOfSpeech; // zapisujemy nazwę części mowy
                Definition d = m.definitions[0]; // bierzemy tylko pierwszą definicję
                string definition = d.definition; // zapisujemy definicję
                string example = d.example; // zapisujemy przykład wybranej definicji lub null, jeżeli go nie ma
                List<string> synonyms = d.synonyms;
                if (userWordSet.Contains(word)) continue;
                var synonymIds = new LinkedList<int>();
                foreach (var s in synonyms)
                {
                    if (userSynonymDict.ContainsKey(s))
                        synonymIds.AddLast(userSynonymDict[s]);
                    else
                    {
                        user.Synonyms.Add(s);
                        var index = user.Synonyms.Count - 1;
                        synonymIds.AddLast(index);
                        userSynonymDict.Add(s, index);
                    }
                }
                var newWord = new Model.Word(word, partOfSpeech, definition, example, synonymIds, 0, 0);
                user.Words.Add(newWord);
                userWordSet.Add(word);
            }
        }
    }
}
