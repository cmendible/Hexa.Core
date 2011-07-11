using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Caching;

namespace Hexa.Core.WebServices
{
	/// <summary>
	/// This is the VirtualPathProvider that serves up virtual files. The supported 
	/// requests are of the form "~/Virtual/ClassName.svc".
	/// </summary>
	internal class VirtualPathProvider : System.Web.Hosting.VirtualPathProvider
	{
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        internal static readonly string VirtualDirectory = "~"; 
		protected IWebServicesManager _Manager;

		public VirtualPathProvider(IWebServicesManager svcman)
		{
			_Manager = svcman;
		}

        private static bool IsVirtualDirectory(string appRelativeVirtualPath)
		{
			return appRelativeVirtualPath.Equals(VirtualDirectory, StringComparison.OrdinalIgnoreCase);
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.StartsWith(System.String)")]
        private static string ToAppRelativeVirtualPath(string virtualPath)
		{
			string appRelativeVirtualPath = VirtualPathUtility.ToAppRelative(virtualPath);

			if (!appRelativeVirtualPath.StartsWith("~/"))
			{
				throw new HttpException("Unexpectedly does not start with ~.");
			}
			return appRelativeVirtualPath;
		}

		private bool IsVirtualFile(string appRelativeVirtualPath)
		{
			if (appRelativeVirtualPath.StartsWith(VirtualDirectory + "/", StringComparison.OrdinalIgnoreCase))
			{
				string fname = appRelativeVirtualPath.Substring(VirtualDirectory.Length);
				if (_Manager.IsRegistered(fname))
					return true;
			}
			return false;
		}

		public override bool FileExists(string virtualPath)
		{
			string appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualPath);

			if (IsVirtualFile(appRelativeVirtualPath))
			{
				return true;
			}
			else
			{
				return Previous.FileExists(virtualPath);
			}
		}

		public override bool DirectoryExists(string virtualDir)
		{
			string appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualDir);

			if (IsVirtualDirectory(appRelativeVirtualPath))
			{
				return true;
			}
			else
			{
				return Previous.DirectoryExists(virtualDir);
			}
		}

		public override System.Web.Hosting.VirtualFile GetFile(string virtualPath)
		{
			string appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualPath);

			if (IsVirtualFile(appRelativeVirtualPath))
			{
				return new VirtualFile(virtualPath, virtualPath, typeof(ServiceFactory).FullName);
			}
			else
			{
				return Previous.GetFile(virtualPath);
			}
		}

		public override System.Web.Hosting.VirtualDirectory GetDirectory(string virtualDir)
		{
			string appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualDir);

			if (IsVirtualDirectory(appRelativeVirtualPath))
			{
				return new VirtualDirectory(virtualDir, typeof(ServiceFactory).FullName);
			}

			return Previous.GetDirectory(virtualDir);
		}

		/// <summary>
		/// Creates a cache dependency based on the specified virtual paths.
		/// In this specific implementation null is returned for virtual paths, and 
		/// Parent.GetCacheDependency() is called for non-virtual paths.
		/// </summary>
		/// <param name="virtualPath">The path to the primary virtual resource.</param>
		/// <param name="virtualPathDependencies">An array of paths to other resources required by the primary virtual resource.</param>
		/// <param name="utcStart">The UTC time at which the virtual resources were read.</param>
		/// <returns>
		/// A <see cref="T:System.Web.Caching.CacheDependency"/> object for the specified virtual resources.
		/// </returns>
		public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
		{
			string appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualPath);

			if (IsVirtualFile(appRelativeVirtualPath) || IsVirtualDirectory(appRelativeVirtualPath))
			{
				return null;
			}
			else
			{
				return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
			}
		}
	}

	// This is the virtual .svc file.
	// When this file is accessed, a <%@ServiceHost%> tag is returned that gives
	// the service and factory associated with this VirtualFile.
	internal class VirtualFile : System.Web.Hosting.VirtualFile
	{
		private string _Service;
		private string _Factory;

		public VirtualFile(string vp, string service, string factory)
			: base(vp)
		{
			_Service = service;
			_Factory = factory;
		}

		public override Stream Open()
		{
			MemoryStream ms = new MemoryStream();
			StreamWriter tw = new StreamWriter(ms);
			tw.Write(string.Format(CultureInfo.InvariantCulture,
			  "<%@ServiceHost language=c# Debug=\"true\" Service=\"{0}\" Factory=\"{1}\"%>",
			  HttpUtility.HtmlEncode(_Service), HttpUtility.HtmlEncode(_Factory)));
			tw.Flush();
			ms.Position = 0;
			return ms;
		}
	}

	internal class VirtualDirectory : System.Web.Hosting.VirtualDirectory
	{
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "factory")]
        public VirtualDirectory(string vpath, string factory)
			: base(vpath)
		{

			throw new NotImplementedException("This code is not fully functional");
		}

		private ArrayList children = new ArrayList();
		public override IEnumerable Children
		{
			get { return children; }
		}

		private ArrayList directories = new ArrayList();
		public override IEnumerable Directories
		{
			get { return directories; }
		}

		private ArrayList files = new ArrayList();
		public override IEnumerable Files
		{
			get { return files; }
		}

	}

}