namespace Hexa.Core.Web.UI.Controls
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Web;

    /// <summary>
    /// Cached Image Service
    /// Add the following configuration to web.config
    /// <httpHandlers>
    ///   <remove verb="*" path="*.asmx"/>
    ///   <add verb="GET" path="cachedimageservice.axd" type="Hexa.Core.Web.UI.Controls.CachedImageService" />
    /// </httpHandlers>
    /// </summary>
    public class CachedImageService : IHttpHandler
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            string storageKey = "";

            // Retrieve the DATA query string parameter
            if (context.Request["data"] == null)
            {
                WriteError();
                return;
            }
            else
            {
                storageKey = context.Request["data"];
            }

            // Grab data from the cache
            object o = HttpContext.Current.Cache[storageKey];
            if (o == null)
            {
                WriteError();
                return;
            }

            var bytes = o as byte[];
            if (bytes != null)
            {
                WriteImageBytes(bytes);
            }
            else
            {
                var image = o as Image;
                if (image != null)
                {
                    WriteImage(image);
                }
            }
        }

        /// <summary>
        /// Writes the error.
        /// </summary>
        private static void WriteError()
        {
            HttpContext.Current.Response.Write("No image specified");
        }

        /// <summary>
        /// Writes the image.
        /// </summary>
        /// <param name="img">The img.</param>
        private static void WriteImage(Image img)
        {
            HttpContext.Current.Response.ContentType = "image/jpeg";
            img.Save(HttpContext.Current.Response.OutputStream, ImageFormat.Jpeg);
        }

        /// <summary>
        /// Writes the image bytes.
        /// </summary>
        /// <param name="img">The img.</param>
        private static void WriteImageBytes(byte[] img)
        {
            HttpContext.Current.Response.ContentType = "image/jpeg";
            HttpContext.Current.Response.OutputStream.Write(img, 0, img.Length);
        }

        #endregion Methods
    }
}