using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace SamlMdTool;

public class Entity
{
    public string EntityId { get; private set; }
    public string Name { get; private set; }
    public string[] SsoCertificates { get; private set; }

    public Entity()
    {
        EntityId = "";
        Name = "";
        SsoCertificates = new string[] { };
    }

    public X509Certificate2[] ParsedCertificates()
    {
        return SsoCertificates.Select(
                s => X509Certificate2.CreateFromPem(new ReadOnlySpan<char>(s.ToCharArray()))
            )
            .ToArray();
    }

    public override string ToString()
    {
        return "Entity " + EntityId + "\n" + " org: " + Name + "\n" +
               string.Join("\n", ParsedCertificates().Select(c => CertDesc(c)));
    }

    private static string CertDesc(X509Certificate2 cert)
    {
        return $"    {cert.SerialNumber}\n        {cert.Subject}\n        {cert.NotBefore} - {cert.NotAfter}";
    }

    private static Entity FromElement(XElement element, XNamespace ds, XNamespace md)
    {
        var entityId = element.Attribute("entityID")
            ?.Value ?? "(entity id not available)";

        var fiNames =
            (element.Element(md + "Organization") ?? new XElement(md + "Organization"))
                .Elements(md + "OrganizationName")
                .Where(el => "fi".Equals((string?)el.Attribute(XNamespace.Xml + "lang")))
                .Select(a => a.Value);

        var certificates =
            element.Descendants(ds + "X509Certificate")
                .Select(e => Util.CertWrapPemHeaders(e.Value))
                .ToArray();

        return new Entity
        {
            EntityId = entityId,
            Name = fiNames.FirstOrDefault("Name not known"),
            SsoCertificates = certificates
        };
    }

    public static Entity[] LoadFromXml(Stream source)
    {
        XNamespace md = "urn:oasis:names:tc:SAML:2.0:metadata";
        XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
        XElement metadata = XElement.Load(source);

        return metadata.DescendantsAndSelf(md + "EntityDescriptor").Select(e => FromElement(e, ds, md)).ToArray();
    }
}