using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CrawlerX
{
    class PDFExtractor
    {
        ElasticClient elasticClient = Broker.EsClient();
        StringBuilder text = null;
        PdfReader pdfReader = null;
        public void DatosOvino()
        {

            string file = "C:/Users/Miguel Angel/Documents/Datos/Lonjas/LonjaTalavera/Ovino/historico_ovino.pdf";

            text = new StringBuilder();
            pdfReader = new PdfReader(file);
            for (int page = 1; page <= pdfReader.NumberOfPages; page++)
            {

                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                text.Append(currentText);

            }
            pdfReader.Close();
            string tabla = text.ToString();
            string[] filas = tabla.Split('\n');
            for (int i = 0; i < filas.Length; i++)
            {
                string[] partes = null;
                if (filas[i].ToLower().Contains("leche") && filas[i].ToLower().Contains("cabra"))
                {
                    string codigo = "LC";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);



                }
                if (filas[i].ToLower().Contains("leche") && filas[i].ToLower().Contains("oveja") && filas[i].ToLower().Contains("con"))
                {
                    string codigo = "LOCDO";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);


                }
                if (filas[i].ToLower().Contains("leche") && filas[i].ToLower().Contains("oveja") && filas[i].ToLower().Contains("sin") && !filas[i].ToLower().Contains("alfalfa"))
                {
                    string codigo = "LOSDO";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cabrito") && filas[i].ToLower().Contains("basto"))
                {
                    string codigo = "C7B10";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cabrito") && filas[i].ToLower().Contains("fino"))
                {
                    string codigo = "C7F9";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("con") && filas[i].ToLower().Contains("10.5"))
                {
                    string codigo = "CM10CI15";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);


                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("con") && filas[i].ToLower().Contains("15.1"))
                {
                    string codigo = "CM15CI19";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("con") && filas[i].ToLower().Contains("19.1"))
                {
                    string codigo = "CM19CI23";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("con") && filas[i].ToLower().Contains("23.1"))
                {
                    string codigo = "CM23CI25";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("con") && filas[i].ToLower().Contains("25.5"))
                {
                    string codigo = "CM25CI28";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("con") && filas[i].ToLower().Contains("28.1"))
                {
                    string codigo = "CM28CI34";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("con") && filas[i].ToLower().Contains("media"))
                {
                    string codigo = "CMCIM10";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("sin") && filas[i].ToLower().Contains("10.5"))
                {
                    string codigo = "CSI10YO15";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("sin") && filas[i].ToLower().Contains("15.1"))
                {
                    string codigo = "CSI15YO19";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("sin") && filas[i].ToLower().Contains("19.1"))
                {
                    string codigo = "CSI19YO23";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("sin") && filas[i].ToLower().Contains("23.1"))
                {
                    string codigo = "CSI23YO25";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("sin") && filas[i].ToLower().Contains("25.1"))
                {
                    string codigo = "CSI25YO28";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("sin") && filas[i].ToLower().Contains("28.1"))
                {
                    string codigo = "CSI28YO34";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }
                if (filas[i].ToLower().Contains("cordero") && filas[i].ToLower().Contains("sin") && filas[i].ToLower().Contains("media"))
                {
                    string codigo = "CSIYOM10";
                    partes = filas[i].Split(' ');
                    PreciosOvino(partes, codigo);

                }

            }




        }
        public void PreciosOvino(string[] partes, string code)
        {
            int año = 0;
            int num_mes = 0;
            string nom_mes = null;
            int semana = 0;
            int dia = 0;
            string medida = null;
            double max = 0;
            double min = 0;
            string fecha = null;

            Precio p1 = new Precio();
            Precio p2 = new Precio();
            año = System.Convert.ToInt16(partes[partes.Length - 1]);
            num_mes = System.Convert.ToInt16(partes[partes.Length - 2]);
            nom_mes = partes[partes.Length - 3];
            semana = System.Convert.ToInt16(partes[partes.Length - 4]);
            dia = System.Convert.ToInt16(partes[partes.Length - 5]);
            medida = partes[partes.Length - 6];
            max = double.Parse(partes[partes.Length - 7]);
            min = double.Parse(partes[partes.Length - 8]);
            if (dia<10)
            {
                fecha = "0"+dia + "-" + CompruebaMes(num_mes) + "-" + año;
            }
            fecha = dia + "-" + CompruebaMes(num_mes) + "-" + año;
            p1.codigo = code;
            p1.precio = max;
            p1.dia = dia;
            p1.semana = semana;
            p1.numMes = num_mes;
            p1.nomMes = nom_mes;
            p1.año = año;
            p1.fecha = fecha;
            p1.tipoPrecio = "max";
            p1.fuente = "Talavera";
            p1.medida = medida;
            
            p2.codigo = code;
            p2.precio = min;
            p2.dia = dia;            
            p2.semana = semana;
            p2.numMes = num_mes;
            p2.nomMes = nom_mes;
            p2.año = año;
            p2.fecha = fecha;                      
            p2.tipoPrecio = "min";
            p2.fuente = "Talavera";            
            p2.medida = medida;
            partes = null;
            GuardaPrecio(p1);
            GuardaPrecio(p2);


        }
        public void DatosVacuno()
        {
            foreach (string file in Directory.EnumerateFiles("C:/Users/Miguel Angel/Documents/Datos/Lonjas/LonjaTalavera/Vacuno", "*.pdf"))
            {
                text = new StringBuilder();
                pdfReader = new PdfReader(file);
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
                string tabla = text.ToString();
                string[] filas = tabla.Split('\n');
                for (int i = 0; i < filas.Length; i++)
                {
                    if (filas[i].ToLower().Contains("ternero") && filas[i].ToLower().Contains("cruzado") && filas[i].ToLower().Contains("base"))
                    {
                        if (filas[i].ToLower().Contains("1ª"))
                        {
                            string codigo1 = "TO200C1";
                            string[] partes = filas[i].Split(' ');
                            PreciosVacuno(partes, codigo1, file);

                        }
                        else
                        {
                            string codigo2 = "TO200C2";
                            string[] partes = filas[i].Split(' ');
                            PreciosVacuno(partes, codigo2, file);
                        }


                    }
                    if (filas[i].ToLower().Contains("ternera") && filas[i].ToLower().Contains("cruzada") && filas[i].ToLower().Contains("base"))
                    {
                        if (filas[i].ToLower().Contains("1ª"))
                        {
                            string codigo1 = "TA200C1";
                            string[] partes = filas[i].Split(' ');
                            PreciosVacuno(partes, codigo1, file);
                        }
                        else
                        {
                            string codigo2 = "TA200C2";
                            string[] partes = filas[i].Split(' ');
                            PreciosVacuno(partes, codigo2, file);
                        }



                    }
                    if (filas[i].ToLower().Contains("ternero") && filas[i].ToLower().Contains("pais") && filas[i].ToLower().Contains("base"))
                    {
                        string codigo = "TO200PA";
                        string[] partes = filas[i].Split(' ');
                        PreciosVacuno(partes, codigo, file);

                    }
                    if (filas[i].ToLower().Contains("ternera") && filas[i].ToLower().Contains("pais") && filas[i].ToLower().Contains("base"))
                    {
                        string[] partes = filas[i].Split(' ');
                        string codigo = "TA200PA";
                        PreciosVacuno(partes, codigo, file);
                    }
                }
            }
        }
        public void PreciosVacuno(string[] partes, string code, string file)
        {
            string[] partes_nombre = file.Split('_');
            string[] nombre = partes_nombre[1].Split('.');
            int año = System.Convert.ToInt16(nombre[0]);
            int num_mes = 0;
            string nom_mes = null;
            int semana = 0;
            int dia = 25;
            string medida = "Euros/kg";
            string tipo_precio = "med";
            string fecha = null;



            List<double> precios = new List<double>();
            for (int j = 6; j < partes.Length; j++)
            {
                if (partes[j] != "" && partes[j].Contains("."))
                {
                    string[] datos = partes[j].Split('.');
                    string dato = datos[0] + "," + datos[1];
                    precios.Add(double.Parse(dato));
                }
                if (partes[j] != "" && !partes[j].Contains("."))
                {
                    precios.Add(double.Parse(partes[j]));
                }
                if (partes[j] == "")
                {
                    precios.Add(0);
                }

            }
            int contador = 1;
            foreach (double dato in precios)
            {
                num_mes = contador;
                nom_mes = mes(contador);
                fecha = dia + "-" + CompruebaMes(num_mes) + "-" + año;
                contador++;
                Precio p = new Precio();
                p.codigo = code;
                p.precio = dato;
                p.dia = dia;
                p.semana = semana;
                p.numMes = num_mes;
                p.nomMes = nom_mes;               
                p.año = año;
                p.fecha = fecha;
                p.tipoPrecio = tipo_precio;
                p.fuente = "Talavera";
                p.medida = medida;
                GuardaPrecio(p);
            }


        }
        public string CompruebaMes(int mes)
        {
            string month = null;
            if (mes < 10)
            {
                month = "0" + mes;
            }
            else
            {
                month = string.Concat(mes);
            }
            return month;
        }
        public string mes(int posicion)
        {
            string mes = null;
            switch (posicion)
            {
                case 1:
                    mes = "enero";
                    break;
                case 2:
                    mes = "febrero";
                    break;
                case 3:
                    mes = "marzo";
                    break;
                case 4:
                    mes = "abril";
                    break;
                case 5:
                    mes = "mayo";
                    break;
                case 6:
                    mes = "junio";
                    break;
                case 7:
                    mes = "julio";
                    break;
                case 8:
                    mes = "agosto";
                    break;
                case 9:
                    mes = "septiembre";
                    break;
                case 10:
                    mes = "octubre";
                    break;
                case 11:
                    mes = "noviembre";
                    break;
                case 12:
                    mes = "diciembre";
                    break;
                case 13:
                    mes = "media";
                    break;
            }




            return mes;
        }
        public void GuardaPrecio(Precio p)
        {
            elasticClient.Index(p, es => es
                                            .Index("agroesi")
                                           .Type("precio")
                              );
        }
        public void BalanceCereales()
        {
            string file = "C:/Users/Miguel Angel/Documents/Datos/Mapama/BalanceCereales.pdf";

            text = new StringBuilder();
            pdfReader = new PdfReader(file);

            int contador = 0;
            List<double> superficies = new List<double>();
            List<double> producciones = new List<double>();
            List<double> importaciones = new List<double>();
            List<double> exportaciones = new List<double>();

            for (int page = 1; page <= pdfReader.NumberOfPages; page++)
            {
                if (contador > 0)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    text.Append(currentText);
                }
                contador++;
                
            }
            pdfReader.Close();
            string tabla = text.ToString();
            string[] filas = tabla.Split('\n');
            for (int i=0;i<filas.Length;i++)
            {
                if (filas[i].ToLower().Contains("superficie"))
                {
                    List<string> datos = new List<string>();
                    
                    string[] partes = filas[i].Split(' ');
                    for (int j=0;j<partes.Length;j++)
                    {
                        if (partes[j]!="")
                        {
                            datos.Add(partes[j]);
                        }

                    }
                    superficies.Add(double.Parse(datos[3]));
                    superficies.Add(double.Parse(datos[4]));
                    superficies.Add(double.Parse(datos[5]));
                    superficies.Add(double.Parse(datos[6]));
                }
                if (filas[i].ToLower().Contains("producción"))
                {
                    List<string> datos = new List<string>();
                   
                    string[] partes = filas[i].Split(' ');
                    for (int j = 0; j < partes.Length; j++)
                    {
                        if (partes[j] != "")
                        {
                            datos.Add(partes[j]);
                        }

                    }
                    producciones.Add(double.Parse(datos[1]));
                    producciones.Add(double.Parse(datos[2]));
                    producciones.Add(double.Parse(datos[3]));
                    producciones.Add(double.Parse(datos[4]));
                }
                if (filas[i].ToLower().Contains("importaciones"))
                {
                    List<string> datos = new List<string>();
                   
                    string[] partes = filas[i].Split(' ');
                    for (int j = 0; j < partes.Length; j++)
                    {
                        if (partes[j] != "")
                        {
                            datos.Add(partes[j]);
                        }

                    }
                    importaciones.Add(double.Parse(datos[1]));
                    importaciones.Add(double.Parse(datos[2]));
                    importaciones.Add(double.Parse(datos[3]));
                    importaciones.Add(double.Parse(datos[4]));
                }
                if (filas[i].ToLower().Contains("ex") && filas[i].ToLower().Contains("portaciones"))
                {
                    List<string> datos = new List<string>();
                    
                    string[] partes = filas[i].Split(' ');
                    for (int j = 0; j < partes.Length; j++)
                    {
                        if (partes[j] != "")
                        {
                            datos.Add(partes[j]);
                        }

                    }
                    exportaciones.Add(double.Parse(datos[2]));
                    exportaciones.Add(double.Parse(datos[3]));
                    exportaciones.Add(double.Parse(datos[4]));
                    exportaciones.Add(double.Parse(datos[5]));
                }

            }
            CreaEstadisticas(superficies, producciones, importaciones, exportaciones); 


        }
        public void CreaEstadisticas(List<double> superficies, List<double> producciones, List<double> importaciones, List<double> exportaciones)
        {
            int contador = 0;
            for (int i=0;i<4;i++)
            {
                Estadistica e = new Estadistica();
                e.codigo = codigo(contador);
                e.campaña = "2014/15";
                e.superficie = superficies[i];
                e.produccion = producciones[i];
                e.importacines = importaciones[i];
                e.exportaciones = exportaciones[i];
                contador++;
                GuardaEstadistica(e);

            }
            contador = 0;
            for (int i = 4; i < 8; i++)
            {
                Estadistica e = new Estadistica();
                e.codigo = codigo(contador);
                e.campaña = "2015/16";
                e.superficie = superficies[i];
                e.produccion = producciones[i];
                e.importacines = importaciones[i];
                e.exportaciones = exportaciones[i];
                contador++;
                GuardaEstadistica(e);
            }
            contador = 0;
            for (int i = 8; i < 12; i++)
            {
                Estadistica e = new Estadistica();
                e.codigo = codigo(contador);
                e.campaña = "2016/17";
                e.superficie = superficies[i];
                e.produccion = producciones[i];
                e.importacines = importaciones[i];
                e.exportaciones = exportaciones[i];
                contador++;
                GuardaEstadistica(e);
            }
            

        }
        public string codigo(int contador)
        {
            string code = null;

            switch (contador)
            {
                case 0:
                    code = "TB";
                    break;
                case 1:
                    code = "TD";
                    break;
                case 2:
                    code = "CBD";
                    break;
                case 3:
                    code = "MZ";
                    break;

            }
            return code;
        }
        public void GuardaEstadistica(Estadistica e)
        {
            elasticClient.Index(e, es => es
                                            .Index("cereales")
                                           .Type("estadistica")
                              );
        }
        public void ArrozPDF()
        {
            foreach (string file in Directory.EnumerateFiles("C:/Users/Miguel Angel/Documents/Datos/Mapama/Arroz/PDF", "*.pdf"))
            {
                string code = null;
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                string[] datos = null;
                string[] datosF = null;
                string fecha = null;
                double precio = 0;
                int dia = 0;
                int mes = 0;
                int año = 0;
                int semana = 0;
                Calendar cal = dfi.Calendar;
                text = new StringBuilder();
                pdfReader = new PdfReader(file);
                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {

                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    text.Append(currentText);

                }
                pdfReader.Close();

                string tabla = text.ToString();
                string[] filas = tabla.Split('\n');
                if (file.Contains("indica") && file.Contains("2010"))
                {
                    code = "AZI";
                    for (int i = 1; i < 37; i++)
                    {
                        datos = filas[i].Split(' ');
                        fecha = datos[0];
                        precio = double.Parse(datos[1]);
                        datosF=fecha.Split('/');
                        dia = System.Convert.ToInt16(datosF[0]);
                        mes = System.Convert.ToInt16(datosF[1]);
                        año = System.Convert.ToInt16(datosF[2]);
                        
                        DateTime date = new DateTime(año, mes, dia);
                        semana=System.Convert.ToInt16(cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek).ToString());
                        creaPre(code, precio, dia, semana, mes, año, fecha);
                    }

                }
                if (file.Contains("indica") && file.Contains("2012"))
                {
                    code = "AZI";
                    for (int i = 10; i < 34; i++)
                    {
                        datos = filas[i].Split(' ');
                        fecha = datos[0];
                        precio = double.Parse(datos[1]);
                        datosF = fecha.Split('/');
                        dia = System.Convert.ToInt16(datosF[0]);
                        mes = System.Convert.ToInt16(datosF[1]);
                        año = System.Convert.ToInt16(datosF[2]);

                        DateTime date = new DateTime(año, mes, dia);
                        semana = System.Convert.ToInt16(cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek).ToString());
                        creaPre(code, precio, dia, semana, mes, año, fecha);
                    }

                }
                if (file.Contains("japonica") && file.Contains("2010"))
                {
                    code = "AZJ";
                    for (int i = 1; i < 44; i++)
                    {
                        if (i != 20)
                        {
                            datos = filas[i].Split(' ');
                            fecha = datos[0];
                            precio = double.Parse(datos[1]);
                            datosF = fecha.Split('/');
                            dia = System.Convert.ToInt16(datosF[0]);
                            mes = System.Convert.ToInt16(datosF[1]);
                            año = System.Convert.ToInt16(datosF[2]);

                            DateTime date = new DateTime(año, mes, dia);
                            semana = System.Convert.ToInt16(cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek).ToString());
                            creaPre(code, precio, dia, semana, mes, año, fecha);
                        }
                        
                    }

                }
                if (file.Contains("japonica") && file.Contains("2012"))
                {
                    code = "AZJ";
                    for (int i = 1; i < 45; i++)
                    {
                        if (i==1 || (i>5 && i<45))
                        {
                            datos = filas[i].Split(' ');
                            fecha = datos[0];
                            precio = double.Parse(datos[1]);
                            datosF = fecha.Split('/');
                            dia = System.Convert.ToInt16(datosF[0]);
                            mes = System.Convert.ToInt16(datosF[1]);
                            año = System.Convert.ToInt16(datosF[2]);

                            DateTime date = new DateTime(año, mes, dia);
                            semana = System.Convert.ToInt16(cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek).ToString());
                            creaPre(code, precio, dia, semana, mes, año, fecha);

                        }
                        
                    }

                }


            }            

        }
        public void creaPre(string code,double dato,int dia,int semana, int num_mes,int año,string fecha)
        {
            string[] parFecha = fecha.Split('/');
            Precio p = new Precio();
            p.codigo = code;
            p.precio = dato;
            p.dia = dia;
            p.semana = semana;
            p.numMes = num_mes;
            p.nomMes = mes(num_mes);
            p.año = año;
            p.fecha = parFecha[0] + "-" + parFecha[1] + "-" + parFecha[2];
            p.tipoPrecio = "max";
            p.fuente = "Gobierno";
            p.medida = "Euros/t";
            GuardaPrecio(p);
        }
        public void BalanceSoja()
        {
            List<double> importaciones = new List<double>();
            List<double> exportaciones = new List<double>();
            List<double> superficies = new List<double>();
            List<double> producciones = new List<double>();
            List<string> campañas = new List<string>();
            foreach (string file in Directory.EnumerateFiles("C:/Users/Miguel Angel/Documents/Datos/Mapama/Soja", "*.pdf"))
            {
                text = new StringBuilder();
                pdfReader = new PdfReader(file);
                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {

                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    text.Append(currentText);

                }
                pdfReader.Close();

                string tabla = text.ToString();
                string[] filas = tabla.Split('\n');

                if (file.Contains("comercio"))
                {
                    string [] importacion = filas[14].Split(' ');
                    string[] exportacion = filas[17].Split(' ');
                    string[] campaña = filas[11].Split(' ');
                    for(int i = 1; i < 7; i++)
                    {
                        importaciones.Add(double.Parse(importacion[i]));
                        exportaciones.Add(double.Parse(exportacion[i]));
                    }
                    for(int j = 0; j < 6; j++)
                    {
                        if (j != 5)
                        {
                            campañas.Add(campaña[j]);
                        }
                    }
                    campañas.Add("2015/16");
                }
                if (file.Contains("Superficies"))
                {
                    string[] superficie = filas[70].Split(' ');
                    string[] produccion = filas[85].Split(' ');
                    for(int i = 1; i < 7; i++)
                    {
                        superficies.Add(double.Parse(superficie[i]));
                        producciones.Add(double.Parse(produccion[i]));
                    }
                }

                
            }
            EstadisticaSoja(campañas,superficies, producciones, importaciones, exportaciones);

       }
        public void EstadisticaSoja(List<string> campañas, List<double> superficies, List<double> producciones, List<double> importaciones, List<double> exportaciones)
        {
            for (int i = 0; i < 6; i++)
            {
                Estadistica e = new Estadistica();
                e.codigo = "SJ";
                e.campaña = campañas[i];
                e.superficie = superficies[i];
                e.produccion = producciones[i];
                e.importacines = importaciones[i];
                e.exportaciones = exportaciones[i];
                GuardaEstadistica(e);
            }

        }

    }
}
