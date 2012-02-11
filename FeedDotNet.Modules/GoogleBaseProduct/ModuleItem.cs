using System;
using System.Collections.Generic;
using System.Text;
using FeedDotNet.Common;
using System.Diagnostics;
using System.Xml;
using FeedDotNet.Utilities;

namespace FeedDotNet.Modules.GoogleBaseProduct
{
    [Serializable]
    public class ModuleItem : IModuleItem
    {
        private static readonly System.Globalization.CultureInfo euroFormat = new System.Globalization.CultureInfo( "it-IT" );

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string localName = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string mpn = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string upc = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string isbn = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string id = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Decimal? price = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string image_link = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string product_type = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? quantity = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isDiscontinued = false;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string brand = String.Empty;
        
        public string Mpn
        {
          get { return mpn; }
          set { mpn = value; }
        }
        public string Upc
        {
          get { return upc; }
          set { upc = value; }
        }
        public string Isbn
        {
          get { return isbn; }
          set { isbn = value; }
        }
        public string Id
        {
          get { return id; }
          set { id = value; }
        }
        public Decimal? Price
        {
          get { return price; }
          set { price = value; }
        }
        public string ImageLink
        {
          get { return image_link; }
          set { image_link = value; }
        }
        public string ProductType
        {
          get { return product_type; }
          set { product_type = value; }
        }
        public int? Quantity
        {
          get { return quantity; }
          set { quantity = value; }
        }
        public bool IsDiscontinued
        {
          get { return isDiscontinued; }
          set { isDiscontinued = value; }
        }
        public string Brand
        {
            get { return brand; }
            set { brand = value; }
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

        public void Parse(System.Xml.XmlReader xmlReader)
        {
          //track a bit of state on whether or not we are currently reading shipping info, because
          //we need to know to ignore the price, and not confuse with the product's price
          bool currentlyReadingShipping = false;

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                  switch (xmlReader.LocalName)
                  {
                    case "mpn":
                      xmlReader.MoveToContent();
                      mpn = xmlReader.ReadString();
                      break;
                    case "upc":
                      xmlReader.MoveToContent();
                      upc = xmlReader.ReadString();
                      break;
                    case "isbn":
                      xmlReader.MoveToContent();
                      isbn = xmlReader.ReadString();
                      break;
                    case "id":
                    case "ID":
                      xmlReader.MoveToContent();
                      id = xmlReader.ReadString();
                      break;
                    case "image_link":
                      xmlReader.MoveToContent();
                      // only read the image if we don't have one - consider the first image
                      // the primary one.
                      if(String.IsNullOrEmpty(image_link))
                        image_link = xmlReader.ReadString();
                      break;
                    case "product_type":
                      xmlReader.MoveToContent();
                      product_type = xmlReader.ReadString();
                      break;
                    case "quantity":
                      xmlReader.MoveToContent();
                      string q = xmlReader.ReadString();
                      int quantity;
                      if (null != q && Int32.TryParse(q, out quantity))
                        Quantity = quantity;
                      break;
                    case "price":
                      if (!currentlyReadingShipping)
                      {
                        xmlReader.MoveToContent();
                        string pr = xmlReader.ReadString();
                        if (null != pr && ParsePrice(pr).HasValue)
                          price = ParsePrice(pr).Value;
                      }
                      break;
                    case "discontinued":
                      //"discontinued" isn't a proper property of google base products, but it's a concept we require
                      //treat "yes", "true" and "1" all as boolean affirmative.
                      xmlReader.MoveToContent();
                      string d = xmlReader.ReadString();
                      if (null != d)
                      {
                        d = d.Trim();
                        if (d.Equals("1") || 
                            d.Equals("yes", StringComparison.OrdinalIgnoreCase) || 
                            d.Equals("true", StringComparison.OrdinalIgnoreCase))
                          IsDiscontinued = true;
                      }
                      break;
                    case "brand":
                      xmlReader.MoveToContent();
                      brand = xmlReader.ReadString();
                      break;
                    case "shipping":
                      currentlyReadingShipping = true;
                      break;
                  }
                }
                else if (xmlReader.NodeType == XmlNodeType.EndElement)
                {
                  switch (xmlReader.LocalName)
                  {
                    case "shipping":
                      currentlyReadingShipping = false;
                      break;
                  }
                }
            }
        }

        #endregion

        public static decimal? ParsePrice(string priceString)
        {

          if(String.IsNullOrEmpty(priceString))
            return (decimal?)null;

          decimal price;

          // Google Base doesn't provide a country - based on the format of the price
          // string, determine the culture we should use to parse the price.
          System.Globalization.CultureInfo ci = ParsePriceCultureHeuristic( priceString );
          if (decimal.TryParse(priceString, System.Globalization.NumberStyles.Any, ci, out price))
            return price;

          string priceStringAdjusted = priceString.Replace("usd", "").Trim();

          if( decimal.TryParse( priceStringAdjusted, System.Globalization.NumberStyles.Any, ci, out price ) )
            return price;

          return (decimal?)null;

        }

        /// <summary>
        /// Heuristic to guess how we should convert a string into a decimal, which varies by culture. For example, "65,7" should be 65.7 -- this
        /// is how Italians represent the currency decimal. The Google Base Format does not provide the country or the currency explicitly, so
        /// if there is a comma in the price, and it has only one or two digits after it, we assume that when we parse the price, we should use
        /// the number formatting assumptions of the Italian culture.
        /// </summary>
        /// <param name="priceString"></param>
        /// <returns>Culture information which will tell us how to parse this price string.</returns>
        public static System.Globalization.CultureInfo ParsePriceCultureHeuristic( string priceString )
        {
            System.Globalization.CultureInfo priceCulture = System.Globalization.CultureInfo.CurrentCulture;

            // The logic here relies on the assumption that their number of digits after the comma in a US price will never be
            // the same number of digits were it to be a decimal.
            Debug.Assert( euroFormat.NumberFormat.CurrencyDecimalSeparator != priceCulture.NumberFormat.CurrencyDecimalSeparator
                          && euroFormat.NumberFormat.CurrencyDecimalDigits != priceCulture.NumberFormat.CurrencyGroupSizes[0] );
            int euroCommaIndex = priceString.LastIndexOf( euroFormat.NumberFormat.CurrencyDecimalSeparator );
            if( euroCommaIndex >= 0 && euroFormat.NumberFormat.CurrencyDecimalDigits >= ( priceString.Length - ( euroCommaIndex + 1 ) ) )
            {
                priceCulture = euroFormat;
            }
            return priceCulture;
        }
    
    }


}
