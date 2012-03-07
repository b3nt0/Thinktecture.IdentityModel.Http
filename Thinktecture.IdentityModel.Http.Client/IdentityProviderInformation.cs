using System;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;

namespace Thinktecture.IdentityModel.Http
{
    [DataContract]
    public class IdentityProviderInformation
    {
        private BitmapImage _image;

        /// <summary>
        /// The display name for the identity provider.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The url used for Login to the identity provider.
        /// </summary>
        [DataMember]
        public string LoginUrl { get; set; }

        /// <summary>
        /// The url that is used to retrieve the image for the identity provider
        /// </summary>
        [DataMember]
        public string ImageUrl { get; set; }

        /// <summary>
        /// A list fo email address suffixes configured for the identity provider.
        /// </summary>
        [DataMember]
        public string[] EmailAddressSuffixes { get; set; }

        /// <summary>
        /// The image populated by calling LoadImageFromImageUrl
        /// </summary>
        public BitmapImage Image
        {
            get
            {
                return _image;
            }
        }

        /// <summary>
        /// Retieves the image from ImageUrl
        /// </summary>
        /// <returns>The image from the url as a BitmapImage</returns>
        public BitmapImage LoadImageFromImageUrl()
        {
            _image = null;

            if (string.Empty != ImageUrl)
            {
                BitmapImage imageBitmap = new BitmapImage();
                Uri imageUrlUri = new Uri(ImageUrl);
                imageBitmap.UriSource = imageUrlUri;
                _image = imageBitmap;
            }

            return _image;
        }
    }
}
