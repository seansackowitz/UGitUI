using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UGitUI
{
    [Serializable]
    public class RepositoryServer : IEquatable<RepositoryServer>
    {
        public string ItemName;
        public List<Repository> Repos = new List<Repository>();
        [NonSerialized]
        public TreeViewItem TreeItem = new TreeViewItem();

        public RepositoryServer(string url)
        {
            ItemName = Regex.Match(url, @"([a-z0-9]+\.)*[a-z0-9]+\.[a-z]+").Value;
            TreeItem = new TreeViewItem();
            TreeItem.Tag = this;
            TreeItem.Focusable = false;
            TreeItem.Header = ToString();
            TreeItem.Style = (System.Windows.Style)TreeItem.FindResource("RepositoryServerStyle");
        }
        
        public void Add(Repository item)
        {
            Repos.Add(item);
            TreeItem.Items.Add(item.TreeItem);
        }
        
        public void Remove(Repository item)
        {
            Repos.Remove(item);
            TreeItem.Items.Remove(item.TreeItem);
        }

        public bool HasMatchingRepository(string value)
        {
            TreeItem.Items.Filter = new Predicate<object>(i => ((Repository)((TreeViewItem)i).Tag).ToString().ToLower().Contains(value.ToLower()));
            TreeItem.Items.Refresh();
            return TreeItem.Items.Cast<object>().Where(i => ((Repository)((TreeViewItem)i).Tag).ToString().ToLower().Contains(value.ToLower())).Count() > 0;
        }

        [OnDeserialized]
        void PrepareTreeView(StreamingContext context)
        {
            TreeItem = new TreeViewItem();
            TreeItem.Tag = this;
            TreeItem.Focusable = false;
            TreeItem.Header = ToString();
            TreeItem.Style = (System.Windows.Style)TreeItem.FindResource("RepositoryServerStyle");

            foreach (Repository repo in Repos)
                TreeItem.Items.Add(repo.TreeItem);
        }
        
        public override string ToString()
        {
            return ItemName;
        }

        public new bool Equals(object obj)
        {
            if (obj == null) return false;
            RepositoryServer objAsPart = obj as RepositoryServer;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public bool Equals(RepositoryServer other)
        {
            return ItemName == other.ItemName;
        }
    }
}
