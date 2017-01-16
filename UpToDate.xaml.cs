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
using FKUpdater.Assets.Structs;
using FKUpdater.Assets.Enum;
using System.IO;
using System.Diagnostics;

namespace FKUpdater
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    /// 

    [ImplementPropertyChanged]
    public partial class UpToDate : UserControl, IControl<UserControl>
    {
        public string Title { get; set; } = "Everything patched";
        public UserControl Control { get; set;}

        public WindowsVersion CurrentVersion => MainWindow.instance.VersionSelected;

        public UpToDate()
        {
            InitializeComponent();
            Control = this;
            DataContext = this;
            MainWindow.instance.NextPage = null;

        }

        private void StartFK(object sender, MouseButtonEventArgs e)
        {
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = MainWindow.GetFileSlug(CurrentVersion == WindowsVersion.Windows32Bit ? "FindersKeepers.exe" : "FindersKeepers64.exe"),
                    UseShellExecute = true
                }
            };

            p.Start();

            Application.Current.Shutdown();
        }
    }
    public class VersionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
