 using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
namespace BT.Manage.Core
{
   
   

    public class QueryableContainsMethodProcessor : BaseTypeSwitcher
    {
        private string _converter;
        private IQueryable _list;

        public QueryableContainsMethodProcessor(IQueryable list, Type type, string converter) : base(type)
        {
            this._list = list;
            this._converter = converter;
        }

        private void FillConverter<T>(IEnumerable<T> list)
        {
            if (!list.Any<T>())
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
            IEnumerable<bool> list = (IEnumerable<bool>) this._list;
            this.FillConverter<bool>(list);
        }

        protected override void ProcessBooleanNullable()
        {
            IEnumerable<bool> list = from x in (IEnumerable<bool?>) this._list
                where x.HasValue
                select x.Value;
            this.FillConverter<bool>(list);
        }

        protected override void ProcessByte()
        {
            IEnumerable<byte> list = (IEnumerable<byte>) this._list;
            this.FillConverter<byte>(list);
        }

        protected override void ProcessByteNullable()
        {
            IEnumerable<byte> list = from x in (IEnumerable<byte?>) this._list
                where x.HasValue
                select x.Value;
            this.FillConverter<byte>(list);
        }

        protected override void ProcessDateTime()
        {
            IEnumerable<DateTime> list = (IEnumerable<DateTime>) this._list;
            this.FillConverter<DateTime>(list);
        }

        protected override void ProcessDateTimeNullable()
        {
            IEnumerable<DateTime> list = from x in (IEnumerable<DateTime?>) this._list
                where x.HasValue
                select x.Value;
            this.FillConverter<DateTime>(list);
        }

        protected override void ProcessDecimal()
        {
            IEnumerable<decimal> list = (IEnumerable<decimal>) this._list;
            this.FillConverter<decimal>(list);
        }

        protected override void ProcessDecimalNullable()
        {
            IEnumerable<decimal> list = from x in (IEnumerable<decimal?>) this._list
                where x.HasValue
                select x.Value;
            this.FillConverter<decimal>(list);
        }

        protected override void ProcessDouble()
        {
            IEnumerable<double> list = (IEnumerable<double>) this._list;
            this.FillConverter<double>(list);
        }

        protected override void ProcessDoubleNullable()
        {
            IEnumerable<double> list = from x in (IEnumerable<double?>) this._list
                where x.HasValue
                select x.Value;
            this.FillConverter<double>(list);
        }

        protected override void ProcessEnumNullable()
        {
            base.Type = Enum.GetUnderlyingType(base.Type);
            base.IsNullable = false;
            base.SwitchBaseType();
        }

        protected override void ProcessFloat()
        {
            IEnumerable<float> list = (IEnumerable<float>) this._list;
            this.FillConverter<float>(list);
        }

        protected override void ProcessFloatNullable()
        {
            IEnumerable<float> list = from x in (IEnumerable<float?>) this._list
                where x.HasValue
                select x.Value;
            this.FillConverter<float>(list);
        }

        protected override void ProcessInt16()
        {
            IEnumerable<short> list = (IEnumerable<short>) this._list;
            this.FillConverter<short>(list);
        }

        protected override void ProcessInt16Nullable()
        {
            IEnumerable<short> list = from x in (IEnumerable<short?>) this._list
                where x.HasValue
                select x.Value;
            this.FillConverter<short>(list);
        }

        protected override void ProcessInt32()
        {
            IEnumerable<int> list = (IEnumerable<int>) this._list;
            this.FillConverter<int>(list);
        }

        protected override void ProcessInt32Nullable()
        {
            IEnumerable<int> list = from x in (IEnumerable<int?>) this._list
                where x.HasValue
                select x.Value;
            this.FillConverter<int>(list);
        }

        protected override void ProcessInt64()
        {
            IEnumerable<long> list = (IEnumerable<long>) this._list;
            this.FillConverter<long>(list);
        }

        protected override void ProcessInt64Nullable()
        {
            IEnumerable<long> list = from x in (IEnumerable<long?>) this._list
                where x.HasValue
                select x.Value;
            this.FillConverter<long>(list);
        }

        protected override void ProcessString()
        {
            IEnumerable<string> list = (IEnumerable<string>) this._list;
            this.FillConverter<string>(list);
        }

   
    }
}

