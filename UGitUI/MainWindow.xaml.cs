using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LibGit2Sharp;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.IO;
using System.Diagnostics;

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
            
            Data.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer());

            DataFile.LoadDataFile();
            refreshTreeView(null, null);
        }

        #region DragDrop Funtionality
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
        #endregion DragDrop Funtionality

        #region Toolbar Button Events
        private void button_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)e.Source).Foreground = (SolidColorBrush)this.FindResource("toolBarButtonMouseHover");
        }

        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)e.Source).Foreground = (SolidColorBrush)this.FindResource("toolBarButtonMouseLeave");
        }
        #endregion Toolbar Button Events

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

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem != null && ((TreeViewItem)treeView.SelectedItem).Tag != null && ((TreeViewItem)treeView.SelectedItem).Tag.GetType() == typeof(Repository))
            {
                CurrentRepo = ((Repository)((TreeViewItem)treeView.SelectedItem).Tag);
                ChangeList.Items.Clear();
                Data.Document = new ICSharpCode.AvalonEdit.Document.TextDocument();

                foreach (var c in CurrentRepo.Repo.Diff.Compare<Patch>(CurrentRepo.Repo.Head.Tip.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory))
                {
                    if (c.Status == ChangeKind.Untracked)
                        ChangeList.Items.Add(new DiffFile(c.Path, c.Patch.Substring(c.Patch.IndexOf('@')), false));
                    else if (c.Status == ChangeKind.Added || c.Status == ChangeKind.Modified || c.Status == ChangeKind.Deleted)
                        ChangeList.Items.Add(new DiffFile(c.Path, c.Patch.Substring(c.Patch.IndexOf('@')), true));
                }
            }
        }
        
        #region Treeview ContextMenu
        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void treeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
            else
            {
                if (treeView.SelectedItem != null)
                    ((TreeViewItem)treeView.SelectedItem).IsSelected = false;
            }
        }

        private void treeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (treeView.SelectedItem == null)
                e.Handled = true;
        }

        private void OpenExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (treeView.SelectedItem != null)
                Process.Start(((Repository)((TreeViewItem)treeView.SelectedItem).Tag).Repo.Info.WorkingDirectory);
        }

        private void RemoveRepository_Click(object sender, RoutedEventArgs e)
        {
            if (treeView.SelectedItem != null)
            {
                RepositoryManager.RemoveRepository((Repository)((TreeViewItem)treeView.SelectedItem).Tag);
                refreshTreeView(null, null);
                DataFile.SaveDataFile();
            }
        }
        #endregion Treeview ContextMenu

        private void ChangeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChangeList.SelectedItem == null || ChangeList.SelectedItem.GetType() != typeof(DiffFile))
                return;

            DiffFile dFile = (DiffFile)ChangeList.SelectedItem;
            Data.Document = new ICSharpCode.AvalonEdit.Document.TextDocument();
            Data.Document.Text = dFile.Diff;
        }
    }
}