/* ModuleItem.cs
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
using System.Diagnostics;
using System.Xml;

namespace FeedDotNet.Modules.Geo
{
    [Serializable]
    public class ModuleItem : IModuleItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string localName = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string latitude = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string longitude = String.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string altitude = String.Empty;

        /// <summary>
        /// The WGS84 latitude (decimal degrees)
        /// </summary>
        public string Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        /// <summary>
        /// The WGS84 longitude (decimal degrees)
        /// </summary>
        public string Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        /// <summary>
        /// The WGS84 altitude (decimal meters above the local reference ellipsoid)
        /// </summary>
        public string Altitude
        {
            get { return altitude; }
            set { altitude = value; }
        }

        public ModuleItem(string localName)
        {
            this.localName = localName;
        }

        #region IModuleItem Members

        public string LocalName
        {
            get
            {
                return localName;
            }
        }

        public void Parse(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.LocalName)
                    {
                        case "lat":
                            xmlReader.MoveToContent();
                            latitude = xmlReader.ReadString();
                            break;
                        case "long":
                            xmlReader.MoveToContent();
                            longitude = xmlReader.ReadString();
                            break;
                        case "alt":
                            xmlReader.MoveToContent();
                            altitude = xmlReader.ReadString();
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
