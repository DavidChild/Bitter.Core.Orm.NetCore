using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Manage.Frame.Base.NetCore.Common
{
    public class CommonSecurity
    {
        /// <summary>
        /// 公开签名字段
        /// </summary>
        public const string OPENAPISIGN = "openapisign";

        /// <summary>
        /// 获取签名内容（公共契约）
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static string GetSignContent(IDictionary<string, string> dictionary)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(dictionary);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
            
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key.ToLower()).Append("=").Append(value).Append("&");
                }
            }
            //var checkContent = string.Join("&", sortedParams.Select(d => string.Concat(d.Key, "=", d.Value.UrlDecode())));

            if (query.Length == 0)
            {
                return "";
            }

            return query.ToString().Substring(0, query.Length - 1);
        }
    }
}
