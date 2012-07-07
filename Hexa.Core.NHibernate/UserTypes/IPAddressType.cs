namespace Hexa.Core.Domain
{
    using System;
    using System.Data;
    using System.Net;

    using NHibernate;
    using NHibernate.SqlTypes;
    using NHibernate.UserTypes;

    public class IPAddressType : IUserType
    {
        #region Properties

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
                return typeof(IPAddress);
            }
        }

        public SqlType[] SqlTypes
        {
            get
            {
                return new[] {NHibernateUtil.String.SqlType};
            }
        }

        #endregion Properties

        #region Methods

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

            return new IPAddress(((IPAddress) value).GetAddressBytes());
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
            Int32 index = rs.GetOrdinal(names[0]);
            if (rs.IsDBNull(index))
            {
                return null;
            }

            try
            {
                return IPAddress.Parse(rs[index].ToString());
            }
            catch (FormatException)
            {
                //The uri is malformed, maybe it is worth to doing something else.
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

            var obj = (IPAddress) value;
            NHibernateUtil.String.Set(cmd, obj.ToString(), index);
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        #endregion Methods
    }
}
