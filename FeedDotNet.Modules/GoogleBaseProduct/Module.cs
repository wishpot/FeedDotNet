using System;
using System.Collections.Generic;
using System.Text;
using FeedDotNet.Common;
using System.Diagnostics;
using System.Xml;
using FeedDotNet.Utilities;

namespace FeedDotNet.Modules.GoogleBaseProduct
{
    /// <summary>
    /// http://base.google.com/ns/1.0
    /// </summary>
    [Serializable]
    public class Module : IModule
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string localName = String.Empty;


        #region IModule Members

        public string Title
        {
            get 
            {
                return "Google Base Product Module";
            }
        }

        public string NS
        {
            get
            {
                return "http://base.google.com/ns/1.0";
            }
        }

        public string LocalName
        {
            get
            {
                return localName;
            }
            set
            {
                localName = value;
            }
        }

        public IModuleItem CreateModuleItem()
        {
            return new ModuleItem(localName);
        }

        public void Parse(XmlReader xmlReader)
        {
          throw new NotImplementedException();
        }

        #endregion
    }
}
