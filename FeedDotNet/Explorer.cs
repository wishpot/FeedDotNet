/* Explorer.cs
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
using System.Text.RegularExpressions;
using FeedDotNet.Common.Enums;
using System.Net;
using System.IO;

namespace FeedDotNet
{
	public sealed class Explorer
	{
        private static List<Exception> errors = new List<Exception>();
		private string text = String.Empty;

        /// <summary>
        /// Discover feed meta information on a web page
        /// </summary>
        /// <param name="uri">Uri of a web page</param>
        /// <returns>List of feed Urls</returns>
		public static List<ExternalFeedLink> Discover(Uri uri)
		{
            string text = getWebsiteContent(uri);
			Explorer explorer = new Explorer(text);
			List<ExternalFeedLink> externalFeedLinks = explorer.discover();
			correctRelativeLinks(externalFeedLinks, uri);
			return externalFeedLinks;
		}

        private static string getWebsiteContent(Uri uri)
        {
            Stream stream = null;
            StreamReader streamReader = null;
            String content = String.Empty;

            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
                // Some firewalls allow browser clients only. So let's be a browser ;)
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; MyIE2; .NET CLR 1.1.4322; .NET CLR 2.0.50727; WinFX RunTime 3.0.50727; .NET CLR 3.0.04506.30)";
                WebResponse resp = req.GetResponse();

                stream = resp.GetResponseStream();
                streamReader = new StreamReader(stream);
                content = streamReader.ReadToEnd();

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
                if (streamReader != null)
                    streamReader.Close();

                if (stream != null)
                    stream.Close();
            }

            return content;
        }

		private List<ExternalFeedLink> discover()
		{
			List<ExternalFeedLink> externalFeedLinks = new List<ExternalFeedLink>();

			List<string> rssLines = getLines("rss");
			List<string> atomLines = getLines("atom");

			foreach (string line in rssLines)
			{
				ExternalFeedLink externalFeedLink = new ExternalFeedLink(FeedType.RSS);
                externalFeedLink.Title = getTitle(line, FeedType.RSS);
				externalFeedLink.Uri = getUri(line);
				externalFeedLinks.Add(externalFeedLink);
			}

			foreach (string line in atomLines)
			{
				ExternalFeedLink externalFeedLink = new ExternalFeedLink(FeedType.Atom);
                externalFeedLink.Title = getTitle(line, FeedType.Atom);
                externalFeedLink.Uri = getUri(line);
				externalFeedLinks.Add(externalFeedLink);
			}

			return externalFeedLinks;
		}

		private List<string> getLines(string rssType)
		{
			List<string> lines = new List<string>();

			try
			{
                Regex regex = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>",
                            RegexOptions.Multiline | RegexOptions.IgnoreCase);

                MatchCollection matches = regex.Matches(text);

                foreach (Match match in matches)
                {
                    if (match.Value.StartsWith("<link", StringComparison.InvariantCultureIgnoreCase) && match.Value.Contains("application/" + rssType + "+xml"))
                        lines.Add(match.Value);
                }
			}
			catch (Exception ex)
			{
                errors.Add(ex);
			}

			return lines;
		}

        private string getTitle(string line, FeedType feedType)
		{
            try
            {
                Match title = Regex.Match(line, "(?<=title=).*", RegexOptions.IgnoreCase);
                if (title.Success)
                {
                    int end = title.Value.IndexOf("\"", 1);
                    if (end == -1)
                        end = title.Value.IndexOf("'", 1);

                    return title.Value.Substring(0, end).Replace("\"", "").Replace("'", "");
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }

            return String.Empty;
		}

		private string getUri(string line)
		{
			try
			{
				Match uri = Regex.Match(line, "(?<=href=).*", RegexOptions.IgnoreCase);
				if (uri.Success)
				{
					int end = uri.Value.IndexOf(" ", 1);
					if(end == -1)
						end = uri.Value.IndexOf("\"", 1); 

					if(end == -1)
						end = uri.Value.IndexOf("'", 1);

					return uri.Value.Substring(0, end).Replace("\"", "").Replace("'", "");
				}
			}
			catch (Exception ex)
			{
                errors.Add(ex);
			}

			return String.Empty;
		}

		private static void correctRelativeLinks(List<ExternalFeedLink> externalFeedLinks, Uri baseuri)
		{
			foreach (ExternalFeedLink link in externalFeedLinks)
			{
				Uri uri = null;

				if (!Uri.IsWellFormedUriString(link.Uri, UriKind.Absolute))
				{
					Uri.TryCreate(baseuri, link.Uri, out uri);

					if (uri != null)
						link.Uri = uri.ToString();
				}
			}
		}

		private Explorer(string text)
		{
			this.text = text;
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
