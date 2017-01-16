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
using FKUpdater.Assets.Enum;
using PropertyChanged;

namespace FKUpdater
{
    /// <summary>
    /// </summary>
    /// 

    [ImplementPropertyChanged]
    public partial class VersionSelecter : UserControl, IControl<UserControl>
    {
        public string Title { get; set; } = "Select your operativing system";
        public UserControl Control { get; set;}

        public VersionSelecter()
        {
            InitializeComponent();
            Control = this;

            MainWindow.instance.VersionSelected = WindowsVersion.Windows64Bit;

            if (MainWindow.FirstInstallation)
                MainWindow.instance.NextPage = (() => MainWindow.SetPage<StartPage>()); 
            else
                MainWindow.instance.NextPage = (() => MainWindow.SetPage<Connect>());

        }

        private void SetVersion(object sender, MouseButtonEventArgs e)
        {
            Border element = (Border)sender;
            WindowsVersion Version = (WindowsVersion)Enum.Parse(typeof(WindowsVersion), element.Name);

            foreach (Border border in Versions.Children.OfType<Border>())
            {
                border.Background = Brushes.Transparent;
                border.BorderBrush = Brushes.Transparent;
            }

            element.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#155975"));
            element.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#0b1426"));
            MainWindow.instance.VersionSelected = Version;

            if (MainWindow.FirstInstallation)
                MainWindow.instance.NextPage = (() => MainWindow.SetPage<StartPage>());
            else
                MainWindow.instance.NextPage = (() => MainWindow.SetPage<Connect>());
        }

        private void MouseOver(object sender, MouseEventArgs e)
        {
            Border element = (Border)sender;
            WindowsVersion Version = (WindowsVersion)Enum.Parse(typeof(WindowsVersion), element.Name);

            if (Version == MainWindow.instance.VersionSelected)
                return;

            element.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#1f1f1f"));
            element.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#101010"));
        }

        private void MouseLeave(object sender, MouseEventArgs e)
        {
            Border element = (Border)sender;
            WindowsVersion Version = (WindowsVersion)Enum.Parse(typeof(WindowsVersion), element.Name);

            if (Version == MainWindow.instance.VersionSelected)
                return;

            element.Background = Brushes.Transparent;
            element.BorderBrush = Brushes.Transparent;
        }
    }
}
