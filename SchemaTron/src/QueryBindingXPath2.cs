using System.Xml;
using Wmhelp.XPath2.AST;
using Wmhelp.XPath2;
using System.Collections.Generic;
using System;
using System.Xml.XPath;

namespace SchemaTron
{
    internal class QueryBindingXPath2 : IQueryBinding
    {
        internal QueryBindingXPath2()
        {
            _expressions = new();
            _map = new();
            _emptyVariables = new();
        }

        private readonly List<XPath2Expression> _expressions;
        private readonly Dictionary<string, int> _map;
        private readonly Dictionary<XmlQualifiedName, object> _emptyVariables;

        public string Name { get => "XPath 2.0"; }

        public int CompileExpression(string expression, IXmlNamespaceResolver resolver)
        {
            if (_map.TryGetValue(expression, out var index))
            {
                return index;
            }

            try
            {

                var result = XPath2Expression.Compile(expression, resolver);
                Visit(result.ExpressionTree);
                index = _expressions.Count;
                _expressions.Add(result);
                _map.Add(expression, index);

                return index;
            }
            catch (XPath2Exception ex)
            {
                throw new QueryBindingException(ex.Message, ex);
            }
        }

        public bool ContextViolates(int handle, XPathNavigator context)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(handle, 0, nameof(handle));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(handle, _expressions.Count, nameof(handle));

            var expression = _expressions[handle];

            // evaluate test
            object objResult = context.XPath2Evaluate(expression, null);

            // resolve object result
            bool isViolated = false;
            switch (expression.GetResultType(_emptyVariables))
            {
                case XPath2ResultType.Boolean:
                    {
                        isViolated = !Convert.ToBoolean(objResult);
                        break;
                    }
                case XPath2ResultType.Number:
                    {
                        double value = Convert.ToDouble(objResult);
                        isViolated = double.IsNaN(value);
                        break;
                    }
                case XPath2ResultType.NodeSet:
                    {
                        XPath2NodeIterator iterator = (XPath2NodeIterator)objResult;
                        isViolated = (iterator.Count == 0);
                        break;
                    }
                default:
                    throw new InvalidOperationException(String.Format("'{0}'.", expression.Expression));
            }

            return isViolated;
        }

        public IEnumerable<XPathItem> EvaluateToNodeSet(int handle, XPathNavigator navigator)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(handle, 0, nameof(handle));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(handle, _expressions.Count, nameof(handle));

            var expression = _expressions[handle];

            XPath2NodeIterator contextSet = navigator.XPath2Select(expression, null);

            while (contextSet.MoveNext())
            {
                XPathItem current = contextSet.Current;
                yield return current;
            }
        }

        public string EvaluateToFirstStringResult(int handle, XPathNavigator context)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(handle, 0, nameof(handle));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(handle, _expressions.Count, nameof(handle));

            var expression = _expressions[handle];

            object objDiagResult = context.XPath2Evaluate(expression);

            // resolve diag result object
            switch (expression.GetResultType(_emptyVariables))
            {
                case XPath2ResultType.Number:
                case XPath2ResultType.String:
                    {
                        return objDiagResult.ToString();
                    }
                case XPath2ResultType.Boolean:
                    {
                        return objDiagResult.ToString().ToLower();
                    }
                case XPath2ResultType.NodeSet:
                    {
                        XPath2NodeIterator iterator = (XPath2NodeIterator)objDiagResult;
                        if (iterator.Count > 0)
                        {
                            foreach (var x in iterator)
                            {
                                return x.ToString();
                            }
                        }
                        return string.Empty;
                    }
                default:
                    return string.Empty;
            }
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
