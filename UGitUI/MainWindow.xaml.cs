using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibGit2Sharp;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.IO;

namespace UGitUI
{
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow Instance;

        public RoutedEventHandler refreshTreeView;

        public Repository CurrentRepo;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            refreshTreeView = delegate (object s, RoutedEventArgs args) {
                RepositoryManager.TreeViewServers = new List<TreeViewItem>();
                foreach (RepositoryServer repoServ in RepositoryManager.Servers)
                    RepositoryManager.TreeViewServers.Add(repoServ.TreeItem);
                treeView.ItemsSource = null;
                treeView.ItemsSource = RepositoryManager.TreeViewServers;
            };

            DataFile.LoadDataFile();
            refreshTreeView(null, null);
        }

        private void MetroWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string s in files)
                {
                    if ((File.GetAttributes(s) & FileAttributes.Directory) == FileAttributes.Directory)
                        RepositoryManager.AddPreClonedRepository(s);
                }
            }
        }

        private void treeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        private void treeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem != null && ((TreeViewItem)treeView.SelectedItem).Tag != null && ((TreeViewItem)treeView.SelectedItem).Tag.GetType() == typeof(Repository))
            {
                Data.Text = "";
                CurrentRepo = ((Repository)((TreeViewItem)treeView.SelectedItem).Tag);

                foreach (var c in CurrentRepo.Repo.Diff.Compare<Patch>(CurrentRepo.Repo.Head.Tip.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory))
                {
                    if (c.Status == ChangeKind.Added || c.Status == ChangeKind.Modified || c.Status == ChangeKind.Deleted)
                    {
                        Data.Text += "\n~~~~ Patch file ~~~~\n";
                        Data.Text += c.Patch.Substring(c.Patch.IndexOf('@')) + "\n";
                    }
                }
            }
        }

        private void button_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)e.Source).Foreground = (SolidColorBrush)this.FindResource("toolBarButtonMouseHover");
        }

        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)e.Source).Foreground = (SolidColorBrush)this.FindResource("toolBarButtonMouseLeave");
        }

        private void addRepository_Click(object sender, RoutedEventArgs e)
        {
            AddRepositoryDialog d = new AddRepositoryDialog();
            this.ShowMetroDialogAsync(d);
            RoutedEventHandler close = delegate (object s, RoutedEventArgs args) { this.HideMetroDialogAsync(d); };
            d.EndDialog += close;
            d.EndDialog += refreshTreeView;
            d.cancel.Click += close;
        }

        private void repositoryFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            treeView.Items.Filter = new Predicate<object>(x => ((RepositoryServer)((TreeViewItem)x).Tag).HasMatchingRepository(repositoryFilter.Text));
            treeView.Items.Refresh();
        }
    }
}