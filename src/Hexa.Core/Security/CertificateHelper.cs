namespace Hexa.Core.Security
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    public static class CertificateHelper
    {
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
        [SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:DisposeObjectsBeforeLosingScope",
            Justification = "The Close method calls the Dispose method.")]
        public static X509Certificate2 GetCertificate(StoreLocation location, string subjectName)
        {
            X509Certificate2 cert = null;
            var store = new X509Store(StoreName.My, location);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                X509Certificate2Collection certs = store.Certificates.Find(
                                                       X509FindType.FindBySubjectName,
                                                       subjectName,
                                                       false);
                if (certs.Count == 1)
                {
                    cert = certs[0];
                }
                else
                {
                    cert = null;
                }
            }
            finally
            {
                if (store != null)
                {
                    store.Close();
                }
            }

            return cert;
        }
    }
}