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
using System.Collections.ObjectModel;
using PropertyChanged;
using FKUpdater.Assets.Structs;
using System.ComponentModel;

namespace FKUpdater
{
    /// <summary>
    /// </summary>
    /// 

    [ImplementPropertyChanged]
    public partial class DownloadFiles : UserControl, IControl<UserControl>
    {
        public string Title { get; set; } = "Downloading files";
        public UserControl Control { get; set;}

        public VersionSchema Schema { get; set; }


        public DownloadFiles()
        {
            InitializeComponent();
            DataContext = this;
            Control = this;

            GetJSONSchema();
        }

        private async void Download()
        {
            Schema = VersionSchema.Instance();
           // Schema.AddRange(Schema.AvailableFiles);

            foreach (VersionSchema.Entry Entry in Schema.ToPerform)
            {
                using (WebSocket Socket = new WebSocket())
                {
                    try
                    {
                        await Socket.Download(Entry);
                    }

                    catch (Exception e)
                    {
                        System.Windows.MessageBox.Show(e.ToString());
                    }
                }
            }

            MainWindow.instance.NextPage = (() => MainWindow.SetPage<UpToDate>());
        }

        private async void GetJSONSchema()
        {
            using (WebSocket Socket = new WebSocket())
            {
                try
                {
                    VersionSchema.Set(await Socket.DownloadString<VersionSchema>());

                    Download();
                }

                catch (Exception e)
                {
                    MessageBox.Show("Exception thrown" + e.ToString());
                }
            }
        }
    }

    public class PercentageSmallConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
             return String.Format("{0} {1}", value, "MB");
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            double val = (double)value;

            return (val == 100) 
                ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#70c944"))
                : Brushes.DodgerBlue;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return ((double)value) * 4;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
