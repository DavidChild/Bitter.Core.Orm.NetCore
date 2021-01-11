using BT.Manage.Core;
using System;
using System.Collections.Generic;
using System.Text;
using BT.Manage.Tools.Utils;
namespace BT.Manage.Core
{
    public class InsertIns<T>:BaseQuery where T:BaseModel
    {
      

        public InsertIns(T data,string targetdb=null)
        {
           
            excutParBag = new ExcutParBag_Insert();
            this.SetTargetDb(targetdb.ToSafeString());
            data._targetdb = targetdb;
            ((ExcutParBag_Insert)excutParBag).excutEnum = ExcutEnum.Insert;
            ((ExcutParBag_Insert)excutParBag).data = data;
        
            ((ExcutParBag_Insert)excutParBag).isOutIdentity = false; //
            CheckedIdentityVlaue();

        }

      

        public InsertIns(T data,bool isOutIdentity)
        {
            
             excutParBag = new ExcutParBag_Insert();
            ((ExcutParBag_Insert)excutParBag).excutEnum = ExcutEnum.Insert;
            ((ExcutParBag_Insert)excutParBag).SetType(typeof(T));
            ((ExcutParBag_Insert)excutParBag).data = data;
            ((ExcutParBag_Insert)excutParBag).isOutIdentity = true; //插入后获取 Identity
            CheckedIdentityVlaue();

        }

        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName"></param>
        public InsertIns<T> SetTableName(string tableName)
        {
            excutParBag.SetTableName(tableName);
            return this;
        }



        private void CheckedIdentityVlaue()
        {
            var  key = Utils.GetKeyFiledName(((ExcutParBag_Insert)excutParBag).data);
            var value = Utils.GetObjectPropertyValue(((ExcutParBag_Insert)excutParBag).data, key);
            var isIdentity = Utils .CheckedIsIdentity(((ExcutParBag_Insert)excutParBag).data.GetType(), key);
            if ((isIdentity && value.ToSafeString("0") != "0"))
            {
                throw new Exception("违反主键自增长约束： Insert 语句具有主键自增长的ID，在插入的时候不能具有自增长值.");
            }
            

        }
    }
}
