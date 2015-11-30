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
        }

        [OnDeserialized]
        void PrepareTreeView(StreamingContext context)
        {
            TreeItem = new TreeViewItem();
            TreeItem.Tag = this;
            TreeItem.Header = ToString();
            TreeItem.Style = (System.Windows.Style)TreeItem.FindResource("RepositoryStyle");
            Repo = new LibGit2Sharp.Repository(Directory);
        }
        
        public override string ToString()
        {
            return ItemName;
        }
    }
}
