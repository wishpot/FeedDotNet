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
using FeedDotNet.Utilities;

namespace FeedDotNet.Modules.DublinCore
{
    [Serializable]
    public class ModuleItem : IModuleItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string localName = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string title = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string creator = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string subject = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string description = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string publisher = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string contributor = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DateTime? date;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string type = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string format = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string identifier = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string source = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string language = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string relation = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string coverage = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string rights = String.Empty;

        public string TitleDC
        {
            get { return title; }
            set { title = value; }
        }
        public string Creator
        {
            get { return creator; }
            set { creator = value; }
        }
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string Publisher
        {
            get { return publisher; }
            set { publisher = value; }
        }
        public string Contributor
        {
            get { return contributor; }
            set { contributor = value; }
        }
        public DateTime? Date
        {
            get { return date; }
            set { date = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public string Format
        {
            get { return format; }
            set { format = value; }
        }
        public string Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }
        public string Source
        {
            get { return source; }
            set { source = value; }
        }
        public string Language
        {
            get { return language; }
            set { language = value; }
        }
        public string Relation
        {
            get { return relation; }
            set { relation = value; }
        }
        public string Coverage
        {
            get { return coverage; }
            set { coverage = value; }
        }
        public string Rights
        {
            get { return rights; }
            set { rights = value; }
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
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.LocalName)
                    {
                        case "title":
                            xmlReader.MoveToContent();
                            title = xmlReader.ReadString();
                            break;
                        case "creator":
                            xmlReader.MoveToContent();
                            creator = xmlReader.ReadString();
                            break;
                        case "subject":
                            xmlReader.MoveToContent();
                            subject = xmlReader.ReadString();
                            break;
                        case "description":
                            xmlReader.MoveToContent();
                            description = xmlReader.ReadString();
                            break;
                        case "publisher":
                            xmlReader.MoveToContent();
                            publisher = xmlReader.ReadString();
                            break;
                        case "contributor":
                            xmlReader.MoveToContent();
                            contributor = xmlReader.ReadString();
                            break;
                        case "date":
                            xmlReader.MoveToContent();
                            date = DTHelper.ParseDateTime(xmlReader.ReadString());
                            break;
                        case "type":
                            xmlReader.MoveToContent();
                            type = xmlReader.ReadString();
                            break;
                        case "format":
                            xmlReader.MoveToContent();
                            format = xmlReader.ReadString();
                            break;
                        case "identifier":
                            xmlReader.MoveToContent();
                            identifier = xmlReader.ReadString();
                            break;
                        case "source":
                            xmlReader.MoveToContent();
                            source = xmlReader.ReadString();
                            break;
                        case "language":
                            xmlReader.MoveToContent();
                            language = xmlReader.ReadString();
                            break;
                        case "relation":
                            xmlReader.MoveToContent();
                            relation = xmlReader.ReadString();
                            break;
                        case "coverage":
                            xmlReader.MoveToContent();
                            coverage = xmlReader.ReadString();
                            break;
                        case "rights":
                            xmlReader.MoveToContent();
                            rights = xmlReader.ReadString();
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
