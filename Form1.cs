using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileChecksum
{
    public partial class Form1 : Form
    {

        // First implement basic functionality to work without any issues then continue forwoard
        // There is another problem with race conditions in this implementations


        /*
        --When OFD opnes select directory

        -- When the directory is selected list of all files loactions is avalable

        -- Process all files and show them in the list with progress bar

        -- For now you can disable file extensions untill you make basic functionality to work
      */

        //==================================================================================================================================================================================================

        /*
         * I managed to create progress bar updatig nicly in parallel foreeach
         * Files are shown corectly in the listView
         * 
         * There is a problem with progres control labels 
         * 
         * Right now it seems that I'm on the right way
         * 
         * I found solution for parralel foreach here
         * https://stackoverflow.com/questions/61608441/updating-progressbar-in-parallel-loop
          */


        /* Program doesent work as expected
         * Progress bar doesent upadte fluently
         * Preceisng file label doesen't update at all
         * While procesing the CPU usage goes at 100%
         * 
         * The UpdateProgress method need to bee modified 
         * Cancelation token needs to be added when progress reaches 100% or cancel button is pressed
         */

        //====================================================================================================================

        /*
         * Big improvment
         * 
         * Progres bar now updates fluently
         * All labels are being updated as expected
         * 
         * Next step is to add cancelation token
         * whacth tutorial movie in the program directory 
         * You will find the soulution there 
         * 
         * 
         * Good work!!
         */

        //===================================================================================================================================

        /*
         * I managed to add basic cancelation token
         * Need to do much more improvments and testing for this logic
         * 
         * There is a problem while procesing files, click event is not trigerd
         * Cancel button doesen't work while proccesing files
         */

        // ========================================================================================================================
        /*
         * The main problem now is when progers bar is active UI is blocked 
         * You will have to add something like background worker to make UI responsive while processing
         *  https://stackoverflow.com/questions/5483565/how-to-use-wpf-background-worker check out this link
         *  
         *  
         *  Also the video "C# Tutorial - BackgroundWorker _ FoxLearn" can help 
         *  You can find the video in the program directory
         *  
         *  Just continue to shape this program, you can learn more from this 
         *  
         *  You resloved much different issues you will reslove this as well, you are doing well
         *  
         *  Just continue slowly and theraly
         */

        // ====================================================================================================================

        /*
         * The solution for blocked UI in progress form is to implement the "BackgroundWorker" thread
         * Compleate exsample can be found in video "C# Tutorial - BackgroundWorker _ FoxLearn" in the program directory
         * 
         * 
         * Implement this BackgroundWorker in progeress form

         */


        //==============================================================================================================================

        /*
         *  I have started to implemet background worker as in the video C# Tutorial - BackgroundWorker _ FoxLearn
         *  
         *  I probobly need more time to finsh implementation because it didn't worked as expected
         *  
         *  in the video it is windows forms implementation and I started with my own solution
         *  
         *  
         *  
         *  the best way is to make it work 
         *  
         *  So add this code to the git because the progres bar and label updateds are working great
         *  
         *  
         *  Implement everithung as described in video C# Tutorial - BackgroundWorker _ FoxLearn
         *  
         *  
         *  Later when you make it work you can try diferent approuche
         */


        //=================================================================================================

        /*
         * The new branch has been created for implementing background worker 
         *          branch name "background_worker"
         *          
         *          
         *          
         *   
         * The background worker from  C# Tutorial - BackgroundWorker _ FoxLearn has been partaly implemented
         * 
         * basic functionlaity works
         * 
         * 
         * processing files label doesent update fluently
         * 
         * 
         * It seems that the UI is still blocked while procesing files
         * 
         * 
         * 
         * Do some more testing and try to implement this background worker logic to work as expected
         * 
         * 
         * 
         * This is a very good excersise, just continue nice and temeljno. Make this program perfect
         *   
         */

        public enum HashAlgorithems
        {
            Md5,
            Sha1,
            Sha256,
            Sha384,
        }

        string Globalpath = Directory.GetCurrentDirectory();

        HashAlgorithm hash = new MD5CryptoServiceProvider();
       
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

         

            //var files = Directory.GetFiles(Globalpath + @"\TestFolder", "*", SearchOption.AllDirectories);
            //string[] extensions = files.Select(c => Path.GetExtension(c.ToLower())).Distinct().ToArray();

            //comboChecksum.SelectedText = Enum.GetNames(typeof(HashAlgorithems))[0];


            //comboExtension.Items.Clear();
            //comboChecksum.Items.AddRange(Enum.GetNames(typeof(HashAlgorithems)));
            //extFiles = new List<FileModel>();
            //Parallel.ForEach(files, currentFile =>
            //{
            //    extFiles.Add(new FileModel(Path.GetFileName(currentFile), Path.GetExtension(currentFile).ToLower(), currentFile));
            //});


            //FileModel.LoadFiles(listView, extFiles, hash);


            //foreach (var item in extensions)
            //{
            //    comboExtension.Items.Add(item);
            //}

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog ofd = new FolderBrowserDialog())
            {


                if (ofd.ShowDialog() == DialogResult.OK)
                {
                   
                    listView.Items.Clear();
                   
                    var extensions = new List<string>();
                    extensions.Clear();
                    string[] files = Directory.GetFiles(ofd.SelectedPath, "*", SearchOption.AllDirectories);
                  
                    extensions = files.Select(c => Path.GetExtension(c.ToLower())).Distinct().ToList();
                    comboExtension.Items.Clear();


                    var progressForm = new ProgressForm(files.ToList());
                    progressForm.Show();
                    progressForm.FormClosed += ProgressForm_FormClosed;
               



                    foreach (var item in extensions)
                    {
                        comboExtension.Items.Add(item);
                    }
                }
            }

        }

        private void ProgressForm_FormClosed(object sender, FormClosedEventArgs e)
        {
          
            FileModel.LoadFiles(listView,ProgressForm.fileModel , hash);
         
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.ResetText();
            if (listView.SelectedItems.Count > 0)
            {

                FileInfo info = new FileInfo(((ListViewItem)listView.SelectedItems[0]).Tag.ToString());


                richTextBox1.AppendText("Name:" + info.Name + "\n");
                richTextBox1.AppendText("Date created:" + info.CreationTime + "\n");
                richTextBox1.AppendText("Extension:" + info.Extension + "\n");
                richTextBox1.AppendText("Size:" + info.Length / 1024 + "Kb\n");
                richTextBox1.AppendText("Checksum:" + FileModel.GetHash(info.ToString(), hash));

            }
        }

        private void comboChecksum_SelectedValueChanged(object sender, EventArgs e)
        {
            switch (comboChecksum.SelectedItem)
            {
                case "Md5":
                    hash = new MD5CryptoServiceProvider();
                    break;
                case "Sha1":
                    hash = new SHA1CryptoServiceProvider();
                    break;
                case "Sha256":
                    hash = new SHA256CryptoServiceProvider();
                    break;
                case "Sha384":
                    hash = new SHA384CryptoServiceProvider();
                    break;
                default:
                    break;
            }

            FileModel.LoadFiles(listView, ProgressForm.fileModel, hash);
        }

        private void comboExtension_SelectedValueChanged(object sender, EventArgs e)
        {
            listView.Items.Clear();

            try
            {
                foreach (var item in ProgressForm.fileModel.Where(f => f.FileExtension == comboExtension.SelectedItem.ToString()))
                {

                    FileInfo info = new FileInfo(item.FileLocation);
                    ListViewItem Litem = new ListViewItem(info.Name);
                    Litem.SubItems.Add(FileModel.GetHash(item.FileLocation, hash));
                    listView.Items.Add(Litem);
                    Litem.Tag = item.FileLocation;

                }
            }
            catch (Exception)
            {

            }

            

        }

       
    }
}
