using System;
using System.Collections.Generic;
using System.Text;
using FeedDotNet.Common;
using System.Diagnostics;
using System.Xml;
using FeedDotNet.Utilities;


namespace FeedDotNet.Modules.GoogleContent
{
    /// <summary>
    /// This class is written to support the Format: https://content.googleapis.com/content/v1/{0}/items/products/generic
    /// There is another Format: https://content.googleapis.com/content/v1/{0}/items/products/schema. We will succeed at parsing
    /// this format, but a lot of data will be missing - anything that we current pull from <sc:attribute>
    /// See http://code.google.com/apis/shopping/content/getting-started/
    /// </summary>
    [Serializable]
    public class ModuleItem : IModuleItem
    {
        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string localName = String.Empty;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string title = String.Empty;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string description = String.Empty;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string id = String.Empty;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private Decimal? price = null;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private Decimal? sale_price = null;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private List<String> image_links = new List<String>();

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string product_type = String.Empty;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private List<String> keywords = new List<String>();

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private int? quantity = null;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private String currency = String.Empty;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string brand = String.Empty;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string link = String.Empty;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string availability = String.Empty;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string gtin = String.Empty;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string mpn = String.Empty;

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
        public Decimal? SalePrice
        {
            get { return sale_price; }
            set { sale_price = value; }
        }
        public List<String> ImageLinks
        {
            get { return image_links; }
            set { image_links = value; }
        }
        public string ProductType
        {
            get { return product_type; }
            set { product_type = value; }
        }
        public List<String> Keywords
        {
            get { return keywords; }
            set { keywords = value; }
        }
        public int? Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public String Currency
        {
            get { return currency; }
            set { currency = value; }
        }
        public string Brand
        {
            get { return brand; }
            set { brand = value; }
        }
        public string Availability
        {
            get { return availability; }
            set { availability = value; }
        }
        public string Mpn
        {
            get { return mpn; }
            set { mpn = value; }
        }

        // See http://code.google.com/apis/shopping/content/getting-started/requirements-products.html#attributes_xml
        public const String InStock = "in stock";
        public const String OutOfStock = "out of stock";
        public const String LimitedStock = "limited availability";
        public const String AvailableOrder = "available for order";
        public const String PreOrder = "preorder";

        public ModuleItem( string localName )
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
        /// See http://code.google.com/apis/shopping/search/v1/reference-response-format.html#product-resource for
        /// documentation on the return format, and http://www.google.com/support/merchants/bin/answer.py?answer=188494
        /// for information on all the fields.
        /// </summary>
        /// <param name="subTree"></param>
        public void Parse( System.Xml.XmlReader subTree )
        {
            while( subTree.Read() )
            {
                if( subTree.NodeType == XmlNodeType.Element )
                {
                    switch( subTree.LocalName )
                    {
                        // If this is provided, override the id of the item. Google Content provides the vendor's ID/SKU
                        // which we need for Whitelabels.
                        case "id":
                            subTree.MoveToContent();
                            this.Id = subTree.ReadString();
                            break;
                        case "image_link":
                        case "additional_image_link":
                            subTree.MoveToContent();
                            String imageLink = subTree.ReadString();
                            if( !String.IsNullOrEmpty( imageLink ) )
                            {
                                this.ImageLinks.Add( imageLink );
                            }
                            break;
                        // Product attributes can be specified in generic or schema. The former, the /generic format, 
                        // they are all different attributes.  For the later (not supported here), they are in the "scp" namespace
                        // instead of "sc"
                        case "attribute":
                            subTree.MoveToContent();
                            ParseAttribute( subTree );
                            break;
                        // TODO "target_country" - do we support products in multiple countries for a given vendor though?       
                    }
                }
            }
        }

        protected void ParseAttribute( System.Xml.XmlReader subTree )
        {
            String attribName = subTree.GetAttribute( "name" );
            if( !String.IsNullOrEmpty( attribName ) )
            {
                switch( attribName )
                {
                    case "brand":
                        this.brand = subTree.ReadString();
                        break;
                    // Ignore the sale currency. It better be the same as the price.
                    case "sale price":
                        String strSalePrice = subTree.ReadString();
                        if( !String.IsNullOrEmpty( strSalePrice ) )
                        {
                            decimal tempPrice;
                            if( Decimal.TryParse( strSalePrice, out tempPrice ) )
                            {
                                this.SalePrice = tempPrice;
                            }
                        }
                        break;
                    case "price":
                        subTree.MoveToContent();
                        if( subTree.HasAttributes )
                        {
                            this.currency = subTree.GetAttribute( "currency" );
                        }
                        String strPrice = subTree.ReadString();
                        if( !String.IsNullOrEmpty( strPrice ) )
                        {
                            decimal tempPrice;
                            if( Decimal.TryParse( strPrice, out tempPrice ) )
                            {
                                this.Price = tempPrice;
                            }
                        }
                        break;
                    case "availability":
                        this.availability = subTree.ReadString();
                        break;
                    // TODO this is the google category & taxonomy. This would be valuable to use at somepoint, since
                    // we could create categories in our DB to match.
                    case "google product category":
                        break;
                    // product_type - this is the vendor's category versus the equivalent google category
                    // For now, we will prefer this one since it is what the vendor uses on their website.
                    case "product type":
                        this.product_type = subTree.ReadString();
                        break;
                    // Treat all of these items are keywords.
                    case "keywords":
                    case "gender":
                    case "materials":
                    case "color":
                    case "age group":
                    case "size":
                        this.keywords.Add( subTree.ReadString() );
                        break;
                    case "gtin":
                        gtin = subTree.ReadString();
                        break;
                    case "mpn":
                        mpn = subTree.ReadString();
                        break;
                    // TODO Support Item variants with item_group_id!
                    // TODO Use the shipping tags! 
                }
            }
        }
        #endregion

    }
}
