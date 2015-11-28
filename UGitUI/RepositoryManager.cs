using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System.Windows.Controls;

namespace UGitUI
{
    public static class RepositoryManager
    {
        public static List<RepositoryServer> Servers = new List<RepositoryServer>();

        static Thread workerThread;

        public static void AddRepository(string directory, string url, string username, string password, ProgressBar pbar)
        {
            workerThread = new Thread(Clone);
            workerThread.Start((object)(new object[] { directory, url, username, password, pbar }));
        }

        static void Clone(object o)
        {
            object[] a = (object[])o;
            ProgressBar pBar = (ProgressBar)a[4];
            CloneOptions co = new CloneOptions();
            co.OnCheckoutProgress = (url, b, c) => {
                pBar.Dispatcher.Invoke(new ToDoDelegate(() => pBar.Maximum = c));
                pBar.Dispatcher.Invoke(new ToDoDelegate(() => pBar.Value = b));
            };
            co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = (string)a[2], Password = (string)a[3] };
            if (((string)a[1]) == "")
                return;
            string dir = Directory.CreateDirectory(Path.Combine((string)a[0], System.Text.RegularExpressions.Regex.Match((string)a[1], @"[^\/]+(?=\.git)").Value)).FullName;

            try
            {
                LibGit2Sharp.Repository.Clone((string)a[1], dir, co);
            }
            catch (LibGit2SharpException e)
            {
                if (e.Message.Contains("401"))
                    throw new NotImplementedException("Unauthorized");
                else
                    throw new NotImplementedException();
            }
        }

        private delegate void ToDoDelegate();
    }
}