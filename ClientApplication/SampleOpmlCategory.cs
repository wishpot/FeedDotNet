using System;
using System.Collections.Generic;
using System.Text;
using FeedDotNet.Common;

namespace ClientApplication
{
    class SampleOpmlCategory : IOpmlCategory
    {
        private string title = String.Empty;
        private List<IOpmlCategory> categories = new List<IOpmlCategory>();
        private List<Feed> feeds = new List<Feed>();

        #region IOpmlCategory Members

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        public List<IOpmlCategory> Categories
        {
            get
            {
                return categories;
            }
            set
            {
                categories = value;
            }
        }

        public List<Feed> Feeds
        {
            get
            {
                return feeds;
            }
            set
            {
                feeds = value;
            }
        }

        #endregion
    }
}
