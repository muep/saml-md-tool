namespace SamlMdTool;

public class Util
{
    public static string CertWrapPemHeaders(string almostPem)
    {
        return "-----BEGIN CERTIFICATE-----\n" + almostPem.Trim() + "\n-----END CERTIFICATE-----";
    }
}