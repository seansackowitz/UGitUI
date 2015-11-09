using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LibGit2Sharp;

namespace UGitUI
{
    public class Repository : TreeViewItem
    {
        public string ItemName = "";
        public string Directory = "";


        
        public override string ToString()
        {
            return ItemName;
        }
    }
}
