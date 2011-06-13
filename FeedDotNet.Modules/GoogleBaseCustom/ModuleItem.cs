using System;
using System.Collections.Generic;
using System.Text;
using FeedDotNet.Common;
using System.Diagnostics;
using System.Xml;
using FeedDotNet.Utilities;

namespace FeedDotNet.Modules.GoogleBaseCustom
{
    [Serializable]
    public class ModuleItem : IModuleItem
    {
  
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string localName = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string video_html = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string keywords = String.Empty;
      
        public ModuleItem(string localName)
        {
            this.localName = localName;
        }

        public string VideoHtml
        {
          get { return video_html; }
          set { video_html = value; }
        }

        public string Keywords
        {
          get { return keywords; }
          set { keywords = value; }
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
                case "video":
                  xmlReader.MoveToContent();
                  video_html = xmlReader.ReadString();
                  break;

                //we don't use this, but versafeed does it
                case "keywords":
                  xmlReader.MoveToContent();
                  keywords = xmlReader.ReadString();
                  break;
              }
            }
          }
        }

        #endregion

        
     
    
    }


}
