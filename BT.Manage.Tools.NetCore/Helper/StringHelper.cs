using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BT.Manage.Tools.Helper
{
    public class StringHelper
    {
        #region 解密银行卡

        ///<summary><![CDATA[字符串DES解密函数]]></summary>
        ///<param name="str"><![CDATA[被解密字符串 ]]></param>
        ///<param name="key"><![CDATA[密钥 ]]></param>
        ///<returns><![CDATA[解密后字符串]]></returns>
        public static string Decode(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    string key = "f2d77a2d8f0622c173ccebe3494ec634";
                    DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                    provider.Key = Encoding.ASCII.GetBytes(key.Substring(0, 8));
                    provider.IV = Encoding.ASCII.GetBytes(key.Substring(0, 8));
                    byte[] buffer = new byte[str.Length / 2];
                    for (int i = 0; i < (str.Length / 2); i++)
                    {
                        int num2 = Convert.ToInt32(str.Substring(i * 2, 2), 0x10);
                        buffer[i] = (byte)num2;
                    }
                    MemoryStream stream = new MemoryStream();
                    CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
                    stream2.Write(buffer, 0, buffer.Length);
                    stream2.FlushFinalBlock();
                    stream.Close();
                    //return Encoding.GetEncoding("GB2312").GetString(stream.ToArray());
                    return DecodeBase64(Encoding.UTF8.GetString(stream.ToArray()));
                }
                catch (Exception) { return ""; }
            }
            return "";
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string DecodeBase64(string result)
        {
            byte[] bpath = Convert.FromBase64String(result);
            return System.Text.ASCIIEncoding.UTF8.GetString(bpath);
        }

        #endregion 解密银行卡

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="obj">字符串</param>
        /// <param name="num">截取的长度</param>
        /// <returns></returns>
        public static string CutString(Object obj, int num)
        {
            if (obj == null)
                return "";
            if (obj.ToString().Length == 0)
                return "";
            if (obj.ToString().Length > num)
                return obj.ToString().Substring(0, num) + "...";
            else
                return obj.ToString();
        }

        /// <summary>
        /// 把从网页中直接拷贝来的内容中的特殊字符都换成“”。
        /// </summary>
        /// <param name="str">需要换的文本</param>
        /// <returns>换好的文本</returns>
        public static string DealHtml(string str)
        {
            str = Regex.Replace(str, @"\<(img)[^>]*>|<\/(img)>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<(table|tbody|tr|td|th|)[^>]*>|<\/(table|tbody|tr|td|th|)>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<(div|blockquote|fieldset|legend)[^>]*>|<\/(div|blockquote|fieldset|legend)>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<(font|i|u|h[1-9]|s)[^>]*>|<\/(font|i|u|h[1-9]|s)>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<(style|strong)[^>]*>|<\/(style|strong)>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<a[^>]*>|<\/a>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<(meta|iframe|frame|span|tbody|layer)[^>]*>|<\/(iframe|frame|meta|span|tbody|layer)>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<br[^>]*", "", RegexOptions.IgnoreCase);
            return str;
        }

        /// <summary>
        /// 获取中文字符串的首字母
        /// </summary>
        /// <param name="strText">中文字符串</param>
        /// <returns></returns>
        public static string GetChineseSpell(string strText)
        {
            int len = strText.Length;
            string myStr = "";
            for (int i = 0; i < len; i++)
            {
                if (IsChina(strText.Substring(i, 1)))
                    myStr += getSpell(strText.Substring(i, 1));
            }
            return myStr;
        }

        /// <summary>
        /// 将字符串转换成decimal型，若字符串为空，返回0
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转后的decimal值</returns>
        public static decimal GetDecimal(object o)
        {
            try
            {
                if (o == null)
                    return 0;
                if ((o.ToString()).Length == 0)
                    return 0;
                else
                {
                    if (decimal.Parse(o.ToString()) == 0)
                        return 0;
                    else
                        return decimal.Parse(o.ToString());
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MenuFParameters"></param>
        /// <returns></returns>
        public static string GetFstatus(string MenuFParameters)
        {
            string Result = "";

            if (!string.IsNullOrWhiteSpace(MenuFParameters))
            {
                //先按等号分隔字符串
                string[] Parameters = MenuFParameters.Split('=');

                if (Parameters.Length > 0)
                {
                    for (int i = 0; i < Parameters.Length; i++)
                    {
                        //获取第一个包含FStatus的参数
                        if (Parameters[i].Contains("FStatus"))
                        {
                            //判断Fstatus 是否存在值 去出发FStatus的值
                            try
                            {
                                string value = Parameters[i + 1];
                                if (value.IndexOf(" ") > -1)
                                {
                                    value = value.Substring(0, value.IndexOf(" "));
                                }
                                Result = value;
                            }
                            catch
                            {
                            }
                            i = Parameters.Length;
                        }
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// 抓取内容中的图片HTTP地址，返回图片地址列表：以“,”号分隔
        /// </summary>
        /// <param name="StrContxt">内容</param>
        /// <returns></returns>
        public static string GetHtmlPicList(string StrContxt)
        {
            string StrPic = "";
            ArrayList list = new ArrayList();
            string reg = @"http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            Regex regex = new Regex(reg, RegexOptions.IgnoreCase);
            MatchCollection mc = regex.Matches(StrContxt);
            for (int i = 0; i < mc.Count; i++)
            {
                bool hasExist = false;
                String name = mc[i].ToString();
                foreach (String one in list)
                {
                    if (name == one)
                    {
                        hasExist = true;
                        break;
                    }
                }
                if (!hasExist)
                {
                    if (name.ToLower().IndexOf(".jpg") != -1 || name.ToLower().IndexOf(".jpeg") != -1 || name.ToLower().IndexOf(".gif") != -1 || name.ToLower().IndexOf(".png") != -1 || name.ToLower().IndexOf(".bmp") != -1)
                        StrPic += name + ",";
                }
            }
            return StrPic;
        }

        /// <summary>
        /// 普通抓取：UTF-8
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetHtmlStr(string path)
        {
            System.Text.Encoding.GetEncoding("utf-8");
            System.Net.WebRequest wReq = System.Net.WebRequest.Create(path);
            System.Net.WebResponse wResp = wReq.GetResponse();
            System.IO.Stream respStream = wResp.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(respStream, System.Text.Encoding.GetEncoding("utf-8"));
            string strTemp = reader.ReadToEnd();
            reader.Close();
            return strTemp;
        }

        /// <summary>
        /// 普通抓取：GB2312
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetHtmlStr_gb2312(string path)
        {
            System.Net.WebRequest wReq = System.Net.WebRequest.Create(path);
            System.Net.WebResponse wResp = wReq.GetResponse();
            System.IO.Stream respStream = wResp.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(respStream, System.Text.Encoding.GetEncoding("gb2312"));
            string strTemp = reader.ReadToEnd();
            reader.Close();
            return strTemp;
        }

        /// <summary>
        /// Gzip压缩抓取：UTF-8
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetHttpHtmlStrGzip(string path)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(path);
            httpRequest.Method = "GET";
            httpRequest.Credentials = CredentialCache.DefaultCredentials;
            string content = "";
            HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        content = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                content = reader.ReadToEnd();
                reader.Close();
            }
            return content;
        }

        /// <summary>
        /// 将字符串转换成int型，若字符串为空，返回0
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转后的int值</returns>
        public static int GetIntValue(object o)
        {
            try
            {
                if (o == null)
                    return 0;
                if ((o.ToString()).Length == 0)
                    return 0;
                else
                {
                    if (Int32.Parse(o.ToString()) == 0)
                        return 0;
                    else
                        return Int32.Parse(o.ToString());
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5Hash(String input)
        {
            string cl = input;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));

            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                pwd = pwd + s[i].ToString("x");
            }
            return pwd;
        }

        /// <summary>
        /// 是null就返回空，否则返回原字符串
        /// </summary>
        /// <param name="str">需要处理的字符串</param>
        /// <returns>转换后的值</returns>
        public static string GetNullToString(object o)
        {
            try
            {
                if (o == null)
                    return "";
                else
                    return o.ToString();
            }
            catch
            {
                return "";
            }
        }

       

        /// <summary>
        /// 根据特殊字符分割错误信息
        /// </summary>
        /// <param name="msg">错误信息</param>
        /// <returns></returns>
        public static string[] GetSplitException(string msg)
        {

            if (msg.IndexOf('★') < 0)
            {
                msg = msg + "★" + "";
            }
            string[] lst = msg.Split('★');
            return lst;
        }

        /// <summary>
        /// 检测字符是否为中文
        /// </summary>
        /// <param name="CString">单个字符</param>
        /// <returns></returns>
        public static bool IsChina(string CString)
        {
            Regex rx = new Regex("^[\u4e00-\u9fa5]$");
            if (rx.IsMatch(CString))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 检测字符是否包含中文
        /// </summary>
        /// <param name="CString">字符串</param>
        /// <returns></returns>
        public static bool IsChinaStr(string CString)
        {
            bool IsC = false;
            for (int i = 0; i < CString.Length; i++)
            {
                Regex rx = new Regex("^[\u4e00-\u9fa5]$");
                if (rx.IsMatch(CString[i].ToString()))
                    IsC = true;
            }
            return IsC;
        }

        /// <summary>
        /// 字段串是否为Null或为""(空)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(string str)
        {
            if (str == null || str.Trim() == string.Empty)
                return false;
            return true;
        }

        /// <summary>
        /// 检查是否为数字
        /// </summary>
        /// <param name="str">待处理字符串</param>
        /// <returns></returns>
        public static bool isnumeric(string str)
        {
            char[] ch = new char[str.Length];
            ch = str.ToCharArray();
            for (int i = 0; i < ch.Length; i++)
            {
                if (ch[i] < 48 || ch[i] > 57)
                    return false;
            }
            return true;
        }

        //6位随机函数
        public static string MakeRandomNumber6()
        {
            string Numchars = "abcdefghijklmnopqrstuvwxyz1234567890";
            int Numlen = 6;
            string tmpstr = "";
            int iRandNum;
            Random rnd = new Random();
            for (int i = 0; i < Numlen; i++)
            {
                iRandNum = rnd.Next(Numchars.Length);
                tmpstr += Numchars[iRandNum];
            }
            return tmpstr;
        }

        //获取n位随机数
        public static string MakeUpRandomNumber(int n)
        {
            string Numchars = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int Numlen = n;
            string tmpstr = "";
            int iRandNum;
            Random rnd = new Random();
            for (int i = 0; i < Numlen; i++)
            {
                iRandNum = rnd.Next(Numchars.Length);
                tmpstr += Numchars[iRandNum];
            }
            return tmpstr;
        }

       

        public static string MD5s(string str)
        {
            byte[] b = Encoding.Default.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }

        /// <summary>
        /// 截取字符串片段
        /// </summary>
        /// <param name="s"></param>
        /// <param name="num">字符数</param>
        /// <param name="num">超出长度的显示字符串类似“…”</param>
        /// <returns></returns>
        public static string PartSubString(string s, int num, string strback)
        {
            if (s.Length > num)
            {
                return s.Substring(0, num) + strback;
            }
            return s;
        }

        //post请求并获取返回结果
        public static string PostString(string postData, string PostUrl)
        {
            byte[] data = Encoding.GetEncoding("GB2312").GetBytes(postData);
            // Prepare web request
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(PostUrl);
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = data.Length;
            Stream newStream = myRequest.GetRequestStream();
            //write the data.
            newStream.Write(data, 0, data.Length);

            //Get response
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("GB2312"));
            string content = reader.ReadToEnd();

            newStream.Close();
            newStream.Dispose();
            reader.Close();
            reader.Dispose();
            return content;
        }

        /// <summary>
        /// 检测是否包含sql关键词
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static bool ProcessSqlStr(string Str)
        {
            Str = Str.ToLower();
            bool ReturnValue = true;
            try
            {
                if (Str != "")
                {
                    string SqlStr = " and |exec |insert |select |delete |update | count | from | * |chr |mid |master |truncate |char |declare |drop |asc |char |or |chr |mid |master |truncate |char |declear |SiteName |net user |xp_cmdshell |add |exec master.dbo.xp_cmdshell |net localgroup administrators  |or%20";

                    string[] anySqlStr = SqlStr.Split('|');
                    foreach (string ss in anySqlStr)
                    {
                        if (Str.ToLower().IndexOf(ss) >= 0)
                        {
                            ReturnValue = false;
                        }
                    }
                }
            }
            catch
            {
                ReturnValue = false;
            }
            return ReturnValue;
        }

        /// <summary>
        /// 清除字符中的所有超链接
        /// </summary>
        /// <param name="content">要处理的字符串</param>
        /// <returns></returns>
        public static string ReplaceHref(string content)
        {
            Regex reg = new Regex(@"(?is)</?a\b.*?href=(['""]?)(?!(?:http://)?www\.abc\.com)[^'""\s>]+\1[^>]*>(?<text>(?:(?!</?a).)*)</a>");
            string result = reg.Replace(content, "${text}");
            return result;
        }

        /// <summary>
        /// 特殊字符替换为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceStr1(string str)//特殊字符替换为空
        {
            str = str.Replace("\"", "\\\"");
            str = str.Replace("<FONT face=宋体 size=2>", "");
            str = str.Replace("<FONT face=宋体>", "");
            str = str.Replace("<P>", "");
            str = str.Replace("</P>", "");
            str = str.Replace("<p>", "");
            str = str.Replace("</p>", "");
            str = str.Replace("</FONT>", "");
            str = str.Replace("&nbsp;", " ");
            str = str.Replace("&nbsp", " ");
            str = str.Replace("<br>", "");
            str = str.Replace("<br/>", "");
            str = str.Replace("<BR>", "");
            str = str.Replace("<", "");
            str = str.Replace("B", "");
            str = str.Replace("R", "");
            str = str.Replace(">", "");
            str = str.Trim();
            return str;
        }

        /// <summary>
        /// 字符窜函数替换（进）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceStrIn(string str)
        {
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("'", "");
            str = str.Replace("\n", "");
            str = str.Replace("\r\n", "");
            str = str.Replace("\r", "");
            str = str.Trim();
            return str;
        }

        /// <summary>
        /// 字符窜函数替换（出）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceStrOut(string str)//字符窜函数替换（出）
        {
            str = str.Replace("&lt;", "<");
            str = str.Replace("&gt;", ">");
            str = str.Replace("''", "'");
            str = str.Replace("&nbsp;", " ");
            str = str.Replace("&nbsp", " ");
            str = str.Replace("<br/>", "\n");
            str = str.Replace("<BR>", "\n");
            str = str.Replace("<br/>", "\r\n");
            str = str.Trim();
            return str;
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                    return new string[] { strContent };

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }

        /// <summary>
        /// 数据库防注入
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool SqlInjection(string str)//
        {
            str = str.ToLower();
            // if(str.IndexOf(" ",0)!=-1) return false;
            if (str.IndexOf("select", 0) != -1)
                return false;
            if (str.IndexOf("insert", 0) != -1)
                return false;
            if (str.IndexOf("delete", 0) != -1)
                return false;
            if (str.IndexOf("from", 0) != -1)
                return false;
            if (str.IndexOf("count", 0) != -1)
                return false;
            if (str.IndexOf("drop", 0) != -1)
                return false;
            if (str.IndexOf("table", 0) != -1)
                return false;
            if (str.IndexOf("update", 0) != -1)
                return false;
            if (str.IndexOf("truncate", 0) != -1)
                return false;
            if (str.IndexOf("asc", 0) != -1)
                return false;
            if (str.IndexOf("mid", 0) != -1)
                return false;
            if (str.IndexOf("char", 0) != -1)
                return false;
            if (str.IndexOf("cmdshell", 0) != -1)
                return false;
            if (str.IndexOf("exec", 0) != -1)
                return false;
            if (str.IndexOf("master", 0) != -1)
                return false;
            if (str.IndexOf("net localgroup administrators", 0) != -1)
                return false;
            if (str.IndexOf("and", 0) != -1)
                return false;
            if (str.IndexOf("net user", 0) != -1)
                return false;
            if (str.IndexOf("or", 0) != -1)
                return false;
            if (str.IndexOf("*", 0) != -1)
                return false;
            if (str.IndexOf("\"", 0) != -1)
                return false;
            if (str.IndexOf("'", 0) != -1)
                return false;
            if (str.IndexOf("[", 0) != -1)
                return false;
            if (str.IndexOf("]", 0) != -1)
                return false;
            if (str.IndexOf("%", 0) != -1)
                return false;
            if (str.IndexOf(":", 0) != -1)
                return false;
            if (str.IndexOf(";", 0) != -1)
                return false;
            if (str.IndexOf("+", 0) != -1)
                return false;
            if (str.IndexOf("{", 0) != -1)
                return false;
            if (str.IndexOf("}", 0) != -1)
                return false;
            if (str.IndexOf("!", 0) != -1)
                return false;
            if (str.IndexOf("~", 0) != -1)
                return false;
            if (str.IndexOf("@", 0) != -1)
                return false;
            if (str.IndexOf("#", 0) != -1)
                return false;
            if (str.IndexOf("$", 0) != -1)
                return false;
            if (str.IndexOf("^", 0) != -1)
                return false;
            if (str.IndexOf("*", 0) != -1)
                return false;
            if (str.IndexOf("(", 0) != -1)
                return false;
            if (str.IndexOf(")", 0) != -1)
                return false;
            if (str.IndexOf("=", 0) != -1)
                return false;
            if (str.IndexOf("|", 0) != -1)
                return false;
            if (str.IndexOf(".", 0) != -1)
                return false;
            if (str.IndexOf("?", 0) != -1)
                return false;
            if (str.IndexOf("/", 0) != -1)
                return false;
            if (str.IndexOf("<", 0) != -1)
                return false;
            if (str.IndexOf(">", 0) != -1)
                return false;
            if (str.IndexOf(",", 0) != -1)
                return false;

            return true;
        }

        /// <summary>
        /// 截取字符串片段
        /// </summary>
        /// <param name="s"></param>
        /// <param name="num">字符数，以英文字母数为标准</param>
        /// <param name="num">超出长度的显示字符串类似“…”</param>
        /// <returns></returns>
        public static string SubString(string s, int num, string strback)
        {
            int startidx = 0;
            int Lengthb = SubStrLengthb(s);
            if (startidx + 1 > Lengthb)
            {
                return "";
            }

            int j = 0;
            int l = 0;
            int strw = 0;//字符的宽度
            bool b = false;
            string rstr = "";
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (j >= startidx)
                {
                    rstr = rstr + c;
                    b = true;
                }
                if (SubStrIsChinese(c))
                {
                    strw = 2;
                }
                else
                {
                    strw = 1;
                }
                j = j + strw;
                if (b)
                {
                    l = l + strw;
                    if ((l + 1) >= num) break;
                }
            }
            return rstr + strback;
        }

        public static bool SubStrIsChinese(char c)
        {
            return (int)c >= 0x4E00 && (int)c <= 0x9FA5;
        }

       

       

        /// <summary>
        /// 把字符串等分成n份
        /// </summary>
        /// <param name="str"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static List<string> WholeChunks(string str, int chunkSize)
        {
            List<string> results = new List<string>();
            for (int i = 0; i < str.Length; i += chunkSize)
                results.Add(str.Substring(i, chunkSize));

            return results;
        }

        /// <summary>
        /// 获取单个中文的首字母
        /// </summary>
        /// <param name="cnChar">中文字符</param>
        /// <returns></returns>
        private static string getSpell(string cnChar)
        {
            byte[] arrCN = Encoding.Default.GetBytes(cnChar);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };

                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25)
                    {
                        max = areacode[i + 1];
                    }
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new byte[] { (byte)(97 + i) });
                    }
                }
                return "*";
            }
            else return cnChar;
        }

        //获得字节长度
        private static int SubStrLengthb(string str)
        {
            return System.Text.Encoding.Default.GetByteCount(str);
        }

        #region 替换网页中的换行和引号

        /**********************************
         * 函数名称:ReplaceEnter
         * 功能说明:替换网页中的换行和引号
         * 参    数:HtmlCode:html源代码
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string Url = @"http://www.baidu.com";
         *          strion HtmlCode = o.GetRemoteHtmlCode(Url);
         *          string s = o.ReplaceEnter(HtmlCode);
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/

        /// <summary>
        /// 替换网页中的换行和引号
        /// </summary>
        /// <param name="HtmlCode">HTML源代码</param>
        /// <returns></returns>
        public static string ReplaceEnter(string HtmlCode)
        {
            string s = "";
            if (HtmlCode == null || HtmlCode == "")
                s = "";
            else
                s = HtmlCode.Replace("\"", "");
            s = s.Replace("\r\n", "");
            return s;
        }

        #endregion 替换网页中的换行和引号
    }
}