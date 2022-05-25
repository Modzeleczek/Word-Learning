using System.ComponentModel;
using System.Threading;
using Word_Learning.Core;

namespace Word_Learning.MVVM.ViewModel
{
    class DownloadViewModel : ObservableObject
    {
        private double progress;
        public double Progress {
            get { return progress; }
            set
            {
                progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        public DownloadViewModel()
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 100; ++i)
            {
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(250);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}
