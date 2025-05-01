using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.XPath;

namespace X4SectorCreator.Helpers
{
    public class XmlPatcher
    {
        private readonly XDocument _targetDoc;

        public XmlPatcher(string targetFilePath)
        {
            _targetDoc = XDocument.Load(targetFilePath);
        }

        public XmlPatcher(XDocument targetDoc)
        {
            _targetDoc = targetDoc;
        }

        public void ApplyPatch(string patchFilePath)
        {
            var patchDoc = XDocument.Load(patchFilePath);
            ApplyPatch(patchDoc);
        }

        public void ApplyPatch(XDocument patchDoc)
        {
            foreach (var patch in patchDoc.Root.Elements())
            {
                if (!patchDoc.Root.Name.LocalName.Equals("diff", StringComparison.OrdinalIgnoreCase))
                {
                    // Assume this is an add (no diff)
                    ApplyAdd(_targetDoc.Root, patch);
                    continue;
                }

                var selAttr = patch.Attribute("sel") ?? 
                    throw new Exception($"Patch element <{patch.Name}> is missing 'sel' attribute.");

                string xpath = selAttr.Value;
                switch (patch.Name.LocalName.ToLower())
                {
                    case "add":
                        ApplyAdd(xpath, patch);
                        break;
                    case "remove":
                        ApplyRemove(xpath);
                        break;
                    case "replace":
                        ApplyReplace(xpath, patch);
                        break;
                    default:
                        throw new Exception($"Unknown patch operation: {patch.Name.LocalName}");
                }
            }
        }

        private static void ApplyAdd(XElement parent, XElement addElement)
        {
            parent.Add(new XElement(addElement));
        }

        private void ApplyAdd(string xpath, XElement addElement)
        {
            var parent = _targetDoc.XPathSelectElement(xpath);
            if (parent == null)
            {
                Debug.WriteLine($"Add operation failed: Could not find node for XPath: {xpath}");
                return;
            }

            foreach (var child in addElement.Elements())
            {
                parent.Add(new XElement(child));
            }
        }

        private void ApplyRemove(string xpath)
        {
            var nodeToRemove = FindNode(xpath);
            if (nodeToRemove == null)
            {
                Debug.WriteLine($"Remove operation failed: Could not find node for XPath: {xpath}");
                return;
            }

            if (nodeToRemove is XElement element)
            {
                element.Remove();
            }
            else if (nodeToRemove is XAttribute attribute)
            {
                attribute.Remove();
            }
        }

        private void ApplyReplace(string xpath, XElement replaceElement)
        {
            var nodeToReplace = FindNode(xpath);
            if (nodeToReplace == null)
            {
                Debug.WriteLine($"Replace operation failed: Could not find node for XPath: {xpath}");
                return;
            }

            if (nodeToReplace is XElement element)
            {
                // Assume the first child inside <replace> is the replacement
                var newContent = replaceElement.Elements().ToArray();
                if (newContent.Length > 1)
                {
                    element.ReplaceWith(newContent.Select(a => new XElement(a)).ToArray());
                }
                else if (newContent.Length == 1)
                {
                    element.ReplaceWith(new XElement(newContent[0]));
                }
                else if (newContent.Length == 0)
                {
                    throw new Exception($"Replace operation failed: No content inside <replace>.");
                }
            }
            else if (nodeToReplace is XAttribute attribute)
            {
                attribute.Value = replaceElement.Value;
            }
        }

        private object FindNode(string xpath)
        {
            var result = _targetDoc.XPathEvaluate(xpath);
            if (result is IEnumerable<object> nodes)
            {
                return nodes.Cast<object>().FirstOrDefault();
            }
            return result;
        }

        public void Save(string outputPath)
        {
            var dir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            _targetDoc.Save(outputPath);
        }

        public XDocument GetResult()
        {
            return _targetDoc;
        }
    }
}
