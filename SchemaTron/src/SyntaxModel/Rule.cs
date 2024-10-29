using System.Collections.Generic;
using Wmhelp.XPath2;


namespace SchemaTron.SyntaxModel
{
    /// <summary>
    /// Represents a rule.
    /// </summary>  
    internal sealed class Rule
    {
        public string Id { get; set; }

        public string Context { get; set; }

        public int CompiledContext { get; set; }

        public IEnumerable<Assert> Asserts { get; set; }
    }
}
