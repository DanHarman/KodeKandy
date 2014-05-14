using System.ComponentModel;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.ObservableObjectTests
{
    [TestFixture]
    public class Given_PropertyChangeSubject : ReactiveTest
    {
        [Test]
        public void When_Disposed_Then_OnCompleted()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new PropertyChangeSubject(this);
            var observer = scheduler.CreateObserver<IPropertyChange>();
            var expected = new[]
            {
                OnCompleted<IPropertyChange>(90)
            };
            sut.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(90);
            sut.Dispose();

            // Assert
            observer.Messages.AssertEqual(expected);
        }

        [Test]
        public void When_SetPropertyValue_Then_Fires_PropertyChange_With_All_Values()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new PropertyChangeSubject(this);
            var observer = scheduler.CreateObserver<IPropertyChange>();
            var expected = new[]
            {
                OnNext(10, PropertyChange.Create(this, 17, "Age", "UserData-1")),
            };
            int age = 10;
            sut.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(10);
            sut.SetPropertyValue(ref age, 17, "Age", "UserData-1");
            scheduler.AdvanceTo(90);

            // Assert
            observer.Messages.AssertEqual(expected);
        }

//        [Test]
//        public void When_SetPropertyValue_Then_Fires_NotifyPropertyChanged_With_All_Values()
//        {
//            // Arrange
//            event PropertyChangedEventHandler PropertyChanged;
//            var sut = new PropertyChangeSubject(this, () => propertyChangeSubject);
//            var observer = scheduler.CreateObserver<IPropertyChange>();
//            var expected = new[]
//            {
//                OnNext(10, PropertyChange.Create(this, 17, "Age", "UserData-1")),
//            };
//            int age = 10;
//            sut.Subscribe(observer);
//
//            // Act
//            scheduler.Start();
//            scheduler.AdvanceTo(10);
//            sut.SetPropertyValue(ref age, 17, "Age", "UserData-1");
//            scheduler.AdvanceTo(90);
//
//            // Assert
//            observer.Messages.AssertEqual(expected);
//        }

        [Test]
        public void When_NotifyPropertyValueChanged_Then_Fires_PropertyChange_With_All_Values()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new PropertyChangeSubject(this);
            var observer = scheduler.CreateObserver<IPropertyChange>();
            var expected = new[]
            {
                OnNext(30, PropertyChange.Create(this, 70, "Age", "UserData-2"))
            };
            sut.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(30);
            sut.NotifyPropertyValueChanged(70, "Age", "UserData-2");
            scheduler.AdvanceTo(90);

            // Assert
            observer.Messages.AssertEqual(expected);
        }
    }
}