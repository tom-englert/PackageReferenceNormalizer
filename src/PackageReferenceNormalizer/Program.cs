using System.Xml.Linq;

var fileSpec = args.FirstOrDefault();

if (string.IsNullOrEmpty(fileSpec))
{
    Console.WriteLine(@"You must specify a file name or pattern:");
    Console.WriteLine(@"e.g. PackageReferenceNormalizer c:\Dev\MyProject\*.csproj");
    return;
}

var directoryName = Path.GetDirectoryName(fileSpec);
if (string.IsNullOrEmpty(directoryName))
{
    directoryName = ".";
}
var searchPattern = Path.GetFileName(fileSpec);

try
{
    foreach (var fileName in Directory.EnumerateFiles(directoryName, searchPattern, SearchOption.AllDirectories))
    {
        Console.WriteLine(fileName);

        Engine.NormalizeFile(fileName);
    }
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
    Console.WriteLine("Program terminated!");
}

