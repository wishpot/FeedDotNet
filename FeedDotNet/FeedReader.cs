/* FeedReader.cs
 * ==========
 * 
 * FeedDotNet (http://www.codeplex.com/FeedDotNet/)
 * Copyright � 2007 Konstantin Gonikman. All Rights Reserved.
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
using System.Net;
using System.IO;
using System.Xml;
using System.Net.Cache;
using FeedDotNet.Modules;

namespace FeedDotNet
{
    public sealed class FeedReader
    {
        private static List<Exception> errors = new List<Exception>();

        private FeedReader()
        {
        }

        public static Feed Read(string uri)
        {
            return Read(uri, null);
        }

        public static Feed Read(string uri, FeedReaderSettings settings)
        {
            Stream stream = null;
            XmlReader xmlReader = null;

            if (settings == null)
                settings = new FeedReaderSettings();

            try
            {
				HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
                
                // Some firewalls allow browser clients only. So let us be a browser ;)
                if(!String.IsNullOrEmpty(settings.HttpUserAgentString))
                    req.UserAgent = settings.HttpUserAgentString;

                // Add Accept string
                if (!String.IsNullOrEmpty(settings.HttpAcceptString))
                    req.Accept = settings.HttpAcceptString;

                // Add custom headers
                foreach (KeyValuePair<HttpRequestHeader, string> kvp in settings.HttpHeaders)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }


				WebResponse resp = req.GetResponse();

				stream = resp.GetResponseStream();

                XmlReaderSettings xmlSettings = new XmlReaderSettings();
                xmlSettings.ProhibitDtd = false;
                xmlReader = XmlReader.Create(stream, xmlSettings);
                Parser parser = null;

                Feed feed = initialize(xmlReader);
                if (feed != null)
                {
                    xmlReader.MoveToContent();

                    switch (feed.Version)
                    {
                        case FeedVersion.Atom03:
                        case FeedVersion.Atom10:
                            parser = new AtomParser(xmlReader.ReadSubtree(), feed);
                            parser.ReadModules = settings.ReadModules;
                            parser.Parse();
                            break;
                        case FeedVersion.RSS090:
                        case FeedVersion.RSS10:                            
                            parser = new RssOldParser(xmlReader.ReadSubtree(), feed);
                            parser.ReadModules = settings.ReadModules;
                            parser.Parse();
                            break;
                        case FeedVersion.RSS091:
                        case FeedVersion.RSS092:
                        case FeedVersion.RSS20:
                            XmlReader subReader20 = xmlReader.ReadSubtree();
                            while (subReader20.Read())
                                if (subReader20.NodeType == XmlNodeType.Element && subReader20.Name == "channel")
                                    break;
                            parser = new RssParser(subReader20.ReadSubtree(), feed);
                            parser.ReadModules = settings.ReadModules;
                            parser.Parse();
                            break;
                    }

                    if (feed.XmlUri == null)
                        feed.XmlUri = new FeedUri(uri);

					if (feed.WebUri == null)
					    feed.WebUri = new FeedUri(String.Empty);
                }

                return feed;
            }
            catch (WebException ex)
            {
                errors.Add(ex);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
            finally
            {
                if (xmlReader != null)
                    xmlReader.Close();

                if (stream != null)
                    stream.Close();
            }

            return null;
        }

        public static Feed Read(Uri uri, FeedReaderSettings settings)
        {
            return Read(uri.ToString(), settings);
        }

        public static Feed Read(Uri uri)
        {
            return Read(uri.ToString(), null);
        }

        private static Feed initialize(XmlReader xmlReader)
        {
            xmlReader.MoveToContent();

            Feed feed = null;

            if (xmlReader.LocalName == "RDF")
            {
                feed = new Feed();

                while (xmlReader.MoveToNextAttribute())
                {
                    if (xmlReader.Name == "xmlns")
                    {
                        switch (xmlReader.Value)
                        {
                            case "http://my.netscape.com/rdf/simple/0.9/": feed.Version = FeedVersion.RSS090; break;
                            case "http://purl.org/rss/1.0/": feed.Version = FeedVersion.RSS10; break;
                            default: feed.Version = FeedVersion.Unknown; break;
                        }
                    }
                    else
                    {
                        IModule module = ModuleManager.Instance.GetModule(xmlReader.Value);
                        if (module != null)
                        {
                            module.LocalName = xmlReader.LocalName;
                            feed.Modules.Add(xmlReader.LocalName, module);
                        }
                    }
                }

                return feed;
            }
            else if (xmlReader.LocalName == "rss")
            {
                feed = new Feed();
                while (xmlReader.MoveToNextAttribute())
                {
                    if (xmlReader.Name == "version")
                    {
                        switch (xmlReader.Value)
                        {
                            case "0.91": feed.Version = FeedVersion.RSS091; break;
                            case "0.92": feed.Version = FeedVersion.RSS092; break;
                            case "2.0": feed.Version = FeedVersion.RSS20; break;
                            default: feed.Version = FeedVersion.Unknown; break;
                        }
                    }
                    else
                    {
                        IModule module = ModuleManager.Instance.GetModule(xmlReader.Value);
                        if (module != null)
                        {
                            module.LocalName = xmlReader.LocalName;
                            feed.Modules.Add(xmlReader.LocalName, module);
                        }
                    }
                }
            }
            else if (xmlReader.LocalName == "feed")
            {
                feed = new Feed();
                while (xmlReader.MoveToNextAttribute())
                {
                    switch (xmlReader.Name)
                    {
                        case "xmlns":
                            switch (xmlReader.Value)
                            {
                                case "http://www.w3.org/2005/Atom": feed.Version = FeedVersion.Atom10; break;
                                case "http://purl.org/atom/ns#": feed.Version = FeedVersion.Atom03; break;
                                default: feed.Version = FeedVersion.Unknown; break;
                            }
                            break;
                        case "xml:lang":
                            feed.Language = xmlReader.Value;
                            break;
                        case "xml:base":
                            //feed._base = new Uri(xmlReader.Value);        // Relative URIs not supported yet
                            break;
                    }
                }
            }

            return feed;
        }

        public static bool HasErrors
        {
            get
            {
                if (errors.Count > 0)
                    return true;
                return false;
            }
        }

        public static List<Exception> GetErrors()
        {
            List<Exception> returnList = new List<Exception>(errors);
            errors.Clear();
            return returnList;
        }
    }
}
