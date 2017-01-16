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
using System.IO;
using System.Diagnostics;

namespace FKUpdater
{
    [ImplementPropertyChanged]
    public partial class Connect : UserControl, IControl<UserControl>
    {
        public string Title { get; set; } = "Fetching information from server";
        public UserControl Control { get; set; }

        public Helper Listener { get; set; }

        public Connect()
        {
            InitializeComponent();
            DataContext = this;
            Control = this;
            MainWindow.instance.NextPage = null;

            Listener = new Helper
            {
                Text = "Connecting to FindersKeepersD3.com"
            };

            GetJSONSchema();
        }

        private async void GetJSONSchema()
        {
            using (WebSocket Socket = new WebSocket())
            {
                try
                {
                    VersionSchema.Set(await Socket.DownloadString<VersionSchema>(Listener));
                    Listener.Text = "Comparing with local files";
                    CompareLocalFiles();
                }

                catch (Exception e)
                {
                    MessageBox.Show("Exception thrown" + e.ToString());
                }
            }
        }


        private void CompareLocalFiles()
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    VersionSchema.Instance().CheckFiles = GetAvailableFiles();

                    foreach (VersionSchema.Entry Entry in VersionSchema.Instance().CheckFiles)
                    {
                        string FileSlug = MainWindow.GetFileSlug(Entry.Name);

                        if (File.Exists(FileSlug))
                        {
                            FileVersionInfo Info = FileVersionInfo.GetVersionInfo(FileSlug);

                            Version FileVersion = new Version(
                                Info.FileMajorPart,
                                Info.FileMinorPart,
                                Info.FileBuildPart,
                                0
                            );

                            /* Already have the latest version, skip download */
                            if ((new FileInfo(FileSlug).Length == Entry.Size) && FileVersion == Entry.Version)
                                   continue;

                            if (IsFileLocked(FileSlug) || !TryDelete(FileSlug))
                                throw new UnauthorizedAccessException();
                        }

                        VersionSchema.Instance().ToPerform.Add(Entry);
                    }
                    System.Threading.Thread.Sleep(1000);

                    if (VersionSchema.Instance().ToPerform.Count == 0)
                        MainWindow.SetPage<UpToDate>();
                    else
                        MainWindow.SetPage<DownloadFiles>();
                });

            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private IEnumerable<IEntry> GetAvailableFiles() {
             return VersionSchema.Instance().Executables
                .Where(x => x.WindowsVersion == MainWindow.instance.VersionSelected || x.WindowsVersion == Assets.Enum.WindowsVersion.WindowsAll)
                    .Concat(VersionSchema.Instance().Libraries.
                        Where(x => x.WindowsVersion == MainWindow.instance.VersionSelected || x.WindowsVersion == Assets.Enum.WindowsVersion.WindowsAll)
                );
        }

        private bool TryDelete(string FileSlug)
        {
            try{
                File.Delete(FileSlug);
                return true;
            }

            catch (UnauthorizedAccessException e)
            {
                return false;
            }
        }

        public bool IsFileLocked(string file)
        {
            try
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Write))
                return false;
            }
            catch (UnauthorizedAccessException e)
            {
                return true;
            }
        }
    }

    public class PercentageTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            double val = (double)value;

            return String.Format("{0}{1}", Math.Round(val, 0), "%");
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
