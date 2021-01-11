using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using BT.Manage.Tools.Utils;
using System.Data.SqlClient;

namespace BT.Manage.Core
{
    internal class MySqlQuery : BaseQuery
    {
        //构建新增
        protected override void InsertCommandText(bool IsOutIdentity = false)
        {
            base.InsertCommandText();
            ExcutParBag_Insert bagPar = (ExcutParBag_Insert)this.excutParBag;
            string fields = string.Empty;
            string values = string.Empty;
            foreach (FiledProperty filed in bagPar.PropertyFileds)
            {
                string propertyValue = string.Empty;

                if ((filed.isKey && !filed.isIdentity) || (!filed.isIdentity))
                {
                    var shortguid = "_" + BT.Manage.Tools.Utils.GuidExtends.ShortGuid();
                    fields += filed.filedName + ',';
                    values += "@" + filed.filedName + shortguid + ',';
                    var val = filed.value;

                    this.Parameters.Add("@" + filed.filedName + shortguid, ((val == null && filed.isNull) || (filed.typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(val == null ? string.Empty : val.ToString()))) ? DBNull.Value : val, Utils.SqlTypeString2SqlType(filed.typeName.ToLower()));
                }
            }
            var selectParimaryKey = "SELECT last_insert_id() as FKey;";
            if (!bagPar.isIdentityModel)
            {
                var keyName = bagPar.keyName;
                var keyValue = (from p in bagPar.PropertyFileds where p.isKey = true select p).Select(x => x.value);
                selectParimaryKey = "SELECT '" + keyValue + "' AS FKey;";
            }
            this.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2});{3}", bagPar.tableName, fields.Substring(0, fields.Length - 1), values.Substring(0, values.Length - 1), selectParimaryKey);
        }

        //构建删除
        protected override void DeleteCommandText()
        {
            base.DeleteCommandText();
            ExcutParBag_Delete bagPar = (ExcutParBag_Delete)this.excutParBag;
            StringBuilder deleteSQL = new StringBuilder();  //delete语句
            deleteSQL.Append("DELETE FROM ").Append(bagPar.tableName);
            StringBuilder whereSQL = new StringBuilder();  //delete条件

            if (bagPar.condition != null)
            {
                whereSQL.Append(" WHERE ");
                whereSQL.AppendFormat(this.SetWhere(bagPar.condition));
            }
            else if (bagPar.data != null)
            {
                whereSQL.Append(" WHERE ");
                var keyName = bagPar.keyName;
                var keyFiled = (from p in bagPar.PropertyFileds where p.isKey = true select p).FirstOrDefault();
                var shortguid = "_" + BT.Manage.Tools.Utils.GuidExtends.ShortGuid();
                this.Parameters.Add("@" + bagPar.keyName + shortguid, ((keyFiled == null && keyFiled.isNull) || (keyFiled.typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(keyFiled.value == null ? string.Empty : keyFiled.value.ToString()))) ? DBNull.Value : keyFiled.value, Utils.SqlTypeString2SqlType(keyFiled.typeName.ToLower()));
                whereSQL.AppendFormat("{0}=@{1}", keyFiled.filedName, keyFiled.filedName + shortguid);
            }
            this.CommandText = string.Format("{0}{1};", deleteSQL.ToString(), whereSQL.ToString());

        }


        //构建更新
        protected override void UpdateCommandText()
        {
            base.UpdateCommandText();
            ExcutParBag_Update bagPar = (ExcutParBag_Update)this.excutParBag;
            StringBuilder updateSQL = new StringBuilder();  //update语句
            StringBuilder whereSQL = new StringBuilder();  //update条件语句
            updateSQL.Append("UPDATE ").Append(bagPar.tableName).Append("  SET  ");

            if (bagPar.condition != null) whereSQL.AppendFormat(this.SetWhere(bagPar.condition));
            if (bagPar.data != null)
            {
                whereSQL.Append(" WHERE ");
                var keyValue = (from p in bagPar.PropertyFileds where p.isKey == true select p).FirstOrDefault();
                var shortguid = "_" + BT.Manage.Tools.Utils.GuidExtends.ShortGuid();
                whereSQL.AppendFormat(keyValue.filedName + "=@{0} ", keyValue.filedName + shortguid);



                this.Parameters.Add("@" + keyValue.filedName + shortguid, keyValue.value, keyValue);
            }
            else
            {
                if (bagPar.condition != null)
                {
                    whereSQL.Append(" WHERE ");
                    whereSQL.AppendFormat(this.SetWhere(bagPar.condition));
                }
            }
            if (bagPar.updatePair != null && bagPar.updatePair.Count > 0)
            {
                foreach (UpdatePair p in bagPar.updatePair)
                {
                    var pf = bagPar.PropertyFileds.Where(px => px.filedName == p.columnName).FirstOrDefault();
                    var shortguid = "_" + BT.Manage.Tools.Utils.GuidExtends.ShortGuid();
                    updateSQL.AppendFormat("{0}=@{1},", p.columnName, p.columnName + shortguid);
                    this.Parameters.Add("@" + p.columnName + shortguid, p.columnValue, pf);
                }
            }
            else
            {
                if (bagPar.data != null)
                {
                    foreach (var p in bagPar.PropertyFileds)
                    {


                        var shortguid = "_" + BT.Manage.Tools.Utils.GuidExtends.ShortGuid();
                        updateSQL.AppendFormat("{0}=@{1},", p.filedName, p.filedName + shortguid);
                        this.Parameters.Add("@" + p.filedName + shortguid, p.value, p);
                    }
                }
            }

            this.CommandText = string.Format("{0}{1};", updateSQL.ToString().Substring(0, updateSQL.ToString().Length - 1), whereSQL.ToString());


        }
        //构建检索
        protected override void SelectCommandText()
        {
            base.SelectCommandText();
            ExcutParBag_Select bagPar = (ExcutParBag_Select)this.excutParBag;
            StringBuilder selectSQL = new StringBuilder();  //语句
            StringBuilder whereSQL = new StringBuilder();  //条件语句
            StringBuilder order = new StringBuilder("");  //条件语句
            if (bagPar.orders != null && bagPar.orders.Count > 0)
            {
                order.Append(" ORDER BY ");
                List<string> orderlist = new List<string>();
                bagPar.orders.ForEach(p =>
                {
                    orderlist.Add(p.orderName + " " + p.orderBy.ToString());
                });
                order.Append(string.Join(",", orderlist));
            }
            selectSQL.Append("SELECT ");
          
            if (bagPar.selectColumns != null && bagPar.selectColumns.Count() > 0)
            {
                selectSQL.Append(string.Join(",", bagPar.selectColumns));
            }
            else
            {
                selectSQL.Append(" * ");
            }
            selectSQL.Append(" FROM  " + bagPar.tableName);
            var where = this.SetWhere(bagPar.condition);
            if (!string.IsNullOrEmpty(where))
            {
                whereSQL.AppendFormat(" WHERE ");
                whereSQL.AppendFormat(where);
            }
            if (bagPar.topSize.HasValue)
            {
                this.CommandText = string.Format("{0}{1}{2}{3}", selectSQL.ToString(), whereSQL.ToString(), order.ToString(), " LIMIT " + bagPar.topSize.Value + " ");
               
            }
            else
            {
                this.CommandText = string.Format("{0}{1}{2}", selectSQL.ToString(), whereSQL.ToString(), order.ToString());
            }

          

        }
        //检索总数量
        protected override void CountCommandText()
        {
            base.SelectCommandText();
            ExcutParBag_Count bagPar = (ExcutParBag_Count)this.excutParBag;
            StringBuilder selectSQL = new StringBuilder();  //语句
            StringBuilder whereSQL = new StringBuilder();  //条件语句
            selectSQL.Append("SELECT COUNT(0) ");
            selectSQL.Append("FROM  " + bagPar.tableName + " ");
            var where = this.SetWhere(bagPar.condition);
            if (!string.IsNullOrEmpty(where))
            {
                whereSQL.Append(" WHERE ");
                whereSQL.Append(where);
            }
            this.CommandText = string.Format("{0}{1};", selectSQL.ToString(), whereSQL.ToString());

        }

        //构建自定义SQL语句
        protected override void ExcutCommandText()
        {
            base.ExcutCommandText();
            ExcutParBag_Excut bagPar = (ExcutParBag_Excut)this.excutParBag;
            this.CommandText = bagPar.commandText;
            this.SetDynamicParameters(bagPar.dynamicParma);

        }

        protected override void PageCommandText()
        {
            MyPageManage.SelectPageData(this);
        }





    }
}
