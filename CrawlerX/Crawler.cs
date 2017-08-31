using System;
using AbotX.Parallel;
using AbotX.Poco;
using log4net.Config;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                //new SiteToCrawl{ Uri = new Uri("http://www.mapama.gob.es/es/estadistica/temas/estadisticas-agrarias/economia/precios-medios-nacionales/pmn_tabla.asp#")}
                //new SiteToCrawl{ Uri = new Uri("http://www.efeagro.com/?cat=3&submit=Buscar&s=+")}
                new SiteToCrawl { Uri = new Uri("http://mercadoganado.talavera.es/content/mesa-de-precios-de-ganado")}
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
                //FileItem fi = new FileItem { Source = new Uri("http://www.mapama.gob.es/es/estadistica/temas/estadisticas-agrarias/economia/precios-medios-nacionales/pmn_tabla.asp#"), Destination = @"C:\Users\Miguel Angel\Documents\Datos\" + "historico0" + ".html" };
                //files.Add(fi);
                //FileItem fi = new FileItem { Source = new Uri("http://www.efeagro.com/?cat=3&submit=Buscar&s=+"), Destination = @"C:\Users\Miguel Angel\Documents\Datos\RSS\" + "efeagro1" + ".html" };
                //files.Add(fi);
                System.Environment.Exit(1);//Terminar la ejecución.
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
                    /*
                    if (abotEventArgs.CrawledPage.Uri.ToString().Contains("pmn_historico"))
                    {
                        var name = abotEventArgs.CrawledPage.Uri.ToString().Substring(107, 9) + abotEventArgs.CrawledPage.Uri.ToString().Substring(128) + ".html";
                        FileItem fi = new FileItem { Source = abotEventArgs.CrawledPage.Uri, Destination = @"C:\Users\Miguel Angel\Documents\Datos\" + name };
                        files.Add(fi);

                    }
                    */
                    /*
                    if (abotEventArgs.CrawledPage.Uri.ToString().Contains("?cat=3"))
                    {
                        var name = "efeagro"+i+".html";
                        FileItem fi = new FileItem { Source = abotEventArgs.CrawledPage.Uri, Destination = @"C:\Users\Miguel Angel\Documents\Datos\RSS\" + name };
                        files.Add(fi);
                        i++;

                    }
                    */
                    /*
                    if (abotEventArgs.CrawledPage.Uri.ToString().Contains("ws-83480"))
                    {
                        var name = abotEventArgs.CrawledPage.Uri.ToString().Substring(31,4);
                        if (!name.Contains("ws"))
                        {
                            string nomArchivo = name + ".html";
                            FileItem fi = new FileItem { Source = abotEventArgs.CrawledPage.Uri, Destination = @"C:\Users\Miguel Angel\Documents\Datos\Clima\" + nomArchivo };
                            files.Add(fi);
                        }
                    }
                    */
                    if (abotEventArgs.CrawledPage.Uri.ToString().Contains("pdf"))
                    {
                        var name = abotEventArgs.CrawledPage.Uri.ToString().Substring(122);
                        string nomArchivo = name + ".pdf";
                        FileItem fi = new FileItem { Source = abotEventArgs.CrawledPage.Uri, Destination = @"C:\Users\Miguel Angel\Documents\Datos\Lonjas\LonjaTalavera\Vacuno\" + nomArchivo };
                        files.Add(fi);

                    }

                };
            };

            crawlEngine.StartAsync();

        }

        public List<FileItem> getfiles()
        {
            return files;
        }
    }
}
