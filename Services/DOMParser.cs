using System;
using System.Collections.Generic;
using System.Xml;

public class DOMParser : IXmlParsingStrategy
{
    public string Analyze(string xmlFilePath)
    {
        List<string> analysisResults = new List<string>
        {
            "Аналіз за допомогою DOM:",
            new string('-', 30)
        };

        var doc = LoadXmlDocument(xmlFilePath);
        if (doc == null) return "Помилка завантаження XML документа.";

        var resources = doc.GetElementsByTagName("resource");
        foreach (XmlNode resource in resources)
        {
            analysisResults.AddRange(ProcessResourceNode(resource));
            analysisResults.Add(new string('-', 30));
        }

        return string.Join(Environment.NewLine, analysisResults);
    }

    private XmlDocument LoadXmlDocument(string xmlFilePath)
    {
        var doc = new XmlDocument();
        try
        {
            doc.Load(xmlFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при завантаженні XML: {ex.Message}");
            return null;
        }
        return doc;
    }

    private IEnumerable<string> ProcessResourceNode(XmlNode resource)
    {
        var results = new List<string>();

        if (resource.Attributes != null)
        {
            results.AddRange(ProcessAttributes(resource.Attributes));
        }

        results.AddRange(ProcessChildNodes(resource.ChildNodes));
        return results;
    }

    private IEnumerable<string> ProcessAttributes(XmlAttributeCollection attributes)
    {
        foreach (XmlAttribute attr in attributes)
        {
            yield return $"{attr.Name}: {attr.Value}";
        }
    }

    private IEnumerable<string> ProcessChildNodes(XmlNodeList childNodes)
    {
        foreach (XmlNode child in childNodes)
        {
            yield return $"{child.Name}: {child.InnerText}";
        }
    }
}
