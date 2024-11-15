using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace DepartmentResourcesApp
{
    public partial class MainPage : ContentPage
    {
        private List<string> freeAtributes = new List<string>();
        private readonly XmlParserContext parserForContext;
        private string xmlFilePath;
        private string xslFilePath;
        public MainPage()
        {
            InitializeComponent();
            parserForContext = new XmlParserContext(new SAXParser());
        }

        private async void OnAnalyzeClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(xmlFilePath) || strategyPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Помилка", "Виберіть XML-файл і метод обробки", "OK");
                return;
            }

            IXmlParsingStrategy strategy = strategyPicker.SelectedItem.ToString() switch
            {
                "SAX" => new SAXParser(),
                "DOM" => new DOMParser(),
                "LINQ" => new LINQParser(),
                _ => null
            };

            if (strategy == null) return;

            parserForContext.SetStrategy(strategy);
            outputEditor.Text = parserForContext.Analyze(xmlFilePath);
        }

        private async void OnTransformClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(xmlFilePath) || string.IsNullOrEmpty(xslFilePath))
            {
                await DisplayAlert("Помилка", "Виберіть XML та XSL файли", "OK");
                return;
            }

            var htmlContent = XslTransformer.Transform(xmlFilePath, xslFilePath);
            if (!string.IsNullOrEmpty(htmlContent))
            {
                webView.Source = new HtmlWebViewSource { Html = htmlContent };
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            outputEditor.Text = string.Empty;
            webView.Source = new HtmlWebViewSource { Html = string.Empty };
            attributePicker.SelectedIndex = -1;
            attributeValueEntry.Text = string.Empty;
            filePathLabel.Text = "Файл не вибрано";
            xslPathLabel.Text = "Файл не вибрано";
            xmlFilePath = null;
            xslFilePath = null;
        }

        private async void OnExitClicked(object sender, EventArgs e)
        {
            bool confirmExit = await DisplayAlert("Вихід", "Чи дійсно ви хочете вийти з програмки(Не треба,тут класно)?", "Я люблю мат.лог", "Ні");
            if (confirmExit)
            {
                Application.Current.Quit();
            }
        }

        private void UpdateAvailableAttributes()
        {
            if (!string.IsNullOrEmpty(xmlFilePath))
            {
                freeAtributes = XmlAttributeExtractor.ExtractAttributes(xmlFilePath, "resource");
                attributePicker.ItemsSource = freeAtributes;
            }
        }

        private async void OnFilterApplyClicked(object sender, EventArgs e)
        {
            if (attributePicker.SelectedIndex == -1 || string.IsNullOrEmpty(attributeValueEntry.Text))
            {
                await DisplayAlert("Error", "Виберіть атрибут і введіть значення для фільтрування", "OK");
                return;
            }

            string selectedAttribute = attributePicker.SelectedItem.ToString();
            string attributeValue = attributeValueEntry.Text;
            string operatorSymbol = ParseOperator(ref attributeValue);

            try
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                var filteredElements = FilterElements(doc, selectedAttribute, attributeValue, operatorSymbol);

                if (filteredElements.Any())
                {
                    string filteredText = FormatFilteredElements(filteredElements);
                    outputEditor.Text = filteredText;
                    SaveFilteredData(filteredElements);
                }
                else
                {
                    outputEditor.Text = "Не знайдено елементів із зазначеним атрибутом і значенням.";
                }
            }
            catch (Exception ex)
            {
                outputEditor.Text = "Помилка під час фільтрації: " + ex.Message;
            }
        }

        private static string ParseOperator(ref string attributeValue)
        {
            string[] operators = { "<=", "<", ">=", ">" };
            foreach (var op in operators)
            {
                if (attributeValue.StartsWith(op))
                {
                    attributeValue = attributeValue.Substring(op.Length).Trim();
                    return op;
                }
            }
            return string.Empty;
        }

        private IEnumerable<XElement> FilterElements(XDocument doc, string attributeName, string attributeValue, string operatorSymbol)
        {
            return doc.Descendants("resource").Where(el =>
            {
                var attribute = el.Attribute(attributeName)?.Value;
                if (attribute == null) return false;

                if (double.TryParse(attribute, out double attrNum) && double.TryParse(attributeValue, out double filterNum))
                {
                    return operatorSymbol switch
                    {
                        "<=" => attrNum <= filterNum,
                        "<" => attrNum < filterNum,
                        ">=" => attrNum >= filterNum,
                        ">" => attrNum > filterNum,
                        _ => attribute == attributeValue
                    };
                }
                return operatorSymbol switch
                {
                    "<=" => string.Compare(attribute, attributeValue) <= 0,
                    "<" => string.Compare(attribute, attributeValue) < 0,
                    ">=" => string.Compare(attribute, attributeValue) >= 0,
                    ">" => string.Compare(attribute, attributeValue) > 0,
                    _ => attribute.Contains(attributeValue, StringComparison.OrdinalIgnoreCase)
                };
            });
        }

        private string FormatFilteredElements(IEnumerable<XElement> elements)
        {
            return string.Join("\n", elements.Select(el =>
                string.Join(", ", el.Attributes().Select(attr => $"{attr.Name}: {attr.Value}"))));
        }

        private void SaveFilteredData(IEnumerable<XElement> elements)
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), "filteredData.xml");
            new XDocument(new XElement("resources", elements)).Save(tempFilePath);
            xmlFilePath = tempFilePath;
        }

        public static class XmlAttributeExtractor
        {
            public static List<string> ExtractAttributes(string xmlFilePath, string nodeName)
            {
                var attributes = new List<string>();
                var doc = new XmlDocument();
                doc.Load(xmlFilePath);
                var nodes = doc.GetElementsByTagName(nodeName);

                foreach (XmlNode node in nodes)
                {
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        if (!attributes.Contains(attribute.Name))
                        {
                            attributes.Add(attribute.Name);
                        }
                    }
                }
                return attributes;
            }
        }

        public static class XslTransformer
        {
            public static string Transform(string xmlFilePath, string xslFilePath)
            {
                var xslTransform = new System.Xml.Xsl.XslCompiledTransform();
                xslTransform.Load(xslFilePath);

                using var xmlReader = XmlReader.Create(xmlFilePath);
                using var stringWriter = new StringWriter();
                using var xmlWriter = XmlWriter.Create(stringWriter);

                xslTransform.Transform(xmlReader, xmlWriter);
                return stringWriter.ToString();
            }
        }
        private async void OnFileLoadClicked(object sender, EventArgs e)
        {
            var xmlFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.xml" } },
                { DevicePlatform.Android, new[] { "application/xml" } },
                { DevicePlatform.WinUI, new[] { ".xml" } },
                { DevicePlatform.MacCatalyst, new[] { "public.xml" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Виберіть XML файл",
                FileTypes = xmlFileType
            });

            if (result != null)
            {
                xmlFilePath = result.FullPath;
                filePathLabel.Text = $"Вибрано файл: {Path.GetFileName(xmlFilePath)}";
                UpdateAvailableAttributes();
            }
        }

        private async void OnXslFileLoadClicked(object sender, EventArgs e)
        {
            var xslFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.xml" } },
                { DevicePlatform.Android, new[] { "application/xml" } },
                { DevicePlatform.WinUI, new[] { ".xsl" } },
                { DevicePlatform.MacCatalyst, new[] { "public.xml" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Виберіть XSL файл",
                FileTypes = xslFileType
            });

            if (result != null)
            {
                xslFilePath = result.FullPath;
                xslPathLabel.Text = $"Вибрано файл: {Path.GetFileName(xslFilePath)}";
            }
        }
    }

}
