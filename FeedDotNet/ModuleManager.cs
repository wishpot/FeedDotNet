/* ModuleManager.cs
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

namespace FeedDotNet
{
    internal sealed class ModuleManager
    {
        private List<IModule> modules = new List<IModule>();

        static readonly ModuleManager instance = new ModuleManager();

        ModuleManager()
        {
            initialize();
        }

        static ModuleManager()
        {
        }

        public static ModuleManager Instance
        {
            get
            {
                return instance;
            }
        }

        public void initialize()
        {
            modules.Add(new FeedDotNet.Modules.Itunes.Module());
            modules.Add(new FeedDotNet.Modules.DublinCore.Module());
            modules.Add(new FeedDotNet.Modules.Content.Module());
            modules.Add(new FeedDotNet.Modules.Syndication.Module());
            modules.Add(new FeedDotNet.Modules.Geo.Module());
            // initialize others here
        }

        public IModule GetModule(string ns)
        {
            foreach (IModule module in modules)
            {
                if (module.NS == ns)
                    return module;
            }

            return null;
        }
    }
}
