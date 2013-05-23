using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace updater
{
    class Util
    {
        public static string SEPERATOR = "" + Path.DirectorySeparatorChar;

        public static FileInfo getModulePath()
        {

            string mp = System.Reflection.Assembly.GetExecutingAssembly().Location;

            mp = mp.Substring(0, mp.LastIndexOf(SEPERATOR));

            return new FileInfo(mp);
        }

        public static List<string> readFile(FileInfo fi)
        {

            //   int counter = 0;
            string line;
            List<string> ret = new List<string>();
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(fi.ToString());
            while ((line = file.ReadLine()) != null)
            {
                ret.Add(line);
                // counter++;
            }

            file.Close();

            return ret;
        }

        public static string[] readFileRemoteFile(Uri url)
        {

            List<string> ret = new List<string>();


            var result = string.Empty;
            using (var webClient = new System.Net.WebClient())
            {
                result = webClient.DownloadString(concutUrl(url.ToString(), "version.ini.html"));
            }
            string[] stringSeparators = new string[] { "\n", "\r" };
            return result.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);


        }

        //with trailing separator
        public static FileInfo getTempPath()
        {
            return new FileInfo(System.IO.Path.GetTempPath());
        }

        public static string getUpdateDirPath()
        {
           string ret = ( Path.Combine(Util.getModulePath().ToString(),"update")+SEPERATOR);
           if (!Directory.Exists(ret))
                Directory.CreateDirectory(ret);
           return ret;
        }


        public static string concutUrl(string f, string s)
        {
            return f.EndsWith(@"/") ? f + s : f + @"/" + s;
        }



        public static void unZip(string archiveFilenameIn, string outFolder)
        {
            ZipFile zf = null;
            try
            {
                FileStream fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);

                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entry names against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096];     // 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }



    }
}
