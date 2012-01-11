/* FeedReader.cs
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

        public static Feed Read(string uri, FeedReaderSettings settings=null)
        {
            Stream stream = null;
            

            if (settings == null)
                settings = new FeedReaderSettings();

            try
            {
				      HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);

              if (settings.HttpTimeout.HasValue)
                req.Timeout = settings.HttpTimeout.Value;
                
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

              if (settings.FeedCredentials != null)
                req.Credentials = settings.FeedCredentials;

				      WebResponse resp = req.GetResponse();

				      stream = resp.GetResponseStream();

              return Read(stream, uri, settings);
            }
          catch (WebException ex)
            {
                errors.Add(ex);
            }

            return null;
        }

      /// <summary>
      /// If you have an open stream, uses it.  The uri is not used except for modifying the feed
      /// </summary>
      /// <param name="stream"></param>
      /// <param name="uri"></param>
      /// <param name="settings"></param>
      /// <returns></returns>
        public static Feed Read(Stream stream, String uri, FeedReaderSettings settings)
        {
          XmlReader xmlReader = XmlReader.Create(stream, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore, XmlResolver = new XmlCachingResolver(true) });
          Parser parser = null;
          try{
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
                        default:
                            IModule module = ModuleManager.Instance.GetModule(xmlReader.Value);
                            if (module != null)
                            {
                              module.LocalName = xmlReader.LocalName;
                              feed.Modules.Add(xmlReader.LocalName, module);
                            }
                            break;
                    }
                }
            }

            if (feed == null)
            {
              errors.Add(new InvalidDataException("Unknown feed format."));
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

    /// <summary>
    /// Caching resolver documented here: http://msdn.microsoft.com/en-us/library/bb669135.aspx
    /// </summary>
    class XmlCachingResolver : XmlUrlResolver
    {
      bool enableHttpCaching;
      ICredentials credentials;

      //resolve resources from cache (if possible) when enableHttpCaching is set to true
      //resolve resources from source when enableHttpcaching is set to false 
      public XmlCachingResolver(bool enableHttpCaching)
      {
        this.enableHttpCaching = enableHttpCaching;
      }

      public override ICredentials Credentials
      {
        set
        {
          credentials = value;
          base.Credentials = value;
        }
      }

      public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
      {
        if (absoluteUri == null)
        {
          throw new ArgumentNullException("absoluteUri");
        }
        //resolve resources from cache (if possible)
        if (absoluteUri.Scheme == "http" && enableHttpCaching && (ofObjectToReturn == null || ofObjectToReturn == typeof(Stream)))
        {
          WebRequest webReq = WebRequest.Create(absoluteUri);
          webReq.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
          if (credentials != null)
          {
            webReq.Credentials = credentials;
          }
          WebResponse resp = webReq.GetResponse();
          return resp.GetResponseStream();
        }
        //otherwise use the default behavior of the XmlUrlResolver class (resolve resources from source)
        else
        {
          return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }
      }
    }
}
