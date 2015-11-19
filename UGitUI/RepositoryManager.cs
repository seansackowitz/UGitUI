using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace UGitUI
{
    public static class RepositoryManager
    {
        public static List<RepositoryServer> Servers = new List<RepositoryServer>();

        public static void AddRepository(string directory, string url)
        {
            CloneOptions co = new CloneOptions();
            co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = "Sean", Password = "keks49585" };
            
            LibGit2Sharp.Repository.Clone(url, "Z:\\GitTesting\\Git\\", co);
        }
    }
}