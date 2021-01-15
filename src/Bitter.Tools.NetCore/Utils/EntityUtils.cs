using System;
using System.Collections.Generic;
using System.Text;

namespace Bitter.Tools.Utils
{
    public class EntityUtils
    {
        //private const string SQL = "SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,COLUMN_COMMENT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{0}'";
        private const string SQL = "SELECT a.name,b.name,a.length,g.value FROM syscolumns a left join systypes b on a.xusertype=b.xusertype inner join sysobjects d on a.id=d.id and d.xtype='U' and d.name<>'dtproperties' left join syscomments e on a.cdefault=e.id left join sys.extended_properties g on a.id=g.major_id and a.colid=g.minor_id left join sys.extended_properties f on d.id=f.major_id and f.minor_id=0 where d.name='{0}' order by a.id,a.colorder";

        private const string SQL_ALLTBAL = "SELECT * FROM INFORMATION_SCHEMA.TABLES";

        private const string SQL_SELECT_TABLENAME = "SELECT  name  FROM  sys.objects WHERE type='U'";

        /// <summary>
        /// 获取数据库用户表名
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDataBaseTableName()
        {
            //List<string> list = null;
            //using (IDataReader reader = DbHelper.ExecuteReader(SQL_SELECT_TABLENAME))
            //{
            //    if (!reader.IsClosed)
            //    {
            //        list = new List<string>();
            //        while (reader.Read())
            //        {
            //            list.Add(reader.GetString(0));
            //        }
            //    }
            //}
            //return list;
            return null;
        }

        /// <summary>
        /// 生成实体
        /// </summary>
        /// <param name="tabName">表名</param>
        /// <param name="language">平台语言</param>
        public static string WriteEntity(string tabName)
        {
            return WriteEntity(tabName, string.Empty, Language.net);
        }

        /// <summary>
        /// 生成实体
        /// </summary>
        /// <param name="tabName">表名</param>
        /// <param name="language">平台语言</param>
        public static string WriteEntity(string tabName, Language language)
        {
            return WriteEntity(tabName, string.Empty, language);
        }

        /// <summary>
        /// 生成实体
        /// </summary>
        /// <param name="tabName">表名</param>
        /// <param name="namespce">命名空间</param>
        /// <param name="language">平台语言</param>
        /// <returns></returns>
        public static string WriteEntity(string tabName, string namespce, Language language)
        {
            StringBuilder sb = null;
            if (!string.IsNullOrEmpty(tabName))
            {
                sb = new StringBuilder();
                List<EntityModel> list = getDataTableColumns(tabName);
                foreach (EntityModel model in list)
                {
                    sb.AppendFormat("/// <summary>\r\n/// {0}\r\n/// </summary>\r\n", model.FileDesc);
                    sb.AppendFormat("public {0} {1} {2}",
                        DataTypeUtils.ParseType(model.FieldType),
                        model.FieldName, "{ get; set;}");
                    sb.Append(Environment.NewLine);
                }

                sb.Append("//参数开始");
                sb.Append(Environment.NewLine);
                sb.Append("IDataParameter[] Param = new IDataParameter[]{");
                sb.Append(Environment.NewLine);
                int count = list.Count;
                byte index = 1;
                string insertStart = "Insert Into " + tabName + "(";
                string insertEnd = "Values(";
                string updateStart = "Update " + tabName + " Set ";
                foreach (EntityModel model in list)
                {
                    sb.AppendFormat("    DbHelper.MakeParam(\"@{0}\",model.{0})",
                       model.FieldName);
                    insertStart += string.Format("{0}{1}", model.FieldName, index < count ? "," : ")");
                    insertEnd += string.Format("@{0}{1}", model.FieldName, index < count ? "," : ")");
                    updateStart += string.Format("{0}=@{0}{1}", model.FieldName, index < count ? "," : " where ");
                    if (index < count)
                        sb.Append(",");
                    sb.Append(Environment.NewLine);
                    index++;
                }
                sb.Append("};");
                sb.Append(Environment.NewLine);
                sb.Append("//插入语句");
                sb.Append(Environment.NewLine);
                sb.AppendFormat("{0}{1}", insertStart, insertEnd);
                sb.Append(Environment.NewLine);
                sb.Append("//更新语句适当修改");
                sb.Append(Environment.NewLine);
                sb.Append(updateStart);
                list.Clear();
            }
            if (sb != null)
                return sb.ToString(); ;
            return string.Empty;
        }

        #region 读取表字段名及类型

        private static List<EntityModel> getDataTableColumns(string tablName)
        {
            List<EntityModel> list = new List<EntityModel>();
            //string str = string.Format(SQL, tablName);
            //using (IDataReader reader = DbHelper.ExecuteReader(str))
            //{
            //    if (!reader.IsClosed)
            //    {
            //        while (reader.Read())
            //        {
            //            EntityModel model = new EntityModel();
            //            model.FieldName = reader[0].ToString();
            //            model.FieldType = reader[1].ToString();
            //            model.FieldSize = Convert.IsDBNull(reader[2]) ? 0 : Convert.ToInt32(reader[2]);
            //            model.FileDesc = reader[3].ToString();

            //            list.Add(model);
            //        }
            //    }
            //}
            return list;
        }

        #endregion 读取表字段名及类型

        #region 命名空间部分

        private static string getNameSpace(string nameSpace, Language language)
        {
            StringBuilder sb = new StringBuilder();
            if (language == Language.net)
            {
                sb.Append("using System;");
                sb.Append(Environment.NewLine);
                sb.Append("using System.Collections.Generic;");
                sb.Append(Environment.NewLine);
                if (!string.IsNullOrEmpty(nameSpace))
                {
                    sb.AppendFormat("namespace {0}", nameSpace);
                    sb.Append(Environment.NewLine);
                    sb.Append("{");
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        #endregion 命名空间部分

        #region 语言类型

        public enum Language
        {
            net,
            java
        }

        #endregion 语言类型
    }

    [Serializable]
    internal class EntityModel
    {
        public string FieldName { get; set; }
        public int FieldSize { get; set; }
        public string FieldType { get; set; }
        public string FileDesc { get; set; }
        public string NameSpace { get; set; }
    }
}