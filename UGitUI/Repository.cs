using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LibGit2Sharp;
using System.Runtime.Serialization;

namespace UGitUI
{
    [Serializable]
    public class Repository
    {
        public string ItemName;
        public string Directory = "";
        public bool IsValid { get; private set; }
        public bool StorePassword;
        public string Username = @"";
        public string Password = @"";

        [NonSerialized]
        public LibGit2Sharp.Repository Repo;
        [NonSerialized]
        public TreeViewItem TreeItem = new TreeViewItem();

        public Repository(string name, string directory)
        {
            ItemName = name;
            Directory = directory;
            Repo = new LibGit2Sharp.Repository(directory);
            TreeItem = new TreeViewItem();
            TreeItem.Tag = this;
            TreeItem.Header = ToString();
            TreeItem.Style = (System.Windows.Style)TreeItem.FindResource("RepositoryStyle");
            TreeItem.ToolTip = ToString();
        }

        [OnDeserialized]
        void PrepareTreeView(StreamingContext context)
        {
            TreeItem = new TreeViewItem();
            TreeItem.Tag = this;
            TreeItem.Header = ToString();
            TreeItem.Style = (System.Windows.Style)TreeItem.FindResource("RepositoryStyle");
            TreeItem.ToolTip = ToString();
            IsValid = LibGit2Sharp.Repository.IsValid(Directory);
            if (IsValid)
                Repo = new LibGit2Sharp.Repository(Directory);
        }
        
        public override string ToString()
        {
            return ItemName;
        }
    }
}
