namespace Bitter.Core
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2016/6/8 13:36:11
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public static class ExFunc
    {
        public static bool In<T>(this T obj, T[] array)
        {
            return true;
        }

        public static bool Like(this string str, string likeStr)
        {
            return true;
        }

        public static bool NotIn<T>(this T obj, T[] array)
        {
            return true;
        }

        public static bool NotLike(this string str, string likeStr)
        {
            return true;
        }
        public static bool IsNull(this string str, string isnulltostr)
        {
            return true;
        }
        public static bool IsNull(this int intobj, int isnulltoint)
        {
            return true;
        }
        public static bool IsNull(this int? intobj, int isnulltoint)
        {
            return true;
        }
     
    }
}