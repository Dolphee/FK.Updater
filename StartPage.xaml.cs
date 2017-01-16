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
using FKUpdater.Assets.Structs;
using PropertyChanged;

namespace FKUpdater
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    /// 

    [ImplementPropertyChanged]
    public partial class StartPage : UserControl, IControl<UserControl>
    {
        public string Title { get; set; } = "Select installation path";
        public UserControl Control { get; set;}

        public StartPage()
        {
            InitializeComponent();
            Control = this;
            DataContext = MainWindow.instance;

            MainWindow.instance.NextPage = (() => {
                if (!System.IO.Directory.Exists(MainWindow.instance.CurrentDirectory)) {
                    MessageBox.Show("Invalid directory, please check it");
                    return;
                }

                MainWindow.SetPage<Connect>();
            });
        }

        private void OpenFile(object sender, MouseButtonEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description = "Select installation path for FindersKeepers",
                SelectedPath = MainWindow.instance.CurrentDirectory
            };

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                MainWindow.instance.CurrentDirectory = dialog.SelectedPath;
        }
    }
}
