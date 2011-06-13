/* Form1.cs
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FeedDotNet.Common;
using FeedDotNet;

namespace ClientApplication
{
    public partial class TestClientForm : Form
    {
        public TestClientForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            lbResult.Items.Clear();

            DateTime dt1 = DateTime.Now;

            if (!readFeed(txtUrl.Text))
            {
                readWebSite();
            }

            this.Text = "Milliseconds: " + ((TimeSpan)(DateTime.Now - dt1)).TotalMilliseconds;
        }

        private void readWebSite()
        {
            lvFeeds.Items.Clear();
            List<ExternalFeedLink> externalFeedLinks = Explorer.Discover(new Uri(txtUrl.Text));

            if (Explorer.HasErrors)
            {
                foreach (Exception ex in Explorer.GetErrors())
                {
                    MessageBox.Show(ex.Message);
                }
            }

            foreach (ExternalFeedLink link in externalFeedLinks)
            {
                ListViewItem lvi = new ListViewItem(
                    new string[] { link.FeedType.ToString(), link.Title, link.Uri });
                lvi.Tag = link.Uri;
                lvFeeds.Items.Add(lvi);
            }
        }

        private bool readFeed(string uri)
        {
            FeedReaderSettings settings = new FeedReaderSettings();
            settings.ReadModules = true;

            Feed feed = FeedReader.Read(uri, settings);

            if (FeedReader.HasErrors)
            {
                foreach (Exception ex in FeedReader.GetErrors())
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (feed != null)
            {
                showFeedInfo(feed);
                return true;
            }

            return false;
        }

        private void showFeedInfo(Feed feed)
        {
            lbResult.Items.Add("Title: " + feed.Title);
            lbResult.Items.Add("Version: " + feed.Version.ToString());
            lbResult.Items.Add("Description: " + feed.Description);
            lbResult.Items.Add("WebUri: " + feed.WebUri);
            lbResult.Items.Add("XmlUri: " + feed.XmlUri);
            lbResult.Items.Add("Copyright: " + feed.Copyright);
            lbResult.Items.Add("Icon: " + feed.Icon);
            lbResult.Items.Add("ID: " + feed.Id);
            lbResult.Items.Add("Language: " + feed.Language);
            if (feed.Published.HasValue)
                lbResult.Items.Add("Published: " + feed.Published.Value.ToString());
            if (feed.Updated.HasValue)
                lbResult.Items.Add("Updated: " + feed.Updated.Value.ToString());
            lbResult.Items.Add("RssDocumentation: " + feed.RssDocumentation);

            lbResult.Items.Add("Ttl: " + feed.Ttl);
            lbResult.Items.Add("Generator: " + feed.Generator);
            lbResult.Items.Add("Image: " + feed.Image);
            lbResult.Items.Add("WebMaster: " + feed.WebMaster);
            lbResult.Items.Add("Cloud: " + feed.Cloud);

            foreach (Person person in feed.Authors)
            {
                lbResult.Items.Add("Author: " + person);
            }

            foreach (Person person in feed.Contributors)
            {
                lbResult.Items.Add("Contributor: " + person);
            }

            foreach (Category category in feed.Categories)
            {
                lbResult.Items.Add("Category: " + category);
            }

            foreach (KeyValuePair<string, IModule> module in feed.Modules)
            {
                lbResult.Items.Add("Module: " + module.Value.Title);
                lbResult.Items.Add("NS: " + module.Value.NS);
                lbResult.Items.Add("LocalName: " + module.Value.LocalName);

                if (module.Value is FeedDotNet.Modules.Itunes.Module)
                {
                    lbResult.Items.Add(" ItunesModule Author: " + ((FeedDotNet.Modules.Itunes.Module)module.Value).Author);
                    lbResult.Items.Add(" ItunesModule Summary: " + ((FeedDotNet.Modules.Itunes.Module)module.Value).Summary);
                    lbResult.Items.Add(" ItunesModule Subtitle: " + ((FeedDotNet.Modules.Itunes.Module)module.Value).Subtitle);
                }

                if (module.Value is FeedDotNet.Modules.DublinCore.Module)
                {
                    lbResult.Items.Add(" DublinCore Module. Title: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).TitleDC);
                    lbResult.Items.Add(" DublinCore Module. Creator: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Creator);
                    lbResult.Items.Add(" DublinCore Module. Subject: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Subject);
                    lbResult.Items.Add(" DublinCore Module. Description: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Description);
                    lbResult.Items.Add(" DublinCore Module. Publisher: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Publisher);
                    lbResult.Items.Add(" DublinCore Module. Contributor: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Contributor);
                    lbResult.Items.Add(" DublinCore Module. Date: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Date);
                    lbResult.Items.Add(" DublinCore Module. Type: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Type);
                    lbResult.Items.Add(" DublinCore Module. Format: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Format);
                    lbResult.Items.Add(" DublinCore Module. Identifier: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Identifier);
                    lbResult.Items.Add(" DublinCore Module. Source: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Source);
                    lbResult.Items.Add(" DublinCore Module. Language: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Language);
                    lbResult.Items.Add(" DublinCore Module. Relation: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Relation);
                    lbResult.Items.Add(" DublinCore Module. Coverage: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Coverage);
                    lbResult.Items.Add(" DublinCore Module. Rights: " + ((FeedDotNet.Modules.DublinCore.Module)module.Value).Rights);
                }

                if (module.Value is FeedDotNet.Modules.Syndication.Module)
                {
                    lbResult.Items.Add(" SyndicationModule UpdatePeriod: " + ((FeedDotNet.Modules.Syndication.Module)module.Value).UpdatePeriod.ToString());
                    lbResult.Items.Add(" SyndicationModule UpdateFrequency: " + ((FeedDotNet.Modules.Syndication.Module)module.Value).UpdateFrequency.ToString());

                    if (((FeedDotNet.Modules.Syndication.Module)module.Value).UpdateBase.HasValue)
                        lbResult.Items.Add(" SyndicationModule UpdateBase: " + ((FeedDotNet.Modules.Syndication.Module)module.Value).UpdateBase.Value);
                }

                if (module.Value is FeedDotNet.Modules.Geo.Module)
                {
                    lbResult.Items.Add(" GeoModule Latitude: " + ((FeedDotNet.Modules.Geo.Module)module.Value).Latitude);
                    lbResult.Items.Add(" GeoModule Longitude: " + ((FeedDotNet.Modules.Geo.Module)module.Value).Longitude);
                    lbResult.Items.Add(" GeoModule Altitude: " + ((FeedDotNet.Modules.Geo.Module)module.Value).Altitude);
                }
            }

            int i = 1;
            foreach (FeedItem item in feed.Items)
            {
                lbResult.Items.Add("Item " + i);
                lbResult.Items.Add("  Title: " + item.Title);
                lbResult.Items.Add("  Content: " + item.Content);
                lbResult.Items.Add("  Summary: " + item.Summary);
                lbResult.Items.Add("  CommentsUri: " + item.CommentsUri);
                lbResult.Items.Add("  Guid: " + item.Guid);
                lbResult.Items.Add("  Source: " + item.Source);
                lbResult.Items.Add("  Copyright: " + item.Copyright);
                if (item.Published.HasValue)
                    lbResult.Items.Add("  Published: " + item.Published.Value.ToString());
                if (item.Updated.HasValue)
                    lbResult.Items.Add("  Updated: " + item.Updated.Value.ToString());

                foreach (Person person in item.Authors)
                {
                    lbResult.Items.Add("  Author: " + person);
                }

                foreach (Person person in feed.Contributors)
                {
                    lbResult.Items.Add("  Contributor: " + person);
                }

                foreach (Category category in item.Categories)
                {
                    lbResult.Items.Add("  Category: " + category);
                }

                foreach (Enclosure enclosure in item.Enclosures)
                {
                    lbResult.Items.Add("  Enclosure: " + enclosure);
                }

                foreach (FeedUri uri in item.WebUris)
                {
                    lbResult.Items.Add("  WebUri: " + uri);
                }

                foreach (KeyValuePair<string, IModuleItem> moduleItem in item.ModuleItems)
                {
                    lbResult.Items.Add("   ModuleItem LocalName: " + moduleItem.Value.LocalName);

                    if (moduleItem.Value is FeedDotNet.Modules.Itunes.ModuleItem)
                    {
                        lbResult.Items.Add("   ItunesModuleItem Author: " + ((FeedDotNet.Modules.Itunes.ModuleItem)moduleItem.Value).Author);
                        lbResult.Items.Add("   ItunesModuleItem Summary: " + ((FeedDotNet.Modules.Itunes.ModuleItem)moduleItem.Value).Summary);
                    }

                    if (moduleItem.Value is FeedDotNet.Modules.DublinCore.ModuleItem)
                    {
                        lbResult.Items.Add(" DublinCore ModuleItem. Title: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).TitleDC);
                        lbResult.Items.Add(" DublinCore ModuleItem. Creator: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Creator);
                        lbResult.Items.Add(" DublinCore ModuleItem. Subject: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Subject);
                        lbResult.Items.Add(" DublinCore ModuleItem. Description: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Description);
                        lbResult.Items.Add(" DublinCore ModuleItem. Publisher: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Publisher);
                        lbResult.Items.Add(" DublinCore ModuleItem. Contributor: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Contributor);
                        lbResult.Items.Add(" DublinCore ModuleItem. Date: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Date);
                        lbResult.Items.Add(" DublinCore ModuleItem. Type: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Type);
                        lbResult.Items.Add(" DublinCore ModuleItem. Format: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Format);
                        lbResult.Items.Add(" DublinCore ModuleItem. Identifier: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Identifier);
                        lbResult.Items.Add(" DublinCore ModuleItem. Source: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Source);
                        lbResult.Items.Add(" DublinCore ModuleItem. Language: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Language);
                        lbResult.Items.Add(" DublinCore ModuleItem. Relation: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Relation);
                        lbResult.Items.Add(" DublinCore ModuleItem. Coverage: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Coverage);
                        lbResult.Items.Add(" DublinCore ModuleItem. Rights: " + ((FeedDotNet.Modules.DublinCore.ModuleItem)moduleItem.Value).Rights);
                    }

                    if (moduleItem.Value is FeedDotNet.Modules.Content.ModuleItem)
                    {
                        lbResult.Items.Add("   ContentModuleItem Encoded: " + ((FeedDotNet.Modules.Content.ModuleItem)moduleItem.Value).Encoded);
                    }

                    if (moduleItem.Value is FeedDotNet.Modules.Geo.ModuleItem)
                    {
                        lbResult.Items.Add("   GeoModuleItem Latitude: " + ((FeedDotNet.Modules.Geo.ModuleItem)moduleItem.Value).Latitude);
                        lbResult.Items.Add("   GeoModuleItem Longitude: " + ((FeedDotNet.Modules.Geo.ModuleItem)moduleItem.Value).Longitude);
                        lbResult.Items.Add("   GeoModuleItem Altitude: " + ((FeedDotNet.Modules.Geo.ModuleItem)moduleItem.Value).Altitude);
                    }
                }

                i++;
            }
        }

        private void lvFeeds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvFeeds.SelectedItems.Count > 0)
            {
                lbResult.Items.Clear();
                readFeed(lvFeeds.SelectedItems[0].Tag.ToString());
            }
            else
            {
                lbResult.Items.Clear();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            /*
             * Create sample feed
             */
            Feed feed = new Feed();
            feed.Title = tbOPMLFeedTitle.Text;
            feed.WebUri = new FeedUri(tbOPMLFeedWebUri.Text);
            feed.XmlUri = new FeedUri(tbOPMLFeedRss.Text);

            /*
             * Create sample category
             */
            SampleOpmlCategory category = new SampleOpmlCategory();
            category.Title = tbOPMLCategory.Text;
            category.Feeds.Add(feed);


            if (sfdOpml.ShowDialog() == DialogResult.OK)
            {
                OpmlManager.Export(category, sfdOpml.FileName);

                if (OpmlManager.HasErrors)
                {
                    foreach (Exception ex in OpmlManager.GetErrors())
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("File written successfully.", "OPML Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}