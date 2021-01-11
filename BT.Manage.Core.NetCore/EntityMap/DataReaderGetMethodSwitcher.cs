using System;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    public class DataReaderGetMethodSwitcher : BaseTypeSwitcher
    {
        private readonly Expression _indexExp;
        private readonly Expression _readerExp;

        public DataReaderGetMethodSwitcher(Type type, Expression index, Expression reader) : base(type)
        {
            _indexExp = index;
            _readerExp = reader;
        }

        protected override void ProcessBoolean()
        {
            Expression[] arguments = {_indexExp};
            Result = Expression.Call(_readerExp, ReflectorConsts.GetBooleanOfIDataReader, arguments);
        }

        protected override void ProcessBooleanNullable()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessByte()
        {
            Expression[] arguments = {_indexExp};
            Result = Expression.Call(_readerExp, ReflectorConsts.GetByteOfIDataReader, arguments);
        }

        protected override void ProcessByteNullable()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessDateTime()
        {
            Expression[] arguments = {_indexExp};
            Result = Expression.Call(_readerExp, ReflectorConsts.GetDateTimeOfIDataReader, arguments);
        }

        protected override void ProcessDateTimeNullable()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessDecimal()
        {
            Expression[] arguments = {_indexExp};
            Result = Expression.Call(_readerExp, ReflectorConsts.GetDecimalOfIDataReader, arguments);
        }

        protected override void ProcessDecimalNullable()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessDouble()
        {
            Expression[] arguments = {_indexExp};
            Result = Expression.Call(_readerExp, ReflectorConsts.GetDoubleOfIDataReader, arguments);
        }

        protected override void ProcessDoubleNullable()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessEnum()
        {
            base.ProcessEnum();
            Result = Expression.Convert((Expression) Result, RawType);
        }

        protected override void ProcessEnumNullable()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessFloat()
        {
            Expression[] arguments = {_indexExp};
            Result = Expression.Call(_readerExp, ReflectorConsts.GetFloatOfIDataReader, arguments);
        }

        protected override void ProcessFloatNullable()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessInt16()
        {
            Expression[] arguments = {_indexExp};
            Result = Expression.Call(_readerExp, ReflectorConsts.GetInt16OfIDataReader, arguments);
        }

        protected override void ProcessInt16Nullable()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessInt32()
        {
            Expression[] arguments = {_indexExp};
            Result = Expression.Call(_readerExp, ReflectorConsts.GetInt32OfIDataReader, arguments);
        }

        protected override void ProcessInt32Nullable()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessInt64()
        {
            Expression[] arguments = {_indexExp};
            Result = Expression.Call(_readerExp, ReflectorConsts.GetInt64OfIDataReader, arguments);
        }

        protected override void ProcessInt64Nullable()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessString()
        {
            Expression[] arguments = {_indexExp};
            Result = Expression.Call(_readerExp, ReflectorConsts.GetStringOfIDataReader, arguments);
        }
    }
}