namespace Hexa.Core.ServiceModel
{
    #region Enumerations

    // Summary:
    //     Specifies the type of value searched for by the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
    //     method.
    public enum X509FindType
    {
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     method must be a string representing the thumbprint of the certificate.
        FindByThumbprint = 0,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     method must be a string representing the subject name of the certificate.
        //     This is a less specific search than if you use the System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectDistinguishedName
        //     enumeration value. Using the System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName
        //     value, the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     method performs a case-insensitive string compare using the supplied value.
        //     For example, if you pass "MyCert" to the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     method, it will find all certificates with the subject name containing that
        //     string, regardless of other subject values. Searching by distinguished name
        //     is a more precise search.
        FindBySubjectName = 1,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     method must be a string representing the subject distinguished name of the
        //     certificate. This is a more specific search than if you use the System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName
        //     enumeration value. Using the System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectDistinguishedName
        //     value, the Find method performs a case-insensitive string compare for the
        //     entire distinguished name. Searching by subject name is a less precise search.
        FindBySubjectDistinguishedName = 2,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     method must be a string representing the issuer name of the certificate.
        //     This is a less specific search than if you use the System.Security.Cryptography.X509Certificates.X509FindType.FindByIssuerDistinguishedName
        //     enumeration value. Using the System.Security.Cryptography.X509Certificates.X509FindType.FindByIssuerName
        //     value, the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     method performs a case-insensitive string compare using the supplied value.
        //     For example, if you pass "MyCA" to the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     method, it will find all certificates with the issuer name containing that
        //     string, regardless of other issuer values.
        FindByIssuerName = 3,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     method must be a string representing the issuer distinguished name of the
        //     certificate. This is a more specific search than if you use the System.Security.Cryptography.X509Certificates.X509FindType.FindByIssuerName
        //     enumeration value. Using the System.Security.Cryptography.X509Certificates.X509FindType.FindByIssuerDistinguishedName
        //     value, the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     method performs a case-insensitive string compare for the entire distinguished
        //     name. Searching by issuer name is a less precise search.
        FindByIssuerDistinguishedName = 4,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     must be a string representing the serial number of the certificate as it
        //     is displayed by the UI. The serial number must be in reverse order since
        //     it is an integer.
        FindBySerialNumber = 5,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     must be a System.DateTime value in local time, such as System.DateTime.Now.
        //     Note that the union of certificates returned using System.Security.Cryptography.X509Certificates.X509FindType.FindByTimeValid,
        //     System.Security.Cryptography.X509Certificates.X509FindType.FindByTimeNotYetValid
        //     and System.Security.Cryptography.X509Certificates.X509FindType.FindByTimeExpired
        //     should represent all certificates in the queried collection.
        FindByTimeValid = 6,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     must be a System.DateTime value in local time, such as System.DateTime.Now.
        //     Note that the union of certificates returned using System.Security.Cryptography.X509Certificates.X509FindType.FindByTimeValid,
        //     System.Security.Cryptography.X509Certificates.X509FindType.FindByTimeNotYetValid
        //     and System.Security.Cryptography.X509Certificates.X509FindType.FindByTimeExpired
        //     should represent all certificates in the queried collection.
        FindByTimeNotYetValid = 7,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     must be a System.DateTime value in local time, such as System.DateTime.Now.
        //     Note that the union of certificates returned using System.Security.Cryptography.X509Certificates.X509FindType.FindByTimeValid,
        //     System.Security.Cryptography.X509Certificates.X509FindType.FindByTimeNotYetValid
        //     and System.Security.Cryptography.X509Certificates.X509FindType.FindByTimeExpired
        //     should represent all certificates in the queried collection.
        FindByTimeExpired = 8,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     must be a string representing the template name of the certificate, such
        //     as "ClientAuth." A template name is an X509 version 3 extension that specifies
        //     the uses of the certificate.
        FindByTemplateName = 9,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     must be a string representing either the application policy friendly name
        //     or the object identifier (System.Security.Cryptography.Oid) of the certificate.
        //     For example, "Encrypting File System" or "1.3.6.1.4.1.311.10.3.4" can be
        //     used. Note that for an application that is going to be localized, the OID
        //     value must be used since the friendly name is localized.
        FindByApplicationPolicy = 10,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     must be a string representing either the friendly name or the object identifier
        //     (System.Security.Cryptography.Oid) of the certificate policy. The best practice
        //     is to use the OID, such as "1.3.6.1.4.1.311.10.3.4". Note that for an application
        //     that is going to be localized, the OID must be used since the friendly name
        //     is localized.
        FindByCertificatePolicy = 11,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     must be a string describing the extension to find. The object identifier
        //     (OID) is most commonly used to direct the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     method to search for all certificates that have an extension matching that
        //     OID value.
        FindByExtension = 12,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     must be either a string representing the key usage or an integer representing
        //     a bit mask containing all the requested key usages. For the string value,
        //     only one key usage at a time can be specified, but the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     method can be used in a cascading sequence to get the intersection of the
        //     requested usages. For example, the findValue parameter can be set to "KeyEncipherment"
        //     or an integer (0x30 indicates "KeyEncipherment" and "DataEncipherment").
        //     Values of the System.Security.Cryptography.X509Certificates.X509KeyUsageFlags
        //     enumeration can also be used.
        FindByKeyUsage = 13,
        //
        // Summary:
        //     The findValue parameter for the System.Security.Cryptography.X509Certificates.X509Certificate2Collection.Find(System.Security.Cryptography.X509Certificates.X509FindType,System.Object,System.Boolean)
        //     must be a string representing the subject key identifier in hexadecimal,
        //     such as "F3E815D45E83B8477B9284113C64EF208E897112," as displayed in the UI.
        FindBySubjectKeyIdentifier = 14,
        // Summary:
        //	   The findValue parameter indicates a file to load certificate from.
        FindByFile = 15
    }

    #endregion Enumerations
}
