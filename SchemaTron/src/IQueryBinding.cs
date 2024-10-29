using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace SchemaTron
{
    internal interface IQueryBinding
    {
        string Name { get; }
        int CompileExpression(string expression, IXmlNamespaceResolver resolver);
        bool ContextViolates(int handle, XPathNavigator navigator);
        IEnumerable<XPathItem> EvaluateToNodeSet(int handle, XPathNavigator navigator);
        string EvaluateToFirstStringResult(int handle, XPathNavigator context);
    }
}