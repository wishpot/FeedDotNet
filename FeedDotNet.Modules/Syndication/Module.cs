/* Module.cs
 * ==========
 * 
 * FeedDotNet (http://www.codeplex.com/FeedDotNet/)
 * Copyright © 2007 Konstantin Gonikman. All Rights Reserved.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining 
 * a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using FeedDotNet.Common;
using System.Xml;
using System.Diagnostics;
using FeedDotNet.Utilities;

namespace FeedDotNet.Modules.Syndication
{
    /// <summary>
    /// http://purl.org/rss/1.0/modules/syndication/
    /// </summary>
    [Serializable]
    public class Module : IModule
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string localName = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UpdatePeriod updatePeriod = UpdatePeriod.Daily;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int updateFrequency = 1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DateTime? updateBase = null;


        public UpdatePeriod UpdatePeriod
        {
            get { return updatePeriod; }
            set { updatePeriod = value; }
        }

        public int UpdateFrequency
        {
            get { return updateFrequency; }
            set { updateFrequency = value; }
        }

        public DateTime? UpdateBase
        {
            get { return updateBase; }
            set { updateBase = value; }
        }

        #region IModule Members

        public string Title
        {
            get
            {
                return "Syndication Module";
            }
        }

        public string NS
        {
            get
            {
                return "http://purl.org/rss/1.0/modules/syndication/";
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
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.LocalName)
                    {
                        case "updatePeriod":
                            xmlReader.MoveToContent();
                            updatePeriod = (UpdatePeriod)Enum.Parse(typeof(UpdatePeriod), xmlReader.ReadString(), true);
                            break;
                        case "updateFrequency":
                            xmlReader.MoveToContent();
                            updateFrequency = Int32.Parse(xmlReader.ReadString());
                            break;
                        case "updateBase":
                            xmlReader.MoveToContent();
                            updateBase = DTHelper.ParseDateTime(xmlReader.ReadString());
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
