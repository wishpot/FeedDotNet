/* Parser.cs
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
using System.Xml;

namespace FeedDotNet
{
    internal class Parser
    {
        protected XmlReader XmlReader;
        protected Feed Feed;
        protected bool readModules = true;

        public bool ReadModules
        {
            get { return readModules; }
            set { readModules = value; }
        }

        public Parser(XmlReader xmlReader, Feed feed)
        {
            this.XmlReader = xmlReader;
            this.Feed = feed;
        }

        public virtual void Parse()
        {
        }

        /// <summary>Correct some feeds</summary>
        protected void Complete()
        {
            for (int i = 0; i < Feed.Items.Count; i++)
            {
                // If no link element but a PermaLink, then use PermaLink as Link
                if (Feed.Items[i].WebUris.Count == 0 && Feed.Items[i].Guid != null && Feed.Items[i].Guid.PermaLink)
                    Feed.Items[i].WebUris.Add(new FeedUri(Feed.Items[i].Guid.Id));

                // Some feeds use Summary as Content. Correct this:
                if (Feed.Items[i].Content.Length == 0 && Feed.Items[i].Summary.Length > 0)
                {
                    Feed.Items[i].Content = Feed.Items[i].Summary;
                    Feed.Items[i].Summary = String.Empty;
                }
            }
        }

        protected void readModuleItem(XmlReader subReader, FeedItem feedItem)
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
