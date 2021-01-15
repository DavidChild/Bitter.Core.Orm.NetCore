namespace Bitter.Base
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2016/7/14 15:07:07
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public class UntilsPage
    {
        /// <summary>
        /// 获取Page对象
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageRecords"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public static page GetPageObject(int currentPage, int pageRecords, int totalRecords)
        {
            page p = new page() { totalPages = 0, totalRecords = 0, hasNextPage = false, hasPreviousPage = false };
            if (currentPage < 0)
            {
                return p;
            }
            if (pageRecords <= 0)
            {
                return p;
            }
            if (totalRecords <= 0)
            {
                return p;
            }

            //当前页码数
            p.currentPage = currentPage;
            //每页记录数
            p.pageRecords = pageRecords;
            //总记录数
            p.totalRecords = totalRecords;
            //计算页码总数

            if (totalRecords < pageRecords)
            {
                p.totalPages = totalRecords == 0 ? 0 : 1;
            }
            else
            {
                if ((totalRecords % pageRecords) == 0)
                {
                    p.totalPages = (p.totalRecords / p.pageRecords);
                }
                else
                {
                    p.totalPages = ((p.totalRecords / p.pageRecords) + 1);
                }
            }

            //if ((totalRecords % pageRecords) > 0)
            //{
            //    p.totalPages = ((p.totalRecords / p.pageRecords) + 1);
            //}
            //else
            //{
            //    p.totalPages = (p.totalRecords / p.pageRecords);
            //}

            p.totalPages = (p.totalPages - 1);
            //开始记录数
            p.startRecord = 0;
            //判断当前页面是否为最后一页
            if ((currentPage < p.totalPages) && p.totalPages > 0)
            {
                //下一页
                p.nextPage = currentPage + 1;
                //是否有下一页
                p.hasNextPage = true;
            }
            else
            {
                p.hasNextPage = false;
            }

            //判断当前页面是否为第一页
            if ((currentPage > 0) && (currentPage <= p.totalPages) && (p.totalPages > 0))
            {
                //上一页
                p.previousPage = currentPage - 1;
                //是否有上一页
                p.hasPreviousPage = true;
            }
            else
            {
                p.hasPreviousPage = false;
            }
            p.totalPages = (p.totalPages + 1);
            return p;
        }
    }
}