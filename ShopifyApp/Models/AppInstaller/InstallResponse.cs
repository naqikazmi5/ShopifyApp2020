using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.AppInstaller
{
    public class InstallResponse
    {
        public bool IsInstall { get; set; }
        public bool WebhookCreated { get; set; }
        public bool OrderSync { get; set; }
        public bool ProductSync { get; set; }
        public bool CustomerSync { get; set; }
        public bool StoreLocations { get; set; }

    }
    public class SyncResponse
    {
        public bool OrderSync { get; set; }
        public bool ProductSync { get; set; }
        public bool CustomerSync { get; set; }

    }
}
