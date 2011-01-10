using System;
using System.Xml;    
using System.Data;
using System.Data.Common;
using System.Globalization;

using NHibernate;
using NHibernate.Engine;
using NHibernate.UserTypes;
using NHibernate.SqlTypes;

namespace Hexa.Core.Domain
{
    public class XmlType : IUserType
    {
        public new bool Equals(object x, object y)
        {
            XmlDocument xdoc_x = (XmlDocument) x;
            XmlDocument xdoc_y = (XmlDocument) y;
            return xdoc_y.OuterXml == xdoc_x.OuterXml;
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            if (names.Length != 1)
                throw new InvalidOperationException("names array has more than one element. can't handle this!");
            XmlDocument document = new XmlDocument();
            string val = rs[names[0]] as string;
            if (val != null)
            {
                document.LoadXml(val);
                return document;
            }
            return null;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            DbParameter parameter = (DbParameter)cmd.Parameters[index];
            if(value == null)
            {
                parameter.Value = DBNull.Value;
                return;
            }
            parameter.Value = ((XmlDocument)value).OuterXml;
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
            XmlDocument other = (XmlDocument) value;
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(other.OuterXml);
            return xdoc;
        }

        public SqlType[] SqlTypes
        {
            get 
            {
                return new SqlType[] { new SqlXmlType() };
            }
        }

        public System.Type ReturnedType
        {
            get { return typeof(XmlDocument); }
        }

        public bool IsMutable
        {
            get { return true; }
        }
    }
    
    public class SqlXmlType : SqlType
    {
        public SqlXmlType() : base(System.Data.DbType.Xml)
        {
        }
    }
}
