using NLog.Internal;
using System.Configuration;
namespace Bitter.Core
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/1/10 9:39:38
    ** desc：DC:获取数据库链接地址
    ** Ver.: V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    **************************************************************3*******************/

    public class dc
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetdb"></param>
        /// <returns></returns>
        
        public static DatabaseProperty conn(string targetdb)
        {
            string _targetdb = "MainData";
            if (!string.IsNullOrEmpty(targetdb))
            {
                _targetdb = targetdb;
            }
            return DBSettings.GetDatabaseProperty(_targetdb); 
        }



        public static string ConnectionInfo(string targetdb)
        {
            return DBSettings.GetConnectionInfo(targetdb);
        }

    }
}