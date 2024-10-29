﻿
using Wmhelp.XPath2;

namespace SchemaTron.SyntaxModel
{
    /// <summary>
    /// Represents an assertion.
    /// </summary>
    internal sealed class Assert
    {
        public string Id { get; set; }

        public bool IsReport { get; set; }

        public string Test { get; set; }

        public int CompiledTest { get; set; }

        public string Message { get; set; }
       
        public bool[] DiagnosticsIsValueOf { get; set; }

        public string[] Diagnostics { get; set; }

        public int[] CompiledDiagnostics { get; set; }
    }
}
