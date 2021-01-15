using System;
using System.Collections.Generic;
using System.Text;

namespace Bitter.Core.NetCore
{
    public static class ServiceApplication
    {
        public static IServiceProvider Instance { get; set; }

        public static void RegisterApplication(IServiceProvider serviceProvider)
        {
            Instance = serviceProvider;
        }
    }
}
