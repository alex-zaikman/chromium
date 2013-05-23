using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;

namespace updater
{
    class ServerIni
    {
        public ServerIni()
        {
            zipFiles = new Dictionary<string, string> ();
        }

        public double version
        {
            get;
            set;
        }

        public int filesCount
        {
            get;
            set;
        }
        //                  md5     path
        public Dictionary<string, string> zipFiles;
   
        public bool addIfNonFile(string md5 , string path){
            if (zipFiles.ContainsKey(md5))
                return false;
            else
            {
                zipFiles.Add(md5,path);
                return true;
            }
        }


       
    }
}
