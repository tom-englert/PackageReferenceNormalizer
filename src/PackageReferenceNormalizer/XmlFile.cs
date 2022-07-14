using System.Text;
using System.Xml;
using System.Xml.Linq;

internal sealed class XmlFile : IDisposable
{
    private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

    private readonly string _filePath;
    private readonly Encoding _encoding;
    private readonly string _fingerprint;

    public XDocument Document { get; }

    public XmlFile(string filePath)
    {
        _filePath = filePath;

        using var stream = File.OpenRead(filePath);
        using var reader = new StreamReader(stream, Utf8WithoutBom, true);

        _encoding = reader.CurrentEncoding;

        Document = XDocument.Load(reader);

        _fingerprint = Document.ToString(SaveOptions.DisableFormatting);
    }

    public void Dispose()
    {
        if (_fingerprint == Document.ToString(SaveOptions.DisableFormatting))
            return;

        using var stream = File.Open(_filePath, FileMode.Create);

        if (Document.Declaration == null)
        {
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };

            using var xmlWriter = XmlWriter.Create(stream, settings);

            Document.Save(xmlWriter);
        }
        else
        {
            using var writer = new StreamWriter(stream, _encoding);

            Document.Save(writer);
        }
    }
}