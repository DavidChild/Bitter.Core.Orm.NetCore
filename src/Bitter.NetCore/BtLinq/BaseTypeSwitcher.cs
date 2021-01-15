using System;

namespace BT.Manage.Core
{
    public abstract class BaseTypeSwitcher
    {
        public BaseTypeSwitcher(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            RawType = type;
            Type = TypeHelper.GetUnderlyingType(type);
            IsNullable = TypeHelper.IsNullableType(type);
        }

        // Properties
        public bool IsNullable { get; protected set; }

        public Type RawType { get; private set; }


        public object Result { get; protected set; }

        public Type Type { get; protected set; }

        public void Process()
        {
            if (Type.IsEnum)
            {
                if (IsNullable)
                {
                    ProcessEnumNullable();
                }
                else
                {
                    ProcessEnum();
                }
            }
            else
            {
                SwitchBaseType();
            }
        }

        protected abstract void ProcessBoolean();
        protected abstract void ProcessBooleanNullable();
        protected abstract void ProcessByte();
        protected abstract void ProcessByteNullable();
        protected abstract void ProcessDateTime();
        protected abstract void ProcessDateTimeNullable();
        protected abstract void ProcessDecimal();
        protected abstract void ProcessDecimalNullable();
        protected abstract void ProcessDouble();
        protected abstract void ProcessDoubleNullable();

        protected virtual void ProcessEnum()
        {
            Type = Enum.GetUnderlyingType(Type);
            SwitchBaseType();
        }

        protected abstract void ProcessEnumNullable();
        protected abstract void ProcessFloat();
        protected abstract void ProcessFloatNullable();
        protected abstract void ProcessInt16();
        protected abstract void ProcessInt16Nullable();
        protected abstract void ProcessInt32();
        protected abstract void ProcessInt32Nullable();
        protected abstract void ProcessInt64();
        protected abstract void ProcessInt64Nullable();
        protected abstract void ProcessString();

        protected void SwitchBaseType()
        {
            if (Type == ReflectorConsts.Int16Type)
            {
                if (IsNullable)
                {
                    ProcessInt16Nullable();
                }
                else
                {
                    ProcessInt16();
                }
            }
            else if (Type == ReflectorConsts.Int32Type)
            {
                if (IsNullable)
                {
                    ProcessInt32Nullable();
                }
                else
                {
                    ProcessInt32();
                }
            }
            else if (Type == ReflectorConsts.Int64Type)
            {
                if (IsNullable)
                {
                    ProcessInt64Nullable();
                }
                else
                {
                    ProcessInt64();
                }
            }
            else if (Type == ReflectorConsts.ByteType)
            {
                if (IsNullable)
                {
                    ProcessByteNullable();
                }
                else
                {
                    ProcessByte();
                }
            }
            else if (Type == ReflectorConsts.DoubleType)
            {
                if (IsNullable)
                {
                    ProcessDoubleNullable();
                }
                else
                {
                    ProcessDouble();
                }
            }
            else if (Type == ReflectorConsts.StringType)
            {
                ProcessString();
            }
            else if (Type == ReflectorConsts.BoolType)
            {
                if (IsNullable)
                {
                    ProcessBooleanNullable();
                }
                else
                {
                    ProcessBoolean();
                }
            }
            else if (Type == ReflectorConsts.FloatType)
            {
                if (IsNullable)
                {
                    ProcessFloatNullable();
                }
                else
                {
                    ProcessFloat();
                }
            }
            else if (Type == ReflectorConsts.DecimalType)
            {
                if (IsNullable)
                {
                    ProcessDecimalNullable();
                }
                else
                {
                    ProcessDecimal();
                }
            }
            else
            {
                if (!(Type == ReflectorConsts.DateTimeType))
                {
                    throw new Exception();
                }
                if (IsNullable)
                {
                    ProcessDateTimeNullable();
                }
                else
                {
                    ProcessDateTime();
                }
            }
        }
    }
}