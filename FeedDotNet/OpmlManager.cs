/* OpmlManager.cs
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
using System.Xml;
using FeedDotNet.Common;

namespace FeedDotNet
{
    public static class OpmlManager
    {
        private static List<Exception> errors = new List<Exception>();

        /// <summary>
        /// Save a OPML category to a specified location.
        /// </summary>
        /// <param name="opmlCategory">Category structure</param>
        /// <param name="path">Save location</param>
        public static void Export(IOpmlCategory opmlCategory, string path)
        {
            Export(opmlCategory, path, new OpmlExportSettings());
        }

        /// <summary>
        /// Save a OPML category to a specified location.
        /// </summary>
        /// <param name="opmlCategory">Category structure</param>
        /// <param name="path">Save location</param>
        /// <param name="settings">Optional settings object</param>
        public static void Export(IOpmlCategory opmlCategory, string path, OpmlExportSettings settings)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration decl = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDoc.AppendChild(decl);

                XmlComment elComment = xmlDoc.CreateComment(settings.Comment);
                xmlDoc.AppendChild(elComment);

                XmlElement elOpml = xmlDoc.CreateElement("opml");
                switch (settings.OpmlVersion)
                {
                    case OpmlVersion.V10:
                        elOpml.SetAttribute("version", "1.0");
                        break;
                    case OpmlVersion.V11:
                        elOpml.SetAttribute("version", "1.1");
                        break;
                    case OpmlVersion.V20:
                        elOpml.SetAttribute("version", "2.0");
                        break;
                    default:
                        break;
                }                
                xmlDoc.AppendChild(elOpml);

                XmlElement elHead = xmlDoc.CreateElement("head");
                elOpml.AppendChild(elHead);

                XmlElement elTitle = xmlDoc.CreateElement("title");
                elTitle.InnerText = settings.Title;
                elHead.AppendChild(elTitle);

                XmlElement elDate = xmlDoc.CreateElement("dateCreated");
                elDate.InnerText = DateTime.Now.ToString();
                elHead.AppendChild(elDate);

                XmlElement elOwner = xmlDoc.CreateElement("ownerName");
                elOwner.InnerText = settings.OwnerName;
                elHead.AppendChild(elOwner);

                XmlElement elBody = xmlDoc.CreateElement("body");
                elOpml.AppendChild(elBody);

                if (!settings.UseFirstCategoryAsRoot)
                {
                    XmlElement elOutline = xmlDoc.CreateElement("outline");
                    elOutline.SetAttribute("text", opmlCategory.Title);
                    elOutline.SetAttribute("title", opmlCategory.Title);
                    elBody.AppendChild(elOutline);

                    appendBodyElement(xmlDoc, elOutline, opmlCategory);
                }
                else
                {
                    appendBodyElement(xmlDoc, elBody, opmlCategory);
                }

                xmlDoc.Save(path);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        }

        private static void appendBodyElement(XmlDocument xmlDoc, XmlElement parentElement, IOpmlCategory opmlCategory)
        {
            foreach (IOpmlCategory subCat in opmlCategory.Categories)
            {
                XmlElement elOutline = xmlDoc.CreateElement("outline");
                elOutline.SetAttribute("text", subCat.Title);
                elOutline.SetAttribute("title", subCat.Title);
                parentElement.AppendChild(elOutline);

                appendBodyElement(xmlDoc, elOutline, subCat);
            }

            foreach (Feed feed in opmlCategory.Feeds)
            {
                XmlElement elFeed = xmlDoc.CreateElement("outline");
                elFeed.SetAttribute("text", feed.Title);
                elFeed.SetAttribute("title", feed.Title);
                elFeed.SetAttribute("type", "rss");
                elFeed.SetAttribute("xmlUrl", feed.XmlUri.Uri);
                elFeed.SetAttribute("htmlUrl", feed.WebUri.Uri);
                if (feed.Language.Trim().Length == 2)
                    elFeed.SetAttribute("language", feed.Language);
                parentElement.AppendChild(elFeed);
            }
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
