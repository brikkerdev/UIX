using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UIX.Parsing.Tokens;

namespace UIX.Parsing
{
    /// <summary>
    /// Parses USS (CSS-like) style sheets.
    /// </summary>
    public static class USSParser
    {
        private static readonly Regex VarRegex = new Regex(@"var\s*\(\s*--([a-zA-Z0-9_-]+)\s*(?:,\s*(.+?))?\s*\)", RegexOptions.Compiled);

        public static USSParseResult Parse(string uss, string sourcePath = null)
        {
            var result = new USSParseResult
            {
                SourcePath = sourcePath,
                Rules = new List<StyleRule>(),
                Variables = new List<StyleVariable>()
            };

            var lines = uss.Split('\n');
            var i = 0;
            while (i < lines.Length)
            {
                var line = lines[i];
                var trimmed = line.Trim();
                var lineNum = i + 1;

                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("/*"))
                {
                    if (trimmed.StartsWith("/*"))
                    {
                        while (i < lines.Length && !lines[i].Contains("*/"))
                            i++;
                    }
                    i++;
                    continue;
                }

                if (trimmed.StartsWith(":root"))
                {
                    var block = CollectBlock(lines, ref i);
                    ParseRootVariables(block, result.Variables, lineNum);
                    continue;
                }

                var selectorEnd = trimmed.IndexOf('{');
                if (selectorEnd > 0)
                {
                    var selectorStr = trimmed.Substring(0, selectorEnd).Trim();
                    var block = CollectBlock(lines, ref i);
                    var selector = ParseSelector(selectorStr);
                    var props = ParseProperties(block);
                    if (selector != null && props.Count > 0)
                    {
                        result.Rules.Add(new StyleRule
                        {
                            Selector = selector,
                            Properties = props,
                            LineNumber = lineNum
                        });
                    }
                }

                i++;
            }

            return result;
        }

        public static string ResolveVar(string value, IReadOnlyDictionary<string, string> variables)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return VarRegex.Replace(value, m =>
            {
                var name = "--" + m.Groups[1].Value;
                var fallback = m.Groups[2].Success ? m.Groups[2].Value.Trim() : null;
                return variables.TryGetValue(name, out var v) ? v : (fallback ?? name);
            });
        }

        private static string CollectBlock(string[] lines, ref int i)
        {
            var sb = new System.Text.StringBuilder();
            var depth = 0;
            var started = false;

            while (i < lines.Length)
            {
                var line = lines[i];
                foreach (var c in line)
                {
                    if (c == '{') { depth++; started = true; }
                    else if (c == '}') depth--;
                    sb.Append(c);
                }
                sb.Append('\n');
                i++;
                if (started && depth == 0)
                    break;
            }

            return sb.ToString();
        }

        private static void ParseRootVariables(string block, List<StyleVariable> variables, int startLine)
        {
            var inner = ExtractBlockContent(block);
            if (string.IsNullOrEmpty(inner)) return;

            var declarations = SplitDeclarations(inner);
            foreach (var decl in declarations)
            {
                var colon = decl.IndexOf(':');
                if (colon > 0)
                {
                    var name = decl.Substring(0, colon).Trim();
                    var value = decl.Substring(colon + 1).Trim().TrimEnd(';');
                    if (name.StartsWith("--"))
                    {
                        variables.Add(new StyleVariable { Name = name, Value = value });
                    }
                }
            }
        }

        private static StyleSelector ParseSelector(string selectorStr)
        {
            var selector = new StyleSelector();
            selectorStr = selectorStr.Trim();
            selector.DirectChild = selectorStr.Contains(">");

            var sep = selector.DirectChild ? ">" : " ";
            var parts = selectorStr.Split(new[] { sep }, StringSplitOptions.RemoveEmptyEntries);
            for (var p = 0; p < parts.Length; p++)
                parts[p] = parts[p].Trim();

            var main = parts[parts.Length - 1];
            if (parts.Length > 1)
            {
                selector.Parent = ParseSelector(string.Join(sep, parts, 0, parts.Length - 1));
            }

            var i = 0;
            while (i < main.Length)
            {
                if (main[i] == '.')
                {
                    var start = i + 1;
                    i++;
                    while (i < main.Length && (char.IsLetterOrDigit(main[i]) || main[i] == '-' || main[i] == '_'))
                        i++;
                    selector.Classes.Add(main.Substring(start, i - start));
                }
                else if (main[i] == '#')
                {
                    var start = i + 1;
                    i++;
                    while (i < main.Length && (char.IsLetterOrDigit(main[i]) || main[i] == '-' || main[i] == '_'))
                        i++;
                    selector.Id = main.Substring(start, i - start);
                }
                else if (main[i] == ':')
                {
                    var start = i + 1;
                    i++;
                    while (i < main.Length && (char.IsLetterOrDigit(main[i]) || main[i] == '-' || main[i] == '_'))
                        i++;
                    selector.PseudoClasses.Add(main.Substring(start, i - start));
                }
                else if (char.IsLetter(main[i]) || main[i] == '*')
                {
                    var start = i;
                    while (i < main.Length && (char.IsLetterOrDigit(main[i]) || main[i] == '-' || main[i] == '_'))
                        i++;
                    selector.ElementType = main.Substring(start, i - start).ToLowerInvariant();
                }
                else
                {
                    i++;
                }
            }

            return selector;
        }

        private static Dictionary<string, string> ParseProperties(string block)
        {
            var props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var inner = ExtractBlockContent(block);
            if (string.IsNullOrEmpty(inner)) return props;

            inner = StripBlockComments(inner);
            var declarations = SplitDeclarations(inner);
            foreach (var decl in declarations)
            {
                var colon = decl.IndexOf(':');
                if (colon > 0)
                {
                    var name = decl.Substring(0, colon).Trim();
                    var value = decl.Substring(colon + 1).Trim().TrimEnd(';');
                    if (!string.IsNullOrEmpty(name))
                        props[name] = value;
                }
            }

            return props;
        }

        private static string StripBlockComments(string s)
        {
            var result = "";
            var i = 0;
            while (i < s.Length)
            {
                if (i < s.Length - 1 && s[i] == '/' && s[i + 1] == '*')
                {
                    i += 2;
                    while (i < s.Length - 1 && !(s[i] == '*' && s[i + 1] == '/'))
                        i++;
                    if (i < s.Length - 1) i += 2;
                    continue;
                }
                result += s[i];
                i++;
            }
            return result;
        }

        private static string ExtractBlockContent(string block)
        {
            var start = block.IndexOf('{');
            var end = block.LastIndexOf('}');
            if (start < 0 || end <= start) return null;
            return block.Substring(start + 1, end - start - 1).Trim();
        }

        private static IEnumerable<string> SplitDeclarations(string content)
        {
            var list = new List<string>();
            var current = "";
            var depth = 0;
            foreach (var c in content)
            {
                if (c == '(' || c == '[' || c == '{') depth++;
                else if (c == ')' || c == ']' || c == '}') depth--;
                else if (c == ';' && depth == 0)
                {
                    list.Add(current.Trim());
                    current = "";
                    continue;
                }
                current += c;
            }
            if (!string.IsNullOrWhiteSpace(current))
                list.Add(current.Trim());
            return list;
        }
    }

    public class USSParseResult
    {
        public string SourcePath { get; set; }
        public List<StyleRule> Rules { get; set; }
        public List<StyleVariable> Variables { get; set; }
    }
}
