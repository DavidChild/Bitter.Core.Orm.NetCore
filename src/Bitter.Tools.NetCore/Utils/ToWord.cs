using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Replacing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BT.Manage.Tools.Utils
{
    public class ToWord
    {
        private Document doc;

        public string SetDocument(string localFile)
        {
            try
            {
                doc = new Document(localFile);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Write(string key, string value)
        {
            try
            {
                Regex reg = new Regex(string.Format("&{0}&", key));
                doc.Range.Replace(reg, value, new FindReplaceOptions());
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">图片地址</param>
        /// <returns></returns>
        public string Draw(string key, string value)
        {
            try
            {
                Regex reg = new Regex(string.Format("&{0}&", key));
                doc.Range.Replace(reg, new ReplaceAndInsertImage(value), false);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rImage">自定义替换图片类</param>
        /// <returns></returns>
        public string Draw(string key, ReplaceAndInsertImage rImage)
        {
            try
            {
                Regex reg = new Regex(string.Format("&{0}&", key));
                doc.Range.Replace(reg, rImage, false);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Save(string newFile)
        {
            try
            {
                doc.Save(newFile);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

    public class ReplaceAndInsertImage : IReplacingCallback
    {
        /// <summary>
        /// 需要插入的图片路径
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 图形相对于字符、栏、页边距或页面的水平位置
        /// </summary>
        public RelativeHorizontalPosition horzPos { get; set; }
        /// <summary>
        /// 左距
        /// </summary>
        public double left { get; set; }
        /// <summary>
        /// 图形相对于字符、栏、页边距或页面的垂直位置
        /// </summary>
        public RelativeVerticalPosition vertPos { get; set; }
        /// <summary>
        /// 顶距
        /// </summary>
        public double top { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public double width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public double height { get; set; }
        /// <summary>
        /// 环绕方式
        /// </summary>
        public WrapType wrapType { get; set; }

        public ReplaceAndInsertImage(string url)
        {
            this.url = url;
        }

        public ReplaceAction Replacing(ReplacingArgs e)
        {
            //获取当前节点
            var node = e.MatchNode;
            //获取当前文档
            Document doc = node.Document as Document;
            DocumentBuilder builder = new DocumentBuilder(doc);
            //将光标移动到指定节点
            builder.MoveTo(node);
            //插入图片
            builder.InsertImage(url, horzPos, left, vertPos, top, width, height, wrapType);
            return ReplaceAction.Replace;
        }
    }

}
