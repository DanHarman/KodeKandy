using KodeKandy.Panopticon.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.QueryLanguageTests
{
    [TestFixture]
    public class Given_Observe_For_Poco_Object : ReactiveTest
    {
        //        [Test]
        //        public void When_Subscribe_POCO_Then_OnNext_First_Value_Only()
        //        {
        //            // Arrange
        //            var obj = new TestPoco { Age = 5 };
        //            var scheduler = new TestScheduler();
        //            var observer = scheduler.CreateObserver<int>();
        //            var expected = new[]
        //            {
        //                OnNext(0, 5),
        //                OnNext(15, 3),
        //            };
        //
        //            var sut = Observable.Return(obj).Concat(Observable.Never<TestPoco>()).When("Age", x => x.Age);
        //
        //            // Act
        //            sut.Subscribe(observer);
        //            scheduler.AdvanceTo(15);
        //            obj.Age = 3;
        //
        //            // Assert
        //            Assert.AreEqual(expected, observer.Messages);
        //        }

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