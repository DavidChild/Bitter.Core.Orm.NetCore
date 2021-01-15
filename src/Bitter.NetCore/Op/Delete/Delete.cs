using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Bitter.Tools.Utils;
namespace Bitter.Core
{
    public class Delete<T>:WhereQuery<T> where T:BaseModel
    {
        
       
        public  Delete(string targetdb = null)
        {
            excutParBag = new ExcutParBag_Delete();
            this.SetTargetDb(targetdb.ToSafeString());
            excutParBag.excutEnum = ExcutEnum.Delete;
            excutParBag.SetType(typeof(T));
        }
        public Delete<T> Where(Expression<Func<T, bool>> condition)
        {
            base.Where(condition);
            ((ExcutParBag_Delete)excutParBag).condition = this._Condition;
            return this;
        }
    }
}
