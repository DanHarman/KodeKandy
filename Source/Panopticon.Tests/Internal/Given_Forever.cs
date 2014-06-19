using KodeKandy.Panopticon.Linq.ObservableImpl;
using KodeKandy.QualityTools;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.Internal
{
    [TestFixture]
    public class Given_Forever : ReactiveTest
    {
        [Test]
        public void When_Subscribed_To_Then_Returns_Value_And_Does_Not_Complete()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new Forever<int>(100);
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(0, 100),
            };
            sut.Subscribe(observer);
            
            // Act
            scheduler.Start();
            
            // Assert
            KKAssert.AreEqualByValue(expected, observer.Messages);
        }
    }
}