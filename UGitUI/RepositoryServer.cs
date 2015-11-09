using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UGitUI
{
    public class RepositoryServer : TreeViewItem
    {
        public string ItemName;

        public RepositoryServer(string name)
        {
            ItemName = name;
            Focusable = false;
        }
        
        public void Add(Repository item)
        {
            Items.Add(item);
        }
        
        public void Remove(Repository item)
        {
            Items.Remove(item);
        }

        public bool HasMatchingRepository(string value)
        {
            return Items.Cast<object>().Where(i => i.ToString().Contains(value)).Count() > 0;
        }
        
        public override string ToString()
        {
            return ItemName;
        }
    }
}
