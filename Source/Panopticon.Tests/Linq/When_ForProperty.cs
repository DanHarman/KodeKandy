using KodeKandy.Panopticon.Linq;
using KodeKandy.Panopticon.Tests.TestEntities;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.Linq
{
    [TestFixture]
    public class When_ForProperty : ReactiveTest
    {
        [Test]
        public void When_Subscribed_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new TestObservableObject { ObservableChild = new TestObservableObject { Age = 3 } };
            var replacementChild = new TestObservableObject { Age = 5 };
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<TestObservableObject>();
            var expected = new[]
            {
                OnNext(000, obj.ObservableChild),
                OnNext(010, replacementChild),
                OnNext(010, replacementChild),
                OnNext(020, replacementChild),
            };

            var sut = obj.WhenAny(x => x.ObservableChild).ForProperty<TestObservableObject>("Age", "Name");

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.ObservableChild = replacementChild;
            obj.ObservableChild.Age = 17;
            scheduler.AdvanceTo(20);
            obj.ObservableChild.Name = "Billy";

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }
    }
}