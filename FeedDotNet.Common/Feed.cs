/* Feed.cs
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
using System.Diagnostics;
using System.Runtime.Serialization;

namespace FeedDotNet.Common
{
    [DataContract]
	[Serializable]
    public class Feed
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string id = String.Empty;
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string title = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FeedUri webUri;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FeedUri xmlUri;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FeedUri nextUri;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FeedUri prevUri;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string description = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string language = String.Empty;
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FeedVersion version = FeedVersion.Unknown;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DateTime? published;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DateTime? updated;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Person> authors = new List<Person>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Person> contributors = new List<Person>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Person webMaster;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Category> categories = new List<Category>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Generator generator;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string icon = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Image image;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string copyright = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string rssDocumentation = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Cloud cloud;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<FeedItem> items = new List<FeedItem>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int ttl;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<short> skipHours = new List<short>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Days> skipDays = new List<Days>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, IModule> modules = new Dictionary<string, IModule>();

        //public Uri _base;

        [DataMember]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        
        [DataMember]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [DataMember]
        public DateTime? Published
        {
            get { return published; }
            set { published = value; }
        }

        [DataMember]
        public DateTime? Updated
        {
            get { return updated; }
            set { updated = value; }
        }

        [DataMember]
        public string Language
        {
            get { return language; }
            set { language = value; }
        }

        [DataMember]
        public FeedVersion Version
        {
            get { return version; }
            set { version = value; }
        }

        [DataMember]
        public List<Person> Authors
        {
            get { return authors; }
            set { authors = value; }
        }

        [DataMember]
        public List<Person> Contributors
        {
            get { return contributors; }
            set { contributors = value; }
        }

        [DataMember]
        public Person WebMaster
        {
            get { return webMaster; }
            set { webMaster = value; }
        }

        [DataMember]
        public List<Category> Categories
        {
            get { return categories; }
            set { categories = value; }
        }

        [DataMember]
        public Generator Generator
        {
            get { return generator; }
            set { generator = value; }
        }

        [DataMember]
        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        [DataMember]
        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        [DataMember]
        public string Copyright
        {
            get { return copyright; }
            set { copyright = value; }
        }

        [DataMember]
        public FeedUri WebUri
        {
            get { return webUri; }
            set { webUri = value; }
        }

        [DataMember]
        public FeedUri XmlUri
        {
            get { return xmlUri; }
            set { xmlUri = value; }
        }

        [DataMember]
        public FeedUri NextUri
        {
            get { return nextUri; }
            set { nextUri = value; }
        }

        [DataMember]
        public FeedUri PrevUri
        {
            get { return prevUri; }
            set { prevUri = value; }
        }

        [DataMember]
        public string RssDocumentation
        {
            get { return rssDocumentation; }
            set { rssDocumentation = value; }
        }

        [DataMember]
        public Cloud Cloud
        {
            get { return cloud; }
            set { cloud = value; }
        }

        [DataMember]
        public List<FeedItem> Items
        {
            get { return items; }
            set { items = value; }
        }

        /// <summary> TTL in minutes. It's problematic, prefer HTTP 1.1 cache control</summary>
        [DataMember]
        public int Ttl
        {
            get { return ttl; }
            set { ttl = value; }
        }

        [DataMember]
        public List<Days> SkipDays
        {
            get { return skipDays; }
            set { skipDays = value; }
        }

        [DataMember]
        public List<short> SkipHours
        {
            get { return skipHours; }
            set { skipHours = value; }
        }

        [DataMember]
        public Dictionary<string, IModule> Modules
        {
            get { return modules; }
            set { modules = value; }
        }

		public string VersionAsText
		{
			get
			{
				string text = String.Empty;

				switch (version)
				{
					case FeedVersion.Atom10: text = "Atom 1.0"; break;
					case FeedVersion.Atom03: text = "Atom 0.3"; break;
					case FeedVersion.RSS090: text = "RSS 0.90"; break;
					case FeedVersion.RSS091: text = "RSS 0.91"; break;
					case FeedVersion.RSS092: text = "RSS 0.92"; break;
					case FeedVersion.RSS10: text = "RSS 1.0"; break;
					case FeedVersion.RSS20: text = "RSS 2.0"; break;
					default: text = String.Empty; break;
				}

				return text;
			}
			set { }
		}

        public IModule GetModule(string localName)
        {
            if (modules.ContainsKey(localName))
                return modules[localName];

            return null;
        }

        public Feed()
        {
        }

        public Feed(Feed feed)
        {
            this.authors = feed.authors;
            this.categories = feed.categories;
            this.cloud = feed.cloud;
            this.contributors = feed.contributors;
            this.copyright = feed.copyright;
            this.description = feed.description;
            this.generator = feed.generator;
            this.icon = feed.icon;
            this.id = feed.id;
            this.image = feed.image;
            this.items = feed.items;
            this.language = feed.language;
            this.modules = feed.modules;
            this.published = feed.published;
            this.rssDocumentation = feed.rssDocumentation;
            this.skipDays = feed.skipDays;
            this.skipHours = feed.skipHours;
            this.title = feed.title;
            this.ttl = feed.ttl;
            this.updated = feed.updated;
            this.version = feed.version;
            this.webMaster = feed.webMaster;
            this.webUri = feed.webUri;
            this.xmlUri = feed.xmlUri;
        }

        public override string ToString()
        {
            if (title.Length > 0)
                return title;

            return base.ToString();
        }
    }
}
