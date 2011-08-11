using System;
using System.Collections.Generic;
using System.Text;
using FeedDotNet.Common;
using System.Diagnostics;
using System.Xml;
using FeedDotNet.Utilities;

namespace FeedDotNet.Modules.Shopify
{
    /// <summary>
    /// http://jadedpixel.com/-/spec/shopify
    /// </summary>
    [Serializable]
    public class Module : IModule
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string localName = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int account_level = 0;

        #region IModule Members

        public string Title
        {
            get 
            {
                return "Shopify";
            }
        }

        public string NS
        {
            get
            {
              return "http://jadedpixel.com/-/spec/shopify";
            }
        }

        public string LocalName
        {
            get { return localName; }
            set  {  localName = value; }
        }

        public int AccountLevel
        {
          get { return account_level; }
          set { account_level = value; }
        }

        public IModuleItem CreateModuleItem()
        {
            return new ModuleItem(localName);
        }

        public void Parse(XmlReader xmlReader)
        {
          //NO-OP
        }

        #endregion
    }
}
