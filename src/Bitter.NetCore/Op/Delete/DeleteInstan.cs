using Bitter.Tools.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitter.Core
{
    public class DeleteIns<T>: BaseQuery  where T: BaseModel
    {

        public DeleteIns(T data, string targetdb = null)
        {
            excutParBag = new ExcutParBag_Delete();
            this.SetTargetDb(targetdb.ToSafeString());
            excutParBag.excutEnum = ExcutEnum.Delete;
            this.excutParBag.data = data;
        }
        public DeleteIns()
        {
            excutParBag = new ExcutParBag_Delete();
            excutParBag.excutEnum = ExcutEnum.Delete;
        }
    }
}
