
using System;

namespace Bitter.Base
{
    /// <summary>
    /// 功能描述：BaseResource 创 建 者：徐燕 创建日期：2017/4/6 16:37:43
    /// </summary>
    public static class BaseResource
    {
        public static string GetResourceValue(string key)
        {
           
            //System.Resources.ResourceManager rs = new System.Resources.ResourceManager("Bitter.Application.BaseEntity.Properties.Resource", typeof(Resource).Assembly);
               
            string value = "none";
            try
            {
                value = "";//rs.GetString(key);
            }
            catch (Exception ex)
            {
                value = "none";
            }
            return value;
        }
    }
}