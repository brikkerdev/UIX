using System.Collections.Generic;
using System.IO;
using UIX.Parsing;
using UIX.Parsing.Nodes;

namespace UIX.Editor.Pipeline
{
    /// <summary>
    /// Validates UIX XML and USS files.
    /// </summary>
    public static class UIXValidation
    {
        public class ValidationResult
        {
            public bool Valid;
            public List<ValidationError> Errors = new List<ValidationError>();
            public List<ValidationError> Warnings = new List<ValidationError>();
        }

        public class ValidationError
        {
            public string File;
            public int Line;
            public string Message;
            public string Severity;
        }

        public static ValidationResult ValidateXml(string path, string content)
        {
            var result = new ValidationResult { Valid = true };

            try
            {
                var root = XMLParser.Parse(content, path);
                if (root == null)
                {
                    result.Errors.Add(new ValidationError { File = path, Message = "Failed to parse XML", Severity = "error" });
                    result.Valid = false;
                }
            }
            catch (System.Exception ex)
            {
                result.Errors.Add(new ValidationError { File = path, Message = ex.Message, Severity = "error" });
                result.Valid = false;
            }

            return result;
        }

        public static ValidationResult ValidateUss(string path, string content)
        {
            var result = new ValidationResult { Valid = true };

            try
            {
                var parseResult = USSParser.Parse(content, path);
                foreach (var rule in parseResult.Rules)
                {
                    foreach (var prop in rule.Properties)
                    {
                        if (!Styling.CSSProperties.IsSupported(prop.Key))
                            result.Warnings.Add(new ValidationError { File = path, Line = rule.LineNumber, Message = $"Unknown property: {prop.Key}", Severity = "warning" });
                    }
                }
            }
            catch (System.Exception ex)
            {
                result.Errors.Add(new ValidationError { File = path, Message = ex.Message, Severity = "error" });
                result.Valid = false;
            }

            return result;
        }
    }
}
