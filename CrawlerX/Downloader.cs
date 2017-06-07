using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace CrawlerX
{
    class Downloader
    {
        WebClient client = new WebClient();
        public Downloader()
        {

        }
        public void Download(List<FileItem> files)
        {
            
            foreach (FileItem fi in files)
            {
                client.DownloadFile(fi.Source, fi.Destination);
            }

        }
        public void Manual_Download()
        {
            int contador = 1;
            List<FileItem> files = new List<FileItem>();
            for (int i=0; i<= 1496;i+=11)
            {
                FileItem fi = new FileItem { Source = new Uri("https://www.oviespana.com/informacion-de-ovino/servicio-diario-de-noticias/en-portada?limitstart="+i), Destination = @"C:\Users\Miguel Angel\Documents\Datos\RSS\" + "oviespana"+contador+ ".html" };
                files.Add(fi);
                contador++;

            }
            foreach (FileItem fi in files)
            {
                client.DownloadFile(fi.Source, fi.Destination);
            }


        }
    }
}
