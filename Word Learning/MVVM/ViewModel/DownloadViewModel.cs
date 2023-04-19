using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Word_Learning.Core;
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
            try { words = GetRandomWords(30); }
            catch (AggregateException) { throw randomEx; }
            if (words == null)
            { // Error occurred while downloading random words.
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
                    // If the word could not be downloaded, skip it.
                    catch (AggregateException) { }
                    /* If the word does not satisfy application's conditions,
                    skip it. */
                    if (detailedWord != null)
                        detailedWords.AddLast(detailedWord);
                }
                // If the word could not be downloaded, skip it.
                catch (JsonSerializationException) { }
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

        // Called every ReportProgress call
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        // Called after returning from DoWork, even if the user cancelled.
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Error occurred.
            if (e.Error != null) Status = new Model.Status(1, e.Error.Message);
            // The user cancelled.
            else if (e.Cancelled) Status = new Model.Status(0, "");
            // Successfully finished.
            else Status = (Model.Status)e.Result;
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
            // (synonym, index)
            var userSynonymDict = new Dictionary<string, int>();
            for (int i = 0; i < user.Synonyms.Count; ++i)
            {
                var content = user.Synonyms[i];
                if (userSynonymDict.ContainsKey(content))
                    throw new Exception($"User has duplicated synonym: {content}.");
                userSynonymDict.Add(content, i);
            }
            foreach (Word dw in detailedWords)
            {
                string word = dw.word; // Save the word's content.
                Meaning m = dw.meanings[0]; // Take only the first meaning.
                // Save part of speech name.
                string partOfSpeech = m.partOfSpeech;
                // Take only the first definition.
                Definition d = m.definitions[0];
                // Save the definition.
                string definition = d.definition;
                /* Save an example of the selected definition or null if no
                example exists. */
                string example = d.example;
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
