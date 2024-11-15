using System;
using System.Collections.Generic;
using System.Xml.Linq;

public class LINQParser : IXmlParsingStrategy
{
    public string Analyze(string xmlFilePath)
    {
        List<string> analysisResults = new List<string>
        {
            "Аналіз за допомогою LINQ:",
            new string('-', 40)
        };

        XDocument doc = XDocument.Load(xmlFilePath);
        var resources = doc.Descendants("resource");

        foreach (var resource in resources)
        {
            AddAttributes(resource, analysisResults);
            AddElements(resource, analysisResults);
            analysisResults.Add(new string('-', 40));
        }

        return string.Join(Environment.NewLine, analysisResults);
    }

    private void AddAttributes(XElement resource, List<string> analysisResults)
    {
        foreach (var attr in resource.Attributes())
        {
            analysisResults.Add($"{attr.Name}: {attr.Value}");
        }
    }

    private void AddElements(XElement resource, List<string> analysisResults)
    {
        foreach (var element in resource.Elements())
        {
            analysisResults.Add($"{element.Name}: {element.Value}");
        }
    }
}
