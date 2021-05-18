using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileChecksum
{
    public partial class ProgressForm : System.Windows.Forms.Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        public List<string> files { get; set; }
        private List<FileModel> _fileModel { get; set; }

        private Task ProcessData(List<string> list,List<FileModel> fileModel, IProgress<ProgressReport> progress)
        {
            fileModel.Clear();
            ParallelOptions parallelOptions = new ParallelOptions();

            string[] _files = files.ToArray();



            int index = 1;
            int totalProcess = files.Count;
            FileModel[] fileModels = new FileModel[list.Count];
            var progressReport = new ProgressReport();
            parallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;

            return Task.Run(()=> {
                for (int i = 0; i < totalProcess; i++)
                {
                    progressReport.percentComplete = index++ * 100 / totalProcess;
                    progressReport.procesedData = files[i];
                    progress.Report(progressReport);
                    Parallel.ForEach(files, parallelOptions, item =>
                    {
                        fileModels[i] = new FileModel(Path.GetFileName(item), Path.GetExtension(item).ToLower(), item);

                    });
                }


            });

        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            

            Button btn = (Button)sender;
            btn.Enabled = false;



            lblStatus.Text = "Working...";
            var progress = new Progress<ProgressReport>();
            progress.ProgressChanged += (o, report) =>
            {
                procesingFile.Text = string.Format("Procesing file: {0}", report.procesedData);
                lblStatus.Text = string.Format("Percent completed: {0}%", report.percentComplete);
                progressBar.Value = report.percentComplete;
                progressBar.Update();

                if (report.percentComplete ==100)
                {
                    this.Close();
                }

            };
            await ProcessData(files,_fileModel, progress);
           
        }

        public ProgressForm(List<string> list, List<FileModel> fileModels)
        {
            this.files = files;
            this._fileModel = fileModels;
        }
    }
}
