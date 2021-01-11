using System;
using System.Web;
using System.Web.UI;

namespace BT.Manage.Tools.Utils
{
    /// <summary>
    ///Session操作类
    /// </summary>
    public class SessionHelper : System.Web.UI.Page
    {
        //pageload之前执行的类，进行重写
        protected override void OnInit(EventArgs e)
        {
            //重写此类
            base.OnInit(e);
        }

        #region 读取

        /// <summary>
        /// 读SessionName值,并对Session进行url编码
        /// </summary>
        /// <param name="SessionName">名称</param>
        /// <returns>Session值</returns>
        public static string GetSession(string SessionName)
        {
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionName] != null)
            {
                return System.Web.HttpContext.Current.Server.UrlDecode(HttpContext.Current.Session[SessionName].ToString());
            }
            return "";
        }

        /// <summary>
        /// 判断COOKIE是否存在
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static bool GetSessionIsExist(Page page, string cookieName)
        {
            if (GetSession(cookieName) == "")
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断COOKIE是否存在与value值是否相等
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        /// <returns></returns>
        public static bool GetSessionIsExist(string cookieName, string cookieValue)
        {
            string cookie = GetSession(cookieName);
            if (cookie == "")
            {
                return false;
            }
            return (cookie == cookieValue);
        }

        #endregion 读取

        #region 删除操作

        /// <summary>
        /// 移出指定COOKIE
        /// </summary>
        /// <param name="cookieName">cookie名称</param>
        public static void RemoveSession(string SessionName)
        {
            if (HttpContext.Current.Session[SessionName] != null)
            {
                HttpContext.Current.Session[SessionName] = null;
                HttpContext.Current.Session.Remove(SessionName);
            }
        }

        /// <summary>
        /// 移出全部COOKIE
        /// </summary>
        public static void RemoveSession()
        {
            HttpContext.Current.Session.Abandon();
        }

        #endregion 删除操作
    }
}