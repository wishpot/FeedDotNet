using System;
using System.Collections.Generic;
using System.Text;
using FeedDotNet.Common;
using System.Diagnostics;
using System.Xml;
using FeedDotNet.Utilities;

namespace FeedDotNet.Modules.EBayBaseComponents
{
    [Serializable]
    public class ModuleItem : IModuleItem
    {
  
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string localName = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Decimal? buyItNowPrice = null;

        public Decimal? BuyItNowPrice
        {
          get { return buyItNowPrice; }
          set { buyItNowPrice = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Decimal? currentPrice = null;

        public Decimal? CurrentPrice
        {
          get { return currentPrice; }
          set { currentPrice = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string category = null;

        public string Category
        {
          get { return category; }
          set { category = value; }
        }
      
        public ModuleItem(string localName)
        {
            this.localName = localName;
        }


        #region IModuleItem Members

        public string LocalName
        {
            get
            {
                return localName;
            }
        }

        /// <summary>
        /// Elements to parse on an item-by-item basis.
        /// </summary>
        /// <param name="xmlReader"></param>
        public void Parse(System.Xml.XmlReader xmlReader)
        {
          while (xmlReader.Read())
          {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
              switch (xmlReader.LocalName)
              {
                case "BuyItNowPrice":
                  xmlReader.MoveToContent();
                  buyItNowPrice = ParsePrice(xmlReader.ReadString());
                  break;

                case "CurrentPrice":
                  xmlReader.MoveToContent();
                  currentPrice = ParsePrice(xmlReader.ReadString());
                  break;

                case "Category":
                  xmlReader.MoveToContent();
                  category = xmlReader.ReadString();
                  break;
              }
            }
          }
        }

        #endregion

        public static decimal? ParsePrice(string priceString)
        {
          if (String.IsNullOrEmpty(priceString))
            return (decimal?)null;

          decimal price;
          if (decimal.TryParse(priceString, out price))
            return price/100;

          return (decimal?)null;

        }
     
    
    }


}
