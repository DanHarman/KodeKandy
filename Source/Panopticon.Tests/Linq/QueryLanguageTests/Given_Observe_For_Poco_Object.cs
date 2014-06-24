using KodeKandy.Panopticon.Linq;
using KodeKandy.Panopticon.Tests.QueryLanguageTests;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.Linq.QueryLanguageTests
{
    [TestFixture]
    public class Given_Observe_For_Poco_Object : ReactiveTest
    {
        [Test]
        public void When_Subscribe_With_One_Node_Path_To_Property_Then_OnNext_And_No_Complete()
        {
            // Arrange
            var obj = new TestPoco() { Age = 5 };
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(0, 5),
            };

            var sut = Opticon.Observe(obj, x => x.Age);

            // Act
            sut.Subscribe(observer);
            obj.Age = 3; // This should not fire through.

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }
    }
}