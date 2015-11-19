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

namespace UGitUI
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void treeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        private void treeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

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
            d.accept.Click += close;
            d.cancel.Click += close;
        }
    }
}