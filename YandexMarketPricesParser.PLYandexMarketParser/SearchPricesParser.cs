using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using WatiN.Core;
using WatiN.Core.Native.InternetExplorer;

using YandexMarketPricesParser.BLCommoditiesInShopsPrices;

namespace YandexMarketPricesParser.PLYandexMarketParser
{
    public class SearchPricesParser : IDisposable
    {
        private String _searchPricesStartURL;
        private Commodity _currCommodity;
        private Browser _currBrowser;
        private Int32 _pageTimeout;

        /// <summary>
        /// перейдем на начальную страницу яндекс поиска и выполним поиск по текущему товару
        /// </summary>
        private void ResetURLAndStartSearch()
        {
            // перейдем на начальную страницу яндекс поиска
            _currBrowser.GoTo(_searchPricesStartURL);
            _currBrowser.WaitForComplete();

            // выполним поиск по текущему товару
            TextField searchInputField = _currBrowser.TextField(Find.ByClass("b-form-input__input"));

            searchInputField.Focus();
            _currBrowser.WaitForComplete();

            Random rndGen = new Random();
            foreach (Char currCharOfName in _currCommodity.CommodityName)
            {
                int currsleepTime = rndGen.Next(0, 50);
                Thread.Sleep(currsleepTime);

                searchInputField.KeyPress(currCharOfName);
                _currBrowser.WaitForComplete();
            }
            _currBrowser.WaitForComplete();

            Button searchButton = _currBrowser.Button(Find.ByClass("b-form-button__input"));
            searchButton.Click();
            _currBrowser.WaitForComplete();

            // prevent unauthorized access error inside watin, when scan html elements !!!
            // prevent yandex human check fault (may be)
            // внимание ! если возникает ошибка unauthorized access закройте все IE и приложения парсинга, подождите 15 секунд, потом запускайте заново
            Thread.Sleep(_pageTimeout); 
        }
        
        /// <summary>
        /// перейдем на следующую страницу результатов поиска, если она существует
        /// </summary>
        /// <returns> true - переход осуществлен, false - следующая страница не существует </returns>
        private Boolean GoToNextPageInSearchResults()
        {
            // ищем ссылку <следующая страница>
            Link nextPageLink = _currBrowser.Link(Find.ByClass("b-pager__next"));
           
            // ссылка не найдена
            if (nextPageLink==null || nextPageLink.Exists == false)
            {
                return false;
            }            

            // переходим на следующую страницу
            nextPageLink.Click();
            _currBrowser.WaitForComplete();

            return true;
        }

        private void ScanCurrentPageOfSearchResults()
        {
            YandexMarketSearchResultsPageParser pageParser = new YandexMarketSearchResultsPageParser(_currBrowser, _currCommodity);
            pageParser.ScanIt();
        }

        // -------------------------------------------------------------------- интерфейсные методы PUBLIC ------------------------------------------------------------------------------

        /// <summary>
        /// устанавливает внутренние параметры, создает и открывает браузер
        /// </summary>
        public SearchPricesParser(String searchPricesStartURL, Int32 pageTimeout)
        {
            _searchPricesStartURL = searchPricesStartURL;
            _pageTimeout = pageTimeout;

            _currBrowser = new IE();
            _currBrowser.BringToFront();
        }

        public void CloseBrowserDoneFinishAllSearch()
        {
            _currBrowser.Close();
            _currBrowser = null;
        }

        /// <summary>
        /// ищет цены по текущему установленному товару
        /// парсер не переконструируется и браузер не перезагружается при сканировании разных товаров
        /// </summary>
        public void ScanPricesForCurrCommodity(Commodity currCommodity)
        {
            _currCommodity = currCommodity;

            ResetURLAndStartSearch();

            // сохраним в текущий товар его урл в поиске яндекс маркет
            currCommodity.SetCommodityURL( _currBrowser.Url );

            do
            {
                ScanCurrentPageOfSearchResults();
            }
            while (GoToNextPageInSearchResults());
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
                    if (_currBrowser != null)
                    {
                        CloseBrowserDoneFinishAllSearch();
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

        ~SearchPricesParser()
        {
            Dispose(false);
        }
    }
}
