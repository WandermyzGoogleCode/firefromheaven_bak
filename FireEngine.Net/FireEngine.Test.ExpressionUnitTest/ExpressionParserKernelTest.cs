using FireEngine.FireML.Library.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FireEngine.FireML.Library;

namespace FireEngine.Test.ExpressionUnitTest
{
    
    
    /// <summary>
    ///This is a test class for ExpressionParserKernelTest and is intended
    ///to contain all ExpressionParserKernelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExpressionParserKernelTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ExpressionParserKernel Constructor
        ///</summary>
        [TestMethod()]
        public void ExpressionParserKernelConstructorTest()
        {
            CompilerKernel kernel = null; // TODO: Initialize to an appropriate value
            string expr = string.Empty; // TODO: Initialize to an appropriate value
            Location loc = null; // TODO: Initialize to an appropriate value
            ExpressionParserKernel target = new ExpressionParserKernel(kernel, expr, loc);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for MatchToken
        ///</summary>
        [TestMethod()]
        [DeploymentItem("FireEngine.FireML.Library.dll")]
        public void MatchTokenTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ExpressionParserKernel_Accessor target = new ExpressionParserKernel_Accessor(param0); // TODO: Initialize to an appropriate value
            int currentPos = 0; // TODO: Initialize to an appropriate value
            int currentPosExpected = 0; // TODO: Initialize to an appropriate value
            int end = 0; // TODO: Initialize to an appropriate value
            string matched = string.Empty; // TODO: Initialize to an appropriate value
            string matchedExpected = string.Empty; // TODO: Initialize to an appropriate value
            ExpressionParserKernel_Accessor.TokenType expected = null; // TODO: Initialize to an appropriate value
            ExpressionParserKernel_Accessor.TokenType actual;
            actual = target.MatchToken(ref currentPos, end, out matched);
            Assert.AreEqual(currentPosExpected, currentPos);
            Assert.AreEqual(matchedExpected, matched);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
