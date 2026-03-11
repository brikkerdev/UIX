using System;
using System.Collections.Generic;
using System.Xml;
using UIX.Parsing.Nodes;

namespace UIX.Parsing
{
    /// <summary>
    /// Parses UIX XML markup into an AST.
    /// </summary>
    public static class XMLParser
    {
        private static readonly HashSet<string> VoidElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "text", "image", "input", "slot", "br", "hr"
        };

        public static RootNode Parse(string xml, string sourcePath = null)
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };
            using var reader = XmlReader.Create(new System.IO.StringReader(xml), settings);
            var doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.Load(reader);

            var root = doc.DocumentElement;
            if (root == null)
                return null;

            return ParseRoot(root, sourcePath);
        }

        private static RootNode ParseRoot(XmlElement root, string sourcePath)
        {
            var name = root.Name;
            var isComponent = string.Equals(name, "component", StringComparison.OrdinalIgnoreCase);
            var isScreen = string.Equals(name, "screen", StringComparison.OrdinalIgnoreCase);

            if (!isComponent && !isScreen)
                throw new XmlException($"Root element must be <component> or <screen>, got <{name}>");

            var rootNode = new RootNode
            {
                Name = root.GetAttribute("name"),
                IsComponent = isComponent,
                ViewModelType = root.GetAttribute("viewmodel"),
                SourcePath = sourcePath
            };

            foreach (XmlNode child in root.ChildNodes)
            {
                if (child is XmlElement elem)
                {
                    if (string.Equals(elem.Name, "props", StringComparison.OrdinalIgnoreCase))
                    {
                        ParseProps(elem, rootNode.Props);
                    }
                    else if (string.Equals(elem.Name, "template", StringComparison.OrdinalIgnoreCase))
                    {
                        ParseTemplateChildren(elem, rootNode.Children);
                    }
                    else
                    {
                        rootNode.Children.Add(ParseElement(elem, sourcePath));
                    }
                }
            }

            return rootNode;
        }

        private static void ParseProps(XmlElement propsElement, List<PropDefinition> props)
        {
            foreach (XmlNode child in propsElement.ChildNodes)
            {
                if (child is XmlElement elem && string.Equals(elem.Name, "prop", StringComparison.OrdinalIgnoreCase))
                {
                    props.Add(new PropDefinition
                    {
                        Name = elem.GetAttribute("name"),
                        Type = elem.GetAttribute("type") ?? "string",
                        Default = elem.GetAttribute("default"),
                        Optional = string.Equals(elem.GetAttribute("optional"), "true", StringComparison.OrdinalIgnoreCase)
                    });
                }
            }
        }

        private static void ParseTemplateChildren(XmlElement templateElement, List<UIXNode> children)
        {
            foreach (XmlNode child in templateElement.ChildNodes)
            {
                var node = ParseNode(child, null);
                if (node != null)
                    children.Add(node);
            }
        }

        private static UIXNode ParseNode(XmlNode xmlNode, string sourcePath)
        {
            if (xmlNode is XmlElement elem)
                return ParseElement(elem, sourcePath);

            if (xmlNode is XmlText text && !string.IsNullOrWhiteSpace(text.Value))
            {
                return new TextNode
                {
                    Content = text.Value.Trim(),
                    SourcePath = sourcePath
                };
            }

            return null;
        }

        private static UIXNode ParseElement(XmlElement elem, string sourcePath)
        {
            var tagName = elem.Name;

            UIXNode CreateNode(UIXNode node)
            {
                node.SourcePath = sourcePath;
                return node;
            }

            if (string.Equals(tagName, "slot", StringComparison.OrdinalIgnoreCase))
            {
                return CreateNode(new SlotNode
                {
                    SlotName = elem.GetAttribute("name")
                });
            }

            if (string.Equals(tagName, "slot-content", StringComparison.OrdinalIgnoreCase))
            {
                var slotContent = new SlotContentNode
                {
                    SlotName = elem.GetAttribute("name")
                };
                foreach (XmlNode child in elem.ChildNodes)
                {
                    var node = ParseNode(child, sourcePath);
                    if (node != null)
                        slotContent.Children.Add(node);
                }
                return CreateNode(slotContent);
            }

            if (string.Equals(tagName, "foreach", StringComparison.OrdinalIgnoreCase))
            {
                var foreachNode = new ForeachNode
                {
                    ItemsExpression = elem.GetAttribute("items")?.Trim(' ', '{', '}'),
                    VarName = elem.GetAttribute("var") ?? "item",
                    IndexName = elem.GetAttribute("index")
                };
                foreach (XmlNode child in elem.ChildNodes)
                {
                    var node = ParseNode(child, sourcePath);
                    if (node != null)
                        foreachNode.Children.Add(node);
                }
                return CreateNode(foreachNode);
            }

            if (IsComponentTag(tagName))
            {
                var componentNode = new ComponentNode
                {
                    ComponentName = tagName
                };
                foreach (XmlAttribute attr in elem.Attributes)
                {
                    componentNode.Props[attr.Name] = attr.Value;
                }
                var defaultSlot = new List<UIXNode>();
                var namedSlots = new Dictionary<string, List<UIXNode>>();

                foreach (XmlNode child in elem.ChildNodes)
                {
                    if (child is XmlElement childElem)
                    {
                        if (string.Equals(childElem.Name, "slot-content", StringComparison.OrdinalIgnoreCase))
                        {
                            var slotName = childElem.GetAttribute("name") ?? "default";
                            var slotChildren = new List<UIXNode>();
                            foreach (XmlNode slotChild in childElem.ChildNodes)
                            {
                                var node = ParseNode(slotChild, sourcePath);
                                if (node != null)
                                    slotChildren.Add(node);
                            }
                            namedSlots[slotName] = slotChildren;
                        }
                        else
                        {
                            var node = ParseElement(childElem, sourcePath);
                            if (node != null)
                                defaultSlot.Add(node);
                        }
                    }
                    else if (child is XmlText text && !string.IsNullOrWhiteSpace(text.Value))
                    {
                        defaultSlot.Add(new TextNode { Content = text.Value.Trim(), SourcePath = sourcePath });
                    }
                }

                componentNode.DefaultSlotContent = defaultSlot;
                foreach (var kv in namedSlots)
                    componentNode.Slots[kv.Key] = kv.Value;

                return CreateNode(componentNode);
            }

            var elementNode = new ElementNode { TagName = tagName.ToLowerInvariant() };

            foreach (XmlAttribute attr in elem.Attributes)
            {
                var key = attr.Name.ToLowerInvariant();
                var value = attr.Value;
                elementNode.Attributes[key] = value;

                if (key == "id") elementNode.Id = value;
                if (key == "class") elementNode.Class = value;
            }

            if (!VoidElements.Contains(elementNode.TagName))
            {
                foreach (XmlNode child in elem.ChildNodes)
                {
                    var node = ParseNode(child, sourcePath);
                    if (node != null)
                        elementNode.Children.Add(node);
                }
            }

            return CreateNode(elementNode);
        }

        private static bool IsComponentTag(string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) return false;
            var c = tagName[0];
            return char.IsUpper(c) || (tagName.Length > 1 && tagName.StartsWith("UIX", StringComparison.OrdinalIgnoreCase));
        }
    }
}
