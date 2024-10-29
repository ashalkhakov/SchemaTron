using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace SchemaTron
{
    internal class QueryBindingXPath : IQueryBinding
    {
        internal QueryBindingXPath()
        {
            _expressions = new();
            _map = new();
        }

        public string Name { get => "XPath 1.0"; }

        private readonly List<XPathExpression> _expressions;
        private readonly Dictionary<string, int> _map;

        public int CompileExpression(string expression, IXmlNamespaceResolver resolver)
        {
            if (_map.TryGetValue(expression, out var index))
            {
                return index;
            }

            try
            {

                var result = XPathExpression.Compile(expression, resolver);
                index = _expressions.Count;
                _expressions.Add(result);
                _map.Add(expression, index);

                return index;
            }
            catch (XPathException ex)
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
            object objResult = context.Evaluate(expression);

            // resolve object result
            bool isViolated = false;
            switch (expression.ReturnType)
            {
                case XPathResultType.Boolean:
                    {
                        isViolated = !Convert.ToBoolean(objResult);
                        break;
                    }
                case XPathResultType.Number:
                    {
                        double value = Convert.ToDouble(objResult);
                        isViolated = double.IsNaN(value);
                        break;
                    }
                case XPathResultType.NodeSet:
                    {
                        XPathNodeIterator iterator = (XPathNodeIterator)objResult;
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

            XPathNodeIterator contextSet = navigator.Select(expression);

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

            object objDiagResult = context.Evaluate(expression);

            // resolve diag result object
            switch (expression.ReturnType)
            {
                case XPathResultType.Number:
                case XPathResultType.String:
                    {
                        return objDiagResult.ToString();
                    }
                case XPathResultType.Boolean:
                    {
                        return objDiagResult.ToString().ToLower();
                    }
                case XPathResultType.NodeSet:
                    {
                        XPathNodeIterator iterator = (XPathNodeIterator)objDiagResult;
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
    }
}
