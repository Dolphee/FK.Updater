using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FKUpdater.Assets.Enum;
using Newtonsoft.Json;
using FKUpdater.Assets.Factory;
using PropertyChanged;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace FKUpdater.Assets.Structs
{
    public interface IEntry
    {
        string Name { get; set; }
        uint Size { get; set; }
        Version Version { get; set; }
        WindowsVersion WindowsVersion { get; set; }
        long BytesTotal { get; set; }
        long BytesRecieved { get; set; }
        double DownloadPercentage { get; }
    }


    [ImplementPropertyChanged]
    public class Helper
    {
        public double DownloadPercentage { get; set; }
        public string Text { get; set; }
    }


    [ImplementPropertyChanged]
    public class VersionSchema : Singleton<VersionSchema> 
    {
        [JsonProperty("Executables")]
        public List<Entry> Executables { get; } = new List<Entry>();

        [JsonProperty("Library")]
        public List<Entry> Libraries { get; } = new List<Entry>();

        [JsonIgnore]
        public IEnumerable<IEntry> CheckFiles { get; set; }

        [JsonIgnore]
        public BindingList<Entry> ToPerform { get; set; }

        public VersionSchema()
        {
            ToPerform = new BindingList<Entry>
            {
                AllowEdit = true,
                AllowNew = true,
                AllowRemove = true,
                RaiseListChangedEvents = true
            };

            ToPerform.ListChanged += ((s, e) =>
            {
                Total = (double)ToPerform.Sum(x => x.Size);
                TotalDownloaded = (double)ToPerform.Sum(x => x.BytesRecieved);
            });
        }

        [JsonIgnore]
        public double Total { get; set; }

        [JsonIgnore]
        public double TotalDownloaded { get; set; }

        [JsonIgnore]
        public double TotalPercentage => (TotalDownloaded / Total) * 100;

        public string FileInfo => string.Format("{0}/{1}", ((long)TotalDownloaded).BytesToMegaBytes(), ((long)Total).BytesToMegaBytes());

        [ImplementPropertyChanged]
        public class Entry : IEntry
        {
            public string Name { get; set; }
            public uint Size { get; set; }
            public Version Version { get; set; }
            public WindowsVersion WindowsVersion { get; set; }
            [JsonIgnore]
            public long BytesTotal { get; set; }
            [JsonIgnore]
            public long BytesRecieved { get; set; }
            [JsonIgnore]
            public double DownloadPercentage => (BytesTotal == 0) ? 0 : ((double)BytesRecieved / (double)BytesTotal) * 100;
            public string FileInfo => BytesTotal == 0 ? string.Format("{0}/{1}", 0, ((long)Size).BytesToMegaBytes()) : string.Format("{0}/{1}", BytesRecieved.BytesToMegaBytes(), BytesTotal.BytesToMegaBytes());
         }
    }
}
