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
using FKUpdater.Assets.Factory;
using PropertyChanged;
using FKUpdater.Assets.Factory;
using FKUpdater.Assets.Enum;
using System.IO;

namespace FKUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    [ImplementPropertyChanged]
    public partial class MainWindow : Window
    {
        public static MainWindow instance { get; set; }
        public IControl<UserControl> UserPage { get; set; }
        public WindowsVersion VersionSelected { get; set; }
        public Action NextPage { get; set; }

        public static bool FirstInstallation => (MainWindow.instance.VersionSelected == WindowsVersion.Windows32Bit) 
            ? !File.Exists(GetFileSlug("FindersKeepers.exe")) 
            : !File.Exists(GetFileSlug("FindersKeepers64.exe")
        );

        public string CurrentDirectory { get; set; } = System.AppDomain.CurrentDomain.BaseDirectory;
        public static string Directory => instance.CurrentDirectory;
        public static string GetFileSlug(string file) => System.IO.Path.Combine(instance.CurrentDirectory, file);

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            instance = this;

            /* Win32 , skip check for 64 bit */
            if (!Environment.Is64BitOperatingSystem)
            {
                MainWindow.instance.VersionSelected = WindowsVersion.Windows32Bit;

                if (MainWindow.FirstInstallation)
                    MainWindow.SetPage<StartPage>();
                else
                    MainWindow.SetPage<Connect>();
            }

            else
            {
                 SetPage<VersionSelecter>();
            }
        }


        public static void SetPage<T>(object parameters = null) where T : UserControl
        {
            WebExtensions.UIThread(() =>
            {
                MainWindow.instance.UserPage = Create<T>(parameters);
            });
        }

        public static IControl<UserControl> Create<T>(object parameters = null) where T : UserControl
        {
            if(parameters != null)
                return (IControl<UserControl>)Activator.CreateInstance(typeof(T),parameters);

            return (IControl<UserControl>)Activator.CreateInstance<T>();
        }

        private void Close(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MoveNext(object sender, MouseButtonEventArgs e)
        {
            NextPage?.Invoke();
        }
    }


    public class VisabilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            
            return value == null ?  Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
