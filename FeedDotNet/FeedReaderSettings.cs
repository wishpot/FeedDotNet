/* FeedReaderSettings.cs
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
using System.Net;

namespace FeedDotNet
{
    public class FeedReaderSettings
    {
        private bool readModules = true;
        private string httpUserAgentString = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; MyIE2; .NET CLR 1.1.4322; .NET CLR 2.0.50727; WinFX RunTime 3.0.50727; .NET CLR 3.0.04506.30)";
        private string httpAcceptString = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
        private Dictionary<HttpRequestHeader, string> httpHeaders = new Dictionary<HttpRequestHeader, string>();
        private int? httpTimeout = null;


        /// <summary>
        /// Allows you to override the timeout used when fetching from http... the default is 
        /// the built-in default of the HttpWebRequest.Timeout object
        /// </summary>
        public int? HttpTimeout
        {
          get { return httpTimeout; }
          set { httpTimeout = value; }
        }

        /// <summary>
        /// Set to false, if no module information should be parsed. Default is True.
        /// </summary>
        public bool ReadModules
        {
            get { return readModules; }
            set { readModules = value; }
        }

        /// <summary>
        /// User Agent. See http://www.user-agents.org for reference
        /// </summary>
        public string HttpUserAgentString
        {
            get { return httpUserAgentString; }
            set { httpUserAgentString = value; }
        }

        /// <summary>
        /// Accept header
        /// </summary>
        public string HttpAcceptString
        {
            get { return httpAcceptString; }
            set { httpAcceptString = value; }
        }

        /// <summary>
        /// Custom http header collection
        /// </summary>
        public Dictionary<HttpRequestHeader, string> HttpHeaders
        {
            get { return httpHeaders; }
            set { httpHeaders = value; }
        }

        public FeedReaderSettings()
        {
        }

        public FeedReaderSettings(bool readModules)
        {
            this.readModules = readModules;
        }
    }
}
