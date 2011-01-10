using System;
using System.Security.Cryptography.X509Certificates;

namespace Hexa.Core.Security
{
    public static class CertificateHelper
    {

        /// <summary>
        /// Gets a X509 certificate from windows store. Asks the user for the correct certificate.
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 GetCertificate()
        {
            X509Store st = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            st.Open(OpenFlags.ReadOnly);
            X509Certificate2 card = null;
            try
            {
                X509Certificate2Collection col = st.Certificates;
                X509Certificate2Collection sel = X509Certificate2UI.SelectFromCollection(col, "Certificates", "Select one to sign", X509SelectionFlag.SingleSelection);
                if (sel.Count > 0)
                {
                    X509Certificate2Enumerator en = sel.GetEnumerator();
                    en.MoveNext();
                    card = en.Current;
                }
            }
            finally
            {
                st.Close();
            }
            return card;
        }

        /// <summary>
        /// Gets a X509 certificate from a pfx file.
        /// </summary>
        /// <param name="path">Path whre the pfx file is stored.</param>
        /// <param name="password">Password for pfx file.</param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificate(string path, string password)
        {
            return new X509Certificate2(path, password);
        }

        /// <summary>
        /// Gets a X509 specific certificate from windows store withoit asking the user.
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 GetCertificate(StoreLocation location, string subjectName)
        {
            X509Certificate2 cert = null;
            X509Store store = new X509Store(StoreName.My, location);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, false);
                if (certs.Count == 1)
                {
                    cert = certs[0];
                }
                else
                    cert = null;
            }
            finally
            {
                if (store != null)
                    store.Close();
            }
            return cert;
        }

        /// <summary>
        /// Load a certificate from the specified file.
        /// The filename can contains the password when using this format: file|password.
        /// The file can be a relative file or an absolute file.
        /// Return null if the file is not specified.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static X509Certificate2 LoadFromFile(string file)
        {
            if (file != null)
                file = file.Trim();

            if (string.IsNullOrEmpty(file))
                return null;

            string[] parts = file.Split('|');
            if (parts.Length > 2)
                throw new ArgumentException(Hexa.Core.Resources.Resource.CertificateFileNameFormatNotValidFilePassword);

            string fullPath = LocateServerPath(parts[0].Trim());

            string password = string.Empty;
            if (parts.Length == 2)
                password = parts[1];

            return new X509Certificate2(fullPath, password);
        }

        private static string LocateServerPath(string path)
        {
            if (System.IO.Path.IsPathRooted(path) == false)
                path = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, path);

            return path;
        }
    }
}
