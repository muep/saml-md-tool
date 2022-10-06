using System.CommandLine;
using System.Security.Cryptography.X509Certificates;
using SamlMdTool;

var sourceArgument = new Argument<FileInfo>("source", "Location of metadata");
var searchKeywordArgument = new Argument<string?>("keyword", () => null, "Search expression");
var certSerialArgument = new Argument<string>("serial", "Serial number of certificate");

var listCmd = new Command("list", "List contents of metadata");
listCmd.AddArgument(sourceArgument);
listCmd.AddArgument(searchKeywordArgument);
listCmd.SetHandler((source, searchKeyword) =>
{
    var entities = Entity.LoadFromXml(source.OpenRead())
        .Where(e => searchKeyword is null ||
                    e.EntityId.Contains(searchKeyword) ||
                    e.ParsedCertificates().Count(c => c.Subject.Contains(searchKeyword)) > 0);

    foreach (Entity e in entities)
    {
        Console.WriteLine(e);
    }
}, sourceArgument, searchKeywordArgument);

var dumpPemCmd = new Command("dump-pem", "Print out certificate by its serial");
dumpPemCmd.AddArgument(sourceArgument);
dumpPemCmd.AddArgument(certSerialArgument);
dumpPemCmd.SetHandler((source, serial) =>
    {
        var certs =
            Entity.LoadFromXml(source.OpenRead())
                .SelectMany(e => e.SsoCertificates)
                .Where(c => X509Certificate2.CreateFromPem(c).SerialNumber == serial)
                .ToArray();

        Console.WriteLine(certs.FirstOrDefault("Did not find certificate with serial " + serial));
    },
    sourceArgument, certSerialArgument);

var cmd = new RootCommand("SamlMdTool");
cmd.AddCommand(listCmd);
cmd.AddCommand(dumpPemCmd);

return cmd.Invoke(args);