using System;
using AbotX.Parallel;
using AbotX.Poco;
using log4net.Config;
using System.Net;
using System.Collections.Generic;

namespace CrawlerX
{
    class Crawler
    {
        private List<FileItem> files = null;
        public Crawler()
        {

        }
        public void start_crawler()
        {
            XmlConfigurator.Configure();//So the logger works
            var siteToCrawlProvider = new SiteToCrawlProvider();
            files = new List<FileItem>();
            siteToCrawlProvider.AddSitesToCrawl(new List<SiteToCrawl>
            {
                new SiteToCrawl{ Uri = new Uri("http://www.mapama.gob.es/es/estadistica/temas/estadisticas-agrarias/economia/precios-medios-nacionales/pmn_tabla.asp#")}
                
        });

            /*
             siteToCrawlProvider.AddSitesToCrawl(new List<SiteToCrawl>
            {
                new SiteToCrawl{ Uri = new Uri("http://www.tutiempo.net/clima/") },
                new SiteToCrawl{ Uri = new Uri("http://www.preciopetroleo.net/") },
                new SiteToCrawl{ Uri = new Uri("http://www.lonja.com/pub/0438lonjaciudadreal.html") },
            });


             */
            //Create the crawl engine instance
            var crawlEngine = new ParallelCrawlerEngine(new ParallelImplementationContainer
            {
                SiteToCrawlProvider = siteToCrawlProvider
            });

            //Register for site level events
            crawlEngine.AllCrawlsCompleted += (sender, eventArgs) =>
            {
                FileItem fi = new FileItem { Source = new Uri("http://www.mapama.gob.es/es/estadistica/temas/estadisticas-agrarias/economia/precios-medios-nacionales/pmn_tabla.asp#"), Destination = @"C:\Users\Miguel Angel\Documents\Datos\" + "historico0" + ".html" };
                files.Add(fi);
                Console.WriteLine("Pulse cualquier tecla e intro para continuar");
                Console.Read();
            };

            crawlEngine.SiteCrawlCompleted += (sender, eventArgs) =>
            {
                Console.WriteLine("Completed crawling site {0}", eventArgs.CrawledSite.SiteToCrawl.Uri);
                               
            };

            crawlEngine.CrawlerInstanceCreated += (sender, eventArgs) =>
            {
                //Register for crawler level events. These are Abot's events!!!
                eventArgs.Crawler.PageCrawlCompleted += (abotSender, abotEventArgs) =>
                {
                    //Console.WriteLine("You have the crawled page here in abotEventArgs.CrawledPage...");
                    if (abotEventArgs.CrawledPage.Uri.ToString().Contains("pmn_historico"))
                    {
                        var name = abotEventArgs.CrawledPage.Uri.ToString().Substring(107, 9) + abotEventArgs.CrawledPage.Uri.ToString().Substring(128) + ".html";
                        FileItem fi = new FileItem { Source = abotEventArgs.CrawledPage.Uri, Destination = @"C:\Users\Miguel Angel\Documents\Datos\" + name };
                        files.Add(fi);

                    }


                };
            };
            crawlEngine.StartAsync();
            
            //Console.WriteLine("Press enter key to stop");
            //Console.Read();

        }

        public List<FileItem> getfiles()
        {
            return files;
        }
    }
}
