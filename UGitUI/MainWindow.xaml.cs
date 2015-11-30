﻿using System;
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

            RepositoryManager.LoadDataFile();
            //RepositoryServer tempS = RepositoryManager.AddRepositoryServer(@"http://git.roadturtlegames.com/Sean/UGitUI.git");
            //tempS.Add(new Repository("UGitUI", @"E:\Programming\Gitlab\UGitUI\.git"));
            //RepositoryManager.SaveDataFile();
            refreshTreeView(null, null);
        }

        private void treeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        private void treeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem != null)
            {
                Data.Text = "";
                CurrentRepo = ((Repository)((TreeViewItem)treeView.SelectedItem).Tag);

                foreach (TreeEntryChanges c in CurrentRepo.Repo.Diff.Compare<TreeChanges>(CurrentRepo.Repo.Head.Tip.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory))
                {
                    Data.Text += c.Status + " : " + c.Path + "\n";
                }

                Data.Text += "\n\n";

                foreach (var item in CurrentRepo.Repo.ObjectDatabase)
                {
                    Data.Text += item.Id + "\n";
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