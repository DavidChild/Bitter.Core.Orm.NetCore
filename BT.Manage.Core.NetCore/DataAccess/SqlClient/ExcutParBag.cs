using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BT.Manage.Tools.Utils;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace BT.Manage.Core
{
    public class ExcutParBag
    {
        //操作类型
        internal ExcutEnum excutEnum { get; set; }




        private string _tableName { get; set; }

        private Type _type { get; set; }

        private string _keyName { get; set; }

        private bool? _isIdentityModel { get; set; }

        internal BaseModel data { get; set; }

        List<FiledProperty> _properties { get; set; }


        List<string> _identityFileds { get; set; }





        /// <summary>
        /// 
        /// </summary>
        public Type modelType
        {
            get
            {
                if (data == null && _type == null)
                {
                    return null;
                }
                if (data != null)
                {
                    _type = data.GetType();
                    return _type;
                }
                return _type;

            }
        }


        public string tableName
        {
            get
            {
                if (modelType != null)
                {
                    if (string.IsNullOrEmpty(_tableName))
                    {
                        _tableName = Utils.GetTableName(modelType);
                    }
                }
                return _tableName.ToSafeString("");

            }

        }
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName"></param>
        public void SetTableName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new Exception("table name 不能为空");
            _tableName = tableName;
        }
        public string keyName
        {
            get
            {
                if (modelType != null)
                {
                    if (string.IsNullOrEmpty(_keyName))
                    {
                        _keyName = Utils.GetKeyFiledName(modelType);
                    }
                }
                return _keyName.ToSafeString("");

            }

        }

        public bool isIdentityModel
        {
            get
            {
                if (modelType != null)
                {
                    if (!_isIdentityModel.HasValue)
                    {
                        _isIdentityModel = Utils.CheckedIsIdentityModel(modelType);
                    }
                }
                return _isIdentityModel.ToSafeBool(false);

            }

        }

        public List<FiledProperty> PropertyFileds
        {
            get
            {
                if (modelType != null)
                {

                    if (_properties == null)
                    {
                        PropertyInfo[] tmp = modelType.GetProperties();
                        List<FiledProperty> lis = new List<FiledProperty>();
                        if (tmp != null && tmp.Length > 0)
                        {
                            var t = (from x in tmp where x.Name.IndexOf('_') != 0 select x);
                            foreach (PropertyInfo pi in t)
                            {
                                FiledProperty pro = new FiledProperty();
                                pro.type = pi.PropertyType;
                                if (pro.type.IsGenericType && pro.type.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    pro.typeName = pro.type.GetGenericArguments()[0].Name;
                                    pro.isNull = true;
                                }
                                else
                                {
                                    pro.typeName = pro.type.Name;
                                }
                                pro.filedName = pi.Name;
                                //if the data is not null ,then pro.value={}//
                                if (data != null)
                                {
                                    pro.value = pi.GetValue(data, null);
                                }

                                object[] attrs = pi.GetCustomAttributes(true);
                                for (int i = 0; i < attrs.Count(); i++)
                                {
                                    string memberInfoName = attrs.GetValue(i).GetType().Name;
                                    if (memberInfoName == "DisplayAttribute")
                                    {
                                        DisplayAttribute displayAttr = attrs[i] as DisplayAttribute;
                                        if (displayAttr != null)
                                        {
                                            pro.displayName = displayAttr.Name;
                                        }
                                    }
                                    else if (memberInfoName == "KeyAttribute")
                                    {
                                        KeyAttribute keyAttr = attrs[i] as KeyAttribute;
                                        if (keyAttr != null)
                                        {
                                            pro.isKey = true;
                                        }
                                    }
                                    else if (memberInfoName == "IdentityAttribute")
                                    {
                                        BT.Manage.Tools.Attributes.IdentityAttribute keyAttr = attrs[i] as BT.Manage.Tools.Attributes.IdentityAttribute;
                                        if (keyAttr != null)
                                        {
                                            pro.isIdentity = true;

                                        }
                                    }
                                }
                                lis.Add(pro);
                            }
                        }
                        _properties = lis;

                    }

                }
                return _properties;

            }

        }



        public void SetType(Type type)
        {
            this._type = type;
        }
        public void SetData(BaseModel data)
        {
            this.data = data;
        }

    }
}
