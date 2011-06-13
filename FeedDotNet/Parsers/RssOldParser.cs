/* RssOldParser.cs
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
using System.Xml;
using FeedDotNet.Common;
using FeedDotNet.Utilities;

namespace FeedDotNet
{
    internal class RssOldParser : Parser
    {
        private string imageRsx = String.Empty;
        private List<string> itemsRsx = new List<string>();

        public RssOldParser(XmlReader xmlReader, Feed feed)
            : base(xmlReader, feed)
        {
        }

        public override void Parse()
        {
            base.Parse();

            while (XmlReader.Read())
                if (XmlReader.NodeType == XmlNodeType.Element && XmlReader.Name == "channel")
                    break;

            while (XmlReader.Read())
            {
                if (XmlReader.Depth == 2)
                {
                    if (XmlReader.NodeType == XmlNodeType.Element)
                    {
                        switch (XmlReader.Name)
                        {
                            case "title":
                                XmlReader.MoveToContent();
                                Feed.Title = XmlReader.ReadString();
                                break;
                            case "description":
                                XmlReader.MoveToContent();
                                Feed.Description = XmlReader.ReadString();
                                break;
                            case "link":
                                XmlReader.MoveToContent();
                                Feed.WebUri = new FeedUri(XmlReader.ReadString());
                                break;
                            case "image":
                                while (XmlReader.MoveToNextAttribute())
                                {
                                    if (XmlReader.LocalName == "resource")
                                        imageRsx = XmlReader.Value;
                                }
                                break;
                            case "items":
                                XmlReader.MoveToContent();
                                readItemsResx(XmlReader.ReadSubtree());                                
                                break;
                            default:
                                if (readModules && XmlReader.Prefix != String.Empty)
                                {
                                    IModule module = Feed.GetModule(XmlReader.Prefix);
                                    if (module != null)
                                        module.Parse(XmlReader.ReadSubtree());
                                }
                                break;
                        }
                    }
                }
                else if (XmlReader.Depth == 1)
                {
                    if (XmlReader.NodeType == XmlNodeType.Element)
                    {
                        switch (XmlReader.Name)
                        {
                            case "item":
                                XmlReader.MoveToContent();

                                if (Feed.Version == FeedVersion.RSS10)
                                {
                                    while (XmlReader.MoveToNextAttribute())
                                    {
                                        if (XmlReader.LocalName == "about" && itemsRsx.Contains(XmlReader.Value)) // Take only items, listened under <items><rdf:Seq>.. 
                                        {
                                            XmlReader.MoveToContent();
                                            readItem(XmlReader.ReadSubtree());
                                        }
                                    }
                                }
                                else if (Feed.Version == FeedVersion.RSS090)
                                {
                                    readItem(XmlReader.ReadSubtree());
                                }
                                break;
                            case "image":
                                XmlReader.MoveToContent();
                                while (XmlReader.MoveToNextAttribute())
                                {
                                    if (XmlReader.LocalName == "about" && String.Equals(imageRsx, XmlReader.Value)) // Take only items, listened under <items><rdf:Seq>.. 
                                    {
                                        XmlReader.MoveToContent();
                                        Feed.Image = readImage(XmlReader.ReadSubtree());
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            Complete();
        }

        private void readItemsResx(XmlReader xmlReader)
        {
            xmlReader.MoveToContent();

            XmlReader subReader = xmlReader.ReadSubtree();
            while (subReader.Read())
            {
                if (subReader.Depth == 2 && subReader.LocalName == "li")
                {
                    while (subReader.MoveToNextAttribute())
                    {
                        if (subReader.LocalName == "resource")
                            itemsRsx.Add(subReader.Value);
                    }
                }
            }
        }

        private void readItem(XmlReader subReader)
        {
            subReader.MoveToContent();
            FeedItem feedItem = new FeedItem();

            while (subReader.Read())
            {
                if (subReader.NodeType == XmlNodeType.Element)
                {
                    switch (subReader.Name)
                    {
                        case "title":
                            subReader.MoveToContent();
                            feedItem.Title = subReader.ReadString();
                            break;
                        case "link":
                            subReader.MoveToContent();
                            FeedUri uri = new FeedUri(subReader.ReadString());
                            feedItem.WebUris.Add(uri);
                            break;
                        case "description":
                            subReader.MoveToContent();
                            try
                            {
                                // One feed had a hex character that couldnt be read with XmlReader.
                                // TODO: Build a function TryReadString(XmlReader) for all string elements?...
                                feedItem.Content = subReader.ReadString();
                            }
                            catch { }
                            break;
                        default:
                            if (readModules && subReader.Prefix != String.Empty)
                                readModuleItem(subReader, feedItem);
                            break;
                    }
                }
            }

            Feed.Items.Add(feedItem);
        }

        private Image readImage(XmlReader subReader)
        {
            subReader.MoveToContent();
            Image image = new Image();

            while (subReader.Read())
            {
                if (subReader.NodeType == XmlNodeType.Element)
                {
                    switch (subReader.Name)
                    {
                        case "title":
                            subReader.MoveToContent();
                            image.Title = subReader.ReadString();
                            break;
                        case "link":
                            subReader.MoveToContent();
                            image.Link = subReader.ReadString();
                            break;
                        case "url":
                            subReader.MoveToContent();
                            image.Uri = subReader.ReadString();
                            break;
                    }
                }
            }

            return image;
        }

        private new void readModuleItem(XmlReader subReader, FeedItem feedItem)
        {
            IModuleItem moduleItem = feedItem.GetModuleItem(subReader.Prefix);
            if (moduleItem != null)
            {
                moduleItem.Parse(subReader.ReadSubtree());
            }
            else
            {
                IModule module = Feed.GetModule(subReader.Prefix);
                if (module != null)
                {
                    IModuleItem mi = module.CreateModuleItem();
                    if (mi != null)
                    {
                        mi.Parse(subReader.ReadSubtree());
                        feedItem.ModuleItems.Add(subReader.Prefix, mi);
                    }
                }
            }
        }
    }
}
