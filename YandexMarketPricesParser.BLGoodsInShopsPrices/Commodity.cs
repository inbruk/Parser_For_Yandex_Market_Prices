using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using YandexMarketPricesParser.DAL2CSVFiles;

namespace YandexMarketPricesParser.BLCommoditiesInShopsPrices
{
    /// <summary>
    /// представление о текущем обрабатываемом товаре в системе, со всеми его предложениями внутри
    /// </summary>
    public class Commodity
    {
        private String  _commodityName;
        public  String CommodityName
        {
            get
            {
                return _commodityName;
            }
        }

        private String  _ourPriceStr;

        private String  _commodityURL;
        /// <summary>
        /// метод должен вызываться только из парсера сайта с ценами
        /// </summary>
        public void SetCommodityURL(String  commodityURL)
        {
            _commodityURL = commodityURL;
        }

        private Decimal _minPrice;
        private Decimal _avgPrice;
        private Decimal _maxPrice;

        private Int32 _offersCount; 

        private ShopList _shopList;
        private Dictionary<String, Decimal> _shopCommodityOfferList;

        public Commodity(CSVFileRow inputRow, ShopList sl)
        {
            _shopList = sl;

            _commodityName = inputRow.GetItemByIndex(0);
            _ourPriceStr   = inputRow.GetItemByIndex(1);

            _shopCommodityOfferList = new Dictionary<string, decimal>();
        }

        /// <summary>
        /// этот метод вызывается при хождении по яндекс маркету, каждый раз когда мы находим предложение от магазина
        /// внимание ! если нам уже попадалось предложение этого же товара, от этого же магазина, то оставляем старое значение и не добавляем новое !!!
        /// </summary>
        public void AddShopCommodityOffer( String shopName, Decimal offerPrice )
        {
            _shopList.CheckExistanceAndAddShop(shopName);
            if (_shopCommodityOfferList.ContainsKey(shopName) == false)
            {
                _shopCommodityOfferList.Add(shopName, offerPrice); // первый раз встречаем предложение от этого магазина
                Console.Write(".");
            }
            else
            {
                Console.Write("D");
            }
        }

        /// <summary>
        /// только для внутреннего использования
        /// </summary>
        private void CalculatePricesAndOffersCount()
        {            
            _offersCount = _shopCommodityOfferList.Count();
            if( _offersCount>0 )
            {
                _minPrice = _shopCommodityOfferList.Values.Min(x => x);
                _maxPrice = _shopCommodityOfferList.Values.Max(x => x);
                _avgPrice = _shopCommodityOfferList.Values.Average(x => x);
            }
            else
            {
                _minPrice = 0.0m;
                _maxPrice = 0.0m;
                _avgPrice = 0.0m;           
            }
        }

        /// <summary>
        /// этот метод нужно вызывать когда все предложения уже обошли в браузере
        /// и осталось посчитать цены, сгенерировать строку CSV файла
        /// </summary>
        public CSVFileRow CalculatePricesAndProduceOutputCSVRow()
        {
            CalculatePricesAndOffersCount();

            CSVFileRow res = new CSVFileRow();
            res.AddItem(_commodityName);
            res.AddItem(_commodityURL);
            res.AddItem(_ourPriceStr);
            res.AddItem(_minPrice.ToString("F2", CultureInfo.InvariantCulture));
            res.AddItem(_avgPrice.ToString("F2", CultureInfo.InvariantCulture));
            res.AddItem(_maxPrice.ToString("F2", CultureInfo.InvariantCulture));
            res.AddItem(_offersCount.ToString("F0", CultureInfo.InvariantCulture));

            List<String> shopNames = _shopList.GetShopNamesInPermanentOrder();
            for (int i = 0; i < shopNames.Count; i++ )
            {
                String currShopName = shopNames[i];
                if( _shopCommodityOfferList.ContainsKey(currShopName) )
                {
                    res.AddItem(_shopCommodityOfferList[currShopName].ToString("F2", CultureInfo.InvariantCulture));
                }
                else
                {
                    res.AddItem(String.Empty);
                }
            }

            return res;
        }

        /// <summary>
        /// этот метод нужно вызывать после записи каждой строки и сохранять первую строку - заголовок с названиями 
        /// так как после создания каждой строки состав магазинов мог измениться, удлиниться
        /// </summary>
        public CSVFileRow ProduceHeaderCSVRow()
        {
            CSVFileRow res = new CSVFileRow();

            res.AddItem("Продукт");
            res.AddItem("Ссылка");
            res.AddItem("Наша цена");
            res.AddItem("Мин цена");
            res.AddItem("Средняя цена");
            res.AddItem("Макс цена");
            res.AddItem("Кол.Пред.");

            List<String> shopNames = _shopList.GetShopNamesInPermanentOrder();
            for (int i = 0; i < shopNames.Count; i++)
            {
                String currShopName = shopNames[i];
                res.AddItem(currShopName);
            }

            return res;
        }
    }
}
