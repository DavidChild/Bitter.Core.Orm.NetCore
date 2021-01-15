using Bitter.Tools.Utils;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Text;
using Bitter.Base;

namespace Bitter.Core
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/2/14 12:46:59
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    [Serializable]
    public class PageQuery : BasePageQuery, IPageAccess
    {


        /// <summary>
        /// 返回动态列
        /// </summary>
        private dynamic selectDynamicColumn;

        /// <summary>
        ///  是否已执行all 
        /// </summary>
        private bool isExAll = false;
        /// <summary>
        /// 是否已执行分页
        /// </summary>
        private bool isExSkip = false;
        /// <summary>
        /// 总数量
        /// </summary>
        private Int32 totalCount;
        /// <summary>
        /// 分页数据
        /// </summary>
        private DataTable pagedt;

        private IUnionPageAccess unionPage = new UnionPage();




        public PageQuery(string pageQuery, string targetdb)
        {
            this.excutParBag = new ExcutParBag_Page();
            ((ExcutParBag_Page)this.excutParBag).excutEnum = ExcutEnum.PageQuery;
            ((ExcutParBag_Page)this.excutParBag)._pageMode = PageMode.SelectCount;
            if (!string.IsNullOrEmpty(pageQuery))
            {
                ((ExcutParBag_Page)this.excutParBag).commandText = pageQuery;
            }
            if (!string.IsNullOrEmpty(targetdb))
            {
                this.SetTargetDb(targetdb);
            }


        }

        public PageQuery()
        {
            ((ExcutParBag_Page)this.excutParBag)._pageMode = PageMode.SelectCount;
        }

        private Int32 _totalcount = -1;
        /// <summary>
        /// 获取分页数量
        /// </summary>
        public Int32 Count()
        {
                if (_totalcount == -1)
                {
                    return _totalcount = GetCount();
                }
                else
                {
                    return _totalcount;
                }
            
        }


        private string _lowerCommandText { get; set; }

        private string LowerCommandText
        {
            get
            {
                if (string.IsNullOrEmpty(_lowerCommandText))
                {
                    _lowerCommandText = ((ExcutParBag_Page)this.excutParBag).commandText.ToLower();
                }
                return _lowerCommandText;
            }
        }




        /// <summary>
        /// Colums
        /// </summary>
        private string Columns
        {
            get
            {
                if (LowerCommandText.IndexOf("]") > -1 && ((ExcutParBag_Page)this.excutParBag)._pageMode == PageMode.SelectCount)
                {
                    int indexfrom = LowerCommandText.IndexOf(" from", LowerCommandText.IndexOf("]"), StringComparison.OrdinalIgnoreCase);
                    return ((ExcutParBag_Page)this.excutParBag).commandText.Substring(LowerCommandText.IndexOf("select") + 6, indexfrom - LowerCommandText.IndexOf("select") - 5);
                }
                else
                {
                    return ((ExcutParBag_Page)this.excutParBag).commandText.Substring(LowerCommandText.IndexOf("select") + 6, LowerCommandText.IndexOf(" from") - LowerCommandText.IndexOf("select") - 5);

                }
            }
        }

        /// <summary>
        /// table
        /// </summary>
        private string TableName
        {
            get
            {

                if (LowerCommandText.IndexOf("]") > -1 && ((ExcutParBag_Page)this.excutParBag)._pageMode == PageMode.SelectCount)
                {
                    int indexfrom = LowerCommandText.IndexOf(" from", LowerCommandText.IndexOf("]"), StringComparison.OrdinalIgnoreCase);
                    return ((ExcutParBag_Page)this.excutParBag).commandText.Substring(indexfrom + 5);
                }
                else
                {
                    return ((ExcutParBag_Page)this.excutParBag).commandText.Substring((((ExcutParBag_Page)this.excutParBag).commandText.ToLower().IndexOf(" from") + 5));
                }

            }
        }

        /// <summary>
        /// 获取全部数据，不分页
        /// </summary>
        /// <returns></returns>
        public IPageAccess All()
        {
            ((ExcutParBag_Page)this.excutParBag).pageIndex = 1;
            ((ExcutParBag_Page)this.excutParBag).pageSize = Int32.MaxValue;
            ((ExcutParBag_Page)this.excutParBag).isPage = true;
            isExAll = true;
            return this;
        }



        public IPageAccess OrderBy(string fullOrder)
        {
            if (string.IsNullOrEmpty(((ExcutParBag_Page)this.excutParBag).orderBy.ToSafeString()))
                ((ExcutParBag_Page)this.excutParBag).orderBy.Append(string.Format("{0} ", fullOrder));
            else ((ExcutParBag_Page)this.excutParBag).orderBy.Append(string.Format(",{0} ", fullOrder));
            return this;
        }

        /// <summary>
        /// 设置动态列
        /// </summary>
        /// <param name="selectColums"></param>
        public IPageAccess Select(dynamic selectDynamicColums)
        {
            selectDynamicColumn = selectDynamicColums;
            return this;
        }

        /// <summary>
        /// 第几页
        /// </summary>
        /// <param name="pageIndex"></param>
        public IPageAccess Skip(Int32 pageIndex, bool isApp = false)
        {
            if (isApp)
            {
                ((ExcutParBag_Page)this.excutParBag).pageIndex = (pageIndex + 1);
            }
            else
            {
                ((ExcutParBag_Page)this.excutParBag).pageIndex = pageIndex;
            }
            ((ExcutParBag_Page)this.excutParBag).isPage = true;
            isExSkip = true;
            return this;
        }

        /// <summary>
        /// 每页显示多少
        /// </summary>
        /// <param name="pageSize"></param>
        public IPageAccess Take(Int32 pageSize)
        {
            ((ExcutParBag_Page)this.excutParBag).pageSize = pageSize;
            ((ExcutParBag_Page)this.excutParBag).isPage = true;
            isExSkip = true;
            return this;
        }

        public IPageAccess ThenASC(string filedName)
        {
            if (string.IsNullOrEmpty(((ExcutParBag_Page)this.excutParBag).orderBy.ToSafeString()))
                ((ExcutParBag_Page)this.excutParBag).orderBy.Append(string.Format("{0} ASC", filedName));
            else ((ExcutParBag_Page)this.excutParBag).orderBy.Append(string.Format(",{0} ASC", filedName));
            return this;
        }

        public IPageAccess ThenDESC(string filedName)
        {
            if (string.IsNullOrEmpty(((ExcutParBag_Page)this.excutParBag).orderBy.ToSafeString()))
                ((ExcutParBag_Page)this.excutParBag).orderBy.Append(string.Format("{0} DESC", filedName));
            else ((ExcutParBag_Page)this.excutParBag).orderBy.Append(string.Format(",{0} DESC", filedName));
            return this;
        }


        public page Page
        {
            get
            {
                page page = new page();
                page = UntilsPage.GetPageObject(((ExcutParBag_Page)this.excutParBag).pageIndex, ((ExcutParBag_Page)this.excutParBag).pageSize,_totalcount);
                return page;
            }
        }



        /// <summary>
        /// 返回数据集合
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            try
            {
                if (this.pagedt == null)
                {
                    SetData();
                    return this.pagedt;
                }
                else
                {
                    if (isExAll && isExSkip)
                        SetData();
                    return this.pagedt;
                }

            }
            finally
            {
            }

        }



        /// <summary>
        /// 返回UnionPage
        /// </summary>
        /// <param name="page">pageQuery</param>
        /// <returns></returns>
        public IUnionPageAccess ToUnionPage()
        {
            unionPage.Union(this);
            return unionPage;
        }

        /// <summary>
        /// 关联表
        /// </summary>
        /// <param name="page">pageQuery</param>
        /// <returns></returns>
        public IUnionPageAccess Union(IPageAccess page)
        {
            unionPage.Union(this);
            unionPage.Union(page);
            return unionPage;
        }

        /// <summary>
        /// 返回SetWhere
        /// </summary>
        /// <param name="setwhere"></param>
        /// <param name="dynamicParms"></param>
        public IPageAccess Where(string setwhere, dynamic dynamicParms)
        {
            if (dynamicParms != null)
            {

                ((ExcutParBag_Page)this.excutParBag).dynamics.Add(dynamicParms);
            }
            ((ExcutParBag_Page)this.excutParBag).whereBuiler.Append(string.Format(" and ({0})", setwhere));
            return this;
        }

        /// <summary>
        /// setWhere
        /// </summary>
        /// <param name="setwhere"></param>
        public IPageAccess Where(string setwhere)
        {
            ((ExcutParBag_Page)this.excutParBag).whereBuiler.Append(string.Format(" and ({0})", setwhere));
            return this;
        }


        /// <summary>
        /// Or; 注意：此时的Or, 跟前置条件条件 永远是并列关系
        /// 例如：1：（(x.y=="1") or (x.z=3)） or (x.n=4)
        /// 例如：2：（(x.y=="1") and (x.z=3)） or (x.n=4)
        /// 一定要注意：并没有这样的写法：(x.y=="1") and (x.z=3) or (x.n=4)，杜绝这种错误的写法
        /// 在使用Or时,自动将你之前的所有的条件会放到一个() 中跟你现有的Or 形成并条件 例如： (之前写的条件) or (现有的条件) ， 永远是这种关系
        /// </summary>
        /// <param name="setOr"></param>
        /// <param name="setwhere"></param>
        public IPageAccess Or(string setOr)
        {
            if (!string.IsNullOrEmpty(setOr))
            {
                ((ExcutParBag_Page)this.excutParBag).whereBuiler.Insert(0, "(");
                ((ExcutParBag_Page)this.excutParBag).whereBuiler.Append(")");
                ((ExcutParBag_Page)this.excutParBag).whereBuiler.Append(string.Format(" Or ({0})", setOr));

            }

            return this;
        }
        /// <summary>
        /// Or; 注意：此时的Or, 跟前置条件条件 永远是并列关系
        /// 例如：1：（(x.y=="1") or (x.z=3)） or (x.n=4)
        /// 例如：2：（(x.y=="1") and (x.z=3)） or (x.n=4)
        /// 一定要注意：并没有这样的写法：(x.y=="1") and (x.z=3) or (x.n=4)，杜绝这种错误的写法
        /// 在使用Or时,自动将你之前的所有的条件会放到一个() 中跟你现有的Or 形成并条件 例如： (之前写的条件) or (现有的条件) ， 永远是这种关系
        /// </summary>
        /// <param name="setOr"></param>
        /// <param name="dynamicParms"></param>
        public IPageAccess Or(string setOr, dynamic dynamicParms)
        {

            if (dynamicParms != null)
            {

                ((ExcutParBag_Page)this.excutParBag).dynamics.Add(dynamicParms);
            }
            if (!string.IsNullOrEmpty(setOr))
            {
                ((ExcutParBag_Page)this.excutParBag).whereBuiler.Insert(0, "(");
                ((ExcutParBag_Page)this.excutParBag).whereBuiler.Append(")");
                ((ExcutParBag_Page)this.excutParBag).whereBuiler.Append(string.Format(" Or ({0})", setOr));

            }
            return this;
        }
        /// <summary>
        /// 获取总数量
        /// </summary>
        /// <returns></returns>
        private Int32 GetCount()
        {
            try
            {
                if (_totalcount == -1)//未查询
                {

                    SetData();
                    return _totalcount;
                }
                else
                    return _totalcount;
            }
            finally
            {

            }
        }


        private void SetData()
        {
           
             
            ((ExcutParBag_Page)this.excutParBag).pageTableName = this.TableName;
            ((ExcutParBag_Page)this.excutParBag).pageColumns = this.Columns;
            DataTable dt;
            if (this.databaseType == DatabaseType.MySql)
            {
                dt = this.ReturnDataTable(1);
            }
            else
            {
                dt = this.ReturnDataTable();
            }
         
            
            if (dt.Rows.Count > 0)
            {
                this._totalcount = int.Parse(dt.Rows[0]["CHEOKTOTALCOUNT"].ToSafeString("0"));
                if (((ExcutParBag_Page)this.excutParBag).isPage)
                {
                    dt.Columns.Remove("CHEOKTOTALCOUNT");
                    this.pagedt = dt;
                }
            }
            else
            {
                this._totalcount = 0;
                if (((ExcutParBag_Page)this.excutParBag).isPage)
                {
                    this.pagedt = new DataTable();

                }
            }
             ((ExcutParBag_Page)this.excutParBag).isPage = false;
        }

        public IPageAccess AddPreWith(string withSql)
        {
            if (string.IsNullOrEmpty(withSql)) return this;
            ((ExcutParBag_Page)this.excutParBag)._preWith =
                @"

            " +
                ((ExcutParBag_Page)this.excutParBag)._preWith.ToSafeString("") + withSql
                + @"

            ";

            return this;
        }


        public IPageAccess AddPreWith(string withSql, dynamic parmaters)
        {
            if (string.IsNullOrEmpty(withSql)) return this;
            ((ExcutParBag_Page)this.excutParBag)._preWith =
            @"

        " +
            ((ExcutParBag_Page)this.excutParBag)._preWith.ToSafeString("") + withSql
            + @"

        ";

            if (parmaters != null)
            {

                ((ExcutParBag_Page)this.excutParBag).dynamics.Add(parmaters);
            }

            return this;
        }

        /// <summary>
        /// 设置获取分页记录总数的模式
        /// 默认:COUNTOVER 方式
        /// </summary>
        /// <param name="pageMode"></param>
        public IPageAccess SetPageMode(PageMode pageMode)
        {
            ((ExcutParBag_Page)this.excutParBag)._pageMode = pageMode;
            return this;
        }

    }
}