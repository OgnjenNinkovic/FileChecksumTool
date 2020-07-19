using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApexSQLZadatak
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


        public static void LoadFiles(ListView list, List<FileModel> items, HashAlgorithm hash)
        {
            list.Items.Clear();

            foreach (var item in items)
            {
                if (item != null)
                {
                    FileInfo info = new FileInfo(item.FileLocation);
                    ListViewItem Litem = new ListViewItem(info.Name);
                    Litem.SubItems.Add(GetHash(item.FileLocation, hash));
                    list.Items.Add(Litem);
                    Litem.Tag = item.FileLocation;
                }
            }
        }


        public static string GetHash(string fileLocation, HashAlgorithm hashAlgo)
        {


            using (var stream = File.OpenRead(fileLocation))
            {
                var hash = hashAlgo.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }


        }
    }
}
