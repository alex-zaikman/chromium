using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Collections;


namespace updater
{
    class Program
    {
        public const string LOCAL_INI_NAME = "config.ini";
        public const string EXEC_NAME = "cefclient.exe";
        public const string SERVER_INI_NAME = "version.ini.html";
        public static LocalIni local = null;
        public static ServerIni serv = null;
        public static updateForm form = null;

        static void Main(string[] args)
        {      
            try
            {
                local = getLocalIni(LOCAL_INI_NAME);

                serv = getServerIni(local);

                if (local.version < serv.version)
                {
                    form = new updateForm();
                    form.ShowDialog();

                }
                else
                {
                    Process.Start(Path.Combine(Util.getModulePath().ToString(), EXEC_NAME));
                }
            }
            catch (Exception)
            {
                Process.Start(Path.Combine(Util.getModulePath().ToString(), EXEC_NAME));
            }



        }



        public static void updateLocalIni(string LOCAL_INI_NAME, LocalIni local)
        {
            FileInfo fi = new FileInfo(Path.Combine(Util.getModulePath().ToString(), LOCAL_INI_NAME));


            string[] lines = { local.portalUri.ToString(), local.version.ToString(), local.serverUri.ToString() };
            System.IO.File.WriteAllLines(fi.ToString(), lines);
        }

        public static void extractZip(string zip)
        {
            Util.unZip(zip, Util.getModulePath().ToString());
        }

        public static ServerIni getServerIni(LocalIni local)
        {
            ServerIni ret = new ServerIni();

            string[] versionIni = Util.readFileRemoteFile(local.serverUri);

            double d;
            Double.TryParse(versionIni[0], out d); //read version
            ret.version = d;

            for (int i = 1;i+1 <versionIni.Length ;i+=2 )
            {
                Debug.Assert(i + 1 < versionIni.Length);
                ret.addIfNonFile(versionIni[i+1], versionIni[i]);
            }

            return ret;
        }

        public static LocalIni getLocalIni(string iniName)
        {
            LocalIni ret = new LocalIni();

            FileInfo fi = new FileInfo(Path.Combine(Util.getModulePath().ToString(), LOCAL_INI_NAME));

            List<string> li = Util.readFile(fi);

            ret.portalUri = new Uri(li[0]);

            double v;
            Double.TryParse(li[1], out v);
            ret.version = v;

            ret.serverUri = new Uri(li[2]);

            return ret;
        }


        public static FileInfo downloadUpdate(LocalIni local, ServerIni serv)
        {
            List<string> values = new List<string>(serv.zipFiles.Values);
            List<string> keys = new List<string>(serv.zipFiles.Keys);

            


            using (WebClient webClient = new WebClient())
            {
                int i =0;
            

                for (;  i < values.Count ; i++)
                {
                    //try resume
                    if (File.Exists(Util.getUpdateDirPath() + values[i]) && CheckSum.verifyMd5Hash(Util.getUpdateDirPath() + values[i], keys[i]))
                    {
                        continue;
                    }

                    //download
                    File.WriteAllText(Util.getUpdateDirPath() + values[i],
                        webClient.DownloadString(new Uri(Util.concutUrl(local.serverUri.ToString(), values[i]))),
                             Encoding.Default);
                    //check sum
                    if (!CheckSum.verifyMd5Hash(Util.getUpdateDirPath() + values[i], keys[i]))//second attempt
                    {
                        File.WriteAllText(Util.getUpdateDirPath() + values[i],
                        webClient.DownloadString(new Uri(Util.concutUrl(local.serverUri.ToString(), values[i]))),
                             Encoding.Default);
                        if (!CheckSum.verifyMd5Hash(Util.getUpdateDirPath() + values[i], keys[i]))
                                 throw new Exception("file download error");
                    }
                }
            }

            //combine files
            string updateFile =  Util.getUpdateDirPath() + "update.zip";

            File.Create(updateFile);

            foreach (string file in values){
              string curFile = Util.getUpdateDirPath() + file;
              File.AppendAllText(updateFile, File.ReadAllText(curFile, Encoding.Default), Encoding.Default);
              File.Delete(curFile);
            }

            return new FileInfo(updateFile);
        }




    }
}
