using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace updater
{
    public partial class updateForm : Form
    {
        public updateForm()
        {
            InitializeComponent();
        }

 

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            bg.CancelAsync();
            Process.Start( Path.Combine(Util.getModulePath().ToString(), Program.EXEC_NAME));
            Application.Exit();
        }

 

        private void btn_update_Click(object sender, EventArgs e)
        {
            bg.WorkerReportsProgress = true;

            bg.ProgressChanged +=bg_ProgressChanged;

            bg.DoWork += (z, y) =>
            {
                bg.ReportProgress(10);
                FileInfo zip = Program.downloadUpdate(Program.local, Program.serv);
                bg.ReportProgress(40);
                Program.extractZip(zip.ToString());
                bg.ReportProgress(80);
                File.Delete(zip.ToString());

                Program.local.version = Program.serv.version;
                bg.ReportProgress(90);
                Program.updateLocalIni(Program.LOCAL_INI_NAME, Program.local);
                bg.ReportProgress(100);
                Process.Start(Path.Combine(Util.getModulePath().ToString(), Program.EXEC_NAME));

                Application.Exit();
            };

            btn_update.Enabled = false;
            bg.RunWorkerAsync();
        }

        private void bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }



       
    }
}
