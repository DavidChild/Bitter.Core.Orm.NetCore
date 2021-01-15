using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bitter.Tools.Helper
{
    /// <summary>
    /// HashTable帮助类
    /// </summary>
    public class HashTableHelper
    {
        /// <summary>
        /// 实体类Model转Hashtable(反射)
        /// </summary>
        public static Hashtable GetModelToHashtable<T>(T model)
        {
            Hashtable ht = new Hashtable();
            PropertyInfo[] properties = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo item in properties)
            {
                string key = item.Name;
                ht[key] = item.GetValue(model, null);
            }
            return ht;
        }

        /// <summary>
        /// hashtable转XML
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public static string HashtableToXml(Hashtable ht)
        {
            StringBuilder xml = new StringBuilder("<root>");
            xml.Append(HashtableToNode(ht));
            xml.Append("</root>");
            return xml.ToString();
        }

        /// <summary>
        /// hashtable集合转XML
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static string IListToXML(IList<Hashtable> datas)
        {
            StringBuilder xml = new StringBuilder("<root>");
            foreach (Hashtable ht in datas)
            {
                xml.Append(HashtableToNode(ht));
            }
            xml.Append("</root>");
            return xml.ToString();
        }

        /// <summary>
        /// 自定义格式字符串转换 Hashtable
        /// </summary>
        /// <param name="item">自定义字符串</param>
        /// <returns></returns>
        public static Hashtable List_Key_ValueToHashtable(string item)
        {
            Hashtable ht = new Hashtable();
            foreach (string itemwithin in item.Split('☺'))
            {
                if (itemwithin.Length > 0)
                {
                    string[] str_item = itemwithin.Split('☻');
                    ht[str_item[0]] = str_item[1];
                }
            }
            return ht;
        }

        /// <summary>
        /// 自定义格式字符串转换 Hashtable
        /// </summary>
        /// <param name="array_Key_Value"></param>
        /// <returns></returns>
        public static Hashtable Master_Key_ValueToHashtable(object[] array_Key_Value)
        {
            Hashtable ht = new Hashtable();
            foreach (string item in array_Key_Value)
            {
                if (item.Length > 0)
                {
                    string[] Key_Value = item.Split('☻');
                    ht[Key_Value[0]] = Key_Value[1];
                }
            }
            return ht;
        }

        /// <summary>
        /// 字符串 分割转换 Hashtable ≌; ☻
        /// </summary>
        public static Hashtable String_Key_ValueToHashtable(string str)
        {
            Hashtable ht = new Hashtable();
            if (!string.IsNullOrEmpty(str))
            {
                string[] arrayParm_Key_Value = str.Split('≌');
                foreach (string item in arrayParm_Key_Value)
                {
                    if (item.Length > 0)
                    {
                        string[] Key_Value = item.Split('☻');
                        ht[Key_Value[0]] = Key_Value[1];
                    }
                }
            }
            return ht;
        }

        private static string HashtableToNode(Hashtable ht)
        {
            StringBuilder xml = new StringBuilder("");
            foreach (string key in ht.Keys)
            {
                object value = ht[key];
                xml.Append("<").Append(key).Append(">").Append(value).Append("</").Append(key).Append(">");
            }
            xml.Append("");
            return xml.ToString();
        }
    }
}