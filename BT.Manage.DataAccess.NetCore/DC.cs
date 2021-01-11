using BT.Manage.DataAccess;

namespace BT.Manage.DataAccess
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2017/1/10 9:39:38
    ** desc：DC:获取数据库链接地址
    ** Ver.: V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    **************************************************************3*******************/

    public class dc
    {
        /// <summary>
        /// 主库的连接地址
        /// </summary>
        private static DatabaseProperty conn
        {
            get { return DBSettings.GetDatabaseProperty((new DbDefaultConfig().DbConfig)); }
        }
        /// <summary>
        /// 附属库的连接地址
        /// </summary>
        /// <param name="dbconfig"></param>
        /// <returns></returns>
        private static DatabaseProperty  AttachedConn(IDbConfig dbconfig)
        {
           return DBSettings.GetDatabaseProperty(dbconfig.DbConfig); 
        }
        /// <summary>
        /// 通过字符串实例化
        /// </summary>
        /// <param name="dbconfig"></param>
        /// <returns></returns>
        private static DatabaseProperty AttachedConn(string dbconfig)
        {
            return DBSettings.GetDatabaseProperty(dbconfig);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="dbconfig"></param>
        /// <returns></returns>
        public static DatabaseProperty dbconn(string  dbconfig)
        {
            if (string.IsNullOrEmpty(dbconfig))
            {
                return dc.conn;
            }
            else
            {
                return AttachedConn(dbconfig);
            }
        }
       

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="dbconfig"></param>
        /// <returns></returns>
        public static DatabaseProperty dbconn(IDbConfig dbconfig=null )
        {
            if (dbconfig == null)
            {
                return dc.conn;
            }
            else
            {
                return AttachedConn(dbconfig);
            }
        }
    }
}