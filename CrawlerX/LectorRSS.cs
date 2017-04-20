using System;
using System.Xml;
using System.IO;
using AngleSharp.Parser.Html;
using Nest;

namespace CrawlerX
{
    class LectorRSS
    {
        ElasticClient elasticClient = Broker.EsClient();
        
        public LectorRSS()
        {

        }
        public void RSS_XML()
        {
            try
            {
               
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("http://www.agroes.es/ganaderia?format=feed&type=rss");

                XmlNodeList t = xDoc.GetElementsByTagName("title");
                XmlNodeList l = xDoc.GetElementsByTagName("link");
                XmlNodeList d = xDoc.GetElementsByTagName("description");

                for (int i = 0; i < t.Count; i++)
                {
                  
                    if (i != 0)
                    {
                        Noticia n = new Noticia();
                        n.id = i;
                        n.titulo = t[i].InnerText;
                        n.link = l[i].InnerText;
                        n.descripcion = description(d[i].InnerText);
                        elasticClient.Index(n, es => es
                                                    .Index("agroesi")
                                                    .Type("noticia")
                                                    .Id(n.id)
                                        );
                    }                                  
                }
            }
            catch (Exception)
            {
                Console.WriteLine("No se puede conectar.");
            }
        }
        
        public string description(string description)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(description);
            var itemsCSS = document.GetElementsByTagName("p");
            int contador = 0;
            string desc = null;
            foreach (var item in itemsCSS)
            {
                if(contador == 1)
                {
                    desc=item.TextContent;
                }
                contador++;
            }
            return desc;

        }
       
    
}
}
