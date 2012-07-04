namespace Hexa.Core.Domain
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Xml;
    using NHibernate.SqlTypes;
    using NHibernate.UserTypes;

    public class XmlType : IUserType
    {
        #region IUserType Members

        public new bool Equals(object x, object y)
        {
            var xdoc_x = (XmlDocument) x;
            var xdoc_y = (XmlDocument) y;
            return xdoc_y.OuterXml == xdoc_x.OuterXml;
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            if (names.Length != 1)
                throw new InvalidOperationException("names array has more than one element. can't handle this!");
            var document = new XmlDocument();
            var val = rs[names[0]] as string;
            if (val != null)
            {
                document.LoadXml(val);
                return document;
            }
            return null;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            var parameter = (DbParameter) cmd.Parameters[index];
            if (value == null)
            {
                parameter.Value = DBNull.Value;
                return;
            }
            parameter.Value = ((XmlDocument) value).OuterXml;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object DeepCopy(object value)
        {
            var other = (XmlDocument) value;
            var xdoc = new XmlDocument();
            xdoc.LoadXml(other.OuterXml);
            return xdoc;
        }

        public SqlType[] SqlTypes
        {
            get { return new SqlType[] {new SqlXmlType()}; }
        }

        public Type ReturnedType
        {
            get { return typeof(XmlDocument); }
        }

        public bool IsMutable
        {
            get { return true; }
        }

        #endregion
    }

    public class SqlXmlType : SqlType
    {
        public SqlXmlType() : base(DbType.Xml)
        {
        }
    }
}