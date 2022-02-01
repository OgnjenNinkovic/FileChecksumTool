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
        public static List<FileModel> _fileModel { get; set; }

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

       

        ParallelOptions parallelOptions = new ParallelOptions();

        public ProgressForm(List<string> list)
        {
            InitializeComponent();

            this.files = list;

        }



        private async Task ProcessData(List<string> list, IProgress<ProgressReport> progress)
        {

           

            string[] _files = files.ToArray();




            int totalProcess = files.Count;
            List<FileModel> fileModels = new List<FileModel>(list.Count);
        

            await Task.Run(() =>
            {
               
                parallelOptions.CancellationToken = cancellationTokenSource.Token;
                parallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;

                try
                {
                    Parallel.ForEach(files, parallelOptions, item =>
                   {
                       parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                       fileModels.Add(new FileModel(Path.GetFileName(item), Path.GetExtension(item).ToLower(), item));
                       UpdateProgress(progress, list.Count, item);

                   });

                }
                catch (OperationCanceledException e)
                {

                    Debug.WriteLine("Canncelled OGI " + e.Message);
                }
                finally
                {
                    cancellationTokenSource.Dispose();
                }  

                _fileModel = fileModels.ToList();


            });

        }
        private static volatile object updateLock = new object();
        static volatile int index = 0;

        private void  UpdateProgress(IProgress<ProgressReport> progress, int totalProcess,string fileName)
        {

            lock (updateLock)
            {

                var progressReport = new ProgressReport();
                index++;
                progressReport.percentComplete = index * 100 / totalProcess;
                progressReport.precessingFile = fileName;
                progress.Report(progressReport);

            }
           


        }

        private async void btnStart_Click(object sender, EventArgs e)
        {

            Button btn = (Button)sender;
            btn.Enabled = false;




            lblStatus.Text = "Working...";
            var progress = new Progress<ProgressReport>();
            progress.ProgressChanged += (o, processed) =>
            {
                procesingFile.Text = string.Format("Procesing file: {0}", processed.precessingFile);
                procesingFile.Update();

                lblStatus.Text = string.Format("Percent completed: {0}%", processed.percentComplete);
                lblStatus.Update();

                progressBar.Value = processed.percentComplete;
                progressBar.Update();




            };
            await ProcessData(files, progress);
            this.Close();
            Debug.WriteLine("Done !");

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            cancellationTokenSource.Cancel();
            this.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}
