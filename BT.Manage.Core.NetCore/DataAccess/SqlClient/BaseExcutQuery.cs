using BT.Manage.Core.NetCore;
using BT.Manage.Tools;
using BT.Manage.Tools.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BT.Manage.Core
{
    public class BaseQuery: BaseExcut 
    {
       
        private BaseQuery qQeryy { get; set; }

        //构建新增
        protected virtual void InsertCommandText(bool IsOutIdentity = false) { ClearParmeters();   }


        //构建删除
        protected virtual void DeleteCommandText() { ClearParmeters(); }


        //构建更新
        protected virtual void  UpdateCommandText() { ClearParmeters(); }

        //构建检索
        protected virtual void SelectCommandText() { ClearParmeters(); }

        protected virtual void CountCommandText() { ClearParmeters(); }

        protected virtual void ExcutCommandText() { ClearParmeters(); }

        protected virtual void PageCommandText() { ClearParmeters(); }

        protected string  SetWhere(LambdaExpression lambda )
        {
            if(lambda!=null)
            {
                StringBuilder whereBuilder = new StringBuilder();
                BtConditionBuilder builder = new BtConditionBuilder();
                builder.Build(lambda.Body);
                //获取SQL参数数组 (包括查询参数和赋值参数)
                string sqlCondition = builder.Condition;
                List<string> lt = new List<string>();
                Int32 index = 0;
                builder.Arguments.ToList().ForEach(c =>
                {
              
                    var shortguid = BT.Manage.Tools.Utils.GuidExtends.ShortGuid();
                    lt.Add("@pt" + shortguid.ToSafeString());
                    this.Parameters.Add("@pt" + shortguid.ToSafeString(), c.ToSafeString(),null);
                    index++;
                });

                whereBuilder.AppendFormat(string.Format(sqlCondition, lt.ToArray()));
                return whereBuilder.ToString();
            }
            return " 1=1 ";
           
        }
        internal void SetDynamicParameters(List<dynamic> dynamicParameters)
        {
           
            if (dynamicParameters != null&& dynamicParameters.Count()>0)
            {
                 
                foreach(dynamic item in dynamicParameters)
                {

                    // predicate为一个dynamic
                    Type t = item.GetType();
                    //获取类的所有公共属性
                    System.Reflection.PropertyInfo[] pInfo = t.GetProperties();
                    // 遍历公共属性
                    foreach (System.Reflection.PropertyInfo pio in pInfo)
                    {
                        string fieldName = pio.Name; // 公共属性的Name
                        Type pioType = pio.PropertyType; // 公共属性的类型
                        SqlParameter x = new SqlParameter();

                        // x.DbType = d.DbType;
                        object o = pio.GetValue(item, null);
                        x.ParameterName = fieldName;
                        string propertyTypeName = string.Empty;
                        if (pioType.IsGenericType && pioType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            propertyTypeName = pioType.GetGenericArguments()[0].Name;
                        }
                        else
                        {
                            propertyTypeName = pioType.Name;
                        }
                        x.SqlDbType = Utils.SqlTypeString2SqlType(propertyTypeName.ToLower());
                        if (o == null || propertyTypeName.Equals("DBNull"))
                        {
                            x.Value = DBNull.Value;
                        }
                        else
                        {
                            x.Value = o.ToSafeString();
                        }
                        this.Parameters.Add(x);
                    }
                }
              
            }
         }



        private void ClearParmeters()
        {
            if (this.Parameters != null && this.Parameters.Count > 0) { this.Parameters.Clear(); }
        }
         
        //关键代码 转换实体，用于支持多数据库类型的操作
        internal void Convert(string  targetdb)
        {

            try
            {
                this.SetTargetDb(targetdb);
                qQeryy = this.MapToExutQuery();
            }
            catch (Exception ex)
            {
                LogService.Default.Fatal(
                   "core 映射失败："
                   + "执行类型："
                   + this.excutParBag.excutEnum.ToString()
                   + "异常信息："
                   + ex.Message
                   + "TableName:" + this.excutParBag.tableName
                   + ";"
                   + "|Stack:"
                   + ex.StackTrace);
                throw ex;
            }
          

            try
            {
             
                qQeryy.SetCommadText(); //构建对应的SQL语句
                this.Parameters = qQeryy.Parameters;
                this.CommandText = qQeryy.CommandText;
            }
            catch(Exception ex)
            {
                
                LogService.Default.Fatal(
                    "构建SQL语句出错："
                    +"执行类型：" 
                    + this.excutParBag.excutEnum .ToString()
                    +"异常信息："
                    + ex.Message
                    +"TableName:"+ this.excutParBag.tableName
                   
                    + ";"
                    +"|Stack:"
                    +ex.StackTrace);
                 
                 
            }
           
        }

        private void SetCommadText()
        {
            switch (excutParBag.excutEnum)
            {
                case ExcutEnum.Delete:
                    DeleteCommandText();
                    break;
                case ExcutEnum.Select:
                    SelectCommandText();
                    break;
                case ExcutEnum.Insert:
                     InsertCommandText();
                    break;
                case ExcutEnum.Update:
                    UpdateCommandText();
                    break;
                  case ExcutEnum.Count:
                    CountCommandText();
                    break;
                case ExcutEnum.ExcutQuery:
                    ExcutCommandText();
                    break;
                case ExcutEnum.PageQuery:
                     PageCommandText();
                    break;
                default: break;

            }
        }

        
    }
}