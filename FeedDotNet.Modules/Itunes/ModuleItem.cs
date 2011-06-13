/* ModuleItem.cs
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
using System.Diagnostics;
using System.Xml;

namespace FeedDotNet.Modules.Itunes
{
	[Serializable]
    public class ModuleItem : IModuleItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string author = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string summary = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string subtitle = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string localName = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string keywords = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Explicit _explicit = Explicit.No;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TimeSpan duration;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool block;

        public TimeSpan Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public string Author
        {
            get { return author; }
            set { author = value; }
        }

        public bool Block
        {
            get { return block; }
            set { block = value; }
        }

        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }

        public string Subtitle
        {
            get { return subtitle; }
            set { subtitle = value; }
        }

        public string Keywords
        {
            get { return keywords; }
            set { keywords = value; }
        }

        public Explicit Explicit
        {
            get { return _explicit; }
            set { _explicit = value; }
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

        public void Parse(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.LocalName)
                    {
                        case "summary":
                            xmlReader.MoveToContent();
                            summary = xmlReader.ReadString();
                            break;
                        case "subtitle":
                            xmlReader.MoveToContent();
                            subtitle = xmlReader.ReadString();
                            break;
                        case "author":
                            xmlReader.MoveToContent();
                            author = xmlReader.ReadString();
                            break;
                        case "block":
                            xmlReader.MoveToContent();
                            if (String.Compare(xmlReader.ReadString(), "yes", true) == 0)
                                block = true;
                            break;
                        case "keywords":
                            xmlReader.MoveToContent();
                            keywords = xmlReader.ReadString();
                            break;
                        case "explicit":
                            xmlReader.MoveToContent();
                            string str = xmlReader.ReadString();
                            if (str == "yes")
                                _explicit = Explicit.Yes;
                            else if (str == "clean")
                                _explicit = Explicit.Clean;
                            break;
                        case "duration":
                            xmlReader.MoveToContent();
                            TimeSpan.TryParse(xmlReader.ReadString(), out duration); // BUG! minutes are read as hours and so on...
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
