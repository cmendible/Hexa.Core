namespace Hexa.Core.Domain
{
    using System;
    using System.Data;

    using NHibernate;
    using NHibernate.SqlTypes;
    using NHibernate.UserTypes;

    public class StringToInt : IUserType
    {
        public bool IsMutable
        {
            get
            {
                return true;
            }
        }

        public Type ReturnedType
        {
            get
            {
                return typeof(string);
            }
        }

        public SqlType[] SqlTypes
        {
            get
            {
                return new[] { NHibernateUtil.Int32.SqlType };
            }
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object DeepCopy(object value)
        {
            if (value == null)
            {
                return null;
            }

            return value.ToString();
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        bool IUserType.Equals(object x, object y)
        {
            return Equals(x, y);
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            int index = rs.GetOrdinal(names[0]);
            if (rs.IsDBNull(index))
            {
                return null;
            }

            try
            {
                return rs[index].ToString();
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null || value == DBNull.Value)
            {
                NHibernateUtil.String.NullSafeSet(cmd, null, index);
                return;
            }

            int obj = int.Parse(value.ToString());
            NHibernateUtil.String.Set(cmd, obj, index);
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }
    }
}