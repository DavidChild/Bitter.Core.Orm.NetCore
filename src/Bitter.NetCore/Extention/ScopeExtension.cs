using System;
using System.Collections.Generic;
using System.Text;
using Bitter.Tools.Utils;
using Bitter.Tools;
namespace Bitter.Core
{
   public  static class ScopeExtension
    {
        #region //事务对象收集
        /// <summary>
        /// 加入事务
        /// </summary>
        /// <param name="nlist"></param>
        /// <param name="scopelist"></param>
        public static void AddInScope(this List<BaseQuery> nlist, List<BaseQuery> scopelist)
        {
            scopelist.AddRange(nlist);
        }

        /// <summary>
        /// 将执行的语句加入事务
        /// </summary>
        /// <param name="nlist"></param>
        /// <param name="scopelist"></param>
        public static void AddInScope(this BaseQuery dosqlQuery, List<BaseQuery> scopelist)
        {
            scopelist.Add(dosqlQuery);
        }

        /// <summary>
        /// 将执行的语句加入事务
        /// </summary>
        /// <param name="nlist"></param>
        /// <param name="scopelist"></param>
        public static int AddInScope<T>(this T o, List<BaseModel> objList, string targetdb = null)
            where T : BaseModel, new()
        {
            //先判断是否是新增,如果不是新增就修改
            Int32 affectedCount = -1;
            affectedCount = o.Add();
           
            if (affectedCount>0)
            {
                T f = db.FindQuery<T>().QueryById(affectedCount);
                objList.Add(f);
            }
             return affectedCount;
        }


        /// <summary>
        /// 将对象加入到批量执行操作中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="bulkCopyModels"></param>
        public static void AddInScope<T>(this T o, List<BulkCopyModel> bulkCopyModels) where T : BulkCopyModel, new()
        {
            bulkCopyModels.Add(o);

        }
        /// <summary>
        /// 将对象加入到批量执行操作中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="bulkCopyModels"></param>
        public static void AddInScope<T>(this List<T> o, List<BulkCopyModel> bulkCopyModels) where T : BulkCopyModel, new()
        {
            bulkCopyModels.AddRange(o);

        }
      
        /// <summary>
        /// 将待执行插入的对象转化到BulkCopyModel 对象模型中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="bulkCopyModels"></param>
        public static BulkCopyModel BulkCopy<T>(this T o) where T : BaseModel, new()
        {

            BulkCopyModel copyModel = new BulkCopyModel();
            copyModel.CopyModel = o;
            return copyModel;

        }
        public static List<BulkCopyModel> BulkCopy<T>(this List<T> o) where T : BaseModel, new()
        {

            List<BulkCopyModel> bulks = new List<BulkCopyModel>();
            foreach (T item in o)
            {
                BulkCopyModel copyModel = new BulkCopyModel();
                copyModel.CopyModel = item;
                bulks.Add(copyModel);

            }


            return bulks;

        }
         #endregion
    }
}
