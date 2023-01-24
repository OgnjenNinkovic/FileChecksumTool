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
       
        public Form1()
        {
            InitializeComponent();
            comboChecksum.Items.AddRange(Enum.GetNames(typeof(HashAlgorithems)));
            comboChecksum.Text = HashAlgorithems.Md5.ToString();
        }

      

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog ofd = new FolderBrowserDialog())
            {
               

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    listView.Items.Clear();
                    ProgressForm.listViewItems.Clear();


                    var progressForm = new ProgressForm(ofd.SelectedPath);
                    progressForm.Show();
                    progressForm.FormClosed += ProgressForm_FormClosed;

                }
            }

        }

        private void ProgressForm_FormClosed(object sender, FormClosedEventArgs e)
        {


            this.Invoke(new Action(() => listView.Items.AddRange(ProgressForm.listViewItems.ToArray())));

            comboExtension.Items.Clear();
            foreach (var item in ProgressForm.fileModel.Where(f=>f.FileExtension != string.Empty).Select(f=>f.FileExtension).Distinct())
            {
                comboExtension.Items.Add(item);
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
                richTextBox1.AppendText("Checksum:" + ProgressForm.GetHash(info.ToString(), hash));

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
            if (listView.Items.Count>0)
            {
                listView.Items.Clear();
                this.Invoke(new Action(() => listView.Items.AddRange(FileModel.LoadFiles(hash).Result)));

            }
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
                    Litem.SubItems.Add(ProgressForm.GetHash(item.FileLocation, hash));
                    listView.Items.Add(Litem);
                    Litem.Tag = item.FileLocation;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }



        }

       
    }
}
