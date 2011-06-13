using System;
using System.Collections.Generic;
using System.Text;

namespace FeedDotNet.Modules.Syndication
{
    [Serializable]
    public enum UpdatePeriod
    {
        Hourly, 
        Daily, 
        Weekly, 
        Monthly, 
        Yearly
    }
}
