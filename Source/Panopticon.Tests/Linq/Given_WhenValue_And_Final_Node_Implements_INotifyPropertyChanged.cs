using KodeKandy.Panopticon.Linq;
using KodeKandy.Panopticon.Tests.TestEntities;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.Linq
{
    [TestFixture]
    public class Given_WhenValue_And_Final_Node_Implements_INotifyPropertyChanged : ReactiveTest
    {
        [Test]
        public void When_Subscribe_With_One_Node_Path_To_Property_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new TestObservableObject { Age = 5 };
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(000, 5),
            };

            var sut = obj.WhenValue(x => x.Age);

            // Act
            sut.Subscribe(observer);

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_Two_Node_Path_To_Property_And_Modify_Node_One_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new TestObservableObject { ObservableChild = new TestObservableObject { Age = 3 } };
            var replacementChild = new TestObservableObject { Age = 5 };
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(000, 3),
                OnNext(010, replacementChild.Age),
            };

            var sut = obj.WhenValue(x => x.ObservableChild.Age);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.ObservableChild = replacementChild;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_Two_Node_Path_To_Property_And_Modify_Property_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new TestObservableObject { ObservableChild = new TestObservableObject { Age = 3 } };
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(000, 3),
                OnNext(010, 5),
            };

            var sut = obj.WhenValue(x => x.ObservableChild.Age);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.ObservableChild.Age = 5;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Path_Has_Null_Intermediary_Node_Then_Skips_When_Invalid_Path()
        {
            // Arrange
            var childOne = new TestObservableObject { Age = 5 };
            var childTwo = new TestObservableObject { Age = 17 };
            var obj = new TestObservableObject { ObservableChild = childOne };
            var scheduler = new TestScheduler();

            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(000, childOne.Age),
                OnNext(000, 20),
                OnNext(300, childTwo.Age),
            };

            var sut = obj.WhenValue(x => x.ObservableChild.Age);

            // Act
            sut.Subscribe(observer);
            obj.ObservableChild.Age = 20;
            scheduler.AdvanceTo(100);
            obj.ObservableChild = null;
            scheduler.AdvanceTo(300);
            obj.ObservableChild = childTwo;

            // Assert
            observer.Messages.AssertEqual(expected);
        }
    }
}