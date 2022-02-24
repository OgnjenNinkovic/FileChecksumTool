using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileChecksum
{
    public partial class ProgressForm : System.Windows.Forms.Form
    {

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public List<string> files { get; set; }
        public static List<FileModel> fileModel { get; set; }

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private ProgressReport progressReport = null;

        ParallelOptions parallelOptions = new ParallelOptions();

        public ProgressForm(List<string> list)
        {
            InitializeComponent();

            this.files = list;

        }



        private static volatile object updateLock = new object();
        static volatile int index = 0;

        private ProgressReport UpdateProgress(int totalProcess,string fileName)
        {

            lock (updateLock)
            {

                var progressReport = new ProgressReport();
                index++;
                progressReport.percentComplete = index * 100 / totalProcess;
                progressReport.precessingFile = fileName;
                return progressReport;

            }
           


        }

        private void btnStart_Click(object sender, EventArgs e)
        {

          


            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync(files);
            }

            //Button btn = (Button)sender;
            //btn.Enabled = false;




            //lblStatus.Text = "Working...";
            //var progress = new Progress<ProgressReport>();
            //progress.ProgressChanged += (o, processed) =>
            //{
            //    procesingFile.Text = string.Format("Procesing file: {0}", processed.precessingFile);
            //    procesingFile.Update();

            //    lblStatus.Text = string.Format("Percent completed: {0}%", processed.percentComplete);
            //    lblStatus.Update();

            //    progressBar.Value = processed.percentComplete;
            //    progressBar.Update();




            //};
            //await ProcessData(files, progress);
            //this.Close();
            //Debug.WriteLine("Done !");

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
            }
            //cancellationTokenSource.Cancel();
            this.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> data = ((List<string>)e.Argument).ToList();
            int t = data.Count();

               fileModel = new List<FileModel>(data.Count);


                parallelOptions.CancellationToken = cancellationTokenSource.Token;
                parallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
            
                    Parallel.ForEach(data, parallelOptions, item =>
                    {
                        parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                        fileModel.Add(new FileModel(Path.GetFileName(item), Path.GetExtension(item).ToLower(), item));
                        progressReport = UpdateProgress(data.Count, item);
                        backgroundWorker.ReportProgress(progressReport.percentComplete);


                    });
            
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            procesingFile.Text = string.Format("Procesing file: {0}", progressReport.precessingFile);
            procesingFile.Update();

            progressBar.Value = e.ProgressPercentage;
            progressBar.Update();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int t = files.Count;
            MessageBox.Show($"Proccess has been compleated. {t}", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
