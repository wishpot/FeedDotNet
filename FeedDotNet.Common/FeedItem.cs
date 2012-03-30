/* FeedItem.cs
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
using FeedDotNet.Common.Enums;

namespace FeedDotNet.Common
{
    [DataContract]
	[Serializable]
    public class FeedItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FeedGuid guid;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string title = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<FeedUri> webUris = new List<FeedUri>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string content = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TextType contentType = TextType.Text;
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string summary = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TextType summaryType = TextType.Text;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DateTime? updated;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DateTime? published;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Person> authors = new List<Person>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Person> contributors = new List<Person>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Category> categories = new List<Category>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Enclosure> enclosures = new List<Enclosure>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string copyright = String.Empty;
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string commentsUri = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Source source;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, IModuleItem> moduleItems = new Dictionary<string, IModuleItem>();

        [DataMember]
        public FeedGuid Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        [DataMember]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        [DataMember]
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        [DataMember]
        public TextType ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }

        [DataMember]
        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }

        [DataMember]
        public TextType SummaryType
        {
            get { return summaryType; }
            set { summaryType = value; }
        }

        [DataMember]
        public string ContentOrSummary
        {
            get 
            {
                string text = String.Empty;
                if (summary.Length > 0)
                    text = summary;
                else if (content.Length > 0)
                    text = content;
                return text; 
            }
            set 
            { 
                summary = value;
                content = value; 
            }
        }

        [DataMember]
        public DateTime? Updated
        {
            get { return updated; }
            set { updated = value; }
        }

        [DataMember]
        public DateTime? Published
        {
            get { return published; }
            set { published = value; }
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
        public List<Category> Categories
        {
            get { return categories; }
            set { categories = value; }
        }

        [DataMember]
        public string Copyright
        {
            get { return copyright; }
            set { copyright = value; }
        }

        [DataMember]
        public List<FeedUri> WebUris
        {
            get { return webUris; }
            set { webUris = value; }
        }

        [DataMember]
        public List<Enclosure> Enclosures
        {
            get { return enclosures; }
            set { enclosures = value; }
        }

        [DataMember]
        public string CommentsUri
        {
            get { return commentsUri; }
            set { commentsUri = value; }
        }

        [DataMember]
        public Source Source
        {
            get { return source; }
            set { source = value; }
        }

        [DataMember]
        public Dictionary<string, IModuleItem> ModuleItems
        {
            get { return moduleItems; }
            set { moduleItems = value; }
        }

        public IModuleItem GetModuleItem(string prefix)
        {
            if (moduleItems.ContainsKey(prefix))
                return moduleItems[prefix];

            return null;
        }



        public FeedItem()
        {
        }

        public FeedItem(FeedItem feedItem)
        {
            this.authors = feedItem.authors;
            this.categories = feedItem.categories;
            this.commentsUri = feedItem.commentsUri;
            this.content = feedItem.content;
            this.contentType = feedItem.contentType;
            this.contributors = feedItem.contributors;
            this.copyright = feedItem.copyright;
            this.enclosures = feedItem.enclosures;
            this.guid = feedItem.guid;
            this.moduleItems = feedItem.moduleItems;
            this.published = feedItem.published;
            this.source = feedItem.source;
            this.summary = feedItem.summary;
            this.summaryType = feedItem.summaryType;
            this.title = feedItem.title;
            this.updated = feedItem.updated;
            this.webUris = feedItem.webUris;
        }

        public override string ToString()
        {
            if (title.Length > 0)
                return title;

            if (null != guid)
              return guid.ToString();

            if (!String.IsNullOrEmpty(summary))
              return summary.Substring(0, 20);

            return base.ToString();
        }
    }
}
