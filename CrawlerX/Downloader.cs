using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace CrawlerX
{
    class Downloader
    {
        public Downloader()
        {

        }
        public void Download(List<FileItem> files)
        {
            
            foreach (FileItem fi in files)
            {
                WebClient client2 = new WebClient();
                client2.DownloadFileAsync(fi.Source, fi.Destination);
            }

        }
    }
}
