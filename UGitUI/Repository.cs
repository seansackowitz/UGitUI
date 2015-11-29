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
        public TreeViewItem TreeItem = new TreeViewItem();

        public Repository(string name, string directory)
        {
            ItemName = name;
            Directory = directory;
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
        }
        
        public override string ToString()
        {
            return ItemName;
        }
    }
}
