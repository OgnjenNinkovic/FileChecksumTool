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


        ManualResetEvent mre = new ManualResetEvent(false);

        public string[] files { get; set; }
        public static List<FileModel> fileModel { get; set; }

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private ProgressReport progressReport = null;

        ParallelOptions parallelOptions = new ParallelOptions();

        public static List<ListViewItem> listViewItems = new List<ListViewItem>();

        private string DirPath { get; set; }

        public ProgressForm(string dirPath)
        {
            InitializeComponent();

            this.DirPath = dirPath;

        }



        private static volatile object updateLock = new object();
        static volatile int index = 0;

        private Task<ProgressReport> UpdateProgress(int totalProcess, string fileName)
        {

            lock (updateLock)
            {
                Task<ProgressReport> progress = Task.Run(() =>
                {
                    var progressReport = new ProgressReport();
                    index++;
                    progressReport.percentComplete = index * 100 / totalProcess;
                    progressReport.precessingFile = fileName;
                    return progressReport;
                });
                return progress;

            }



        }

        private void btnStart_Click(object sender, EventArgs e)
        {

        


            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync(DirPath);
            }

            Button btn = (Button)sender;
            btn.Enabled = false;




        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
         
            if (backgroundWorker.IsBusy)
            {
                cancellationTokenSource.Cancel();
                backgroundWorker.CancelAsync();

            }
            mre.WaitOne();
            this.Close();
        }

        private  void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] data = Directory.GetFiles(e.Argument.ToString(), "*", SearchOption.AllDirectories);

            fileModel = new List<FileModel>(data.Length);


            parallelOptions.CancellationToken = cancellationTokenSource.Token;
            parallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;


            try
            {

                Parallel.ForEach(data, parallelOptions, item =>
                  {
                     
                      parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                      fileModel.Add(new FileModel(Path.GetFileName(item), Path.GetExtension(item).ToLower(), item));
                       ListViewItem viewItem = new ListViewItem(Path.GetFileName(item));
                       viewItem.SubItems.Add(GetHash(item, new MD5CryptoServiceProvider()));
                       listViewItems.Add(viewItem);
                       viewItem.Tag = item;
                       progressReport =  UpdateProgress(data.Length, item).Result;
                       backgroundWorker.ReportProgress(progressReport.percentComplete);
                  });





            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            mre.Set();
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            procesingFile.Text = string.Format("Procesing file: {0}", progressReport.precessingFile);
            procesingFile.Update();
            progressBar.Value = e.ProgressPercentage;
            progressBar.Update();
            lblStatus.Text = string.Format($"Percent compleated{e.ProgressPercentage}%");
            lblStatus.Update();



        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mre.WaitOne();
            this.Close();
            MessageBox.Show($"Scan process has been completed. File count:  {fileModel.Count}", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            index = 0;

        }



        public static string GetHash(string fileLocation, HashAlgorithm hashAlgo)
        {
            string hashResult = string.Empty;
            using (var stream = File.OpenRead(fileLocation))
            {

                try
                {
                    var hash = hashAlgo.ComputeHash(stream);
                    hashResult = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                    hashResult = string.Empty;
                }



            }

            return hashResult;

        }
    }
}
