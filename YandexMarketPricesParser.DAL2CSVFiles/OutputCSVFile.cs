using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YandexMarketPricesParser.DAL2CSVFiles
{
    public class OutputCSVFile : IDisposable
    {
        private String _fileName;
        private StreamWriter _srFile;

        public OutputCSVFile(String fileName)
        {
            _fileName = fileName;
            _srFile = new StreamWriter(_fileName, false, Encoding.UTF8);            
        }

        public void PutRow(CSVFileRow csvRow)
        {
            String rowStr = csvRow.GetAllInOneString();
            _srFile.WriteLine(rowStr);
            _srFile.Flush();
        }

        /// <summary>
        /// перемещаемся в файле на начало и очищаем его при этом
        /// </summary>
        public void SeekToBeginAndClear()
        {
            _srFile.Flush();
            _srFile.Close();
            _srFile = null;

            _srFile = new StreamWriter(_fileName, false, Encoding.UTF8);            
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

        ~OutputCSVFile()
        {
            Dispose(false);
        }
    }
}
