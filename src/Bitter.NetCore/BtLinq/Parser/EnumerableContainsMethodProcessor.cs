using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace BT.Manage.Core
{
    
    

    public class EnumerableContainsMethodProcessor : BaseTypeSwitcher
    {
        private string _converter;
        private IEnumerable _set;

        public EnumerableContainsMethodProcessor(Type type, IEnumerable collectionObject, string converter) : base(type)
        {
            this._set = collectionObject;
            this._converter = converter;
        }

        private void FillConverter<T>(IEnumerable<T> list)
        {
            if (list.Count<T>() <= 0)
            {
                base.Result = string.Empty;
            }
            else
            {
                this._converter = string.Format(this._converter, "{0} IN (" + string.Join<T>(",", list) + ")");
                base.Result = this._converter;
            }
        }

        protected override void ProcessBoolean()
        {
            this.ProcessType<bool>();
        }

        protected override void ProcessBooleanNullable()
        {
            this.ProcessNullableType<bool>();
        }

        protected override void ProcessByte()
        {
            this.ProcessType<byte>();
        }

        protected override void ProcessByteNullable()
        {
            List<byte> list = new List<byte>();
            foreach (byte? nullable in this._set)
            {
                if (nullable.HasValue)
                {
                    list.Add(nullable.Value);
                }
            }
            this.FillConverter<byte>(list);
        }

        protected override void ProcessDateTime()
        {
            this.ProcessType<DateTime>();
        }

        protected override void ProcessDateTimeNullable()
        {
            this.ProcessNullableType<DateTime>();
        }

        protected override void ProcessDecimal()
        {
            this.ProcessType<decimal>();
        }

        protected override void ProcessDecimalNullable()
        {
            this.ProcessNullableType<decimal>();
        }

        protected override void ProcessDouble()
        {
            this.ProcessType<double>();
        }

        protected override void ProcessDoubleNullable()
        {
            List<double> list = new List<double>();
            foreach (double? nullable in this._set)
            {
                if (nullable.HasValue)
                {
                    list.Add(nullable.Value);
                }
            }
            this.FillConverter<double>(list);
        }

        protected override void ProcessEnumNullable()
        {
            base.Type = Enum.GetUnderlyingType(base.Type);
            base.IsNullable = false;
            ArrayList list = new ArrayList();
            foreach (object obj2 in this._set)
            {
                if (obj2 != null)
                {
                    list.Add(obj2);
                }
            }
            this._set = list;
            base.SwitchBaseType();
        }

        protected override void ProcessFloat()
        {
            this.ProcessType<float>();
        }

        protected override void ProcessFloatNullable()
        {
            this.ProcessNullableType<float>();
        }

        protected override void ProcessInt16()
        {
            this.ProcessType<short>();
        }

        protected override void ProcessInt16Nullable()
        {
            List<short> list = new List<short>();
            foreach (short? nullable in this._set)
            {
                if (nullable.HasValue)
                {
                    list.Add(nullable.Value);
                }
            }
            this.FillConverter<short>(list);
        }

        protected override void ProcessInt32()
        {
            this.ProcessType<int>();
        }

        protected override void ProcessInt32Nullable()
        {
            List<int> list = new List<int>();
            foreach (int? nullable in this._set)
            {
                if (nullable.HasValue)
                {
                    list.Add(nullable.Value);
                }
            }
            this.FillConverter<int>(list);
        }

        protected override void ProcessInt64()
        {
            this.ProcessType<long>();
        }

        protected override void ProcessInt64Nullable()
        {
            List<long> list = new List<long>();
            foreach (long? nullable in this._set)
            {
                if (nullable.HasValue)
                {
                    list.Add(nullable.Value);
                }
            }
            this.FillConverter<long>(list);
        }

        private void ProcessNullableType<T>() where T: struct
        {
            List<T> list = new List<T>();
            foreach (T? nullable in this._set)
            {
                if (nullable.HasValue)
                {
                    list.Add(nullable.Value);
                }
            }
            this.FillConverter<T>(list);
        }

        protected override void ProcessString()
        {
            this.ProcessType<string>();
        }

        private void ProcessType<T>()
        {
            List<T> list = new List<T>();
            foreach (object obj2 in this._set)
            {
                list.Add((T) obj2);
            }
            this.FillConverter<T>(list);
        }
    }
}

