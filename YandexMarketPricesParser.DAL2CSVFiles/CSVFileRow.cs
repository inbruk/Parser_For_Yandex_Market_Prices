using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YandexMarketPricesParser.DAL2CSVFiles
{
    public class CSVFileRow
    {
        private List<String> _items;

        public CSVFileRow()
        {
            _items = new List<String>();
        }

        public CSVFileRow(int startItemsCount)
        {
            _items = new List<String>();
            for(int i=0;i<startItemsCount;i++)
            {
                AddItem("");
            }
        }

        public void AddItem(string currItem)
        {
            _items.Add(currItem);
        }
        public void SetItemByIndex(int index, String item)
        {
            _items[index] = item;
        }
        public String GetItemByIndex(int index)
        {
            return _items[index];
        }
        public int GetItemsCount()
        {
            return _items.Count;
        }

        public String GetAllInOneString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _items.Count;i++ )
            {
                sb.Append(_items[i]);
                if( i < _items.Count-1 )
                {
                    sb.Append(";");
                }
            }

            return sb.ToString();
        }

        public void AddItemsFromRowString(String rowStr)
        {
            String[] arrRow = rowStr.Split(new char[] { ';' }, StringSplitOptions.None);
            foreach (String currSt in arrRow)
            {
                AddItem(currSt);
            }
        }

    }
}
