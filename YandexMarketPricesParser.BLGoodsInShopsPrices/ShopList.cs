using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YandexMarketPricesParser.BLCommoditiesInShopsPrices
{
    public class ShopList
    {
        private List<String> _shopNames;

        public ShopList()
        {
            _shopNames = new List<string>();
        }

        /// <summary>
        /// проверяет есть ои магазин в списке и добавляет его, если нет 
        /// </summary>
        /// <param name="Name">называние магазина в яндекс маркете</param>
        /// <returns> true - магазин буже был в списке, false добавили новый магазин в список</returns>
        public Boolean CheckExistanceAndAddShop(String Name)
        {
            Boolean res = _shopNames.Any( x => x == Name );
            if(res == false)
            {
                _shopNames.Add(Name);
            }
            return res;
        }

        public List<String> GetShopNamesInPermanentOrder()
        {
            return _shopNames;
        }
    }
}
