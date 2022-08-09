using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YandexDisk.Client;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Http;

namespace testyadisk
{
    public partial class Form1 : Form
    {
        #region yandex

        //github ya.ru api: https://github.com/raidenyn/yandexdisk.client

        async Task UploadSample(string fileEntry, string path)
        {
            //You should have oauth token from Yandex Passport.
            //See https://tech.yandex.ru/oauth/
            StreamReader sr = new StreamReader("authtoken.txt");
            string oauthToken = sr.ReadToEnd();

            // Create a client instance
            IDiskApi diskApi = new DiskHttpApi(oauthToken);

            //Upload file from local
#if DEBUG
            string yaPath = $"/test/{path}/{Path.GetFileName(fileEntry)}";
#else
            string yaPath = $"/rop/{path}/{Path.GetFileName(fileEntry)}";
#endif
            await diskApi.Files.UploadFileAsync(path: yaPath,
                                                overwrite: false,
                                                localFile: fileEntry,
                                                cancellationToken: CancellationToken.None);
        }
        #endregion 

       
        private Task RenameAndMove(string fileEntry, string phoneNum, string Fio)
        {
            if (fileEntry.Contains($"_{phoneNum}_"))
            {
                int ind = fileEntry.LastIndexOf('\\');
                string fname = fileEntry.Insert(ind + 1, $"{Fio}_");
                string name = Path.GetFileName(fname);
#if DEBUG
                string dis = @"K:\ПРОДАЖИ записи TEST\" + Fio;
#else
                string dis = @"K:\ПРОДАЖИ записи\" + Fio;
#endif
                var distPath = $"{dis}\\{name}";

                if (!Directory.Exists(dis))
                    Directory.CreateDirectory(dis);

                if (!File.Exists(distPath))
                {
                    File.Move(fileEntry, distPath);
                    listBox1.Items.Add(Path.GetFileName(fileEntry));
                    var task = UploadSample(distPath, Fio);
                    return task;
                }
                return null;
            }
            return null;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            var tasks = new List<Task>();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string exfold = folderBrowserDialog1.SelectedPath;

                string[] fileEntries = Directory.GetFiles(exfold);

                foreach (string fileEntry in fileEntries)
                {
                    void AddTask(string phone, string name)
                    {
                        var t = RenameAndMove(fileEntry, phone, name);
                        if (t != null)
                            tasks.Add(t);
                    }
                    AddTask("102", "Лев");
                    AddTask("103", "Ерошина");
                    AddTask("104", "Пабуева");
                    AddTask("107", "Латыпов");
                    AddTask("108", "Сотникова");
                    AddTask("109", "Романова");
                    AddTask("110", "Харченко");
                    AddTask("111", "Комиссар");
                }
                await Task.WhenAll(tasks);

                MessageBox.Show("done!");
            }
        }
    }
}
