using System;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Hexa.Core.Domain
{
    public class StringToGuid : IUserType
    {
        #region Equals member

        bool IUserType.Equals(object x, object y)
        {
            return object.Equals(x, y);
        }

        #endregion

        #region IUserType Members

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object DeepCopy(object value)
        {
            if (value == null)
                return null;

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

        public bool IsMutable
        {
            get
                {
                    return true;
                }
        }

        public object NullSafeGet(System.Data.IDataReader rs, string[] names, object owner)
        {
            Int32 index = rs.GetOrdinal(names[0]);
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

        public void NullSafeSet(System.Data.IDbCommand cmd, object value, int index)
        {
            if (value == null || value == DBNull.Value)
                {
                    NHibernateUtil.String.NullSafeSet(cmd, null, index);
                    return;
                }

            var obj = Guid.Parse(value.ToString());
            NHibernateUtil.String.Set(cmd, obj, index);
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public Type ReturnedType
        {
            get
                {
                    return typeof(string);
                }
        }

        public NHibernate.SqlTypes.SqlType[] SqlTypes
        {
            get
                {
                    return new SqlType[] { NHibernateUtil.Guid.SqlType };
                }
        }

        #endregion
    }
}
