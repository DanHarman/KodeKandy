using System;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KodeKandy.QualityTools
{
    public static class KKAssert
    {
        /// <summary>
        ///     Verifies that the exception type and message are as expected.
        /// </summary>
        /// <param name="throwsAction">The action that is expected to throw.</param>
        /// <param name="expectedExceptonType">The expected exception type (optional).</param>
        /// <param name="expectedMessage">The expected message (optional).</param>
        public static void Throws(Action throwsAction, Type expectedExceptonType = null, string expectedMessage = null)
        {
            try
            {
                throwsAction();
            }
            catch (Exception ex)
            {
                if (expectedExceptonType != null && ex.GetType() != expectedExceptonType)
                    Assert.Fail("An exception of type \r\n'{0}'\r\n was thrown, but an exception of type \r\n'{1}'\r\n was expected.", ex.GetType().Name,
                        expectedExceptonType.Name);

                if (expectedMessage != null)
                    Assert.AreEqual(expectedMessage, ex.Message, "An message of \r\n'{0}'\r\n was on the thrown exception, but \r\n'{1}'\r\n was expected.", ex.Message, expectedMessage);

                return;
            }
            Assert.Fail("Expected exception not throw.");
        }

        /// <summary>
        ///     Verifies that the exception type and message are as expected.
        /// </summary>
        /// <typeparam name="TExceptionType">The expected exception type.</typeparam>
        /// <param name="throwsAction">The action that is expected to throw.</param>
        /// <param name="expectedMessage">The expected message (optional).</param>
        public static void Throws<TExceptionType>(Action throwsAction, string expectedMessage = null)
        {
            Throws(throwsAction, typeof(TExceptionType), expectedMessage);
        }

        /// <summary>
        ///     Assert that two objects are identical by value comparison.
        /// </summary>
        /// <remarks>Serializes to JSON to perform the comparison.</remarks>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void AreEqualByValue(object expected, object actual)
        {
            Assert.AreEqual(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }
    }
}