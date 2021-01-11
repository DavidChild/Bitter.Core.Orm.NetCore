using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using BT.Manage.Tools.Utils;
namespace BT.Manage.Core
{
    public sealed class SqlQueryParameterCollection : MarshalByRefObject
    {
        private int m_IntitialCapacity = 10;
        private ArrayList m_Items;

        
        
        public SqlQueryParameterCollection()
        {
        }

        public SqlQueryParameterCollection(int initCapacity)
        {
            this.m_IntitialCapacity = initCapacity;
        }

        public int Count
        {
            get
            {
                if (this.m_Items == null)
                {
                    return 0;
                }
                return this.m_Items.Count;
            }
        }

        public ArrayList Items
        {
            get
            {
                if (this.m_Items == null)
                {
                    this.m_Items = new ArrayList(this.m_IntitialCapacity);
                }
                return this.m_Items;
            }
        }

        public SqlParameter this[int index]
        {
            get
            {
                this.RangeCheck(index);
                return (SqlParameter)this.m_Items[index];
            }
            set
            {
                this.RangeCheck(index);
                this.m_Items[index] = value;
            }
        }

        public SqlParameter this[string parameterName]
        {
            get
            {
                int index = this.RangeCheck(parameterName);
                return (SqlParameter)this.m_Items[index];
            }
            set
            {
                int index = this.RangeCheck(parameterName);
                this.m_Items[index] = value;
            }
        }

        public SqlParameter Add(SqlParameter param)
        {
            if (param.Value == null)
                param.Value = DBNull.Value;
            this.Items.Add(param);
            return param;
        }

        public SqlParameter Add(string parameterName, object val,FiledProperty property)
        {


            if (property != null)
            {
                var d = ((val == null && property.isNull) || (property.typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(val == null ? string.Empty : val.ToString()))) ? DBNull.Value : val;
                SqlDbType dbType = Utils.SqlTypeString2SqlType(property.typeName.ToLower());
                var p = new SqlParameter(parameterName, d);
                p.SqlDbType = dbType;
                return this.Add(p);
            }
            else
            {
               // var d = ((val == null && property.isNull) || (property.typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(val == null ? string.Empty : val.ToString()))) ? DBNull.Value : val;
                 SqlDbType dbType = Utils.SqlTypeString2SqlType(val.GetType().Name.ToLower());
                var p = new SqlParameter(parameterName,val.ToSafeString());
                p.SqlDbType = dbType;
                return this.Add(p);
            }
           
           
        }

        public SqlParameter Add(string parameterName, object value, SqlDbType dbType)
        {
            
            var p = new SqlParameter(parameterName, value);
            p.SqlDbType = dbType;
            return this.Add(p);
           
        }

        public SqlParameter Add(string parameterName, object value, SqlDbType dbType, int size)
        {
            return this.Add(new SqlParameter(parameterName, value)
            {
                SqlDbType = dbType,
                Size = size
            });
        }

        public SqlParameter Add(string parameterName, object value, SqlDbType dbType, int size, ParameterDirection direction)
        {
            return this.Add(new SqlParameter(parameterName, value)
            {
                SqlDbType = dbType,
                Size = size,
                Direction = direction
            });
        }

        public void Clear()
        {
            this.Items.Clear();
        }
        public void Remove(string parameterName)
        {
            var index = this.IndexOf(parameterName);
            if(index>=0)
                this.Items.RemoveAt(index);
        }
        public int IndexOf(string parameterName)
        {
            int result = -1;
            if (this.m_Items != null)
            {
                for (int i = 0; i < this.m_Items.Count; i++)
                {
                    if (((SqlParameter)this.m_Items[i]).ParameterName.Equals(parameterName))
                    {
                        result = i;
                        break;
                    }
                }
            }
            return result;
        }

        private void RangeCheck(int index)
        {
            if (index < 0 || this.Count <= index)
            {
                throw new IndexOutOfRangeException("Number " + index.ToString() + " is out of Range");
            }
        }

        private int RangeCheck(string parameterName)
        {
            int num = this.IndexOf(parameterName);
            if (num < 0)
            {
                throw new IndexOutOfRangeException("ParameterName " + parameterName + " dose not exist");
            }
            return num;
        }

        private void Validate(int index, SqlParameter Value)
        {
        }

        private void ValidateType(object Value)
        {
        }
    }
}