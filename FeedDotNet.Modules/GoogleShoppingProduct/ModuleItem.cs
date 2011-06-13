using System;
using System.Collections.Generic;
using System.Text;
using FeedDotNet.Common;
using System.Diagnostics;
using System.Xml;
using FeedDotNet.Utilities;


namespace FeedDotNet.Modules.GoogleShoppingProduct
{
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
        private List<String> image_links = new List<String>();

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string product_type = String.Empty;

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

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
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
        public string Link
        {
            get { return link; }
            set { link = value; }
        }
        public string Availability
        {
            get { return availability; }
            set { availability = value; }
        }
        public string Gtin
        {
            get { return gtin; }
            set { gtin = value; }
        }

        /// <summary>
        /// Not documented as of yet in the Google Shopping API
        /// https://groups.google.com/group/google-search-api-for-shopping/browse_thread/thread/5ae59ca123460564/2e5613731a28a12a#2e5613731a28a12a
        /// </summary>
        public enum AvailabilityOptions
        {
            Unknown,
            Limited,
            OutOfStock,
            InStock
        }

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
        /// documentation on the return format.
        /// </summary>
        /// <param name="subTree"></param>
        public void Parse( System.Xml.XmlReader subTree )
        {

            while( subTree.Read() )
            {
                if( subTree.NodeType == XmlNodeType.Element )
                {
                    // This here won't work.
                    switch( subTree.LocalName )
                    {
                        case "googleId":
                            // Override the Id of the item 
                            subTree.MoveToContent();
                            this.Id = subTree.ReadString();
                            break;
                        case "title":
                            subTree.MoveToContent();
                            title = subTree.ReadString();
                            break;
                        case "gtin":
                            subTree.MoveToContent();
                            gtin = subTree.ReadString();
                            break;
                        case "description":
                            subTree.MoveToContent();
                            description = subTree.ReadString();
                            break;
                        case "brand":
                            subTree.MoveToContent();
                            brand = subTree.ReadString();
                            break;
                        case "link":
                            subTree.MoveToContent();
                            this.Link = subTree.ReadString();
                            break;
                        // Subnode of "inventories". There may be multiple.  Currently our logic
                        // will just support one.
                        case "inventory":
                            subTree.MoveToContent();
                            if( subTree.HasAttributes )
                            {
                                this.availability = subTree.GetAttribute( "availability" );
                            }
                            break;
                        // Subnode of "inventory". There may be multiple.  Currently our logic
                        // will just support one.
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
                        // Subnode of "images". There may be multiple.  Our logic will compile
                        // them all, though we don't know which is good.
                        case "image":
                            subTree.MoveToContent();
                            String imageLink = subTree.GetAttribute( "link" );
                            if( !String.IsNullOrEmpty( imageLink ) )
                            {
                                this.ImageLinks.Add( imageLink );
                            }
                            break;
                    }
                }
            }
        }

#endregion

    }
}
