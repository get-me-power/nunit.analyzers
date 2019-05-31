using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Analyzers.SameActualExpectedValue;
using NUnit.Framework;

namespace NUnit.Analyzers.Tests.SameActualExpectedValue
{
    public class SameActualExpectedValueTests
    {
        private static readonly DiagnosticAnalyzer analyzer = new SameActualExpectedValueAnalyzer();

        [TestCase(nameof(Is.EqualTo))]
        [TestCase(nameof(Is.EquivalentTo))]
        public void AnalyzeWhenSameVariableProvided(string constraintMethod)
        {
            var testCode = TestUtility.WrapInTestMethod($@"
                var str = ""test"";
                Assert.That(str, Is.{constraintMethod}(↓str));");

            AnalyzerAssert.Diagnostics(analyzer, testCode);
        }

        [Test]
        public void AnalyzeWhenSameExpressionProvided()
        {
            var testCode = TestUtility.WrapInTestMethod(@"
                var str = ""test"";
                Assert.That(str.Trim(), Is.EqualTo(↓str.Trim()));");

            AnalyzerAssert.Diagnostics(analyzer, testCode);
        }

        [Test]
        public void AnalyzeForTwoCombinedConstraintsViaProperty()
        {
            var testCode = TestUtility.WrapInTestMethod(@"
                var str = ""test"";
                Assert.That(str, Is.EqualTo(↓str).And.EquivalentTo(↓str));");

            AnalyzerAssert.Diagnostics(analyzer, testCode);
        }

        [Test]
        public void AnalyzeForMultipleCombinedConstraintsViaProperty()
        {
            var testCode = TestUtility.WrapInTestMethod(@"
                var str = ""test"";
                var str2 = ""test2"";
                Assert.That(str, Is.EqualTo(↓str)
                    .Or.Contains(str2)
                    .Or.EquivalentTo(↓str)
                    .And.StartsWith(↓str));");

            AnalyzerAssert.Diagnostics(analyzer, testCode);
        }

        [Test]
        public void AnalyzeForTwoCombinedConstraintsViaOperator()
        {
            var testCode = TestUtility.WrapInTestMethod(@"
                var str = ""test"";
                Assert.That(str, Is.EqualTo(↓str) | Is.EquivalentTo(↓str));");

            AnalyzerAssert.Diagnostics(analyzer, testCode);
        }

        [Test]
        public void AnalyzeForMultipleCombinedConstraintsViaOperator()
        {
            var testCode = TestUtility.WrapInTestMethod(@"
                var str = ""test"";
                var str2 = ""test2"";
                Assert.That(str, Is.EqualTo(↓str) 
                    | Is.EquivalentTo(str2)
                    & Does.Contain(↓str)
                    | Does.StartWith(↓str));");

            AnalyzerAssert.Diagnostics(analyzer, testCode);
        }

        [TestCase(nameof(Is.EqualTo))]
        [TestCase(nameof(Is.EquivalentTo))]
        public void ValidWhenDifferentVariablesProvided(string constraintMethod)
        {
            var testCode = TestUtility.WrapInTestMethod($@"
                var str1 = ""test1"";
                var str2 = ""test2"";
                Assert.That(str1, Is.{constraintMethod}(str2));");

            AnalyzerAssert.NoAnalyzerDiagnostics(analyzer, testCode);
        }

        [Test]
        public void ValidWhenDifferentExpressionsProvided()
        {
            var testCode = TestUtility.WrapInTestMethod($@"
                var str = ""test"";
                Assert.That(str.Trim(), Is.EqualTo(↓str.TrimEnd()));");

            AnalyzerAssert.NoAnalyzerDiagnostics(analyzer, testCode);
        }
    }
}