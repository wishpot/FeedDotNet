/* RssParser.cs
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
    internal class RssParser : Parser
    {
        public RssParser(XmlReader xmlReader, Feed feed)
            : base(xmlReader, feed)
        {
        }

        public override void Parse()
        {
            base.Parse();

            while (XmlReader.Read())
            {
                if (XmlReader.Depth == 1)
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
                            case "language":
                                XmlReader.MoveToContent();
                                Feed.Language = XmlReader.ReadString();
                                break;
                            case "copyright":
                                XmlReader.MoveToContent();
                                Feed.Copyright = XmlReader.ReadString();
                                break;
                            case "managingEditor":
                                XmlReader.MoveToContent();
                                Feed.Authors.Add(readPerson(XmlReader.ReadString()));
                                break;
                            case "webMaster":
                                XmlReader.MoveToContent();
                                Feed.WebMaster = readPerson(XmlReader.ReadString());
                                break;
                            case "pubDate":
                                XmlReader.MoveToContent();
                                Feed.Published = DTHelper.ParseDateTime(XmlReader.ReadString());
                                break;
                            case "lastBuildDate":
                                XmlReader.MoveToContent();
                                Feed.Updated = DTHelper.ParseDateTime(XmlReader.ReadString());
                                break;
                            case "category":
                                Category category = new Category();
                                while (XmlReader.MoveToNextAttribute())
                                {
                                    if (XmlReader.Name == "domain")
                                        category.Scheme = XmlReader.Value;
                                }
                                XmlReader.MoveToContent();
                                category.Label = category.Term = XmlReader.ReadString();
                                Feed.Categories.Add(category);
                                break;
                            case "generator":
                                Generator generator = new Generator();
                                XmlReader.MoveToContent();
                                generator.Name = XmlReader.ReadString();
                                if (Uri.IsWellFormedUriString(generator.Name, UriKind.Absolute))
                                    generator.Uri = generator.Name;
                                Feed.Generator = generator;
                                break;
                            case "docs":
                                XmlReader.MoveToContent();
                                Feed.RssDocumentation = XmlReader.ReadString();
                                break;
                            case "cloud":
                                Cloud cloud = new Cloud();
                                int port = 0;
                                while (XmlReader.MoveToNextAttribute())
                                {
                                    switch (XmlReader.Name)
                                    {
                                        case "domain": cloud.Domain = XmlReader.Value; break;
                                        case "path": cloud.Path = XmlReader.Value; break;
                                        case "port":
                                            Int32.TryParse(XmlReader.Value, out port);
                                            cloud.Port = port;
                                            break;
                                        case "protocol": cloud.Protocol = XmlReader.Value; break;
                                        case "registerProcedure": cloud.RegisterProcedure = XmlReader.Value; break;
                                    }
                                }
                                Feed.Cloud = cloud;
                                break;
                            case "ttl":
                                int ttl = 0;
                                XmlReader.MoveToContent();
                                Int32.TryParse(XmlReader.ReadString(), out ttl);
                                Feed.Ttl = ttl;
                                break;
                            case "image":
                                XmlReader.MoveToContent();
                                Feed.Image = readImage(XmlReader.ReadSubtree());
                                break;
                            case "rating": break; // PICS is not yet implemented
                            case "textInput": break; // The purpose of the <textInput> element is something of a mystery
                            case "skipHours":
                                XmlReader.MoveToContent();
                                short hour = 0;
                                if (Int16.TryParse(XmlReader.ReadString(), out hour))
                                    Feed.SkipHours.Add(hour);
                                break;
                            case "skipDays":
                                XmlReader.MoveToContent();
                                switch (XmlReader.ReadString())
                                {
                                    case "Monday": Feed.SkipDays.Add(Days.Monday); break;
                                    case "Tuesday": Feed.SkipDays.Add(Days.Tuesday); break;
                                    case "Wednesday": Feed.SkipDays.Add(Days.Wednesday); break;
                                    case "Thursday": Feed.SkipDays.Add(Days.Thursday); break;
                                    case "Friday": Feed.SkipDays.Add(Days.Friday); break;
                                    case "Saturday": Feed.SkipDays.Add(Days.Saturday); break;
                                    case "Sunday": Feed.SkipDays.Add(Days.Sunday); break;
                                }
                                break;
                            case "item":
                                XmlReader.MoveToContent();
                                readItem(XmlReader.ReadSubtree());
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
            }

            Complete();
        }

        private string m_CurrentNode = null;
        internal override string CurrentNodeBeingParsed
        {
          get
          {
            return m_CurrentNode;
          }
        }

        private FeedItem m_CurrentFeedItem = null;
        internal override FeedItem CurrentFeedItemBeingParsed
        {
          get
          {
            return m_CurrentFeedItem;
          }
        }

        private void readItem(XmlReader subReader)
        {
            subReader.MoveToContent();
            m_CurrentFeedItem = new FeedItem();

            while (subReader.Read())
            {
                if (subReader.NodeType == XmlNodeType.Element)
                {
                      m_CurrentNode = subReader.Name;
                      switch (m_CurrentNode)
                        {
                            case "title":
                                subReader.MoveToContent();
                                m_CurrentFeedItem.Title = subReader.ReadString();
                                break;
                            case "description":
                                subReader.MoveToContent();
                                try
                                {
                                    // One feed had a hex character / & that couldnt be read with XmlReader.
                                  m_CurrentFeedItem.Content = subReader.ReadString();
                                }
                                catch(Exception ex)
                                {
                                  System.Diagnostics.Debug.WriteLine("Error reading the description of a feed item.  Ignored usually: "+ex.Message);
                                }
                                break;
                            case "link":
                                subReader.MoveToContent();
                                FeedUri uri = new FeedUri(subReader.ReadString());
                                m_CurrentFeedItem.WebUris.Add(uri);
                                break;
                            case "author":
                                subReader.MoveToContent();
                                m_CurrentFeedItem.Authors.Add(readPerson(subReader.ReadString()));
                                break;
                            case "category":
                                Category category = new Category();
                                while (subReader.MoveToNextAttribute())
                                {
                                    if (subReader.Name == "domain")
                                        category.Scheme = subReader.Value;
                                }
                                subReader.MoveToContent();
                                category.Label = category.Term = subReader.ReadString();
                                m_CurrentFeedItem.Categories.Add(category);
                                break;
                            case "comments":
                                subReader.MoveToContent();
                                m_CurrentFeedItem.CommentsUri = subReader.ReadString();
                                break;
                            case "enclosure":
                                Enclosure enclosure = new Enclosure();
                                while (subReader.MoveToNextAttribute())
                                {
                                    switch (subReader.Name)
                                    {
                                        case "url": enclosure.Uri = subReader.Value; break;
                                        case "length":
                                            int length = 0;
                                            if (Int32.TryParse(subReader.Value, out length))
                                                enclosure.Length = length;
                                            break;
                                        case "type": enclosure.Type = subReader.Value; break;
                                    }
                                }
                                m_CurrentFeedItem.Enclosures.Add(enclosure);
                                break;
                            case "guid":
                                FeedGuid guid = new FeedGuid();
                                if (subReader.MoveToFirstAttribute())
                                {
                                    if (subReader.Name == "isPermaLink" && subReader.Value == "false")
                                        guid.PermaLink = false;
                                }
                                subReader.MoveToContent();
                                guid.Id = subReader.ReadString();
                                m_CurrentFeedItem.Guid = guid;
                                break;
                            case "pubDate":
                                subReader.MoveToContent();
                                m_CurrentFeedItem.Published = DTHelper.ParseDateTime(subReader.ReadString());
                                break;
                            case "source":
                                Source source = new Source();
                                if (subReader.MoveToFirstAttribute())
                                {
                                    if (subReader.Name == "url")
                                        source.Uri = subReader.Value;
                                }
                                subReader.MoveToContent();
                                source.Title = subReader.ReadString();
                                m_CurrentFeedItem.Source = source;
                                break;
                            default:
                                if (readModules && subReader.Prefix != String.Empty)
                                  readModuleItem(subReader, m_CurrentFeedItem);
                                break;
                        }

                    }
            }

            Feed.Items.Add(m_CurrentFeedItem);
        }

        private static Person readPerson(string str)
        {
            Person person = new Person();

            if (str.Contains("@") && str.Contains(".") && !str.Contains(" "))
            {
                person.Name = str;
                person.Email = str;
            }
            else
                person.Name = str;

            return person;
        }

        private Image readImage(XmlReader reader)
        {
            Image image = new Image();
            int previosDepth = -1;

            while (reader.Read())
            {
                if (XmlReader.Depth == 2)
                {
                    previosDepth = XmlReader.Depth;

                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "url":
                                reader.MoveToContent();
                                image.Uri = reader.ReadString();
                                break;
                            case "title":
                                reader.MoveToContent();
                                image.Title = reader.ReadString();
                                break;
                            case "description":
                                reader.MoveToContent();
                                image.Description = reader.ReadString();
                                break;
                            case "link":
                                reader.MoveToContent();
                                image.Link = reader.ReadString();
                                break;
                            case "width":
                                int width = 88;
                                reader.MoveToContent();
                                Int32.TryParse(reader.ReadString(), out width);
                                image.Width = width;
                                break;
                            case "height":
                                int height = 31;
                                reader.MoveToContent();
                                Int32.TryParse(reader.ReadString(), out height);
                                image.Height = height;
                                break;
                        }
                    }
                }
                else if (previosDepth == 2)
                    break;
            }

            return image;
        }
    }
}
