using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Nest;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CrawlerX
{
    class PDFExtractor
    {
        ElasticClient elasticClient = Broker.EsClient();
        public PDFExtractor()
        {

        }
        public void ReadPdfFile()
        {
            int id = 26015;
            
            foreach (string file in Directory.EnumerateFiles("C:/Users/Miguel Angel/Documents/Datos/Lonjas/LonjaTalavera", "*.pdf"))
            {
                StringBuilder text = new StringBuilder();
                PdfReader pdfReader = new PdfReader(file);
                int contador = 0;              
                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    if (contador == 0)
                    {
                        ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                        string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                        currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                        text.Append(currentText);
                    }
                    contador++;
                    
                }
                pdfReader.Close();
                string[] partes_fecha = file.Split('_');
                string[] partes_año = partes_fecha[1].Split('.');
                string campaña = partes_año[0];
                string tabla = text.ToString();
                string[] filas = tabla.Split('\n');
                for (int i = 0; i < filas.Length; i++)
                {
                    if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("kg"))
                    {
                        string[] subpartes = filas[i].Split('.');
                        string producto = subpartes[0];
                        string[] subproductos = producto.Split(' ');
                        string nombre = subproductos[0];
                        string variedad = null;
                        for (int j = 1; j <= subproductos.Length - 1; j++)
                        {
                            variedad = variedad + " " + subproductos[j];
                        }
                        variedad = variedad.Trim(' ');
                        List<double> precios = new List<double>();
                        precios = extrae_precios(subpartes[1]);
                        for (int l=0;l<precios.Count;l++)
                        {
                            Precio p = new Precio();
                            p.id = id;
                            p.producto = producto;
                            p.variedad = variedad;
                            p.precio = precios[l];
                            p.campaña = campaña;
                            p.fecha = "Mes " + (l + 1);
                            actualiza_precio(p);
                            guarda_precio(p);
                            id++;
                        }


                    }
                    if (filas[i].ToLower().Contains("cabrito") && filas[i].ToLower().Contains("lechal"))
                    {
                        string[] subpartes = filas[i].Split(' ');
                        string producto = subpartes[0];
                        string variedad = subpartes[1];
                        List<double> precios = new List<double>();
                        for (int j = 2; j < subpartes.Length; j++)
                        {
                            if (subpartes[j]!="")
                            {
                                precios.Add(double.Parse(subpartes[j]));
                            }

                        }
                        for (int l = 0; l < precios.Count; l++)
                        {
                            Precio p = new Precio();
                            p.id = id;
                            p.producto = producto;
                            p.variedad = variedad;
                            p.precio = precios[l];
                            p.campaña = campaña;
                            p.fecha = "Mes " + (l + 1);
                            actualiza_precio(p);
                            guarda_precio(p);
                            id++;
                        }

                    }
                    if ((filas[i].ToLower().Contains("ternero")|| filas[i].ToLower().Contains("ternera")) && filas[i].ToLower().Contains("base"))
                    {
                        string[] subproductos = filas[i].ToLower().Split(' ');
                        string nombre = subproductos[0];
                        string variedad = null;
                        for (int j = 1; j <= 5; j++)
                        {
                            variedad = variedad + " " + subproductos[j];
                        }
                        variedad = variedad.Trim(' ');
                        List<double> precios = new List<double>();
                        for (int j = 6; j < subproductos.Length; j++)
                        {
                            if (subproductos[j] != "")
                            {
                                precios.Add(double.Parse(subproductos[j]));
                            }

                        }
                        for (int l = 0; l < precios.Count; l++)
                        {
                            Precio p = new Precio();
                            p.id = id;
                            p.producto = nombre;
                            p.variedad = variedad;
                            p.precio = precios[l];
                            p.campaña = campaña;
                            p.fecha = "Mes " + (l + 1);
                            actualiza_precio(p);
                            guarda_precio(p);
                            id++;
                        }

                    }

                }

           }


        }
        public List<double> extrae_precios(string precios_fila)
        {
            List<double> precios = new List<double>();
            precios_fila.Trim(' ');
            string[] partes = precios_fila.Split(' ');
            for (int i = 1; i < partes.Length; i++)
            {
                if (partes[i]!="")
                {
                    precios.Add(double.Parse(partes[i]));
                }
                
            }
            return precios;

        }
        public void actualiza_precio(Precio p)
        {
            p.posicion_comercial = "Lonja Talavera";
            p.localizacion = "Talavera de la Reina";
            p.fuente = "Lonja Talavera";
            p.moneda = "Euro";
            p.unidad_precio = "Euro/kg";
            p.tipo_precio = "Precio medio";
        }
        public void guarda_precio(Precio p)
        {
            elasticClient.Index(p, es => es
                                                    .Index("agroesi")
                                                    .Type("precio")
                                                    .Id(p.id)
                                        );
        }
    }
}
