using AngleSharp.Parser.Html;
using Nest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;

namespace CrawlerX
{
    class Extractor
    {
        ElasticClient elasticClient = Broker.EsClient();
        char[] MyChar = { '\n', '\r', ' ', '\t', '.', ',', 'y' };
        public Extractor()
        {

        }
        public void extraer_datos()
        {
            Hashtable productos = new Hashtable();
            string producto = null;
            string variedad = null;
            string unidad_precio = null;
            string posicion_comercial = null;
            string codigo = null;
            int id = 1;
            char[] MyChar = { '\n', '\r', ' ' };
            List<string> campanias = new List<string>();
            List<string> semanas = new List<string>();
            List<List<double>> precios = new List<List<double>>();


            foreach (string file in Directory.EnumerateFiles("C:/Users/Miguel Angel/Documents/Datos/", "*.html"))
            {
                string contents = File.ReadAllText(file, System.Text.Encoding.UTF7);
                string nombre = file.Substring(38);
                int contador = 0;
                var parser = new HtmlParser();

                var document = parser.Parse(contents);

                var itemsCSS = document.GetElementsByTagName("tr");
                if (nombre == "historico0.html")
                {
                    foreach (var item in itemsCSS)
                    {
                        contador++;
                        if (contador >= 3)
                        {
                            Precio p = new Precio();
                            var celdas = item.GetElementsByTagName("td");
                            int posicion = 0;
                            foreach (var celda in celdas)
                            {
                                switch (posicion)
                                {
                                    case 1:
                                        var codigo_aux = celda.GetElementsByTagName("a");
                                        foreach (var code in codigo_aux)
                                        {
                                            codigo = code.GetAttribute("href");
                                            codigo = codigo.Substring(25);
                                        }
                                        var datos = celda.GetElementsByTagName("span");
                                        int j = 0;
                                        foreach (var dato in datos)
                                        {
                                            if (j == 0)
                                            {
                                                producto = dato.TextContent;
                                                producto = producto.Trim(MyChar);
                                                if (producto.Contains(" "))
                                                {
                                                    string[] partes = producto.Split(' ');
                                                    producto = partes[0];
                                                    for (int i = 1; i <= partes.Length - 1; i++)
                                                    {
                                                        variedad = variedad + " " + partes[i];
                                                    }
                                                    variedad = variedad.Trim(MyChar);
                                                }
                                                else
                                                {
                                                    variedad = "No variedad";
                                                }
                                            }
                                            j++;

                                        }
                                        //p.producto = producto;
                                        //p.variedad = variedad;
                                        variedad = null;
                                        break;

                                    case 2:
                                        unidad_precio = celda.TextContent;
                                        unidad_precio = unidad_precio.Trim(MyChar);
                                        //p.unidad_precio = unidad_precio;
                                        break;
                                    case 3:
                                        posicion_comercial = celda.TextContent;
                                        posicion_comercial = posicion_comercial.Trim(MyChar);
                                        //p.posicion_comercial = posicion_comercial;
                                        productos.Add(codigo, p);
                                        break;
                                }

                                posicion++;
                            }

                        }

                    }
                }
                else
                {
                    contador = 0;
                    string aux = file.Substring(38);
                    string[] partes = aux.Split('.');
                    string key = partes[0];
                    key = key.Substring(9);
                    foreach (var item in itemsCSS)
                    {
                        contador++;

                        if (contador > 4)
                        {
                            var celdas = item.GetElementsByTagName("td");
                            int index = 0;
                            String semana = null;
                            List<double> valores = new List<double>();
                            foreach (var celda in celdas)
                            {
                                if (index == 1)
                                {
                                    semana = celda.TextContent;
                                    semana = semana.Trim(MyChar);

                                }
                                if (index > 1)
                                {
                                    String dato = celda.TextContent;
                                    dato = dato.Trim(MyChar);
                                    if (dato != "")
                                    {
                                        valores.Add(Double.Parse(dato));
                                    }

                                }

                                index++;
                                if (index == 5)
                                {
                                    index = 0;
                                    int i = 0;
                                    precios.Add(valores);
                                    foreach (var precio in valores)
                                    {
                                        Precio p = (Precio)productos[key];
                                        p.precio = precio;
                                        //p.fecha = semana;
                                        // p.id = id;
                                        //p.localizacion = "España";
                                        // p.moneda = "Euro";
                                        p.fuente = "Gobierno";
                                        p.tipoPrecio = "Precio medio";
                                        //p.campaña = campañas(i);
                                        elasticClient.Index(p, es => es
                                                    .Index("agroesi")
                                                    .Type("precio")
                                        //.Id(p.id)
                                        );
                                        id++;
                                        i++;
                                    }

                                }
                            }

                        }
                    }

                }


            }

        }
        public void NoticiasGanaderia()
        {
            List<string> titulos = new List<string>();
            foreach (string file in Directory.EnumerateFiles("C:/Users/Miguel Angel/Documents/Datos/RSS/Ganaderia/", "*.html"))
            {
                string contents = File.ReadAllText(file, System.Text.Encoding.UTF8);
                var parser = new HtmlParser();
                var document = parser.Parse(contents);
                var noticias = document.GetElementsByClassName("itemContainer itemContainerLast");

                foreach (var noticia in noticias)
                {
                    Noticia n = new Noticia();
                    string etiquetas = "";
                    var titulo = noticia.GetElementsByClassName("catItemTitle")[0].TextContent.Trim(' ');
                    var links = noticia.GetElementsByTagName("a");
                    var link = links[0];
                    var enlace = "https://www.oviespana.com" + link.GetAttribute("href");
                    string imagen = "Imagen no disponible";
                    string[] etiquetas_aux = new string[4];
                    if (noticia.GetElementsByClassName("catItemImage").Length > 0)
                    {
                        imagen = "https://www.oviespana.com" + noticia.GetElementsByClassName("catItemImage")[0].GetElementsByTagName("img")[0].GetAttribute("src").ToString();
                    }
                    string descripcion = noticia.GetElementsByClassName("catItemIntroText")[0].TextContent;
                    var fecha = noticia.GetElementsByClassName("catItemDateCreated")[0].TextContent.Split(',')[1].Trim(' ').Split(' ');
                    if (noticia.GetElementsByClassName("catItemTags").Length > 0)
                    {
                        etiquetas_aux = noticia.GetElementsByClassName("catItemTags")[0].TextContent.Trim(' ').Split(' ');
                    }
                    else
                    {

                        string content = extraecontenido(enlace);
                        etiquetas = EtiquetasOviEspa(content);
                    }

                    foreach (string etiqueta in etiquetas_aux)
                    {
                        if (etiqueta != "" && etiqueta != null)
                        {
                            etiquetas = etiquetas + etiqueta.ToLower() + ",";
                        }

                    }
                    etiquetas = etiquetas.Trim(MyChar);
                    int dia = System.Convert.ToInt16(fecha[0]);
                    string nom_mes = fecha[1].ToLower();
                    int año = System.Convert.ToInt16(fecha[2]);
                    string date = fecha[0] + "-" + ConvierteMes(nom_mes) + "-" + año;
                    if (etiquetas!="")
                    {
                        n.codigo = "GN";
                        n.titulo = titulo;
                        n.link = enlace;
                        n.descripcion = descripcion;
                        n.imagen = imagen;
                        n.dia = dia;
                        n.nomMes = nom_mes;
                        n.numMes = System.Convert.ToInt16(ConvierteMes(nom_mes));
                        n.año = año;
                        n.fecha = date;
                        n.etiquetas = etiquetas;
                        if (!titulos.Contains(n.titulo))
                        {
                            GuardaNoticia(n);
                            titulos.Add(n.titulo);
                        }
                    }
                    

                }

            }
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
        public void LonjaAlbacete()
        {
            string contents = File.ReadAllText("C:/Users/Miguel Angel/Documents/Datos/Lonjas/LonjaAlbacete/Albacete.html", System.Text.Encoding.UTF8);
            var parser = new HtmlParser();
            var document = parser.Parse(contents);

            var semanas = document.GetElementsByTagName("h3");
            var tablas = document.GetElementsByClassName("datos");

            List<string> fechas = new List<string>();
            List<int> num_semana = new List<int>();
            List<Precio> precios = new List<Precio>();
            string fecha = null;
            int contador = 0;

            foreach (var semana in semanas)
            {
                fecha = semana.TextContent;
                string[] partes = fecha.Split('.');
                string[] partes_fecha = partes[0].Split(' ');
                string[] partes_semana = partes[1].Trim(' ').Split(' ');
                int n_semana = System.Convert.ToInt16(partes_semana[1]);
                string[] comp_fecha = partes_fecha[3].Split('/');
                fecha = comp_fecha[0] + "-" + comp_fecha[1] + "-" + comp_fecha[2];
                fechas.Add(fecha);
                num_semana.Add(n_semana);

            }
            foreach (var tabla in tablas)
            {
                var filas = tabla.GetElementsByTagName("tr");
                foreach (var fila in filas)
                {
                    var celdas = fila.GetElementsByTagName("td");
                    if (celdas[1].TextContent.ToLower().Contains("leche") && celdas[1].TextContent.ToLower().Contains("cabra"))
                    {

                        string codigo = "LC";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("leche") && celdas[1].TextContent.ToLower().Contains("oveja") && celdas[1].TextContent.ToLower().Contains("con"))
                    {

                        string codigo = "LOCDO";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("leche") && celdas[1].TextContent.ToLower().Contains("oveja") && celdas[1].TextContent.ToLower().Contains("sin"))
                    {

                        string codigo = "LOSDO";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cabrito") && celdas[2].TextContent.ToLower().Contains("basto"))
                    {

                        string codigo = "C7B10";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cabrito") && celdas[2].TextContent.ToLower().Contains("fino"))
                    {

                        string codigo = "C7F9";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("con") && celdas[2].TextContent.ToLower().Contains("10.5"))
                    {

                        string codigo = "CM10CI15";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("con") && celdas[2].TextContent.ToLower().Contains("15.1"))
                    {

                        string codigo = "CM15CI19";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("con") && celdas[2].TextContent.ToLower().Contains("19.1"))
                    {

                        string codigo = "CM19CI23";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("con") && celdas[2].TextContent.ToLower().Contains("23.1"))
                    {

                        string codigo = "CM23CI25";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("con") && celdas[2].TextContent.ToLower().Contains("25.5"))
                    {

                        string codigo = "CM25CI28";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("con") && celdas[2].TextContent.ToLower().Contains("28.1"))
                    {
                        string codigo = "CM28CI34";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("con") && celdas[2].TextContent.ToLower().Contains("media"))
                    {

                        string codigo = "CMCIM10";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("sin") && celdas[2].TextContent.ToLower().Contains("10.5"))
                    {

                        string codigo = "CSI10YO15";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("sin") && celdas[2].TextContent.ToLower().Contains("15.1"))
                    {

                        string codigo = "CSI15YO19";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("sin") && celdas[2].TextContent.ToLower().Contains("19.1"))
                    {

                        string codigo = "CSI19YO23";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("sin") && celdas[2].TextContent.ToLower().Contains("23.1"))
                    {

                        string codigo = "CSI23YO25";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("sin") && celdas[2].TextContent.ToLower().Contains("25.1"))
                    {

                        string codigo = "CSI25YO28";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("sin") && celdas[2].TextContent.ToLower().Contains("28.1"))
                    {

                        string codigo = "CSI28YO34";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }
                    if (celdas[1].TextContent.ToLower().Contains("cordero") && celdas[1].TextContent.ToLower().Contains("sin") && celdas[2].TextContent.ToLower().Contains("media"))
                    {

                        string codigo = "CSIYOM10";
                        CreaPrecio(codigo, celdas[3].TextContent, celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador], num_semana[contador]);

                    }

                }
                contador++;
            }


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
        public void CreaPrecio(string code, string min, string max, string medida, string fecha, int semana)
        {
            string[] datos = fecha.Split('-');
            Precio Pmin = new Precio();
            Pmin.codigo = code;
            Pmin.precio = double.Parse(min);
            Pmin.dia = System.Convert.ToInt16(datos[0]);
            Pmin.semana = semana;
            Pmin.numMes = System.Convert.ToInt16(datos[1]);
            Pmin.nomMes = ConvierteMesInv(datos[1]);
            Pmin.año = System.Convert.ToInt16(datos[2]);
            Pmin.fecha = fecha;
            Pmin.tipoPrecio = "min";
            Pmin.fuente = "Albacete";
            Pmin.medida = medida;
            Precio Pmax = new Precio();
            Pmax.codigo = code;
            Pmax.precio = double.Parse(max);
            Pmax.dia = System.Convert.ToInt16(datos[0]);
            Pmax.semana = semana;
            Pmax.numMes = System.Convert.ToInt16(datos[1]);
            Pmax.nomMes = ConvierteMesInv(datos[1]);
            Pmax.año = System.Convert.ToInt16(datos[2]);
            Pmax.fecha = fecha;
            Pmax.tipoPrecio = "max";
            Pmax.fuente = "Albacete";
            Pmax.medida = medida;
            GuardaPrecio(Pmin);
            GuardaPrecio(Pmax);

        }
        public void GuardaPrecio(Precio p)
        {
            elasticClient.Index(p, es => es
                                 .Index("agroesi")
                                 .Type("precio")
                              );
        }
        public void GuardaNoticia(Noticia n)
        {
            elasticClient.Index(n, es => es
                                            .Index("rss")
                                           .Type("noticia")
                              );
        }
        public void BalanceArroz()
        {
            string contenido = File.ReadAllText("C:/Users/Miguel Angel/Documents/Datos/Mapama/Arroz/BalanceArroz.txt");
            string[] filas = contenido.Split('\n');
            for (int i = 1; i < 7; i++)
            {
                string[] datos = filas[i].Trim('\r').Split(',');
                string campaña = datos[0];
                string code = "AZ";
                double superficie = double.Parse(datos[1]);
                double produccion = double.Parse(datos[2]);
                double importaciones = double.Parse(datos[3]);
                double exportaciones = double.Parse(datos[4]);
                Estadistica e = new Estadistica();
                e.campaña = campaña;
                e.codigo = code;
                e.superficie = superficie;
                e.produccion = produccion;
                e.importacines = importaciones;
                e.exportaciones = exportaciones;
                GuardaEstadistica(e);
            }
        }
        public void GuardaEstadistica(Estadistica e)
        {
            elasticClient.Index(e, es => es
                                            .Index("cereales")
                                           .Type("estadistica")
                              );
        }
        public void Soja()
        {
            string contents = File.ReadAllText("C:/Users/Miguel Angel/Documents/Datos/Mapama/Soja/Soja.html", System.Text.Encoding.UTF8);
            var parser = new HtmlParser();
            var document = parser.Parse(contents);

            var tabla = document.GetElementsByClassName("tblData");
            var filas = tabla[0].GetElementsByTagName("tr");

            int dia = 15;
            string code = "SJ";
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            for (int i = 1; i < 122; i++)
            {
                var celdas = filas[i].Children;
                string mes = celdas[0].TextContent;
                double precio = double.Parse(celdas[1].TextContent);

                string[] parFecha = mes.Split('.');
                string nom_mes = CompletaMes(parFecha[0]);
                int num_mes = System.Convert.ToInt16(ConvierteMes(nom_mes));
                int año = System.Convert.ToInt16(parFecha[1].Trim());
                string fecha = dia + "-" + ConvierteMes(nom_mes) + "-" + año.ToString().Substring(2);

                DateTime date = new DateTime(año, num_mes, dia);
                int semana = System.Convert.ToInt16(cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek).ToString());
                PrecioSoja(code, precio, dia, nom_mes, num_mes, año, fecha, semana);


            }
        }
        public string CompletaMes(string dato)
        {
            string mes = null;
            switch (dato)
            {
                case "ene":
                    mes = "enero";
                    break;
                case "feb":
                    mes = "febrero";
                    break;
                case "mar":
                    mes = "marzo";
                    break;
                case "abr":
                    mes = "abril";
                    break;
                case "may":
                    mes = "mayo";
                    break;
                case "jun":
                    mes = "junio";
                    break;
                case "jul":
                    mes = "julio";
                    break;
                case "ago":
                    mes = "agosto";
                    break;
                case "sep":
                    mes = "septiembre";
                    break;
                case "oct":
                    mes = "octubre";
                    break;
                case "nov":
                    mes = "noviembre";
                    break;
                case "dic":
                    mes = "diciembre";
                    break;
            }


            return mes;

        }
        public void PrecioSoja(string code, double precio, int dia, string nomMes, int numMes, int año, string fecha, int semana)
        {
            Precio p = new Precio();
            p.codigo = code;
            p.precio = precio;
            p.dia = dia;
            p.semana = semana;
            p.numMes = numMes;
            p.nomMes = nomMes;
            p.año = año;
            p.fecha = fecha;
            p.tipoPrecio = "max";
            p.fuente = "Chicago";
            p.medida = "Euros/t";
            GuardaPrecio(p);

        }
        public void NoticiasAgricultura()
        {
            foreach (string file in Directory.EnumerateFiles("C:/Users/Miguel Angel/Documents/Datos/RSS/Agricultura/", "*.html"))
            {
                string contents = File.ReadAllText(file, System.Text.Encoding.UTF8);
                var parser = new HtmlParser();
                var document = parser.Parse(contents);

                if (file.Contains("agroinfo"))
                {
                    var noticias = document.GetElementsByClassName("td-block-span6");
                    foreach (var noticia in noticias)
                    {
                        Noticia n = new Noticia();
                        var imagen = noticia.GetElementsByClassName("td-module-image");
                        var titulo = noticia.GetElementsByClassName("entry-title td-module-title");
                        var meta = noticia.GetElementsByClassName("td-module-meta-info");
                        var descripcion = noticia.GetElementsByClassName("td-excerpt");

                        string tit = titulo[0].TextContent;
                        string link = titulo[0].FirstElementChild.GetAttribute("href");
                        string linkImagen = imagen[0].GetElementsByTagName("img")[0].GetAttribute("src").ToString();
                        string descrip = descripcion[0].TextContent.Trim(MyChar);
                        string fecha = meta[0].GetElementsByClassName("td-post-date")[0].TextContent;

                        string[] parFecha = fecha.Split('/');
                        int dia = System.Convert.ToInt16(parFecha[0]);
                        int mes = System.Convert.ToInt16(parFecha[1]);
                        int año = System.Convert.ToInt16(parFecha[2]);
                        string nomMes = ConvierteMesInv(parFecha[1]);
                        string contenido = extraecontenido(link);
                        string etiquetas = EtiquetasAgroinfo(contenido);
                        n.codigo = "AG";
                        n.titulo = tit;
                        n.link = link;
                        n.descripcion = descrip;
                        n.etiquetas = etiquetas;
                        n.imagen = linkImagen;
                        n.dia = dia;
                        n.numMes = mes;
                        n.nomMes = nomMes;
                        n.año = año;
                        n.fecha = parFecha[0] + "-" + parFecha[1] + "-" + parFecha[2];
                        GuardaNoticia(n);


                    }

                }
                else
                {
                    var noticias = document.GetElementsByClassName("block igualar");
                    foreach (var noticia in noticias)
                    {
                        Noticia n = new Noticia();
                        var fecha = noticia.GetElementsByClassName("entry-header");
                        var imagen = noticia.GetElementsByClassName("img");
                        var titulo = noticia.GetElementsByClassName("entry-title");
                        string date = fecha[0].GetElementsByClassName("header-top")[0].TextContent.ToString().Trim(MyChar).Split('\n')[0];
                        string link = titulo[0].FirstElementChild.GetAttribute("href");
                        string linkImagen = imagen[0].GetElementsByTagName("img")[0].GetAttribute("src").ToString();

                        string[] parFecha = date.Split(' ');
                        int dia = System.Convert.ToInt16(parFecha[0]);
                        int nummes = System.Convert.ToInt16(ConvierteMes(parFecha[1]));
                        int año = System.Convert.ToInt16(parFecha[2]);
                        string nomMes = parFecha[1];

                        string contenido = extraecontenido(link);
                        string etiquetas = EtiquetasEfeagro(contenido);

                        n.codigo = "AG";
                        n.titulo = titulo[0].TextContent;
                        n.link = link;
                        n.descripcion = "";
                        n.etiquetas = etiquetas;
                        n.imagen = linkImagen;
                        n.dia = dia;
                        n.numMes = nummes;
                        n.nomMes = nomMes;
                        n.año = año;
                        n.fecha = parFecha[0] + "-" + ConvierteMes(parFecha[1]) + "-" + parFecha[2];
                        GuardaNoticia(n);

                    }

                }
            }

        }
        public string EtiquetasAgroinfo(string contenido)
        {
            string etiquetas = "";
            string keys = File.ReadAllText(@"C:\Users\Miguel Angel\Desktop\keywords.txt", System.Text.Encoding.UTF7);
            string conjuntores = File.ReadAllText(@"C:\Users\Miguel Angel\Desktop\preposiciones.txt", System.Text.Encoding.UTF7);
            var parser = new HtmlParser();
            var document = parser.Parse(contenido);
            var content = document.GetElementsByClassName("td-post-content");
            string content1 = content[0].TextContent.Trim(MyChar);
            string[] palabras = content1.Split();
            int contador = 0;
            for (int i = 0; i < palabras.Length; i++)
            {
               
                if (palabras[i].Trim(MyChar).ToLower() != "" || palabras[i].Trim(MyChar).ToLower() != " ")
                {
                    if (keys.Contains(palabras[i].Trim(MyChar).ToLower()) & contador < 4 & !etiquetas.Contains(palabras[i].ToLower()) & !conjuntores.ToLower().Contains(palabras[i].ToLower()))
                    {
                        etiquetas = etiquetas + palabras[i].ToLower().Trim(MyChar) + ",";
                        contador++;
                    }
                }
            }
            etiquetas = etiquetas.Trim(MyChar);
            etiquetas = etiquetas.Trim(MyChar);
            return etiquetas;
        }
        public string EtiquetasEfeagro(string contenido)
        {
            string etiquetas = "";
            string keys = File.ReadAllText(@"C:\Users\Miguel Angel\Desktop\keywords.txt", System.Text.Encoding.UTF7);
            string conjuntores = File.ReadAllText(@"C:\Users\Miguel Angel\Desktop\preposiciones.txt", System.Text.Encoding.UTF7);
            var parser = new HtmlParser();
            var document = parser.Parse(contenido);
            var content = document.GetElementsByClassName("post-entry");
            string content1 = content[0].TextContent.Trim(MyChar);

            etiquetas = document.GetElementsByClassName("tags")[0].TextContent.Trim(MyChar);
            etiquetas = document.GetElementsByClassName("tags")[0].TextContent.Trim(MyChar);

            string[] parEtiquetas = etiquetas.Split('\n');
            if (parEtiquetas.Length != 1)
            {
                etiquetas = parEtiquetas[0].Substring(15).Trim(MyChar);
            }
            else
            {
                int contador = 0;
                string[] palabras = content1.Split();
                for (int i = 0; i < palabras.Length; i++)
                {

                    if (palabras[i].Trim(MyChar).ToLower() != "" || palabras[i].Trim(MyChar).ToLower() != " ")
                    {
                        if (keys.Contains(palabras[i].Trim(MyChar).ToLower()) & contador < 4 & !etiquetas.Contains(palabras[i].ToLower()) & !conjuntores.ToLower().Contains(palabras[i].ToLower()))
                        {
                            etiquetas = etiquetas + palabras[i].ToLower().Trim(MyChar) + ",";
                            contador++;
                        }
                    }
                }
                if (etiquetas.Length > 25)
                {
                    etiquetas = etiquetas.Substring(25);
                }

            }

            etiquetas = etiquetas.Trim(MyChar);
            etiquetas = etiquetas.Trim(MyChar);
            return etiquetas;
        }
        public string EtiquetasOviEspa(string contenido)
        {
            string etiquetas = "";
            if (contenido != null)
            {
                string keys = File.ReadAllText(@"C:\Users\Miguel Angel\Desktop\keyganaderia.txt", System.Text.Encoding.UTF7);
                string conjuntores = File.ReadAllText(@"C:\Users\Miguel Angel\Desktop\preposiciones.txt", System.Text.Encoding.UTF7);

                var parser = new HtmlParser();
                var document = parser.Parse(contenido);
                var content = document.GetElementsByClassName("itemFullText");

                string content1 = content[0].TextContent.Trim(MyChar);
                string[] palabras = contenido.Split();
                int contador = 0;
                for (int i = 0; i < palabras.Length; i++)
                {

                    if (palabras[i].Trim(MyChar).ToLower() != "" || palabras[i].Trim(MyChar).ToLower() != " ")
                    {
                        if (keys.Contains(palabras[i].Trim(MyChar).ToLower()) & contador < 4 & !etiquetas.Contains(palabras[i].ToLower()) & !conjuntores.ToLower().Contains(palabras[i].ToLower()))
                        {
                            if (!etiquetas.Contains(palabras[i].ToLower().Trim(MyChar)))
                            {
                                etiquetas = etiquetas + palabras[i].ToLower().Trim(MyChar) + ",";
                                contador++;
                            }

                        }
                    }
                }

                etiquetas = etiquetas.Trim(MyChar);
            }          
            return etiquetas;
        }
        public string extraecontenido(string link)
        {
            string contenido = null; ;                      
            try
            {
                WebRequest req = HttpWebRequest.Create(link);
                req.Method = "GET";
                using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    contenido = reader.ReadToEnd();
                }

            }
            catch (Exception ex) { }
               
            return contenido;
        }
    }
}
        
        

