namespace Hexa.Core.Web.UI.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Web.Caching;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    /// DynamicImage
    /// </summary>
    [DefaultProperty("ImageFile")]
    [ToolboxData("<{0}:DynamicImage runat=server></{0}:DynamicImage>")]
    public class DynamicImage : Image
    {
        #region Fields

        private const string BaseUrl = "~/CachedImageService.axd?data={0}";

        private System.Drawing.Image _image;
        private byte[] _imageBytes;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicImage"/> class.
        /// </summary>
        public DynamicImage()
        {
            PreRender += DynamicImage_PreRender;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        [Browsable(false)]
        public System.Drawing.Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        /// <summary>
        /// Gets or sets the image bytes.
        /// </summary>
        /// <value>The image bytes.</value>
        [SuppressMessage("Microsoft.Performance",
                         "CA1819:PropertiesShouldNotReturnArrays"),
        Browsable(false)]
        public byte[] ImageBytes
        {
            get
            {
                return _imageBytes;
            }
            set
            {
                Page.Cache.Remove(StorageKey);
                ViewState["StorageKey"] = null;
                _imageBytes = value;
                Visible = true;
            }
        }

        /// <summary>
        /// Gets or sets the image file.
        /// </summary>
        /// <value>The image file.</value>
        public string ImageFile
        {
            get
            {
                return Convert.ToString(ViewState["ImageFile"], CultureInfo.InvariantCulture);
            }
            set
            {
                ViewState["ImageFile"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the location of an image to display in the <see cref="T:System.Web.UI.WebControls.Image"/> control.
        /// </summary>
        /// <value></value>
        /// <returns>The location of an image to display in the <see cref="T:System.Web.UI.WebControls.Image"/> control.</returns>
        public override string ImageUrl
        {
            get
            {
                return GetImageUrl();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets or sets the storage key.
        /// </summary>
        /// <value>The storage key.</value>
        protected string StorageKey
        {
            get
            {
                return Convert.ToString(ViewState["StorageKey"], CultureInfo.InvariantCulture);
            }
            set
            {
                ViewState["StorageKey"] = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the PreRender event of the DynamicImage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DynamicImage_PreRender(object sender, EventArgs e)
        {
            if (ImageUrl.Length == 0)
            {
                return;
            }

            // Cache bytes or image
            if (ImageBytes != null)
            {
                StoreImageBytes();
            }
            else if (Image != null)
            {
                StoreImage();
            }

            return;
        }

        /// <summary>
        /// Gets the cached image URL.
        /// </summary>
        /// <returns></returns>
        private string GetCachedImageUrl()
        {
            return string.Format(CultureInfo.InvariantCulture, BaseUrl, StorageKey);
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <returns></returns>
        private string GetImageUrl()
        {
            string url = "";

            // Check ImageFile
            if (ImageFile.Length > 0)
            {
                url = ImageFile;
            }
            else if (ImageBytes != null || Image != null)
            {
                if (StorageKey.Length == 0)
                {
                    Guid g = Guid.NewGuid();
                    StorageKey = g.ToString();
                }
                return GetCachedImageUrl();
            }
            else
            {
                Visible = false;
                return string.Empty;
            }

            return url;
        }

        /// <summary>
        /// Stores the data.
        /// </summary>
        /// <param name="data">The data.</param>
        private void StoreData(object data)
        {
            if (Page.Cache[StorageKey] == null)
            {
                Page.Cache.Add(StorageKey,
                               data,
                               null,
                               Cache.NoAbsoluteExpiration,
                               TimeSpan.FromMinutes(5),
                               CacheItemPriority.High,
                               null);
            }
        }

        /// <summary>
        /// Stores the image.
        /// </summary>
        private void StoreImage()
        {
            StoreData(_image);
        }

        /// <summary>
        /// Stores the image bytes.
        /// </summary>
        private void StoreImageBytes()
        {
            StoreData(_imageBytes);
        }

        #endregion Methods
    }
}