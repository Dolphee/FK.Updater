using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using FKUpdater.Assets.Structs;

namespace FKUpdater
{
    public class WebSocket : IDisposable
    {
        public WebClient WebClient { get; set; }
        private WebServer WebServer { get; } = new WebServer();

        public WebSocket()
        {
            WebClient = GetClient();
        }

        private WebClient GetClient()
        {
            return new WebClient
            {
                UseDefaultCredentials = true,
                Proxy = null
            };
        }
        
        public async Task Download(VersionSchema.Entry File)
        {
            WebClient.DownloadProgressChanged += (s,e) =>
            {
                File.BytesRecieved = e.BytesReceived;
                File.BytesTotal = e.TotalBytesToReceive;
            };

            await WebClient.DownloadFileTaskAsync(
                    WebServer.FileSlug(File.Name),
                    WebServer.FileSlugLocal(File.Name)
            );
        }

        public async Task<T> DownloadString<T>(Helper Listener = null)
        {
            try
            {
                if(Listener != null)
                    WebClient.DownloadProgressChanged += ((s, e) => {
                        Listener.DownloadPercentage = e.ProgressPercentage;
                        System.Diagnostics.Trace.WriteLine(e.ProgressPercentage);
                    });

                string data = await WebClient.DownloadStringTaskAsync(WebServer.GetVersions);
                return JsonConvert.DeserializeObject<T>(data);
            }

            catch (WebException e)
            {
                throw e;
            }
        }

        public void Dispose()
        {
            if (WebClient != null)
                WebClient.Dispose();
        }

        ~WebSocket()
        {
            this.Dispose();
        }
    }

    public class WebServer
    {
        private string Server => "http://www.finderskeepersd3.com";
        private string Version => "FK/versions.json";
        private string Download => ".Download";

        public Uri GetVersions => new Uri(string.Format("{0}/{1}", Server, Version));
        public Uri DownloadFile => new Uri(string.Format("{0}/{1}", Server, Download));
        public string FileSlug(string Filename) => string.Format("{0}/{1}/{2}", Server, Download, Filename);
        public string FileSlugLocal(string Filename) => string.Format("{0}/{1}", MainWindow.Directory, Filename);
    }
}
