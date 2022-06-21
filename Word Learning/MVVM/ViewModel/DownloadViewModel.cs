using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using Word_Learning.Core;
using Word_Learning.MVVM.View;
using static Word_Learning.MVVM.Model.Downloader;

namespace Word_Learning.MVVM.ViewModel
{
    public class DownloadViewModel : ObservableObject
    {
        private double progress;
        public double Progress {
            get { return progress; }
            set { progress = value; OnPropertyChanged(nameof(Progress)); }
        }

        public RelayCommand Close { get; }
        public event EventHandler OnRequestClose;
        public Model.Status Status { get; private set; }

        private BackgroundWorker worker = new BackgroundWorker();

        public DownloadViewModel()
        {
            Close = new RelayCommand(e =>
            {
                worker.CancelAsync();
                // resetEvent.WaitOne();
                // OnRequestClose(thisCopy, new EventArgs());
            });
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync(100);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            /*var worker = (BackgroundWorker)sender;
            LinkedList<string> words = GetRandomWords(20);
            if (words == null)
            { // błąd podczas pobierania losowych słów
                if (!worker.CancellationPending) throw new Exception("Random words could not be downloaded.");
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
            LinkedList<Word> detailedWords = new LinkedList<Word>();
            int progress = 0;
            foreach (string word in words)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                try
                {
                    Word detailedWord = GetFirstHomonym(word);
                    if (detailedWord != null) // jeżeli słowo nie spełnia warunków aplikacji, to je pomijamy
                        detailedWords.AddLast(detailedWord);
                }
                catch (JsonSerializationException) { } // jeżeli nie udało się pobrać słowa, to je pomijamy
                worker.ReportProgress(((++progress) * 100) / words.Count);
            }
            // if (detailedWords.Count < DOWNLOADED_WORDS / 4)
            if (detailedWords.Count == 0)
            {
                if (!worker.CancellationPending) throw new Exception("Word details could not be downloaded.");
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            AddToUser(detailedWords);
            e.Result = new Model.Status(0, "New words downloaded.");*/
            /*var rng = new Random();
            var worker = (BackgroundWorker)sender;
            for (int i = 0; i < 100; ++i)
            {
                if (worker.CancellationPending) return;
                var rn = rng.Next(0, 100);
                if (rn == 32) throw new Exception("Rng gave 32.");
                worker.ReportProgress(i);
                Thread.Sleep(250);
            }*/
            var bgw = sender as BackgroundWorker;
            e.Result = DoSlowProcess((int)e.Argument, bgw, e);
        }

        private int DoSlowProcess(int iterations, BackgroundWorker worker, DoWorkEventArgs e)
        {
            int result = 0;
            for (int i = 0; i <= iterations; i++)
            {
                if (worker != null)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    if (worker.WorkerReportsProgress)
                    {
                        int percentComplete =
                        (int)((float)i / (float)iterations * 100);
                        worker.ReportProgress(percentComplete);
                    }
                }
                Thread.Sleep(100);
                result = i;
            }
            return result;
        }

        // wywoływane co wywołanie ReportProgress
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        // wywoływane po returnie z DoWork, nawet jeżeli użytkownik anulował
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            /*if (e.Error != null)
                Status = new Model.Status(1, e.Error.Message);
            else if (e.Cancelled) Status = new Model.Status(0, "");
            else // prawidłowe zakończenie
                Status = (Model.Status)e.Result;
            // Status = (Model.Status)e.Result;
            OnRequestClose(this, new EventArgs());
            // resetEvent.Set();*/
            if (e.Error != null)
                MessageBox.Show(e.Error.Message);
            else if (e.Cancelled)
                MessageBox.Show("Cancelled");
            else
            {
                MessageBox.Show(e.Result.ToString());
                Progress = 0;
            }
        }

        private void AddToUser(LinkedList<Word> detailedWords)
        {
            var user = Model.User.Instance;
            var userWordSet = new HashSet<string>();
            foreach (var userWord in user.Words)
            {
                var content = userWord.Content;
                if (userWordSet.Contains(content))
                    throw new System.Exception($"User has duplicated word: {content}.");
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
