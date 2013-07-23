namespace Hexa.Core
{
    using System;

    /// <summary>
    /// Contains methods to create and manipulate GUID and COMBGUID unique IDs.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Determines whether [is empty or null] [the specified GUID].
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns>
        /// <c>true</c> if [is empty or null] [the specified GUID]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmptyOrNull(this Guid value)
        {
            return __IsEmptyOrNull(value);
        }

        /// <summary>
        /// Determines whether [is empty or null] [the specified GUID].
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns>
        /// <c>true</c> if [is empty or null] [the specified GUID]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmptyOrNull(this Guid? value)
        {
            return __IsEmptyOrNull(value);
        }

        /// <summary>
        /// Determines whether the specified GUID is valid.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns>
        /// <c>true</c> if the specified GUID is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValid(this Guid value)
        {
            byte[] bits = value.ToByteArray();

            const int VariantShift = 6;
            const int VariantMask = 0x3 << VariantShift;
            const int VariantBits = 0x2 << VariantShift;

            if ((bits[8] & VariantMask) != VariantBits)
            {
                return false;
            }

            const int VersionShift = 4;
            const int VersionMask = 0xf << VersionShift;
            const int VersionBits = 0x4 << VersionShift;

            if ((bits[7] & VersionMask) != VersionBits)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a new Guid object that conforms to the COMBGUID rules.
        /// </summary>
        /// <returns></returns>
        public static Guid NewCombGuid()
        {
            var baseDate = new DateTime(1900, 1, 1);
            DateTime now = DateTime.Now;
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            // Get the days and milliseconds which will be used to build the byte string
            var days = new TimeSpan(now.Ticks - baseDate.Ticks);
            var msecs = new TimeSpan(now.Ticks - new DateTime(now.Year, now.Month, now.Day).Ticks);

            // Convert to a byte array
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333
            byte[] daysArray = BitConverter.GetBytes(days.Days);
            byte[] msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);
            return new Guid(guidArray);
        }

        /// <summary>
        /// Creates a new Guid object that conforms to the COMBGUID rules.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        public static Guid NewCombGuid(this Guid value)
        {
            return value.NewCombGuid();
        }

        /// <summary>
        /// Shows the date.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        public static DateTime ShowDate(this Guid value)
        {
            var dayBits = new byte[4];
            var msecBits = new byte[4];
            byte[] guidBits = value.ToByteArray();

            // First we should obtain msecs and days from guid hex string
            Array.Copy(guidBits, guidBits.Length - 6, dayBits, 2, 2);
            Array.Copy(guidBits, guidBits.Length - 4, msecBits, 0, 4);

            // Now we need to reverse both arrays into an appropiate order
            // so we can work with them.. remember Intel (and .Net) is little endian ;)
            Array.Reverse(dayBits);
            Array.Reverse(msecBits);

            // Now that we have days in a byte array, we can convert them into something
            // more useable.. let's say a TimeSpam objet..
            var days = new TimeSpan(BitConverter.ToInt32(dayBits, 0), 0, 0, 0);
            var date = new DateTime(new DateTime(1900, 1, 1).Ticks + days.Ticks);

            // This should print the year/month/day, but at 00:00:00 hours.

            // Now we need to get the hour.. it is encoded in milliseconds with
            // 1/300th aproximation.. so first, let's pass it to a double
            // and multiply it by 3.33333
            double tmp = BitConverter.ToInt32(msecBits, 0) * 3.333333;

            // Now we can convert this into a TimeSpan, passing milliseconds
            // as Ticks. Remeber, ticks is a "normally" a constant value
            // witch 10000/1 times a second. But you should not assume this value
            // but instead you should use TimeSpan.TicksPerMillisecond constant
            // to operate with ticks vs msecs.
            var msecs = new TimeSpan(((long)tmp) * TimeSpan.TicksPerMillisecond);

            // Now we can obtain the final date
            date = new DateTime(date.Ticks + msecs.Ticks);

            return date;
        }

        /// <summary>
        /// Is the Guid empty or null.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        private static bool __IsEmptyOrNull(this Guid? value)
        {
            if (value == null)
            {
                return true;
            }

            if (string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }

            if (value == new Guid())
            {
                return true;
            }

            return false;
        }
    }
}