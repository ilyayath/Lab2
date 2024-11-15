
public class XmlParserContext
{
    private IXmlParsingStrategy _strategy;

    public XmlParserContext(IXmlParsingStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(IXmlParsingStrategy strategy)
    {
        _strategy = strategy;
    }

    public string Analyze(string xmlFilePath)
    {
        return _strategy.Analyze(xmlFilePath);
    }
}
public interface IXmlParsingStrategy
{
    string Analyze(string xmlFilePath);
}