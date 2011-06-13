/* ExternalFeedLink.cs
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
using System.Runtime.Serialization;
using System.Diagnostics;
using FeedDotNet.Common.Enums;

namespace FeedDotNet.Common
{
	[DataContract]
	[Serializable]
	public class ExternalFeedLink
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		FeedType feedType = FeedType.Unknown;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string title = String.Empty;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string uri = String.Empty;

		[DataMember]
		public FeedType FeedType
		{
			get { return feedType; }
			set { feedType = value; }
		}

		[DataMember]
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		[DataMember]
		public string Uri
		{
			get { return uri; }
			set { uri = value; }
		}

		public ExternalFeedLink(FeedType feedType)
		{
			this.feedType = feedType;
		}

		public ExternalFeedLink()
		{
		}
	}
}
