using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YandexMarketPricesParser.DAL2CSVFiles
{
    public class InputCSVFile : IDisposable
    {
        private StreamReader _srFile;

        public InputCSVFile(String fileName)
        {
            _srFile = new StreamReader(fileName);            
        }
        
        /// <summary>
        /// в случае если строк не осталось, вернет null, иначе строку CSV файла
        /// </summary>
        /// <returns></returns>
        public CSVFileRow GetNextRow()
        {
            String rowStr = _srFile.ReadLine();
            if(rowStr==null)
            {
                return null;
            }

            CSVFileRow csvRow = new CSVFileRow();
            csvRow.AddItemsFromRowString(rowStr);

            return csvRow;
        }

        public void Close()
        {
            _srFile.Close();
            _srFile = null;
        }

        // ----------------------------------------------------------------- все остальное, только для правильной деструкции ---------------------------------------------------------------

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_srFile != null)
                    {
                        _srFile.Close();
                    }
                }
                // Note disposing has been done.
                disposed = true;

            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        ~InputCSVFile()
        {
            Dispose(false);
        }
    }
}
