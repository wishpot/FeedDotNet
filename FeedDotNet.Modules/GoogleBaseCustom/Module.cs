using System;
using System.Collections.Generic;
using System.Text;
using FeedDotNet.Common;
using System.Diagnostics;
using System.Xml;
using FeedDotNet.Utilities;

namespace FeedDotNet.Modules.GoogleBaseCustom
{
    /// <summary>
    /// http://base.google.com/cns/1.0
    /// </summary>
    /// <remarks>
    /// http://base.google.com/support/bin/answer.py?answer=59558&hl=en
    /// </remarks>
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
                return "Google Base Custom Module";
            }
        }

        public string NS
        {
            get
            {
                return "http://base.google.com/cns/1.0";
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
          while (xmlReader.Read())
          {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
              switch (xmlReader.LocalName)
              {
                case "account_level":
                  xmlReader.MoveToContent();
                  string act = xmlReader.ReadString();
                  Int32.TryParse(act, out account_level);
                  break;
              }
            }
          }
        }

        #endregion
    }
}
