using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Security;

namespace BT.Manage.Tools.Utils
{
    public class EncryptUtils
    {
        private static string Key = "Guz(%&hj7x89H$yuBI0456FtmaT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP87jH7A";

        private static SymmetricAlgorithm mobjCryptoService = new RijndaelManaged();

        /// <summary>
        /// Big5编码转中文
        /// </summary>
        /// <param name="Str">编码字符</param>
        /// <returns></returns>
        public static string DecodeBig5(string Str)
        {
            MatchCollection matchs = new Regex(@"\\u(?<1>[a-zA-Z0-9]{2})(?<2>[a-zA-Z0-9]{2})").Matches(Str);
            string str = string.Empty;
            foreach (Match match in matchs)
            {
                byte num = byte.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
                byte num2 = byte.Parse(match.Groups[2].Value, NumberStyles.HexNumber);
                str = str + Encoding.BigEndianUnicode.GetString(new byte[] { num, num2 });
            }
            return str;
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="Source">待解密的串</param>
        /// <returns>经过解密的串</returns>
        public static string DecodeCookies(string Source)
        {
            byte[] bytIn = Convert.FromBase64String(Source);
            MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// 参数解密函数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecodeParam(string str)
        {
            return System.Text.Encoding.Default.GetString(Convert.FromBase64String(str.Replace("%2B", "+")));
        }

        /// <summary>
        /// 站群解密函数
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string DecodeSource(string Str)
        {
            if (string.IsNullOrEmpty(Str))
            {
                return "";
            }
            int li_ret;
            int li_len = Str.Length;
            string ls_code = "";
            li_ret = Convert.ToInt32(Str.ToCharArray(0, 1)[0]) % 10;
            for (int i = 2; i < li_len; i = i + 2)
            {
                int li_asc = Convert.ToInt32(Str.ToCharArray(i, 1)[0]);
                string ls_i = "";
                if (Convert.ToInt32(Str.ToCharArray(i - 1, 1)[0]) % 2 == 0)
                {
                    ls_i = ((char)(li_asc + (i - 1) / 2 + li_ret)).ToString();
                }
                else
                {
                    ls_i = ((char)(li_asc - (i - 1) / 2 - li_ret)).ToString();
                }
                ls_code = ls_code + ls_i;
            }
            return ls_code;
        }

        /// <summary>
        /// 解密函数
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
     /*   public static string DESDecrypt(string strText)
        {
            return DESDecrypt(strText, "86668455");
        }
        **/
        /// <summary>
        /// 解密函数
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        /*  public static string DESDecrypt(string strText, string strKey)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            int num = strText.Length / 2;
            byte[] buffer = new byte[num];
            for (int i = 0; i < num; i++)
            {
                int num3 = Convert.ToInt32(strText.Substring(i * 2, 2), 0x10);
                buffer[i] = (byte)num3;
            }
            provider.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(strKey, "md5").Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(strKey, "md5").Substring(0, 8));
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            return Encoding.Default.GetString(stream.ToArray());
        }
        **/
        /// <summary>
        /// 加密函数
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
     /*    public static string DESEncrypt(string strText)
        {
            return DESEncrypt(strText, "86668455");
        }
        **/
        /// <summary>
        /// 加密函数
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
      /*  public static string DESEncrypt(string strText, string strKey)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(strText);
            provider.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(strKey, "md5").Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(strKey, "md5").Substring(0, 8));
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            StringBuilder builder = new StringBuilder();
            foreach (byte num in stream.ToArray())
            {
                builder.AppendFormat("{0:X2}", num);
            }
            return builder.ToString();
        }
        **/

        /// <summary>
        /// 中文转Big5编码
        /// </summary>
        /// <param name="Str">中文字符</param>
        /// <returns></returns>
        public static string EncodeBig5(string Str)
        {
            string str = string.Empty;
            for (int i = 0; i < Str.Length; i++)
            {
                string s = Str.Substring(i, 1);
                byte[] bytes = Encoding.BigEndianUnicode.GetBytes(s);
                s = @"\u";
                foreach (byte num2 in bytes)
                {
                    string str3 = num2.ToString("x");
                    if (str3.Length == 1)
                    {
                        str3 = "0" + str3;
                    }
                    s = s + str3;
                }
                str = str + s;
            }
            return str;
        }

        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="Source">待加密的串</param>
        /// <returns>经过加密的串</returns>
        public static string EncodeCookies(string Source)
        {
            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
            MemoryStream ms = new MemoryStream();
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            ms.Close();
            byte[] bytOut = ms.ToArray();
            return Convert.ToBase64String(bytOut);
        }

        /// <summary>
        /// 参数加密函数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeParam(string str)
        {
            return Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(str)).Replace("+", "%2B");
        }

        /// <summary>
        /// 站群加密函数
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string EncodeSource(string Str)
        {
            //int li_len, i, li_asc, li_rand, li_head;
            //string ls_i, ls_code = "";
            if (string.IsNullOrEmpty(Str))
            {
                return "";
            }
            int li_len = Str.Length;
            Random rdm1 = new Random(~unchecked((int)DateTime.Now.Ticks));
            int li_head = (int)(rdm1.NextDouble() * 10);
            if (li_head == 0)
            {
                li_head = 1;
            }
            string ls_code = "";
            for (int i = 0; i < li_len; i++)
            {
                Random rdm2 = new Random(~unchecked((int)DateTime.Now.Ticks));
                int rand2 = (int)(rdm2.NextDouble() * 94);
                if (rand2 == 0)
                    rand2 = 1;
                int li_rand = rand2 + 32;
                int li_asc = Convert.ToInt32(Str.ToCharArray(i, 1)[0]);
                string ls_i = ((char)(li_asc - i)).ToString();

                if (li_asc + i + li_head > 126)
                {
                    if (li_rand % 2 == 1)
                    {
                        li_rand = li_rand + 1;
                    }
                    ls_i = ((char)(li_rand)).ToString() + ((char)(li_asc - i - li_head)).ToString();
                }
                else
                {
                    if (li_rand % 2 == 0)
                        li_rand = li_rand + 1;
                    ls_i = ((char)(li_rand)).ToString() + ((char)(li_asc + i + li_head)).ToString();
                }
                ls_code = ls_code + ls_i;
            }
            Random rdm3 = new Random(~unchecked((int)DateTime.Now.Ticks));
            int rand1 = (int)(rdm3.NextDouble() * 9);
            if (rand1 == 0)
            {
                rand1 = 1;
            }
            ls_code = ((char)(rand1 * 10 + li_head + 40)).ToString() + ls_code;
            return ls_code;
        }

        /// <summary>
        /// 返回32位MD5值
        /// </summary>
        /// <param name="Input"></param>
      /*    /// <returns></returns>
        public static string MD5(string Str)
        {
            return MD5(Str, false);
        }
        **/
        /// <summary>
        /// 返回16位MD5值
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Half"></param>
     /*   /// <returns></returns>
        public static string MD5(string Str, bool Half)
        {
            string output = System.Security.Authentication.ExtendedProtection.H(Str, "MD5").ToUpper();
            if (Half)
            {
                output = output.Substring(8, 0x10);
            }
            return output;
        }
     **/
        /// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private static byte[] GetLegalIV()
        {
            string sTemp = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        /// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private static byte[] GetLegalKey()
        {
            string sTemp = Key;
            mobjCryptoService.GenerateKey();
            byte[] bytTemp = mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }



        /// <summary>
        /// 将字符串转为32位的MD5编码
        /// </summary>
        /// <param name="str">输入的字符串</param>
        /// <returns>32位编码的字符串</returns>
        public static string To32Md5(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            StringBuilder sb = new StringBuilder();

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            md5.Clear();
            for (int i = 0; i < s.Length; i++)
            {
                sb.Append(s[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}