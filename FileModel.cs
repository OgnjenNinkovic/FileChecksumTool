using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileChecksum
{
    public class FileModel
    {

        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FileLocation { get; set; }



        public FileModel(string fileName, string fileExtension, string fileLocation)
        {
            FileName = fileName;
            FileExtension = fileExtension;
            FileLocation = fileLocation;

        }


        public static Task<ListViewItem[]> LoadFiles(HashAlgorithm hash)
        {


            List<ListViewItem> viewItems = new List<ListViewItem>();

            Task<ListViewItem[]> listItems = Task.Run(() => {


                foreach (ListViewItem item in ProgressForm.listViewItems)
                {
                    FileInfo info = new FileInfo(item.Tag.ToString());
                    ListViewItem listViewItem = new ListViewItem(info.Name);
                    listViewItem.SubItems.Add(ProgressForm.GetHash(item.Tag.ToString(), hash));
                    viewItems.Add(listViewItem);
                    listViewItem.Tag = item.Tag;
                }
                return viewItems.ToArray();
            });
          
            
            return listItems;

        }






      
    }
}
