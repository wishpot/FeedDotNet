/* AtomParser.cs
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
using FeedDotNet.Common.Enums;

namespace FeedDotNet
{
    internal class AtomParser : Parser
    {
        public AtomParser(XmlReader xmlReader, Feed feed)
            : base(xmlReader, feed)
        {
        }

        public override void Parse()
        {
            base.Parse();

            //Uri currentBase = new Uri(Feed._base.ToString());

            while (XmlReader.Read())
            {
                if (XmlReader.NodeType == XmlNodeType.Element)
                {
                    //if (XmlReader.HasAttributes)
                    //{
                    //    XmlReader.MoveToAttribute("xml:base");
                    //    currentBase = new Uri(Feed._base, XmlReader.Value);
                    //}

                    switch (XmlReader.Name)
                    {
                        case "title" :
                            XmlReader.MoveToContent();
                            Feed.Title = XmlReader.ReadString();
                            break;
                        case "subtitle":
                            XmlReader.MoveToContent();
                            Feed.Description = XmlReader.ReadString();
                            break;
                        case "icon":
                            XmlReader.MoveToContent();
                            Feed.Icon = XmlReader.ReadString();
                            break;
                        case "logo":
                            XmlReader.MoveToContent();
                            Image image = new Image();
                            image.Uri = XmlReader.ReadString();
                            Feed.Image = image;
                            break;
                        case "id":
                            XmlReader.MoveToContent();
                            Feed.Id = XmlReader.ReadString();
                            break;
                        case "updated":
						case "modified":
                            XmlReader.MoveToContent();
                            Feed.Updated = DTHelper.ParseDateTime(XmlReader.ReadString());
                            break;
                        case "author":
                            XmlReader.MoveToContent();
                            Feed.Authors.Add(readPerson(XmlReader.ReadSubtree()));
                            break;
                        case "contributor":
                            XmlReader.MoveToContent();
                            Feed.Contributors.Add(readPerson(XmlReader.ReadSubtree()));
                            break;
                        case "link":
                            Link link = new Link();
                            while (XmlReader.MoveToNextAttribute())
                            {
                                switch (XmlReader.Name)
                                {
                                    case "href": link.href = XmlReader.Value; break;
                                    case "hreflang": link.hreflang = XmlReader.Value; break;
                                    case "length": link.length = XmlReader.Value; break;
                                    case "rel": link.rel = XmlReader.Value; break;
                                    case "title": link.title = XmlReader.Value; break;
                                    case "type": link.type = XmlReader.Value; break;
                                }
                            }
                            normalizeFeedLink(link);
                            break;
                        case "rights":
                            XmlReader.MoveToContent();
                            Feed.Copyright = XmlReader.ReadString();
                            break;
                        case "category":
                            Category category = new Category();
                            while (XmlReader.MoveToNextAttribute())
                            {
                                switch (XmlReader.Name)
                                {
                                    case "term": category.Term = XmlReader.Value; break;
                                    case "scheme": category.Scheme = XmlReader.Value; break;
                                    case "label": category.Label = XmlReader.Value; break;
                                }
                            }
                            if (category.Label.Length == 0)
                                category.Label = category.Term;
                            Feed.Categories.Add(category);
                            break;
                        case "generator":
                            Generator generator = new Generator();
                            while (XmlReader.MoveToNextAttribute())
                            {
                                switch (XmlReader.Name)
                                {
                                    case "uri": generator.Uri = XmlReader.Value; break;
                                    case "version": generator.Version = XmlReader.Value; break;
                                }
                            }
                            XmlReader.MoveToContent();
                            generator.Name = XmlReader.ReadString();
                            Feed.Generator = generator;
                            break;
                        case "entry":
                            XmlReader.MoveToContent();
                            readEntry(XmlReader.ReadSubtree());
                            break;
                    }
                }
            }

            Complete();
        }

        private static Person readPerson(XmlReader reader)
        {
            Person person = new Person();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "name":
                            reader.MoveToContent();
                            person.Name = reader.ReadString();
                            break;
                        case "email":
                            reader.MoveToContent();
                            person.Email = reader.ReadString();
                            break;
                        case "uri":
                            reader.MoveToContent();
                            person.Uri = reader.ReadString();
                            break;
                    }
                }
            }

            return person;
        }

        private void readEntry(XmlReader subReader)
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
                        case "id":
                            subReader.MoveToContent();
                            FeedGuid guid = new FeedGuid(subReader.ReadString());
                            if(!Uri.IsWellFormedUriString(guid.Id, UriKind.Absolute))
                                guid.PermaLink = false;
                            feedItem.Guid = guid;
                            break;
                        case "content":                            
                            bool contentRead = false;
                            while (subReader.MoveToNextAttribute())
                            {
                                if (subReader.Name == "type")
                                {
                                    if (subReader.Value == "xhtml")
                                    {
                                        subReader.MoveToContent();
                                        feedItem.Content = subReader.ReadInnerXml();
                                        feedItem.ContentType = TextType.Xhtml;
                                        contentRead = true;
                                    }
                                    else if (subReader.Value == "html")
                                    {
                                        feedItem.ContentType = TextType.Html;
                                    }
                                }
                            }
                            if (!contentRead)
                            {
                                subReader.MoveToContent();
                                feedItem.Content = subReader.ReadString();
                            }
                            break;
                        case "summary":
                            bool summaryRead = false;
                            while (subReader.MoveToNextAttribute())
                            {
                                if (subReader.Name == "type")
                                {
                                    if (subReader.Value == "xhtml")
                                    {
                                        subReader.MoveToContent();
                                        feedItem.Summary = subReader.ReadInnerXml();
                                        feedItem.SummaryType = TextType.Xhtml;
                                        summaryRead = true;
                                    }
                                    else if (subReader.Value == "html")
                                    {
                                        feedItem.SummaryType = TextType.Html;
                                    }
                                }
                            }
                            if (!summaryRead)
                            {
                                subReader.MoveToContent();
                                feedItem.Summary = subReader.ReadString();
                            }
                            break;
                        case "modified":
						case "updated":
                            subReader.MoveToContent();
                            feedItem.Updated = DTHelper.ParseDateTime(subReader.ReadString());
                            break;
                        case "published":
						case "created":
                            subReader.MoveToContent();
                            feedItem.Published = DTHelper.ParseDateTime(subReader.ReadString());
                            break;
                        case "author":
                            subReader.MoveToContent();
                            feedItem.Authors.Add(readPerson(subReader.ReadSubtree()));
                            break;
                        case "contributor":
                            subReader.MoveToContent();
                            feedItem.Contributors.Add(readPerson(subReader.ReadSubtree()));
                            break;
                        case "rights":
                            subReader.MoveToContent();
                            feedItem.Copyright = subReader.ReadString();
                            break;
                        case "category":
                            Category category = new Category();
                            while (subReader.MoveToNextAttribute())
                            {
                                switch (subReader.Name)
                                {
                                    case "term": category.Term = subReader.Value; break;
                                    case "scheme": category.Scheme = subReader.Value; break;
                                    case "label": category.Label = subReader.Value; break;
                                }
                            }
                            feedItem.Categories.Add(category);
                            break;
                        case "link":
                            Link link = new Link();
                            while (subReader.MoveToNextAttribute())
                            {
                                switch (subReader.Name)
                                {
                                    case "href": link.href = subReader.Value; break;
                                    case "hreflang": link.hreflang = subReader.Value; break;
                                    case "length": link.length = subReader.Value; break;
                                    case "rel": link.rel = subReader.Value; break;
                                    case "title": link.title = subReader.Value; break;
                                    case "type": link.type = subReader.Value; break;
                                }
                            }
                            //In case someone formats a link as <link>href</link>
                            //we can tolerate that as well.
                            if (String.IsNullOrEmpty(link.href))
                            {
                              subReader.MoveToContent();
                              link.href = subReader.ReadString();
                            }
                            normalizeFeedItemLink(feedItem, link);
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
        
        /// <summary>link elements -> normal structure on Feed</summary>
        private void normalizeFeedLink(Link link)
        {
            switch (link.rel)
            {
                case "related": break;
                case "via": break;
                case "replies": break; // not a standard
                case "enclosure": break; // No Enclosure in feed itself. In feedItems only. (?)
                case "self":
                    Feed.XmlUri = new FeedUri(link.href, link.title);
                    break;
                case "next":
                    Feed.NextUri = new FeedUri(link.href, link.title);
                    break;
                case "prev":
                    Feed.PrevUri = new FeedUri(link.href, link.title);
                    break;
                case "":
                case "alternate":
                    Feed.WebUri = new FeedUri(link.href, link.title);
                    break;
            }
        }

        /// <summary>link elements -> normal structure on FeedItem</summary>
        private static void normalizeFeedItemLink(FeedItem feedItem, Link link)
        {
            switch (link.rel)
            {
                case "related": break;
                case "via":
                    Source source = new Source();
                    source.Uri = link.href;
                    source.Title = link.title;
                    feedItem.Source = source;
                    break;
                case "enclosure":
                    int length = 0;
                    Enclosure enclosure = new Enclosure();
                    enclosure.Uri = link.href;
                    enclosure.Title = link.title;
                    enclosure.Type = link.type;
                    if (Int32.TryParse(link.length, out length))
                        enclosure.Length = length;
                    feedItem.Enclosures.Add(enclosure);
                    break;
                case "self": break; // No link on itself (?)
                case "":
                case "alternate":
                    feedItem.WebUris.Add(new FeedUri(link.href, link.title));
                    break;
            }
        }

        private class Link
        {
            public string rel = String.Empty;
            public string href = String.Empty;
            public string type = String.Empty;
            public string hreflang = String.Empty;
            public string title = String.Empty;
            public string length = String.Empty;
        }
    }
}
