using KodeKandy.Panopticon.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.QueryLanguageTests
{
    [TestFixture]
    public class Given_Observe_For_INotifyPropertyChanged_Object : ReactiveTest
    {
        [Test]
        public void When_Subscribe_With_No_Path_Then_OnNext_And_No_Complete()
        {
            // Arrange
            var obj = new TestObservableObject();
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<TestObservableObject>();
            var expected = new[]
            {
                OnNext(0, obj),
            };

            var sut = Opticon.Observe(obj);

            // Act
            sut.Subscribe(observer);

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_One_Node_Path_To_Property_Then_OnNext_And_No_Complete()
        {
            // Arrange
            var obj = new TestObservableObject {Age = 5};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(0, 5),
            };

            var sut = Opticon.Observe(obj, x => x.Age);

            // Act
            sut.Subscribe(observer);

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_Two_Node_Path_To_Property_And_Modify_Node_One_Then_OnNext_And_No_Complete()
        {
            // Arrange
            var obj = new TestObservableObject {Child = new TestObservableObject {Age = 3}};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(0, 3),
                OnNext(10, 5),
            };

            var sut = Opticon.Observe(obj, x => x.Child.Age);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.Child = new TestObservableObject {Age = 5};

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_Two_Node_Path_To_Property_And_Modify_Node_Two_Then_OnNext_Twice_And_No_Complete()
        {
            // Arrange
            var obj = new TestObservableObject {Child = new TestObservableObject {Age = 3}};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(0, 3),
                OnNext(10, 5),
            };

            var sut = Opticon.Observe(obj, x => x.Child.Age);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.Child.Age = 5;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_Twice_With_Two_Node_Path_To_Property_Then_Correct_Notifications_For_Both_Observers()
        {
            // Arrange
            var obj = new TestObservableObject {Child = new TestObservableObject {Age = 3}};
            var scheduler = new TestScheduler();
            var firstObserver = scheduler.CreateObserver<int>();
            var secondObserver = scheduler.CreateObserver<int>();
            var firstObservserExpected = new[]
            {
                OnNext(0, 3),
                OnNext(0, 5),
                OnNext(0, 6),
                OnNext(0, 7),
            };

            var secondObservserExpected = new[]
            {
                OnNext(0, 6),
                OnNext(0, 7),
            };

            var sut = Opticon.Observe(obj, x => x.Child.Age);

            // Act
            sut.Subscribe(firstObserver);
            obj.Child.Age = 5;
            obj.Child.Age = 6;
            sut.Subscribe(secondObserver);
            obj.Child.Age = 7;

            // Assert
            Assert.AreEqual(firstObservserExpected, firstObserver.Messages);
            Assert.AreEqual(secondObservserExpected, secondObserver.Messages);
        }
    }
}