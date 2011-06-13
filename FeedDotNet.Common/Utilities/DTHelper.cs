/* DTHelper.cs
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
using System.Globalization;

namespace FeedDotNet.Utilities
{
    public static class DTHelper
    {
        private static Dictionary<string, string> timeZones = new Dictionary<string, string>();

		static DTHelper()
		{
			timeZones.Clear();
			timeZones.Add("ACDT",   "-1030");
			timeZones.Add("ACST",   "-0930");
			timeZones.Add("ADT",    "+0300");
			timeZones.Add("AEDT",   "-1100");
			timeZones.Add("AEST",   "-1000");
			timeZones.Add("AHDT",   "+0900");
			timeZones.Add("AHST",   "+1000");
			timeZones.Add("AST",    "+0400");
			timeZones.Add("AT",     "+0200");
			timeZones.Add("AWDT",   "-0900");
			timeZones.Add("AWST",   "-0800");
			timeZones.Add("BAT",    "-0300");
			timeZones.Add("BDST",   "-0200");
			timeZones.Add("BET",    "+1100");
			timeZones.Add("BST",    "+0300");
			timeZones.Add("BT",     "-0300");
			timeZones.Add("BZT2",   "+0300");
			timeZones.Add("CADT",   "-1030");
			timeZones.Add("CAST",   "-0930");
			timeZones.Add("CAT",    "+1000");
			timeZones.Add("CCT",    "-0800");
			timeZones.Add("CDT",    "+0500");
			timeZones.Add("CED",    "-0200");
			timeZones.Add("CET",    "-0100");
			timeZones.Add("CST",    "+0600");
			timeZones.Add("EAST",   "-1000");
			timeZones.Add("EDT",    "+0400");
			timeZones.Add("EED",    "-0300");
			timeZones.Add("EET",    "-0200");
			timeZones.Add("EEST",   "-0300");
			timeZones.Add("EST",    "+0500");
			timeZones.Add("FST",    "-0200");
			timeZones.Add("FWT",    "-0100");
			timeZones.Add("GMT",    "+0000");
			timeZones.Add("GST",    "-1000");
			timeZones.Add("HDT",    "+0900");
			timeZones.Add("HST",    "+1000");
			timeZones.Add("IDLE",   "-1200");
			timeZones.Add("IDLW",   "+1200");
			timeZones.Add("IST",    "-0530");
			timeZones.Add("IT",     "-0330");
			timeZones.Add("JST",    "-0900");
			timeZones.Add("JT",     "-0700");
			timeZones.Add("MDT",    "+0600");
			timeZones.Add("MED",    "-0200");
			timeZones.Add("MET",    "-0100");
			timeZones.Add("MEST",   "-0200");
			timeZones.Add("MEWT",   "-0100");
			timeZones.Add("MST",    "+0700");
			timeZones.Add("MT",     "-0800");
			timeZones.Add("NDT",    "+0230");
			timeZones.Add("NFT",    "+0330");
			timeZones.Add("NT",     "+1100");
			timeZones.Add("NST",    "-0630");
			timeZones.Add("NZ",     "-1100");
			timeZones.Add("NZST",   "-1200");
			timeZones.Add("NZDT",   "-1300");
			timeZones.Add("NZT",    "-1200");
			timeZones.Add("PDT",    "+0700");
			timeZones.Add("PST",    "+0800");
			timeZones.Add("ROK",    "-0900");
			timeZones.Add("SAD",    "-1000");
			timeZones.Add("SAST",   "-0900");
			timeZones.Add("SAT",    "-0900");
			timeZones.Add("SDT",    "-1000");
			timeZones.Add("SST",    "-0200");
			timeZones.Add("SWT",    "-0100");
			timeZones.Add("USZ3",   "-0400");
			timeZones.Add("USZ4",   "-0500");
			timeZones.Add("USZ5",   "-0600");
			timeZones.Add("USZ6",   "-0700");
			timeZones.Add("UT",     "+0000");
			timeZones.Add("UTC",    "+0000");
			timeZones.Add("UZ10",   "-1100");
			timeZones.Add("WAT",    "+0100");
			timeZones.Add("WET",    "+0000");
			timeZones.Add("WST",    "-0800");
			timeZones.Add("YDT",    "+0800");
			timeZones.Add("YST",    "+0900");
			timeZones.Add("ZP4",    "-0400");
			timeZones.Add("ZP5",    "-0500");
			timeZones.Add("ZP6",    "-0600");
		}
		
        public static DateTime? ParseDateTime(string dateTime)
        {
            DateTime dt = DateTime.MinValue;

            if (DateTime.TryParse(dateTime, out dt))
            {
                //BUG: Here is a bug. If Zone = +0000, don't need to ToUniversalTime()
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                return dt.ToUniversalTime();
            }


            dateTime = dateTime.ToUpper(CultureInfo.InvariantCulture);

            foreach (KeyValuePair<string, string> kvp in timeZones)
            {
                if (dateTime.Contains(" " + kvp.Key))
                    dateTime = dateTime.Replace(" " + kvp.Key, " " + kvp.Value);
            }

			if (DateTime.TryParse(dateTime, out dt))
			{
				//if (dt.IsDaylightSavingTime())
				//    return dt.AddHours(+1).ToUniversalTime();
				//else

                    //BUG: Here is a bug. If Zone = +0000, don't need to ToUniversalTime()
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
					return dt.ToUniversalTime();
			}
			else
				return null;
        }
    }
}
