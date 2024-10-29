using System;

namespace SchemaTron
{
    internal class QueryBindingException : Exception
    {
        internal QueryBindingException(string message, Exception innerException) : base(message, innerException) { }
    }
}
