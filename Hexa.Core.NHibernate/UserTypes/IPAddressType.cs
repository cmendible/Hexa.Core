using System;
using System.Net;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Hexa.Core.Domain
{
	public class IPAddressType : IUserType
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

			return new IPAddress(((IPAddress)value).GetAddressBytes());
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
			get { return true; }
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
				return IPAddress.Parse(rs[index].ToString());
			}
			catch (FormatException)
			{
				//The uri is malformed, maybe it is worth to doing something else.
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

			var obj = (IPAddress)value;
			NHibernateUtil.String.Set(cmd, obj.ToString(), index);
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public Type ReturnedType
		{
			get { return typeof(IPAddress); }
		}

		public NHibernate.SqlTypes.SqlType[] SqlTypes
		{
			get { return new SqlType[] { NHibernateUtil.String.SqlType }; }
		}

		#endregion
	}
}
