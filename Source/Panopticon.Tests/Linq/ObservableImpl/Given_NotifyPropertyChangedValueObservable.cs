using KodeKandy.Panopticon.Linq;
using KodeKandy.Panopticon.Linq.ObservableImpl;
using KodeKandy.Panopticon.Tests.QueryLanguageTests;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.Linq.ObservableImpl
{
    [TestFixture]
    public class Given_NotifyPropertyChangedValueObservable : ReactiveTest
    {
        [Test]
        public void When_Subscribed_Then_Returns_Value_At_Time_Of_Subscribe_And_Subsequent_Values_And_No_Complete()
        {
            // Arrange
            var obj = new TestObservableObject() { Age = 2 };
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(20, 5),
                OnNext(30, 3),
            };

            var sut = new NotifyPropertyChangedValueObservable<TestObservableObject, int>(Opticon.Forever(obj), "Age", x => x.Age);

            scheduler.AdvanceTo(10);
            obj.Age = 5;
            scheduler.AdvanceTo(20);

            // Act
            
            sut.Subscribe(observer);
            scheduler.AdvanceTo(30);
            obj.Age = 3;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }
    }
}