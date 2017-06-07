using AngleSharp.Parser.Html;
using Nest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CrawlerX
{
    class Extractor
    {
        ElasticClient elasticClient = Broker.EsClient();

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
        public void ExtraeNoticias()
        {
            foreach (string file in Directory.EnumerateFiles("C:/Users/Miguel Angel/Documents/Datos/RSS/", "*.html"))
            {
                string contents = File.ReadAllText(file, System.Text.Encoding.UTF8);
                var parser = new HtmlParser();
                var document = parser.Parse(contents);

                var noticias = document.GetElementsByClassName("itemContainer itemContainerLast");
                foreach (var noticia in noticias)
                {
                    Noticia n = new Noticia();
                    List<string> etiquetas = new List<string>();
                    var titulo = noticia.GetElementsByClassName("catItemTitle")[0].TextContent.Trim(' ');
                    var links = noticia.GetElementsByTagName("a");
                    var link = links[0];
                    var enlace = "https://www.oviespana.com" + link.GetAttribute("href");
                    string imagen = "Imagen no disponible";
                    string[] etiquetas_aux = new string[4];
                    if (noticia.GetElementsByClassName("catItemImage").Length>0)
                    {
                        imagen = "https://www.oviespana.com" + noticia.GetElementsByClassName("catItemImage")[0].GetElementsByTagName("img")[0].GetAttribute("src").ToString();
                    }
                    string descripcion = noticia.GetElementsByClassName("catItemIntroText")[0].TextContent;
                    var fecha = noticia.GetElementsByClassName("catItemDateCreated")[0].TextContent.Split(',')[1].Trim(' ').Split(' ');
                    if (noticia.GetElementsByClassName("catItemTags").Length>0)
                    {
                        etiquetas_aux = noticia.GetElementsByClassName("catItemTags")[0].TextContent.Trim(' ').Split(' ');
                    }else
                    {
                        etiquetas_aux[0] = "otro";
                        etiquetas_aux[1] = "otro";
                    }
                    
                    foreach (string etiqueta in etiquetas_aux)
                    {
                        if (etiqueta != "" && etiqueta!=null)
                        {
                            etiquetas.Add(etiqueta.ToLower());
                        }

                    }
                    int dia = System.Convert.ToInt16(fecha[0]);
                    string nom_mes = fecha[1].ToLower();
                    int año = System.Convert.ToInt16(fecha[2]);

                    string date = dia + "-" +ConvierteMes(nom_mes)+"-"+año;

                    n.titulo = titulo;
                    n.link = enlace;
                    n.descripcion = descripcion;
                    n.imagen = imagen;
                    n.dia = dia;
                    n.nomMes = nom_mes;
                    n.numMes = System.Convert.ToInt16(ConvierteMes(nom_mes));
                    n.año = año;
                    n.fecha = date;
                    EtiquetaNoticias(etiquetas, n);
                    GuardaNoticia(n);
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
                string [] partes_semana = partes[1].Trim(' ').Split(' ');
                int n_semana = System.Convert.ToInt16(partes_semana[1]);
                string[] comp_fecha = partes_fecha[3].Split('/');
                fecha = comp_fecha[0]+"-" +comp_fecha[1] + "-" + comp_fecha[2];
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
                        CreaPrecio(codigo, celdas[3].TextContent,celdas[4].TextContent, celdas[5].TextContent.Trim(' '), fechas[contador],num_semana[contador]);

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
        public void EtiquetaNoticias(List<string> etiquetas, Noticia n)
        {
            bool fin = false;
            while (!fin)
            {
                if (!etiquetas.Contains("ovino") && etiquetas.Contains("caprino"))
                {
                    n.categoria1 = "caprino";
                }
                if (!etiquetas.Contains("caprino") && etiquetas.Contains("ovino"))
                {
                    n.categoria1 = "ovino";
                }
                if (etiquetas.Contains("caprino") && etiquetas.Contains("ovino"))
                {
                    n.categoria1 = "ovinocaprino";
                }
                if (!etiquetas.Contains("caprino") && !etiquetas.Contains("ovino"))
                {
                    n.categoria1 = "otro";
                }
                if (etiquetas.Contains("precios"))
                {
                    n.categoria2 = "precios";
                }
                if (etiquetas.Contains("produccion"))
                {
                    n.categoria2 = "produccion";
                }
                if (etiquetas.Contains("leche"))
                {
                    n.categoria2 = "leche";
                }
                if (etiquetas.Contains("importaciones"))
                {
                    n.categoria2 = "importaciones";
                }
                if (etiquetas.Contains("exportaciones"))
                {
                    n.categoria2 = "exportaciones";
                }
                if (etiquetas.Contains("carne"))
                {
                    n.categoria2 = "carne";
                }
                if (n.categoria1 != null && n.categoria2 != null)
                {
                    fin = true;
                }
                if (n.categoria2 == null)
                {
                    n.categoria2 = "otro";
                }

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
                                            .Index("agroesi")
                                           .Type("noticia")
                              );
        }
    }
}
