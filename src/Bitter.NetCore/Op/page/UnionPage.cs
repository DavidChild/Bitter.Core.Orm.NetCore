using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Bitter.Core
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/2/16 11:29:50
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public class UnionPage : BasePageQuery, IUnionPageAccess
    {
        private Int32 totalCount;
        private List<IPageAccess> unionQueryList = new List<IPageAccess>();

        /// <summary>
        /// 集合总数量
        /// </summary>
        public Int32  Count()
        {
             
                var sum = 0;
                foreach (IPageAccess page in unionQueryList)
                {
                    sum += page.Count();
                }
                return sum;
            
        }

        /// <summary>
        /// 当前第几页
        /// </summary>
        /// <param name="pageIndex"></param>
        public IUnionPageAccess Skip(Int32 pageIndex, bool isApp = false)
        {
            foreach (IPageAccess page in unionQueryList)
            {
                if (isApp)
                {
                    page.Skip(pageIndex + 1);
                }
                else
                {
                    page.Skip(pageIndex);
                }
            }
            return this;
        }

        /// <summary>
        /// 每页显示多少
        /// </summary>
        /// <param name="pageSize"></param>
        public IUnionPageAccess Take(Int32 pageSize)
        {
            foreach (IPageAccess page in unionQueryList)
            {
                page.Take(pageSize);
            }
            return this;
        }

        public IUnionPageAccess ThenASC(string filedName)
        {
            foreach (IPageAccess page in unionQueryList)
            {
                page.ThenASC(filedName);
            }
            return this;
        }

        public IUnionPageAccess ThenDESC(string filedName)
        {
            foreach (IPageAccess page in unionQueryList)
            {
                page.ThenDESC(filedName);
            }
            return this;
        }

        /// <summary>
        /// 求出集合数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataRow> ToDataTable()
        {
            IEnumerable<DataRow> DRS = null;
            DataTable Dt = null;
            foreach (IPageAccess page in unionQueryList)
            {
                if (null == DRS)
                {
                    Dt = page.ToDataTable();
                    DRS = Dt.Rows.Cast<DataRow>();
                }
                else
                {
                    DRS = DRS.Union(page.ToDataTable().Rows.Cast<DataRow>());
                }
            }
            if (DRS != null && DRS.Count() > 0)
            {
                return DRS;
            }
            else
            {
                return (IEnumerable<DataRow>)Dt;
            }
        }

        /// <summary>
        /// 关联的集合的语句
        /// </summary>
        /// <param name="page">并入到集合中</param>
        /// <returns></returns>
        public IUnionPageAccess Union(IPageAccess page)
        {
            unionQueryList.Add(page);
            return this;
        }

        /// <summary>
        /// 关联的集合列表
        /// </summary>
        /// <param name="listPage">并入到集合中</param>
        /// <returns></returns>
        public IUnionPageAccess Union(List<IPageAccess> listPage)
        {
            unionQueryList.AddRange(listPage);
            return this;
        }

        /// <summary>
        /// 返回SetWhere
        /// </summary>
        /// <param name="setwhere"></param>
        /// <param name="dynamicParms"></param>
        public IUnionPageAccess Where(string setwhere, dynamic dynamicParms)
        {
            foreach (IPageAccess page in unionQueryList)
            {
                page.Where(setwhere, dynamicParms);
            }
            return this;
        }

        /// <summary>
        /// setWhere
        /// </summary>
        /// <param name="setwhere"></param>
        public IUnionPageAccess Where(string setwhere)
        {
            foreach (IPageAccess page in unionQueryList)
            {
                page.Where(setwhere);
            }
            return this;
        }

        /// <summary>
        /// Or; 注意：此时的Or, 跟前置条件条件 永远是并列关系
        /// 例如：1：（(x.y=="1") or (x.z=3)） or (x.n=4)
        /// 例如：2：（(x.y=="1") and (x.z=3)） or (x.n=4)
        /// 一定要注意：并没有这样的写法：(x.y=="1") and (x.z=3) or (x.n=4)，杜绝这种错误的写法
        /// </summary>
        /// <param name="setOr"></param>
        public IUnionPageAccess Or(string setOr)
        {
            foreach (IPageAccess page in unionQueryList)
            {
                page.Or(setOr);
            }
            return this;
        }

        /// <summary>
        /// Or; 注意：此时的Or, 跟前置条件条件 永远是并列关系
        /// 例如：1：（(x.y=="1") or (x.z=3)） or (x.n=4)
        /// 例如：2：（(x.y=="1") and (x.z=3)） or (x.n=4)
        /// 一定要注意：并没有这样的写法：(x.y=="1") and (x.z=3) or (x.n=4)，杜绝这种错误的写法
        /// </summary>
        /// <param name="setOr"></param>
        public IUnionPageAccess Or(string setOr, dynamic dynamicParms)
        {
            foreach (IPageAccess page in unionQueryList)
            {
                page.Or(setOr, dynamicParms);
            }
            return this;
        }


        public IUnionPageAccess AddPreWith(string withSql)
        {
            foreach (IPageAccess page in unionQueryList)
            {
                page.AddPreWith(withSql);
            }
            return this;
        }

        public IUnionPageAccess AddPreWith(string withSql, dynamic parmaters)
        {
            foreach (IPageAccess page in unionQueryList)
            {
                page.AddPreWith(withSql, parmaters);
            }
            return this;
        }
        public IUnionPageAccess SetPageMode(PageMode pageMode)
        {
            foreach (IPageAccess page in unionQueryList)
            {
                page.SetPageMode(pageMode);
            }
            return this;
        }
        
    }
}