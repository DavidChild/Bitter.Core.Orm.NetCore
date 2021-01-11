using System;

namespace BT.Manage.Tools.Utils
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2016/5/26 11:42:39
    ** desc： 计算身份证的性别，年龄,出生年月，并用数线隔开
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    *********************************************************************************/

    public static class IDCardUtils
    {
        #region 校验省份证号，并返回性别年龄 生日

        public static string CheckIdcard(string IdCardNum)
        {
            string strReslt = "";
            string FIDcard = IdCardNum;
            if (FIDcard.Length == 18)
            {
                strReslt = CheckIDCard18(FIDcard);
            }
            else if (FIDcard.Length == 15)
            {
                strReslt = CheckIDCard15(FIDcard);
            }
            else
            {
                return "";
            }

            return strReslt;
        }

        /// </summary>
        public static string CheckIDCard15(string idNumber)
        {
            string Age = "";
            string Sex = "";

            string Result = "True";

            long n = 0;
            if (long.TryParse(idNumber, out n) == false || n < Math.Pow(10, 14))
            {
                Result = "";//数字验证
            }

            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";

            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                Result = "";//省份验证
            }
            string birth = idNumber.Substring(6, 6).Insert(4, "-").Insert(2, "-");

            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                Result = "";//生日验证
            }

            if (Result != "")
            {
                Sex = Convert.ToInt32(idNumber.Substring(14, 1)) % 2 == 1 ? "1" : "0";

                Age = GetAge(Convert.ToDateTime(birth));
                Result += "|" + time.ToShortDateString() + "|" + Age + "|" + Sex;
            }
            return Result;
        }

        /// 18位身份证号码验证 </summary>
        public static string CheckIDCard18(string idNumber)
        {
            string Result = "True";
            string Age = "";
            string Sex = "";
            // string Birthday = "";

            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                Result = ""; //数字验证
            }

            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";

            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                Result = ""; //省份验证
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");

            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                Result = "";//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');

            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                Result = ""; //校验码验证
            }
            if (Result != "")
            {
                Sex = Convert.ToInt32(idNumber.Substring(16, 1)) % 2 == 1 ? "1" : "0";

                Age = GetAge(Convert.ToDateTime(birth));
                Result += "|" + time.ToShortDateString() + "|" + Age + "|" + Sex;
            }
            return Result;
        }

        /// <summary>
        /// 返回性别，年龄，生日
        /// </summary>
        /// <param name="IdCarNum"></param>
        /// <returns></returns>
        public static string CheckIdCardNew(string IdCarNum)
        {
            string result = "";
            IdCarNum = IdCarNum.Trim();
            result = CheckIdcard(IdCarNum);
            if (string.IsNullOrEmpty(result))
            {
                result = CheckIdcardAgain(IdCarNum);
            }
            if (string.IsNullOrEmpty(result))
            {
                result = "|||";
            }
            return result;
        }

        /// <summary> <summary> 15位身份证号码验证
        public static string GetAge(DateTime birth)
        {
            int age = 0;
            DateTime Now = System.DateTime.Today;
            if (birth > Now)
            {
                return "";
            }
            else
            {
                age = Now.Year - birth.Year;
                if (birth > Now.AddYears(-age))
                {
                    age--;
                }
            }

            return age.ToString();
        }

        #endregion 校验省份证号，并返回性别年龄 生日

        #region 第二次身份证号截取方法

        /// <summary>
        /// 15位身份证号码截取
        /// </summary>
        public static string CheckIDCard15Again(string idNumber)
        {
            string Age = "";
            string Sex = "";
            string Result = "True";

            string birth = idNumber.Substring(6, 6).Insert(4, "-").Insert(2, "-");

            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                Result = "";//生日验证
            }

            if (Result != "")
            {
                Sex = Convert.ToInt32(idNumber.Substring(14, 1)) % 2 == 1 ? "1" : "0";

                Age = GetAgeAgain(Convert.ToDateTime(birth));
                Result += "|" + time.ToShortDateString() + "|" + Age + "|" + Sex;
            }
            return Result;
        }

        /// <summary>
        /// 18位身份证号码截取
        /// </summary>
        public static string CheckIDCard18Again(string idNumber)
        {
            string Result = "True";
            string Age = "";
            string Sex = "";

            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");

            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                Result = "";//生日验证
            }

            if (Result != "")
            {
                Sex = Convert.ToInt32(idNumber.Substring(16, 1)) % 2 == 1 ? "1" : "0";

                Age = GetAge(Convert.ToDateTime(birth));
                Result += "|" + time.ToShortDateString() + "|" + Age + "|" + Sex;
            }
            return Result;
        }

        //第二次 校验省份证号，并返回性别年龄 生日
        public static string CheckIdcardAgain(string IdCardNum)
        {
            string strReslt = "";
            string FIDcard = IdCardNum;
            if (FIDcard.Length == 18)
            {
                strReslt = CheckIDCard18Again(FIDcard);
            }
            else if (FIDcard.Length == 15)
            {
                strReslt = CheckIDCard15Again(FIDcard);
            }
            else
            {
                return "";
            }
            return strReslt;
        }

        public static string GetAgeAgain(DateTime birth)
        {
            int age = 0;
            DateTime Now = System.DateTime.Today;
            if (birth > Now)
            {
                return "";
            }
            else
            {
                age = Now.Year - birth.Year;
                if (birth > Now.AddYears(-age))
                {
                    age--;
                }
            }
            return age.ToString();
        }

        #endregion 第二次身份证号截取方法
    }
}