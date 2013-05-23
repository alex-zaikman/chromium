using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void open_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileOk += (w, r) => {
                file.Text = openFileDialog1.FileName;

                FileInfo f = new FileInfo(file.Text);
                fileSize.Text =f.Length.ToString();

                volSize.Text = ((int)(f.Length / numericUpDown1.Value)).ToString();
            };
            openFileDialog1.ShowDialog();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (file.Text != string.Empty)
            {
            FileInfo f = new FileInfo(file.Text);
            if (f.Exists)
            {
                fileSize.Text = f.Length.ToString();

                volSize.Text = ((int)(f.Length / numericUpDown1.Value)).ToString();
            }
            }
        }

        private void split_Click(object sender, EventArgs e)
        {
            if (file.Text != string.Empty)
            {
                FileInfo f = new FileInfo(file.Text);
                if (f.Exists)
                {
                    string dir = getFileDir(f.ToString());

                     Dictionary<string, string> files = splitFile( f.ToString(), dir );

                     foreach (KeyValuePair<String, String> entry in files)
                     {
                        string k = Path.GetFileName(entry.Key);
                        string v = entry.Value;
                        output.AppendText("\r\n" +k  );
                        output.AppendText("\r\n" + v);
                     }

                   
                
                }
            }
        }



        private string getFileDir(string filePath)
        {
          return new FileInfo(filePath).Directory.FullName;          
        }

        private Dictionary<string, string> splitFile(string srcFile, string destDir)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            byte[] bytes = File.ReadAllBytes(srcFile);        

            int cunck = bytes.Length/Convert.ToInt32(numericUpDown1.Value);          
            
            string newFile ;
            for (int i = 0; i < numericUpDown1.Value - 1 ; i++)
            {
                newFile = Path.Combine(destDir, (i + ".html"));
                File.WriteAllBytes(newFile, SubArray(bytes ,i*cunck, cunck));
                ret.Add(newFile, getHash(newFile));
            }

            newFile = Path.Combine(destDir, ((numericUpDown1.Value - 1) + ".html"));
            File.WriteAllBytes(newFile, SubArray(bytes, Convert.ToInt32(numericUpDown1.Value - 1) * cunck, bytes.Length - Convert.ToInt32((numericUpDown1.Value - 1) * cunck)));
            ret.Add(newFile, getHash(newFile));
 
            return ret;
        }

        private static byte[] SubArray(byte[] data, int index, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private string getHash(string filename)
        {

            using (MD5 md5 = MD5.Create())
            {
                byte[] stream = File.ReadAllBytes(filename);
                
                byte[] hash = md5.ComputeHash(stream);

                return Convert.ToBase64String(hash, 0, hash.Length);
            }
        }

    }


}
