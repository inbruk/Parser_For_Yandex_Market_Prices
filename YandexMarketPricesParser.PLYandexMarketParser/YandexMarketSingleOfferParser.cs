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
    public class YandexMarketSingleOfferParser
    {
        private Commodity _currCommodity;
        private Browser _currBrowser;
        private Div _offerContainer;

        public YandexMarketSingleOfferParser(Browser currBrowser, Commodity currCommodity, Element offerContainer)
        {
            _currBrowser = currBrowser;
            _currCommodity = currCommodity;
            _offerContainer = (Div) offerContainer;
        }

        /// <summary>
        /// парсим предложение и запихиваем результаты в описание товара 
        /// нужно вытащить имя магазина и цену, если чену не удается вытащить, то игнорируем это предложение
        /// </summary>
        public void ScanIt()
        {
            try
            {
                Div descDiv = _offerContainer.Div(Find.ByClass("b-offers__desc"));
                if (descDiv == null || descDiv.Exists == false) return;

                Div featsDiv = descDiv.Div(Find.ByClass("b-offers__feats"));
                if (featsDiv == null || featsDiv.Exists == false) return;

                Link shopLink = featsDiv.Link(Find.ByClass("shop-link b-address__link"));
                if (shopLink == null || shopLink.Exists == false) return;

                String shopName = shopLink.Text;

                Div infoDiv = _offerContainer.Div(Find.ByClass("b-offers__info"));
                if (infoDiv == null || infoDiv.Exists == false) return;

                Div priceDiv = infoDiv.Div(Find.ByClass("b-offers__price"));
                if (priceDiv == null || priceDiv.Exists == false) return;

                Span priceSpan = priceDiv.Span(Find.ByClass("b-prices__num"));
                if (priceSpan == null || priceSpan.Exists == false) return;

                String priceStr = priceSpan.Text;

                Decimal offerPrice;
                if (Decimal.TryParse(priceStr, out offerPrice))
                {
                    _currCommodity.AddShopCommodityOffer(shopName, offerPrice);
                }
            }
            catch
            {
                Console.Write("E");
            }
        }
    }
}
