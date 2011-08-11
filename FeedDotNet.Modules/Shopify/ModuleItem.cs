using System;
using System.Collections.Generic;
using System.Text;
using FeedDotNet.Common;
using System.Diagnostics;
using System.Xml;
using FeedDotNet.Utilities;
using System.Linq;

namespace FeedDotNet.Modules.Shopify
{
    [Serializable]
    public class ModuleItem : IModuleItem
    {
  
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string localName = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string type = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string vendor = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Decimal? price = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string currency_code = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<string> tags = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Variant> variants = new List<Variant>();
      
        public ModuleItem(string localName)
        {
            this.localName = localName;
        }

        public string Type
        {
          get { return type; }
          set { type = value; }
        }

        public string Vendor
        {
          get { return vendor; }
          set { vendor = value; }
        }

        public Decimal? Price
        {
          get { return price; }
          set { price = value; }
        }

        public string CurrencyCode
        {
          get { return currency_code; }
          set { currency_code = value; }
        }

        public List<string> Tags
        {
          get { return tags; }
        }

        public List<Variant> Variants
        {
          get { return variants; }
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
          bool currentlyReadingVariant = false;
          while (xmlReader.Read())
          {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
              switch (xmlReader.LocalName)
              {
                case "type":
                  xmlReader.MoveToContent();
                  type = xmlReader.ReadString();
                  break;

                case "vendor":
                  xmlReader.MoveToContent();
                  vendor = xmlReader.ReadString();
                  break;

                case "price":
                  string c = xmlReader.GetAttribute("currency");
                  xmlReader.MoveToContent();
                  string pr = xmlReader.ReadString();
                  if (null != pr && ParsePrice(pr).HasValue)
                  {
                    if (!currentlyReadingVariant)
                    {
                      price = ParsePrice(pr).Value;
                      currency_code = c;
                    }
                    else
                    {
                      variants.Last().price = ParsePrice(pr).Value;
                      variants.Last().currency_code = c;
                    }
                  }
                  break;

                case "tag":
                  xmlReader.MoveToContent();
                  tags.Add(xmlReader.ReadString());
                  break;

                case "variant":
                  currentlyReadingVariant = true;
                  variants.Add(new Variant());
                  xmlReader.MoveToContent();
                  break;

                #region variant parts
                case "id":
                  if (currentlyReadingVariant)
                  {
                    xmlReader.MoveToContent();
                    variants.Last().id = xmlReader.ReadString();
                  }
                  break;

                case "title":
                  if (currentlyReadingVariant)
                  {
                    xmlReader.MoveToContent();
                    variants.Last().title = xmlReader.ReadString();
                  }
                  break;

                case "sku":
                  if (currentlyReadingVariant)
                  {
                    xmlReader.MoveToContent();
                    variants.Last().sku = xmlReader.ReadString();
                  }
                  break;

                case "grams":
                  if (currentlyReadingVariant)
                  {
                    xmlReader.MoveToContent();
                    variants.Last().grams = xmlReader.ReadString();
                  }
                  break;

                #endregion

                
              }
            }
            else if (xmlReader.NodeType == XmlNodeType.EndElement)
            {
              switch (xmlReader.LocalName)
              {
                case "variant":
                  currentlyReadingVariant = false;
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
            return price;
          return (decimal?)null;
        }

        public class Variant
        {
          public string id { get; set; }
          public string title { get; set; }
          public decimal? price { get; set; }
          public string currency_code { get; set; }
          public string sku { get; set; }
          public string grams { get; set; }
        }
    
    }


}
