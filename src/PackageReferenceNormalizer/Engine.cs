using System.Xml.Linq;

internal static class Engine
{
    public static void NormalizeFile(string fileName)
    {
        using var file = new XmlFile(fileName);

        var doc = file.Document;

        var packageReferenceNodes = doc.Descendants().Where(item => item.Name.LocalName == "PackageReference"
                                                                 && item.Parent?.Name.LocalName == "ItemGroup"
                                                                 && item.Parent?.Parent == doc.Root);

        foreach (var packageReferenceNode in packageReferenceNodes)
        {
            packageReferenceNode.Normalize("Version");

            var privateAssets = packageReferenceNode.Normalize("PrivateAssets");

            if (string.Equals(privateAssets, "All", StringComparison.OrdinalIgnoreCase))
            {
                packageReferenceNode.Remove("IncludeAssets");
            }
            else
            {
                packageReferenceNode.Normalize("IncludeAssets");
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

    private static string? Normalize(this XElement packageReferenceNode, string itemName)
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

    private static void Remove(this XElement packageReferenceNode, string itemName)
    {
        var element = packageReferenceNode
            .Descendants()
            .SingleOrDefault(item =>
                string.Equals(item.Name.LocalName, itemName, StringComparison.OrdinalIgnoreCase) && item.Parent == packageReferenceNode);

        element?.Remove();

        var attribute = packageReferenceNode.Attribute(itemName);

        attribute?.Remove();
    }
}
