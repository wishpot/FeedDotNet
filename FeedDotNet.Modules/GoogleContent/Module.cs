using System;
using System.Collections.Generic;
using System.Text;
using FeedDotNet.Common;
using System.Diagnostics;
using System.Xml;
using FeedDotNet.Utilities;

namespace FeedDotNet.Modules.GoogleContent
{
    [Serializable]
    public class Module : IModule
    {
        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string localName = String.Empty;

        #region IModule Members

        string IModule.Title
        {
            get { return "Google Content Item"; }
        }

        string IModule.NS
        {
            get { return "http://schemas.google.com/structuredcontent/2009"; }
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
            return new ModuleItem( localName );
        }

        public void Parse( XmlReader xmlReader )
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
