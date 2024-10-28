using System.Xml;
using System.Xml.Linq;
using Wmhelp.XPath2;
using Wmhelp.XPath2.AST;

namespace SchemaTron
{
    internal class XPath2Helper
    {
        internal static XPath2Expression CompileExpression(string xpath2, IXmlNamespaceResolver resolver)
        {
            var result = XPath2Expression.Compile(xpath2, resolver);
            // TODO: check that it is closed i.e. contains no variables
            Visit(result.ExpressionTree);
            return result;
        }

        private static void Visit(AbstractNode node)
        {
            if (node is VarRefNode v)
            {
                throw new XPath2Exception("XPST0008", $"Qname {v.QNVarName} is not defined");
            }
            else
            {
                for (int i = 0; i < node.Count; i++)
                {
                    Visit(node[i]);
                }
            }
        }
    }
}
