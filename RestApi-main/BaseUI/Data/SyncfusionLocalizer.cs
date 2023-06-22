using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseUI.Resources;
using Syncfusion.Blazor;

namespace BaseUI.Data
{
    public class SyncfusionLocalizer : ISyncfusionStringLocalizer
    {
        public string GetText(string key)
        {
            return this.ResourceManager.GetString(key);
        }

        public System.Resources.ResourceManager ResourceManager
        {
            get
            {
                return SfResources.ResourceManager;
            }
        }
    }
}