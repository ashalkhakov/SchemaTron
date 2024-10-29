using SchemaTron.SyntaxModel;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Xunit;

namespace SchemaTron.Test.Unit
{
    public class QueryBindingTest
    {
        private IQueryBinding BindingFromType(QueryBindingType queryBindingType)
        {
            if (queryBindingType == QueryBindingType.XPath2_0)
                return new QueryBindingXPath2();
            return new QueryBindingXPath();
        }

        [Theory]
        [InlineData(QueryBindingType.XPath1_0)]
        [InlineData(QueryBindingType.XPath2_0)]
        internal void CompileSameExpression(QueryBindingType queryBinding)
        {
            var xp = BindingFromType(queryBinding);
            var manager = new XmlNamespaceManager(new NameTable());
            var handle = xp.CompileExpression("1", manager);
            Xunit.Assert.True(handle >= 0);

            var handle2 = xp.CompileExpression("1", manager);
            Xunit.Assert.Equal(handle, handle2);
        }

        [Theory]
        [InlineData(QueryBindingType.XPath1_0)]
        [InlineData(QueryBindingType.XPath2_0)]
        internal void ContextViolatesReturnsTrueIfBooleanIsFalse(QueryBindingType queryBinding)
        {
            var xp = BindingFromType(queryBinding);
            var manager = new XmlNamespaceManager(new NameTable());
            var expr = queryBinding == QueryBindingType.XPath1_0 ? "/root/@a = 1 or /root/@a = 'true'" : "root/@a = true()";
            var handle = xp.CompileExpression(expr, manager);
            var docSources = new[] { @"<root a='1'/>", @"<root a='true'/>", @"<root a='0'/>", @"<root a='false'/>" };

            for (var i = 0; i < docSources.Length; i++)
            {
                var docSrc = docSources[i];
                XDocument xIn = XDocument.Parse(docSrc);

                var result = xp.ContextViolates(handle, xIn.CreateNavigator());
                Xunit.Assert.Equal(i >= 2, result); // only the last two examples violate the constraint
            }
        }

        [Theory]
        [InlineData(QueryBindingType.XPath1_0)]
        [InlineData(QueryBindingType.XPath2_0)]
        internal void ContextViolatesReturnsTrueIfNumberIsNaN(QueryBindingType queryBinding)
        {
            var xp = BindingFromType(queryBinding);
            var manager = new XmlNamespaceManager(new NameTable());
            var handle = xp.CompileExpression("number(/root/@a)", manager);
            var docSources = new[] { @"<root a='1'/>", @"<root a='2.3'/>", @"<root a='NaN'/>", @"<root a='foo'/>" };

            for (var i = 0; i < docSources.Length; i++)
            {
                var docSrc = docSources[i];
                XDocument xIn = XDocument.Parse(docSrc);

                var result = xp.ContextViolates(handle, xIn.CreateNavigator());
                Xunit.Assert.Equal(i >= 2, result); // only the last two examples violate the constraint
            }
        }

        [Theory]
        [InlineData(QueryBindingType.XPath1_0)]
        [InlineData(QueryBindingType.XPath2_0)]
        internal void ContextViolatesReturnsTrueIfNodeSetIsEmpty(QueryBindingType queryBinding)
        {
            var xp = BindingFromType(queryBinding);
            var manager = new XmlNamespaceManager(new NameTable());
            var handle = xp.CompileExpression("//a", manager);
            var docSources = new[] { @"<root></root>", @"<root><a/></root>", @"<root><a/><a/></root>" };

            for (var i = 0; i < docSources.Length; i++)
            {
                var nodeSetLength = i;
                var docSrc = docSources[i];
                XDocument xIn = XDocument.Parse(docSrc);

                var result = xp.ContextViolates(handle, xIn.CreateNavigator());
                Xunit.Assert.Equal(nodeSetLength == 0, result);
            }
        }
    }
}
