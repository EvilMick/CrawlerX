using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerX
{
    class DataExcell
    {
        public void read_data()
        {
            FileStream stream = File.Open("C:/Users/Miguel Angel/Downloads/20170208.xlsx", FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
           
            DataSet result = excelReader.AsDataSet();
            DataTable tabla = result.Tables[0];
            DataRowCollection filas = tabla.Rows;
            int contador = 0;
            foreach (DataRow fila in filas)
            {
                switch (contador)
                {
                    case 0:
                        string mercado = fila[0].ToString();
                    break;
                    case 1:
                        string tipo_precio = fila[0].ToString();
                        string fecha = fila[8].ToString();
                    break;
                    case 2:
                        List<string> productos = new List<string>();
                        productos.Add(fila[1].ToString());
                        productos.Add(fila[3].ToString());
                        productos.Add(fila[5].ToString());
                        productos.Add(fila[7].ToString());
                        productos.Add(fila[9].ToString());
                        productos.Add(fila[11].ToString());
                        break;
                    case 3:
                        List<string> variedades = new List<string>();
                        variedades.Add(fila[1].ToString());
                        variedades.Add(fila[3].ToString());
                        variedades.Add(fila[5].ToString());
                        variedades.Add(fila[7].ToString());
                        variedades.Add(fila[9].ToString());
                        variedades.Add(fila[11].ToString());
                        break;

                }
                if (contador>3 && contador < 28)
                {
                    List<double> precios = new List<double>();
                    string[] partes = null;
                    partes = fila[0].ToString().Split(' ');
                    string fecha = partes[0];
                    for ( int i=1; i<12;i++)
                    {
                        if (i%2 != 0 && !fila[i].ToString().Equals("0"))
                        {
                            precios.Add(Double.Parse(fila[i].ToString()));
                        }
                        

                    }
                }
                contador++;
                
                
            }
            excelReader.Close();

        }
        public DataExcell()
        {

        }
    }
}
