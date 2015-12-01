using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UGitUI
{
    public class DiffFile
    {
        public string Directory { get; set; }
        public string Diff { get; set; }
        public bool Selected { get; set; }

        public DiffFile(string dir, string diff, bool tracked)
        {
            Directory = dir;
            Diff = diff;

            Selected = tracked;
        }
    }
}
