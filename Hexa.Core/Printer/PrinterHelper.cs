#region License

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion

using System;
using System.Management;

namespace Hexa.Core
{
	/// <summary>
	/// Printer helper class based on WMI.
	/// </summary>
	public static class PrinterHelper
	{
		#region Printer State and Status

		private static string[] _Status = { "Other", "Unknown", "Idle", "Printing", "WarmUp", "Stopped Printing", "Offline" };

        private static string[] _ErrorState = {"Unknown", "Other", "No Error", "Low Paper", "No Paper", "Low Toner", "No Toner",
					 "Door Open", "Jammed", "Offline", "Service Requested", "Output Bin Full"};

		/// <summary>
		/// Reads the default printer status.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToInt32(System.Object)")]
		public static string ReadPrinterStatus()
		{
            using (System.Management.ManagementObjectSearcher searcher = new ManagementObjectSearcher
                  ("SELECT * FROM Win32_Printer Where Default = True"))
            {
                foreach (ManagementObject service in searcher.Get())
                {
                    foreach (System.Management.PropertyData pData in service.Properties)
                    {
                        if (pData.Name == "PrinterStatus")
                            return _Status[Convert.ToInt32(pData.Value)];
                    }
                }
            }

			return string.Empty;
		}

		/// <summary>
		/// Reads the default state of the printer.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToInt32(System.Object)")]
		public static string ReadPrinterState()
		{
            using (System.Management.ManagementObjectSearcher searcher = new ManagementObjectSearcher
                  ("SELECT * FROM Win32_Printer Where Default = True"))
            {
                foreach (ManagementObject service in searcher.Get())
                {
                    foreach (System.Management.PropertyData pData in service.Properties)
                    {
                        if (pData.Name == "DetectedErrorState")
                            return _ErrorState[Convert.ToInt32(pData.Value)];
                    }
                }
            }
			return string.Empty;
		}

		/// <summary>
		/// Reads the printer status.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToInt32(System.Object)")]
		public static string ReadPrinterStatus(string name)
		{
            using (System.Management.ManagementObjectSearcher searcher = new ManagementObjectSearcher
                  (string.Format("SELECT * FROM Win32_Printer Where Name = {0}", name)))
            {
                foreach (ManagementObject service in searcher.Get())
                {
                    foreach (System.Management.PropertyData pData in service.Properties)
                    {
                        if (pData.Name == "PrinterStatus")
                            return _Status[Convert.ToInt32(pData.Value)];
                    }
                }
            }

			return string.Empty;
		}

		/// <summary>
		/// Reads the state of the printer.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToInt32(System.Object)")]
		public static string ReadPrinterState(string name)
		{
            using (System.Management.ManagementObjectSearcher searcher = new ManagementObjectSearcher
                  (string.Format("SELECT * FROM Win32_Printer Where Name = {0}", name)))
            {
                foreach (ManagementObject service in searcher.Get())
                {
                    foreach (System.Management.PropertyData pData in service.Properties)
                    {
                        if (pData.Name == "DetectedErrorState")
                            return _ErrorState[Convert.ToInt32(pData.Value)];
                    }
                }
            }
			return string.Empty;
		}

		#endregion

	}
}