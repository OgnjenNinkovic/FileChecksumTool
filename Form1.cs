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

        public enum HashAlgorithems
        {
            Md5,
            Sha1,
            Sha256,
            Sha384,
        }

        string Globalpath = Directory.GetCurrentDirectory();

        HashAlgorithm hash = new MD5CryptoServiceProvider();
        public List<FileModel> extFiles { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(Globalpath + @"\TestFolder", "*", SearchOption.AllDirectories);
            string[] extensions = files.Select(c => Path.GetExtension(c.ToLower())).Distinct().ToArray();

            comboChecksum.SelectedText = Enum.GetNames(typeof(HashAlgorithems))[0];


            comboExtension.Items.Clear();
            comboChecksum.Items.AddRange(Enum.GetNames(typeof(HashAlgorithems)));
            extFiles = new List<FileModel>();
            Parallel.ForEach(files, currentFile =>
            {
                extFiles.Add(new FileModel(Path.GetFileName(currentFile), Path.GetExtension(currentFile).ToLower(), currentFile));
            });


            FileModel.LoadFiles(listView, extFiles, hash);


            foreach (var item in extensions)
            {
                comboExtension.Items.Add(item);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog ofd = new FolderBrowserDialog())
            {


                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    extFiles.Clear();

                    var extensions = new List<string>();
                    string[] files = Directory.GetFiles(ofd.SelectedPath, "*", SearchOption.AllDirectories);
                    extensions.Clear();
                   extensions = files.Select(c => Path.GetExtension(c.ToLower())).Distinct().ToList();
                    comboExtension.Items.Clear();

                    Parallel.ForEach(files, currentFile =>
                    {
                        extFiles.Add(new FileModel(Path.GetFileName(currentFile), Path.GetExtension(currentFile).ToLower(), currentFile));
                    });

                    FileModel.LoadFiles(listView, extFiles, hash);

                    foreach (var item in extensions)
                    {
                        comboExtension.Items.Add(item);
                    }
                }
            }

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

            FileModel.LoadFiles(listView, extFiles, hash);
        }

        private void comboExtension_SelectedValueChanged(object sender, EventArgs e)
        {
            listView.Items.Clear();

            try
            {
                foreach (var item in extFiles.Where(f => f.FileExtension == comboExtension.SelectedItem.ToString()))
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
