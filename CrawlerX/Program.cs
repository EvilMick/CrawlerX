using System;
using System.Collections.Generic;

namespace CrawlerX
{
    class Program
    {
        static void Main(string[] args)
        {
            int userInput = 0;
            Crawler crawler = new Crawler();
            Downloader downloader = new Downloader();
            Extractor extractor = new Extractor();
            LectorRSS reader = new LectorRSS();
            DataExcell excell = new DataExcell();
            PDFExtractor pdfextractor = new PDFExtractor();
            //crawler.start_crawler();
            do
            {
                userInput = DisplayMenu();

                switch (userInput)
                {
                    case 1:
                        downloader.Download(crawler.getfiles());
                        Console.WriteLine("Operacion terminada");
                        break;
                    case 2:
                        //extractor.extraer_datos();
                        //extractor.LonjaAlbacete();
                        extractor.NoticiasGanaderia();
                        //extractor.BalanceArroz();
                        //extractor.Soja();
                        //extractor.NoticiasAgricultura();
                        break;
                    case 3:
                        //reader.RSS_XML();
                        //downloader.Manual_Download();
                        //downloader.EfeAgro_Manual();
                        downloader.Agroinfo_Manual();
                        break;
                    case 4:
                        //excell.read_data();
                        excell.read_arroz();
                        break;

                    case 6:
                        //pdfextractor.DatosOvino();
                        //pdfextractor.DatosVacuno();
                        //pdfextractor.BalanceCereales();
                        //pdfextractor.ArrozPDF();
                        pdfextractor.BalanceSoja();
                        break;
                }

            } while (userInput != 0);
        }
        static public int DisplayMenu()
        {
            Console.WriteLine("AGROESI");
            Console.WriteLine();
            Console.WriteLine("1. Descargar Datos");
            Console.WriteLine("2. Extraer Datos");
            Console.WriteLine("3. Noticias RSS");
            Console.WriteLine("4. Datos ficheros excell");
            Console.WriteLine("6. Lector pdfs");
            Console.WriteLine("0. Salir");
            var result = Console.ReadLine();
            if (result == "" || result == " ")
            {
                return 99;
            }
            return Convert.ToInt32(result);
        }


    }
}
