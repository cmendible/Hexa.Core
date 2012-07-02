namespace Hexa.Core.Domain
{
    using System;
    using System.Data;
    using NHibernate;
    using NHibernate.Engine;
    using NHibernate.SqlTypes;
    using NHibernate.UserTypes;

    /// <summary />
    /// Implements a IUserVersionType based on TicksType, but returned as String instead of DateTime.
    /// </summary />
    public class TicksAsString : IUserVersionType
    {
        #region IUserVersionType Members

        public object Next(object current, ISessionImplementor session)
        {
            return Seed(session);
        }

        public object Seed(ISessionImplementor session)
        {
            return DateTime.UtcNow.Ticks.ToString();
        }

        public object Assemble(object cached, object owner)
        {
            return DeepCopy(cached);
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Disassemble(object value)
        {
            return DeepCopy(value);
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public bool IsMutable
        {
            get { return false; }
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            object ret = rs.GetValue(rs.GetOrdinal(names[0]));

            if (ret == null)
                return null;

            return ret.ToString();
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            NHibernateUtil.Int64.NullSafeSet(cmd, value, index);
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public Type ReturnedType
        {
            get { return typeof (string); }
        }

        public SqlType[] SqlTypes
        {
            get { return new[] {new SqlType(DbType.Int64)}; }
        }

        public int Compare(object x, object y)
        {
            return ((IComparable) x).CompareTo(y);
        }

        bool IUserType.Equals(object x, object y)
        {
            return (x == y);
        }

        #endregion
    }
}