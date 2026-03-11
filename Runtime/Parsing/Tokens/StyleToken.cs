namespace UIX.Parsing.Tokens
{
    /// <summary>
    /// Represents a parsed variable from :root { --name: value; }
    /// </summary>
    public class StyleVariable
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int LineNumber { get; set; }
    }
}
