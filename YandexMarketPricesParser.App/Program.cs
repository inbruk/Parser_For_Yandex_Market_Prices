using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

using YandexMarketPricesParser.PLYandexMarketParser;
using YandexMarketPricesParser.BLCommoditiesInShopsPrices;
using YandexMarketPricesParser.DAL2CSVFiles;

namespace YandexMarketPricesParser.App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // загрузим параметры из файла конфигурации
            String inputCSVPathAndFileName  = ConfigurationSettings.AppSettings["InputCSVPathAndFileName"];
            String outputCSVPathAndFileName = ConfigurationSettings.AppSettings["OutputCSVPathAndFileName"];
            String searchPricesStartURL     = ConfigurationSettings.AppSettings["SearchPricesStartURL"];
            String outputItemsCSVPathAndFileName = outputCSVPathAndFileName + ".items";

            String pageTimeoutStr = ConfigurationSettings.AppSettings["PageTimeout"];
            Int32 pageTimeout = Int32.Parse(pageTimeoutStr);

            // создаем постоянно существующие (для всех товаров) объекты DAL
            InputCSVFile inputFile = new InputCSVFile(inputCSVPathAndFileName);
            OutputCSVFile outputItemsOnlyFile = new OutputCSVFile(outputItemsCSVPathAndFileName);
            
            // создаем постоянно существующие (для всех товаров) объекты BL
            ShopList globalShopList = new ShopList();

            // создаем постоянно существующие (для всех товаров) объекты PL 
            SearchPricesParser globalSearchPricesParser = new SearchPricesParser(searchPricesStartURL, pageTimeout);

            // пропускаем первую строку в исходном файле
            inputFile.GetNextRow();

            CSVFileRow headerCSVRow = null; 

            while (true)
            {
                try
                {
                    // грузим следующую строку из входного CSV файла о товарах
                    CSVFileRow currInputCSVRow = inputFile.GetNextRow();

                    // проверим на окончание входного файла, и выйдем если так
                    if (currInputCSVRow == null) break;

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Сейчас обрабатываем " + currInputCSVRow.GetAllInOneString() + " ");

                    // такие строки считаем битыми
                    if (currInputCSVRow.GetItemsCount() != 2)
                    {
                        Console.Write(" - пустая строка или строка неверного формата !");
                        continue; // и пропускаем их
                    }

                    // загружаем данные о текущем товаре в слой BL
                    Commodity currCommodity = new Commodity(currInputCSVRow, globalShopList);

                    // в слое PL получаем из браузера (яндекс маркета) данные о товаре и переносим их в BL
                    globalSearchPricesParser.ScanPricesForCurrCommodity(currCommodity);

                    // формируем строку csv файла из данных находящихся в объектах BL
                    CSVFileRow currOutputCSVRow = currCommodity.CalculatePricesAndProduceOutputCSVRow();

                    // записываем полученные данные о товаре в выходной файл
                    outputItemsOnlyFile.PutRow(currOutputCSVRow);

                    // создаем текущую первую строку - заголовок (используется вне цикла)
                    headerCSVRow = currCommodity.ProduceHeaderCSVRow();
                }
                catch(Exception e)
                {
                    Console.Write(" - во время обработки случилось исключение ! " + e.Source + " " + e.Message );
                }
            }

            // закрываем теперь не нужные файлы
            inputFile.Close();
            inputFile = null;
            outputItemsOnlyFile.Close();
            outputItemsOnlyFile = null;

            // создаем выходной файл с заголовком и строками отпарсеными --------------------------------------------------------------------------------------------------------------------
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Создаем выходной файл с заголовком и строками отпарсеными.");
            if (headerCSVRow != null) 
            {
                // создаем выходной файл
                OutputCSVFile outputFile = new OutputCSVFile(outputCSVPathAndFileName);

                // здесь нужно сохранять первую строку - заголовок в отдельный файл
                outputFile.PutRow(headerCSVRow);

                // приклеиваем содержимое файла с отпарсеными строками к файлу с заголовком
                InputCSVFile itemsFile = new InputCSVFile(outputItemsCSVPathAndFileName);
                while(true)
                {
                    CSVFileRow currItemsCSVRow = itemsFile.GetNextRow();
                    
                    if (currItemsCSVRow == null) break;

                    outputFile.PutRow(currItemsCSVRow);
                }
                itemsFile.Close();
                itemsFile = null;
                outputFile.Close();
                outputFile = null;
                
                // удаляем временный файл с содержимым парсинга, после того как уже готов основной
                File.Delete(outputItemsCSVPathAndFileName);
            }
            // создаем выходной файл с заголовком и строками отпарсеными --------------------------------------------------------------------------------------------------------------------

            // удаляем/освобождаем постоянно существующие (для всех товаров) объекты PL 
            globalSearchPricesParser.CloseBrowserDoneFinishAllSearch();
            globalSearchPricesParser = null;

            // удаляем/освобождаем постоянно существующие (для всех товаров) объекты BL    
            globalShopList = null;

            // для того, чтобы увидеть лог
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Завершено.");
            Console.ReadKey();
        }
    }
}
