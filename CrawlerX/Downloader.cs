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
            for (int i=0; i<= 1441;i+=11)
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

        public void EfeAgro_Manual()
        {
            FileItem fi0 = new FileItem { Source = new Uri("http://www.efeagro.com/?cat=2&submit=Buscar&s=+"), Destination= @"C:\Users\Miguel Angel\Documents\Datos\RSS\Agricultura\" + "efeagro1" + ".html" };
            client.DownloadFile(fi0.Source,fi0.Destination);
            List<FileItem> files = new List<FileItem>();
            for (int i = 2; i <= 162; i++)
            {
                FileItem fi = new FileItem { Source = new Uri("http://www.efeagro.com/page/"+i+"/"+"?cat=2&submit=Buscar&s=+"), Destination = @"C:\Users\Miguel Angel\Documents\Datos\RSS\Agricultura\" + "efeagro"+i+ ".html" };
                files.Add(fi);

            }
            foreach (FileItem fi in files)
            {
                client.DownloadFile(fi.Source, fi.Destination);
            }

        }
        public void Agroinfo_Manual()
        {
            //FileItem fi0 = new FileItem { Source = new Uri("http://www.agroinformacion.com/category/agricultura/"), Destination = @"C:\Users\Miguel Angel\Documents\Datos\RSS\Agricultura\" + "agroinfo1" + ".html" };
            //client.DownloadFile(fi0.Source, fi0.Destination);
            List<FileItem> files = new List<FileItem>();
            for (int i = 200; i <= 523; i++)
            {
                FileItem fi = new FileItem { Source = new Uri("http://www.agroinformacion.com/category/agricultura/"+"page/" + i + "/"), Destination = @"C:\Users\Miguel Angel\Documents\Datos\RSS\Agricultura\" + "agroinfo" + i + ".html" };
                files.Add(fi);

            }
            foreach (FileItem fi in files)
            {
                client.DownloadFile(fi.Source, fi.Destination);
            }

        }
    }
}
