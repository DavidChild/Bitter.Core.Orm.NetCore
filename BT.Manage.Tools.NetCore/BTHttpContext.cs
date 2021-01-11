using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Manage.Tools
{
    public static class BtHttpContext
    {
        public static IServiceProvider ServiceProvider;

        static BtHttpContext()
        { }


        public static HttpContext Current
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));

                HttpContext context = ((IHttpContextAccessor)factory).HttpContext;

                return context;
            }
        }
        public static string  RequestUrl
        {
            get
            {
                if (BtHttpContext.Current != null &&BtHttpContext.Current.Request!=null && BtHttpContext.Current.Request.Host!=null)
                {
                    string port = Current.Request.Host.Port.HasValue ? "80" : Current.Request.Host.Port.Value.ToString();
                    return Current.Request.Host.ToUriComponent() + ":" + port + "/" + Current.Request.Path.ToUriComponent();
                }
                else
                {
                    return string.Empty;
                }
              
            }
        }

    }
}
