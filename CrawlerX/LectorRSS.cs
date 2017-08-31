using System;
using System.Xml;
using System.IO;
using AngleSharp.Parser.Html;
using Nest;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;

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
            Agroinfo();
            EfeAgro();
            OviEspa();
            Soja();

        }        

        public void Agroinfo()
        {
            try
            {

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("http://www.agroinformacion.com/feed/");

                XmlNodeList items = xDoc.GetElementsByTagName("item");

                string titulo = null;
                string link = null;
                string fechaN = null;
                string descripcion = null;
                int dia = 0;
                int numMes = 0;
                int año = 0;
                string nomMes = null;
                string imagen = null;
                foreach (XmlNode item in items)
                {
                    List<string> etiquetas = new List<string>();
                    foreach (XmlNode dato in item.ChildNodes)
                    {
                        switch (dato.Name)
                        {
                            case "title":
                                titulo = dato.InnerText;
                                break;
                            case "link":
                                link = dato.InnerText;
                                break;
                            case "pubDate":
                                fechaN = dato.InnerText.Split(',')[1].Split('+')[0].Trim(' ');
                                dia = System.Convert.ToInt16(fechaN.Split(' ')[0]);
                                nomMes = CompletaMes(fechaN.Split(' ')[1]);
                                numMes = System.Convert.ToInt16(ConvierteMes(nomMes));
                                año = System.Convert.ToInt16(fechaN.Split(' ')[2]);
                                fechaN = dia + "-" + ConvierteMes(nomMes) + "-" + año;
                                break;
                            case "category":
                                etiquetas.Add(dato.InnerText.ToLower());
                                break;
                            case "content:encoded":
                                descripcion = descriptionAgroinfo(dato.InnerText).Split('<')[0];
                                imagen = descriptionAgroinfo(dato.InnerText).Split('<')[1];
                                break;

                        }
                    }
                    Noticia n = new Noticia();
                    n.codigo = "AG";
                    n.titulo = titulo;
                    n.link = link;
                    n.descripcion = descripcion;
                    foreach(string etiq in etiquetas)
                    {
                        n.etiquetas = n.etiquetas + "," + etiq;
                    }
                    n.etiquetas = n.etiquetas.Trim(',');
                    n.dia = dia;
                    n.numMes = numMes;
                    n.nomMes = nomMes;
                    n.año = año;
                    n.fecha = fechaN;
                    n.imagen = imagen;
                    GuardaNoticiaAG(n);

                }

            }
            catch (Exception)
            {
                Console.WriteLine("No se puede conectar.");
            }

        }

        public void EfeAgro()
        {
            try
            {

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("http://www.efeagro.com/feed/?post_type=noticia");

                XmlNodeList items = xDoc.GetElementsByTagName("item");

                string titulo = null;
                string link = null;
                string fechaN = null;
                string descripcion = null;
                int dia = 0;
                int numMes = 0;
                int año = 0;
                string nomMes = null;
                string imagen = null;
                foreach (XmlNode item in items)
                {
                    List<string> etiquetas = new List<string>();
                    foreach (XmlNode dato in item.ChildNodes)
                    {
                        switch (dato.Name)
                        {
                            case "title":
                                titulo = dato.InnerText;
                                break;
                            case "link":
                                link = dato.InnerText;
                                break;
                            case "pubDate":
                                fechaN = dato.InnerText.Split(',')[1].Split('+')[0].Trim(' ');
                                dia = System.Convert.ToInt16(fechaN.Split(' ')[0]);
                                nomMes = CompletaMes(fechaN.Split(' ')[1]);
                                numMes = System.Convert.ToInt16(ConvierteMes(nomMes));
                                año = System.Convert.ToInt16(fechaN.Split(' ')[2]);
                                fechaN = dia + "-" + ConvierteMes(nomMes) + "-" + año;
                                break;
                            case "category":
                                etiquetas.Add(dato.InnerText.ToLower());
                                break;
                            case "content:encoded":
                                descripcion = descriptionEfeagro(dato.InnerText).Split('<')[0];
                                imagen = descriptionEfeagro(dato.InnerText).Split('<')[1];
                                break;

                        }
                    }
                    Noticia n = new Noticia();
                    n.codigo = "AG";
                    n.titulo = titulo;
                    n.link = link;
                    n.descripcion = descripcion;
                    foreach (string etiq in etiquetas)
                    {
                        n.etiquetas = n.etiquetas + "," + etiq;
                    }
                    n.etiquetas = n.etiquetas.Trim(',');
                    n.dia = dia;
                    n.numMes = numMes;
                    n.nomMes = nomMes;
                    n.año = año;
                    n.fecha = fechaN;
                    n.imagen = imagen;
                    GuardaNoticiaAG(n);
                }

            }
            catch (Exception)
            {
                Console.WriteLine("No se puede conectar.");
            }

        }

        public void OviEspa()
        {
            try
            {

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("https://www.oviespana.com/informacion-de-ovino/servicio-diario-de-noticias?format=feed");

                XmlNodeList items = xDoc.GetElementsByTagName("item");

                string titulo = null;
                string link = null;
                string fechaN = null;
                string etiquetas = null;
                string descripcion = null;
                int dia = 0;
                int numMes = 0;
                int año = 0;
                string nomMes = null;
                string imagen = null;
                foreach (XmlNode item in items)
                {
                    foreach (XmlNode dato in item.ChildNodes)
                    {
                        switch (dato.Name)
                        {
                            case "title":
                                titulo = dato.InnerText;
                                break;
                            case "link":
                                link = dato.InnerText;
                                etiquetas=etiqOviEspa(link);
                                break;
                            case "pubDate":
                                fechaN = dato.InnerText.Split(',')[1].Split('+')[0].Trim(' ');
                                dia = System.Convert.ToInt16(fechaN.Split(' ')[0]);
                                nomMes = CompletaMes(fechaN.Split(' ')[1]);
                                numMes = System.Convert.ToInt16(ConvierteMes(nomMes));
                                año = System.Convert.ToInt16(fechaN.Split(' ')[2]);
                                fechaN = dia + "-" + ConvierteMes(nomMes) + "-" + año;
                                break;
                            case "description":
                                descripcion=descriptionOviEspa(dato.InnerText);
                                imagen = imgOviEspa(link);
                                if (!imagen.Contains("jpg"))
                                {
                                    imagen = "no";
                                }
                                break;

                        }
                    }
                    
                    Noticia n = new Noticia();
                    n.codigo = "GN";
                    n.titulo = titulo;
                    n.link = link;
                    n.descripcion = descripcion;
                    n.etiquetas = etiquetas;
                    n.dia = dia;
                    n.numMes = numMes;
                    n.nomMes = nomMes;
                    n.año = año;
                    n.fecha = fechaN;
                    n.imagen = imagen;
                    GuardaNoticiaGN(n);
                }

            }
            catch (Exception)
            {
                Console.WriteLine("No se puede conectar.");
            }

        }

        public void Soja()
        {
            string contenido = null; ;
            try
            {
                WebRequest req = HttpWebRequest.Create("http://www.ambito.com/economia/mercados/granos/info/?id=Soja");
                req.Method = "GET";
                using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    contenido = reader.ReadToEnd();
                }

            }
            catch (Exception ex) {
            }

            var parser = new HtmlParser();
            var document = parser.Parse(contenido);

            var fecha = document.GetElementById("topMercadosHistorico");
            var precio = document.GetElementById("grafico_hst");

            string date = fecha.Children[1].Children[1].TextContent.Substring(1);
            string prize = precio.Children[0].Children[1].Children[1].TextContent;
            Precio p = new Precio();
            p.codigo = "SJ";
            p.precio = double.Parse(prize.ToString());
            p.dia = System.Convert.ToInt16(date.Split('/')[0]);
            p.numMes = System.Convert.ToInt16(date.Split('/')[1]);
            p.nomMes = ConvierteMesInv(date.Split('/')[1]);
            p.año = System.Convert.ToInt16(date.Split('/')[2]);
            p.fecha = p.dia + "-" + date.Split('/')[1] + "-" + p.año;
            p.tipoPrecio = "med";
            p.fuente = "Chicago";
            p.medida = "$/tn";
            GuardaPrecio(p);
        }

        public string descriptionAgroinfo(string description)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(description);
            var itemsCSS = document.GetElementsByTagName("p");
            int contador = 0;
            string desc = null;
            bool enlace = false;
            string linkImg = null;
            foreach (var item in itemsCSS)
            {
                if (item.InnerHtml.Contains("img") & !enlace)
                {
                    linkImg = item.FirstElementChild.GetAttribute("src");
                    enlace = true;
                }
                if (contador == 0)
                {
                    desc = item.TextContent;
                }
                contador++;
            }
            return desc + "<" + linkImg;

        }

        public string descriptionEfeagro(string description)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(description);
            var imagenes = document.GetElementsByTagName("img");
            var textos = document.GetElementsByTagName("p");

            string desc = null;
            string linkImg = null;
            linkImg = imagenes[0].GetAttribute("src");
            desc = textos[0].TextContent;
            return desc + "<" + linkImg;

        }

        public string descriptionOviEspa(string description)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(description);
            var textos = document.GetElementsByTagName("p");

            string desc = null;
            desc = textos[0].TextContent;
            return desc;

        }

        public string etiqOviEspa(string enlace)
        {
            string etiquetas = null;
            string contenido = null;
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(enlace);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                contenido = sr.ReadToEnd();
                sr.Close();
                myResponse.Close();

            }
            catch (Exception ex)
            {
            }

            var parser = new HtmlParser();
            var document = parser.Parse(contenido);
            var etiq = document.GetElementsByClassName("itemTagsBlock")[0].Children[1].Children;

            int contador = 0;
            foreach (var etiqueta in etiq)
            {
                if (contador == 0)
                {
                    etiquetas = etiqueta.TextContent.Trim(' ').ToLower();
                }
                else
                {
                    etiquetas = etiquetas + "," + etiqueta.TextContent.Trim(' ').ToLower();
                }
                contador++;
                
            }
            return etiquetas;

        }

        public string ConvierteMes(string dato)
        {
            string mes = null;
            switch (dato)
            {
                case "enero":
                    mes = "01";
                    break;
                case "febrero":
                    mes = "02";
                    break;
                case "marzo":
                    mes = "03";
                    break;
                case "abril":
                    mes = "04";
                    break;
                case "mayo":
                    mes = "05";
                    break;
                case "junio":
                    mes = "06";
                    break;
                case "julio":
                    mes = "07";
                    break;
                case "agosto":
                    mes = "08";
                    break;
                case "septiembre":
                    mes = "09";
                    break;
                case "octubre":
                    mes = "10";
                    break;
                case "noviembre":
                    mes = "11";
                    break;
                case "diciembre":
                    mes = "12";
                    break;
            }


            return mes;

        }

        public string CompletaMes(string dato)
        {
            string mes = null;
            switch (dato)
            {
                case "Ene":
                    mes = "enero";
                    break;
                case "Feb":
                    mes = "febrero";
                    break;
                case "Mar":
                    mes = "marzo";
                    break;
                case "Abr":
                    mes = "abril";
                    break;
                case "May":
                    mes = "mayo";
                    break;
                case "Jun":
                    mes = "junio";
                    break;
                case "Jul":
                    mes = "julio";
                    break;
                case "Aug":
                    mes = "agosto";
                    break;
                case "Sep":
                    mes = "septiembre";
                    break;
                case "Oct":
                    mes = "octubre";
                    break;
                case "Nov":
                    mes = "noviembre";
                    break;
                case "Dic":
                    mes = "diciembre";
                    break;
            }


            return mes;

        }

        public string ConvierteMesInv(string dato)
        {
            string mes = null;
            switch (dato)
            {
                case "01":
                    mes = "enero";
                    break;
                case "02":
                    mes = "febrero";
                    break;
                case "03":
                    mes = "marzo";
                    break;
                case "04":
                    mes = "abril";
                    break;
                case "05":
                    mes = "mayo";
                    break;
                case "06":
                    mes = "junio";
                    break;
                case "07":
                    mes = "julio";
                    break;
                case "08":
                    mes = "agosto";
                    break;
                case "09":
                    mes = "septiembre";
                    break;
                case "10":
                    mes = "octubre";
                    break;
                case "11":
                    mes = "noviembre";
                    break;
                case "12":
                    mes = "diciembre";
                    break;
            }


            return mes;

        }

        public string imgOviEspa(string enlace)
        {
            string imagen = null;
            string contenido = null;
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(enlace);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                contenido = sr.ReadToEnd();
                sr.Close();
                myResponse.Close();

            }
            catch (Exception ex)
            {
            }

            var parser = new HtmlParser();
            var document = parser.Parse(contenido);
            imagen = "https://www.oviespana.com" + document.GetElementsByClassName("itemBody")[0].Children[0].GetElementsByTagName("img")[0].GetAttribute("src").ToString();
            return imagen;

        }

        public void GuardaPrecio(Precio p)
        {
            DateTime date1 = new DateTime(p.año, p.numMes, p.dia);
            DateTime date2 = new DateTime(2000, 1, 1);
            Precio pUlt = new Precio();
            var ultimoP = elasticClient.Search<Precio>(s => s
                                                        .Index("agroesi")
                                                        .Type("precio")
                                                        .Size(1)
                                                        .Sort(ss => ss.Descending(not => not.fecha))
                                                        .Query(q => q.Term(not => not.codigo, "SJ".ToLower())));

            foreach (var hit in ultimoP.Hits)
            {
                date2 = new DateTime(hit.Source.año, hit.Source.numMes, hit.Source.dia);
                
                pUlt.precio = hit.Source.precio;
            }

            if (pUlt.precio!=p.precio & date1 >= date2)
            {
                elasticClient.Index(p, es => es
                                       .Index("agroesi")
                                       .Type("precio")
                          );
            }
        }
        public void GuardaNoticiaAG(Noticia n)
        {
            DateTime date1 = new DateTime(n.año, n.numMes, n.dia);
            List<string> titulos = new List<string>();
            DateTime date2 = new DateTime(2000, 1, 1);
            var ultimas_notis = elasticClient.Search<Noticia>(s => s
                                                        .Index("rss")
                                                        .Type("noticia")
                                                        .Size(30)
                                                        .Sort(ss => ss.Descending(not => not.fecha))
                                                        .Query(q => q.Term(not => not.codigo, "AG".ToLower())));

            foreach(var hit in ultimas_notis.Hits)
            {
                titulos.Add(hit.Source.titulo);
                date2 = new DateTime(hit.Source.año, hit.Source.numMes, hit.Source.dia);
            }

            if (!titulos.Contains(n.titulo) & date1>=date2)
            {
                    elasticClient.Index(n, es => es
                                           .Index("rss")
                                           .Type("noticia")
                              );
            }

            
         

        }
        public void GuardaNoticiaGN(Noticia n)
        {
            DateTime date1 = new DateTime(n.año, n.numMes, n.dia);
            List<string> titulos = new List<string>();
            DateTime date2 = new DateTime(2000, 1, 1);
            var ultimas_notis = elasticClient.Search<Noticia>(s => s
                                                        .Index("rss")
                                                        .Type("noticia")
                                                        .Size(20)
                                                        .Sort(ss => ss.Descending(not => not.fecha))
                                                        .Query(q => q.Term(not => not.codigo, "GN".ToLower())));

            foreach (var hit in ultimas_notis.Hits)
            {
                titulos.Add(hit.Source.titulo);
                date2 = new DateTime(hit.Source.año, hit.Source.numMes, hit.Source.dia);
            }

            if (!titulos.Contains(n.titulo) & date1 >= date2)
            {
                elasticClient.Index(n, es => es
                                       .Index("rss")
                                       .Type("noticia")
                          );
            }




        }

    }
}
