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
                string contents = File.ReadAllText(file,System.Text.Encoding.UTF7);            
                string nombre = file.Substring(38);
                int contador = 0;
                var parser = new HtmlParser();
                
                var document = parser.Parse(contents);
                
                var itemsCSS = document.GetElementsByTagName("tr");
                if (nombre=="historico0.html")
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
                                                }else
                                                {
                                                    variedad = "No variedad";
                                                }      
                                            }
                                            j++;

                                        }
                                        p.producto = producto;
                                        p.variedad = variedad;
                                        variedad = null;
                                        break;

                                    case 2:
                                        unidad_precio = celda.TextContent;
                                        unidad_precio = unidad_precio.Trim(MyChar);
                                        p.unidad_precio = unidad_precio;
                                        break;
                                    case 3:
                                        posicion_comercial = celda.TextContent;
                                        posicion_comercial = posicion_comercial.Trim(MyChar);
                                        p.posicion_comercial = posicion_comercial;
                                        productos.Add(codigo,p);                                                                              
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
                                    if (dato == "")
                                    {
                                        valores.Add(-1);
                                    }
                                    else
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
                                        p.fecha = semana;
                                        p.id = id;
                                        p.localizacion = "España";
                                        p.moneda = "Euro";
                                        p.fuente = "Gobierno";
                                        p.tipo_precio = "Precio medio";
                                        p.campaña = campañas(i);
                                        elasticClient.Index(p, es => es
                                                    .Index("agroesi")
                                                    .Type("precio")
                                                    .Id(p.id)
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
        public string campañas(int index)
        {
            string campaña = null;
                switch (index)
                {
                    case 0:
                        campaña = "2016/17";
                        break;
                    case 1:
                        campaña = "2015/16";
                        break;
                    case 2:
                        campaña = "2014/15";
                        break;
                }

           

            return campaña;

        }
    }
}
