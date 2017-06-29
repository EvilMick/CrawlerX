using Excel;
using Nest;
using System.Collections.Generic;
using System.Data;
using System.IO;


namespace CrawlerX
{
    class DataExcell
    {
        ElasticClient elasticClient = Broker.EsClient();
        public void read_data()
        {
            
            foreach (string file in Directory.EnumerateFiles("C:/Users/Miguel Angel/Documents/Datos/Mapama/", "*.xlsx"))
            {
                FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet result1 = excelReader.AsDataSet();

                List<double> precios = new List<double>();
                foreach (DataTable tabla in result1.Tables)
                {
                    DataRowCollection filas = tabla.Rows;
                    int contador = 0;
                    var cabeceras = filas[2];
                    foreach (DataRow fila in filas)
                    {
                        if (contador > 2 && contador < 55)
                        {

                            var datos = fila.ItemArray;
                            string mes = datos[0].ToString();
                            int semana = System.Convert.ToInt16(datos[1]);
                            string[] partes = datos[2].ToString().Split(' ');
                            string fecha = partes[0];
                            if (tabla.TableName.Contains("Medias"))
                            {
                                for (int i = 3; i < 7; i++)
                                {
                                    if (datos[i].ToString() == "")
                                    {
                                        precios.Add(0);
                                    }else
                                    {
                                        precios.Add(double.Parse(datos[i].ToString()));
                                    }                                  
                                }
                                creaPrecio(precios, cabeceras, mes, semana, fecha, tabla.TableName);
                            }
                            else
                            {
                                for (int i = 3; i < 6; i++)
                                {
                                    if (datos[i].ToString() == "")
                                    {
                                        precios.Add(0);
                                    }else
                                    {
                                        precios.Add(double.Parse(datos[i].ToString()));
                                    }
                                    
                                }
                                creaPrecio(precios, cabeceras, mes, semana, fecha, tabla.TableName);
                            }
                        }
                        precios = new List<double>();
                        contador++;
                    }

                }
                excelReader.Close();
            }
                     
        }
        public void creaPrecio(List<double> precios, DataRow cabeceras, string mes, int semana,string fecha,string nomTabla)
        {
            
            int contador = 1;
            foreach (double precio in precios)
            {
                string[] parFecha = fecha.Split('/');
                Precio p = new Precio();
                if (precio == 0)
                {
                    p.precio = precio;
                }
                else
                {
                    p.precio = double.Parse(precio.ToString("#.##"));
                }
                
                p.fecha = parFecha[0]+"-"+parFecha[1]+"-"+parFecha[2];
                p.semana = semana;
                p.nomMes = mes;
                p.numMes = System.Convert.ToInt16(parFecha[1]);
                p.dia = System.Convert.ToInt16(parFecha[0]);
                p.año = System.Convert.ToInt16(parFecha[2]);
                p.medida = "Euros/t";
                if (nomTabla.Contains("Medias"))
                {
                    p.tipoPrecio = "med";
                    p.fuente = "Gobierno";
                }else
                {
                    p.tipoPrecio = "max";
                }
                p.codigo=determinaCodigo(contador, cabeceras, nomTabla, p);
                contador++;
                GuardaPrecio(p);
            }
        }
        public string determinaCodigo(int contador, DataRow cabeceras, string nomTabla, Precio p)
        {
            string codigo = null;
            string code1 = "TB";
            string code2 = "TD";
            string code3 = "CBD";
            string code4 = "MZ";

            string lonja1 = null;
            string lonja2 = null;
            string lonja3 = null;

            if (nomTabla.Contains("Medias"))
            {
                switch (contador)
                {
                    case 1:
                        codigo = code1;
                        break;
                    case 2:
                        codigo = code2;
                        break;
                    case 3:
                        codigo = code3;
                        break;
                    case 4:
                        codigo = code4;
                        break;
                }

            }
            else
            {
                lonja1 = cabeceras.ItemArray[3].ToString();
                lonja2 = cabeceras.ItemArray[4].ToString();
                lonja3 = cabeceras.ItemArray[5].ToString();

                switch (contador)
                {
                    case 1:
                        p.fuente = lonja1;
                        break;
                    case 2:
                        p.fuente = lonja2;
                        break;
                    case 3:
                        p.fuente = lonja3;
                        break;

                }
                if (nomTabla.Contains("TB"))
                {
                    codigo = code1;

                }
                if (nomTabla.Contains("TD"))
                {
                    codigo = code2;
                }
                if (nomTabla.Contains("Cebada"))
                {
                    codigo = code3;
                }
                if (nomTabla.Contains("Maiz"))
                {
                    codigo = code4;
                }
            }





            return codigo;
        }
        public void GuardaPrecio(Precio p)
        {
            elasticClient.Index(p, es => es
                                            .Index("agroesi")
                                           .Type("precio")
                              );
        }
        public void read_arroz()
        {

            foreach (string file in Directory.EnumerateFiles("C:/Users/Miguel Angel/Documents/Datos/Mapama/Arroz/", "*.xlsx"))
            {
                FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet result1 = excelReader.AsDataSet();
                string code = null;
                foreach (DataTable tabla in result1.Tables)
                {
                    if (tabla.TableName.Contains("japonica"))
                    {
                        code = "AZJ";
                    }else
                    {
                        code = "AZI";
                    }

                    DataRowCollection filas = tabla.Rows;
                    int contador = 0;
                    foreach (DataRow fila in filas)
                    {

                        if (contador > 2 && contador < 55)
                        {
                            var datos = fila.ItemArray;

                            if (datos[3].ToString() != "")
                            {
                                string mes = datos[0].ToString();
                                int semana = System.Convert.ToInt16(datos[1]);
                                string[] partes = datos[2].ToString().Split(' ');
                                string fecha = partes[0];
                                double precio = double.Parse(datos[3].ToString());
                                precio = double.Parse(precio.ToString("#.##"));
                                creaPrecioArroz(mes, semana, fecha, precio, code);
                            }
                                                                                                              
                        }
                        
                        contador++;
                    }
                }
                excelReader.Close();
            }

        }
        public void creaPrecioArroz(string mes, int semana, string fecha, double precio,string code)
        {
                string[] parFecha = fecha.Split('/');
                Precio p = new Precio();
                p.precio = precio;
                p.fecha = parFecha[0] + "-" + parFecha[1] + "-" + parFecha[2];
                p.semana = semana;
                p.nomMes = mes;
                p.numMes = System.Convert.ToInt16(parFecha[1]);
                p.dia = System.Convert.ToInt16(parFecha[0]);
                p.año = System.Convert.ToInt16(parFecha[2]);
                p.medida = "Euros/t";
                p.tipoPrecio = "med";
                p.fuente = "Gobierno";
                p.tipoPrecio = "max";
                p.codigo = code;
                GuardaPrecio(p);
            }
        
    }
}
