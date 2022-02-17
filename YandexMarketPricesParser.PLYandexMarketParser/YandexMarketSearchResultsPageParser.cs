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
    public class YandexMarketSearchResultsPageParser
    {
        private Commodity _currCommodity;
        private Browser _currBrowser;
        private Int32 _pageTimeout;

        public YandexMarketSearchResultsPageParser(Browser currBrowser, Commodity currCommodity)
        {
            _currBrowser = currBrowser;
            _currCommodity = currCommodity;
        }

        public void ScanIt()
        {
            // сначала добудем обертки информации о каждом коммерческом предложении
            Div offersListContainer = _currBrowser.Div(Find.ByClass("b-offers__list"));           
            List<Div> offerDivs = new List<Div>();
            ElementCollection offerDivsColl = offersListContainer.Children();
            foreach (Element currChild in offerDivsColl) // LINQ-ом выковырять не удалось, почему-то
            {
                Div currDiv = (Div)currChild;
                if( currDiv!=null && currDiv.Exists==true )
                {
                    if( currDiv.ClassName!=null && currDiv.ClassName.Contains("b-offers ") )
                    {
                        offerDivs.Add( currDiv );
                    }
                }
            }

            // теперь просканируем каждую обертку сканером предложений
            foreach(Div currOfferDiv in offerDivs)
            {
                YandexMarketSingleOfferParser offerParser = new YandexMarketSingleOfferParser(_currBrowser, _currCommodity, currOfferDiv);
                offerParser.ScanIt();
            }
        }
    }
}
