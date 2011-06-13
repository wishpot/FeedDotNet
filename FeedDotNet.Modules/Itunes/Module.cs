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
using System.Diagnostics;
using System.Xml;

namespace FeedDotNet.Modules.Itunes
{
    /// <summary>
    /// http://www.apple.com/itunes/store/podcaststechspecs.html
    /// </summary>
	[Serializable]
    public class Module : IModule
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string localName = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string subtitle = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string author = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string summary = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string image = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Explicit _explicit = Explicit.No;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Owner owner;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool block;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Category> categories = new List<Category>();

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

        public string Image
        {
            get { return image; }
            set { image = value; }
        }

        public List<Category> Categories
        {
            get { return categories; }
        }

        public Explicit Explicit
        {
            get { return _explicit; }
            set { _explicit = value; }
        }

        public Owner Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        #region IModule Members

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

        public string Title
        {
            get
            {
                return "ITunes Podcasting Module";
            }
        }

        public string NS
        {
            get
            {
                return "http://www.itunes.com/dtds/podcast-1.0.dtd";
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
                        // TODO: somewhere is a bug....
                        case "category":
                            xmlReader.MoveToAttribute("text");
                            Category category = new Category(xmlReader.Value);
                            if (!xmlReader.IsEmptyElement)
                            {
                                while (xmlReader.Read())
                                {
                                    if (xmlReader.NodeType == XmlNodeType.Element)
                                    {
                                        xmlReader.MoveToAttribute("text");
                                        category.SubCategories.Add(new Category(xmlReader.Value));
                                    }
                                }
                            }
                            categories.Add(category);
                            break;
                        case "subtitle":
                            xmlReader.MoveToContent();
                            subtitle = xmlReader.ReadString();
                            break;
                        case "summary":
                            xmlReader.MoveToContent();
                            summary = xmlReader.ReadString();
                            break;
                        case "block":
                            xmlReader.MoveToContent();
                            if (String.Compare(xmlReader.ReadString(), "yes", true) == 0)
                                block = true;
                            break;
                        case "author":
                            xmlReader.MoveToContent();
                            author = xmlReader.ReadString();
                            break;
                        case "explicit":
                            xmlReader.MoveToContent();
                            string str = xmlReader.ReadString();
                            if (str == "yes")
                                _explicit = Explicit.Yes;
                            else if (str == "clean")
                                _explicit = Explicit.Clean;
                            break;
                        case "image":
                            xmlReader.MoveToAttribute("href");
                            image = xmlReader.Value;
                            break;
                        case "owner":
                            owner = new Owner();
                            while (xmlReader.Read())
                            {
                                if (xmlReader.NodeType == XmlNodeType.Element)
                                {
                                    if(xmlReader.LocalName == "name")
                                        owner.Name = xmlReader.ReadString();
                                    else if (xmlReader.LocalName == "email")
                                        owner.Email = xmlReader.ReadString();
                                }
                            }
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
