namespace Bitter.Base
{
    /// <summary>
    /// page内置对象
    /// </summary>
    public class page
    {
        public page() { }
        /// <summary>
        /// 实例化page对象 适用手机端/PC端
        /// </summary>
        /// <param name="currPage">当前页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="totalRecords">总记录数</param>
        /// <param name="isFistPageZero">当前第一页是否为0</param>
        public page(int currPage, int pageSize, int totalRecords, bool isFistPageZero = false)
        {
            //测试修改
            //BUG11
            page pa = UntilsPage.GetPageObject(currPage, pageSize, totalRecords);
            this.currentPage = pa.currentPage;
            this.pageRecords = pa.pageRecords;
            this.hasNextPage = pa.hasNextPage;
            this.hasPreviousPage = pa.hasPreviousPage;
            this.nextPage = pa.nextPage;
            this.previousPage = pa.previousPage;
            this.startRecord = pa.startRecord;
            this.totalPages = pa.totalPages;
            this.totalRecords = pa.totalRecords;
        }
        /** 当前页码 */
        public int currentPage = 0;
        /** 每页记录 */
        public int pageRecords = 10;
        /** 页码总数 */
        public bool hasNextPage { get; set; }
        public bool hasPreviousPage { get; set; }
        public int? nextPage { get; set; }
        public int previousPage { get; set; }
        public int? startRecord { get; set; }
        public int? totalPages { get; set; }  
        /** 总记录数 */
        public int? totalRecords { get; set; }
        /** 开始记录 */
        /** 下一页 */
        /** 上一页 */
        /** 是否有下一页 */
        /** 是否有前一页 */
    }
}