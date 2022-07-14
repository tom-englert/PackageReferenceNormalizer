using System.Xml.Linq;

var fileSpec = args.FirstOrDefault();

if (string.IsNullOrEmpty(fileSpec))
{
    Console.WriteLine("You must specify a file name or pattern");
    return;
}

var directoryName = Path.GetDirectoryName(fileSpec);
if (string.IsNullOrEmpty(directoryName))
{
    directoryName = ".";
}

foreach (var fileName in Directory.EnumerateFiles(directoryName, Path.GetFileName(fileSpec), SearchOption.AllDirectories))
{
    NormalizeFile(fileName);
}

static void NormalizeFile(string fileName)
{
    using var file = new XmlFile(fileName);

    var doc = file.Document;

    var packageReferenceNodes = doc.Descendants().Where(item => item.Name.LocalName == "PackageReference"
                                                             && item.Parent?.Name.LocalName == "ItemGroup"
                                                             && item.Parent?.Parent == doc.Root);

    foreach (var packageReferenceNode in packageReferenceNodes)
    {
        Normalize(packageReferenceNode, "Version");

        var privateAssets = Normalize(packageReferenceNode, "PrivateAssets");

        if (string.Equals(privateAssets, "All", StringComparison.OrdinalIgnoreCase))
        {
            Remove(packageReferenceNode, "IncludeAssets");
        }
        else
        {
            Normalize(packageReferenceNode, "IncludeAssets");
        }
    }

    var propertyGroupNodes = doc.Descendants().Where(item => item.Name.LocalName == "PropertyGroup"
                                                          && item.Parent == doc.Root);

    var emptyPropertyGroupNodes = propertyGroupNodes.Where(item => !item.Descendants().Any());

    foreach (var emptyPropertyGroupNode in emptyPropertyGroupNodes.ToArray())
    {
        emptyPropertyGroupNode.Remove();
    }
}

static string? Normalize(XElement packageReferenceNode, string itemName)
{
    var element = packageReferenceNode
        .Descendants()
        .SingleOrDefault(item =>
            string.Equals(item.Name.LocalName, itemName, StringComparison.OrdinalIgnoreCase) && item.Parent == packageReferenceNode);

    if (element != null)
    {
        var descendants = element.DescendantNodes();
        var value = descendants.OfType<XText>().FirstOrDefault()?.Value;
        if (!string.IsNullOrEmpty(value))
        {
            element.Remove();
            packageReferenceNode.Add(new XAttribute(itemName, value));
        }
    }

    return packageReferenceNode.Attribute(itemName)?.Value;
}

static void Remove(XElement packageReferenceNode, string itemName)
{
    var element = packageReferenceNode
        .Descendants()
        .SingleOrDefault(item =>
            string.Equals(item.Name.LocalName, itemName, StringComparison.OrdinalIgnoreCase) && item.Parent == packageReferenceNode);

    element?.Remove();

    var attribute = packageReferenceNode.Attribute(itemName);

    attribute?.Remove();
}