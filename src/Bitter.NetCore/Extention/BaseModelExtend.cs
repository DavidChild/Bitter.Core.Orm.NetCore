using Bitter.Tools;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Bitter.Tools.Utils;

namespace Bitter.Core
{
    public static class BaseModelExtend
    {
        #region //对象update的支持
        public static UpdateIns<T> Update<T>(this T o) where T : BaseModel,new()
        {
            return new UpdateIns<T>(o);
        }
        #endregion

        #region  //Delete
        public static DeleteIns<T> Delete<T>(this T o) where T : BaseModel, new()
        {
            return new DeleteIns<T>(o);
        }
        public static InsertIns<T> Insert<T>(this T o) where T : BaseModel, new()
        {
            return new InsertIns<T>(o,true);
        }

        public static int Add<T>(this T o) where T : BaseModel, new()
        {
            Exception exception = null;
            int identity= 0;
            InsertIns<T> t=new   InsertIns<T>(o, true);
            identity=t.Submit(out exception);
            if(exception!=null)
            {

                throw exception;
            }
            
            return identity;
        }

        #endregion










    }
}
