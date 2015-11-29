using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System.Windows.Controls;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace UGitUI
{
    public static class RepositoryManager
    {
        public static List<RepositoryServer> Servers = new List<RepositoryServer>();
        public static List<TreeViewItem> TreeViewServers = new List<TreeViewItem>();

        static Thread workerThread;

        public static void AddRepository(string directory, string url, string username, string password, AddRepositoryDialog dialog)
        {
            workerThread = new Thread(Clone);
            workerThread.SetApartmentState(ApartmentState.STA);
            workerThread.Start((object)(new object[] { directory, url, username, password, dialog }));
        }

        static void Clone(object o)
        {
            object[] a = (object[])o;
            AddRepositoryDialog dlg = null;
            ProgressBar pBar = null;
            Label mBox = null;

            if (a[4] != null)
            {
                dlg = (AddRepositoryDialog)a[4];
                pBar = dlg.progressBar;
                mBox = dlg.warning;
            }

            CloneOptions co = new CloneOptions();
            co.OnCheckoutProgress = (url, b, c) => {
                if (dlg != null && dlg.IsVisible != false)
                {
                    pBar.Dispatcher.Invoke(new Action(() => pBar.Maximum = c));
                    pBar.Dispatcher.Invoke(new Action(() => pBar.Value = b));
                }
            };
            co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = (string)a[2], Password = (string)a[3] };

            string repoName = "";
            string dir = "";
            string repoDir = "";

            try
            {
                repoName = System.Text.RegularExpressions.Regex.Match((string)a[1], @"[^\/]+(?=\.git)").Value;
                dir = Directory.CreateDirectory(Path.Combine((string)a[0], repoName)).FullName;

                repoDir = LibGit2Sharp.Repository.Clone((string)a[1], dir, co);

                MainWindow.Instance.Dispatcher.Invoke(new Action(() => {
                    Repository repo = new Repository(repoName, repoDir);
                    AddRepositoryServer((string)a[1]).Add(repo);
                    MainWindow.Instance.refreshTreeView(null, null);
                }));

                if (dlg != null && dlg.IsVisible != false)
                    dlg.Dispatcher.Invoke(new Action(() => dlg.EndDialog(null, null)));

                SaveDataFile();
            }
            catch (LibGit2SharpException e)
            {
                if (mBox != null)
                {
                    if (e.Message.Contains("401"))
                        mBox.Dispatcher.Invoke(new Action(() => mBox.Content = "401 Unauthorized Request.\nCheck credentials and try again."));
                    else if (e.Message.Contains("404"))
                        mBox.Dispatcher.Invoke(new Action(() => mBox.Content = "404 Not Found.\nCheck repository url and try again."));
                    else if (e.Message.Contains("503"))
                        mBox.Dispatcher.Invoke(new Action(() => mBox.Content = "503 Service Unavailable.\nServer is down. Try again later."));
                    else if (e.Message.Contains("exists and is not an empty directory"))
                        mBox.Dispatcher.Invoke(new Action(() => mBox.Content = "Output directory is not empty at:\n" + dir));
                    else
                        throw new NotImplementedException(e.Message);
                }
                else
                    throw new NotImplementedException(e.Message);
            }
        }

        static RepositoryServer AddRepositoryServer(string s)
        {
            RepositoryServer tmpServer = new RepositoryServer(s);
            RepositoryServer foundServer = Servers.Find(x => x.ItemName == tmpServer.ItemName);
            if (foundServer == null)
            {
                Servers.Add(tmpServer);
                return tmpServer;
            }
            else
                return foundServer;
        }

        public static void DeleteDirectory(string targetDir)
        {
            File.SetAttributes(targetDir, FileAttributes.Normal);

            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        public static void SaveDataFile()
        {
            try
            {
                using (Stream stream = File.Open("data.bin", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, Servers);
                }
            }
            catch (IOException) { }
        }

        public static void LoadDataFile()
        {
            try
            {
                using (Stream stream = File.Open("data.bin", FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    Servers = (List<RepositoryServer>)bin.Deserialize(stream);
                }
            }
            catch (IOException) { }
        }
    }
}