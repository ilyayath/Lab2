using System;
using System.Collections.Generic;
using System.Xml;

public class SAXParser : IXmlParsingStrategy
{
    public string Analyze(string xmlFilePath)
    {
        var analysisResults = new List<string>
        {
            "Аналіз за допомогою SAX:",
            new string('-', 40)
        };

        using (XmlReader reader = XmlReader.Create(xmlFilePath))
        {
            while (reader.Read())
            {
                ProcessResourceNode(reader, analysisResults);
                ProcessElement(reader, analysisResults);
            }
        }

        return string.Join(Environment.NewLine, analysisResults);
    }

    private void ProcessResourceNode(XmlReader reader, List<string> analysisResults)
    {
        if (reader.NodeType == XmlNodeType.Element && reader.Name == "resource")
        {
            if (reader.HasAttributes)
            {
                AddAttributes(reader, analysisResults);
            }

            analysisResults.Add(new string('-', 40));
        }
    }

    private void AddAttributes(XmlReader reader, List<string> analysisResults)
    {
        while (reader.MoveToNextAttribute())
        {
            analysisResults.Add($"{reader.Name}: {reader.Value}");
        }
    }

    private void ProcessElement(XmlReader reader, List<string> analysisResults)
    {
        if (reader.NodeType == XmlNodeType.Element && reader.Depth > 1)
        {
            string elementName = reader.Name;
            reader.Read();

            if (reader.NodeType == XmlNodeType.Text)
            {
                analysisResults.Add($"{elementName}: {reader.Value}");
            }
        }
    }
}
