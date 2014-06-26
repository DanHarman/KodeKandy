// <copyright file="Given_ObservingObject.cs" company="million miles per hour ltd">
// Copyright (c) 2013-2014 All Right Reserved
// 
// This source is subject to the MIT License.
// Please see the License.txt file for more information.
// All other rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using KodeKandy.Panopticon;
using KodeKandy.Panopticon.Linq;
using KodeKandy.Panopticon.Linq.ObservableImpl;
using KodeKandy.Panopticon.Tests.QueryLanguageTests;
using NUnit.Framework;

namespace Panopticon.PerformanceTests
{
    [TestFixture]
    public class Given_ObservingObject
    {
        private const int Repeats = 5;

        [Test]
        public void When_Disposing_Of_1_Subscription_On_x_Objects()
        {
            Console.Error.WriteLine("Disposing of a subscription on each of 10000 obj:");

            Func<IEnumerable<TestObservableObject>> factory =
                () => Enumerable.Repeat(default(object), 10000).Select(x => new TestObservableObject());

            Benchmark.TimeOperation(Repeats, "[MarkRx2]", () => factory()
                .Select(sut => new Observer<TestObservableObject, TestObservableObject>(sut, x => x.Child, "Child",
                    _ => { }))
                .ToArray(), suts =>
                {
                    foreach (var sut in suts)
                    {
                        sut.Dispose();
                    }
                });


            Benchmark.TimeOperation(Repeats, "[DanRx2]", () => factory()
                .Select(sut => sut.When("Age", x => x.Age).Subscribe(_ => { }))
                .ToArray(), suts =>
                {
                    foreach (var sut in suts)
                    {
                        sut.Dispose();
                    }
                });
        }

        [Test]
        public void When_Notifying_Child_Property_1000000_Times()
        {
            Console.Error.WriteLine("Subscribing to 1 child property, notifying it 1000000x");
            Func<TestObservableObject> factory = () => new TestObservableObject {Age = 10, Child = new TestObservableObject {Age = 20}};
            const int iterations = 1000000;

            Benchmark.TimeOperation(Repeats, "[MarkRx2]", () =>
            {
                var cnt = 0;

                var sut = factory();
                new Observer<TestObservableObject, TestObservableObject>(sut, x => x.Child, "Child")
                    .Chain(x => x.Age, "Age", v => { ++cnt; });

                for (var j = 0; j < iterations; ++j)
                    sut.Child.Age = j;

                Assert.AreEqual(iterations + 1, cnt);
            });

            Benchmark.TimeOperation(5, "[DanRx2]", () =>
            {
                var cnt = 0;

                var sut = factory();

                sut.When("Child", x => x.Child).When("Age", x => x.Age).Subscribe(_ => { ++cnt; });

                for (var j = 0; j < iterations; ++j)
                    sut.Child.Age = j;

                Assert.AreEqual(iterations + 1, cnt);
            });

            Benchmark.TimeOperation(5, "[DanRx2/PropertyValueChanged]", () =>
            {
                var cnt = 0;

                var sut = factory();

                (new NotifyPropertyChangedValueObservable2<TestObservableObject, int>(sut.When("Child", x => x.Child), "Age", x => x.Age)).Select(x => x.Value).Subscribe(_ => { ++cnt; });

                for (var j = 0; j < iterations; ++j)
                    sut.Child.Age = j;

                Assert.AreEqual(iterations + 1, cnt);
            });
        }

        [Test]
        public void When_Subscribing_To_1_Child_Property_On_10000_Objects()
        {
            Console.Error.WriteLine("Subscribing to 1 child property on 10000 obj:");
            var iter = 10000;

            Func<TestObservableObject[]> sutFactory =
                () =>
                    Enumerable.Repeat(default(object), iter).Select(
                        _ => new TestObservableObject {Age = 10, Child = new TestObservableObject {Age = 20}}).ToArray();


            Benchmark.TimeOperation(Repeats, "[MarkRx2]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    new Observer<TestObservableObject, TestObservableObject>(sut, x => x.Child, "Child")
                        .Chain(x => x.Age, "Age", v => { ++cnt; });
                }

                Assert.AreEqual(iter, cnt);
            });

            Benchmark.TimeOperation(Repeats, "[DanRx2]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    sut.When("Child", x => x.Child).When("Age", x => x.Age).Subscribe(_ => { ++cnt; });
                }
                Assert.AreEqual(iter, cnt);
            });

            Expression<Func<TestObservableObject, int>> cached = x => x.Child.Age;

            Benchmark.TimeOperation(Repeats, "[DanRx2-Expression]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    //Opticon.When(sut, x => x.Child.Age).Subscribe(_ => { ++cnt; });
                    sut.When(cached).Subscribe(_ => { ++cnt; });
                }
                Assert.AreEqual(iter, cnt);
            });
        }

        [Test]
        public void When_Subscribing_To_1_Property_On_10000_Objects()
        {
            Console.Error.WriteLine("Subscribing to 1 property on 10000 obj:");
            const int iter = 10000;

            Func<TestObservableObject[]> sutFactory =
                () =>
                    Enumerable.Repeat(default(object), iter).Select(
                        _ => new TestObservableObject {Age = 10}).ToArray();


            Benchmark.TimeOperation(Repeats, "[MarkRx2]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    new Observer<TestObservableObject, int>(sut, x => x.Age, "Age", v => { ++cnt; });
                }

                Assert.AreEqual(iter, cnt);
            });

            Benchmark.TimeOperation(Repeats, "[DanRx2]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    sut.When("Age", x => x.Age).Subscribe(_ => { ++cnt; });
                }
                Assert.AreEqual(iter, cnt);
            });

            //Expression<Func<TestObservableObject, int>> cached = x => x.Child.Age§;

            Benchmark.TimeOperation(Repeats, "[DanRx2-Expression]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    sut.When(x => x.Age).Subscribe(_ => { ++cnt; });
                    //Opticon.When(sut, cached).Subscribe(_ => { ++cnt; });
                }
                Assert.AreEqual(iter, cnt);
            });
        }

        [Test]
        public void When_Subscribing_To_1_Property_On_1_Object_10000_Times()
        {
            Console.Error.WriteLine("Subscribing to 1 property 1000x on 1 obj:");
            const int iterations = 10000;

            Func<TestObservableObject> sutFactory = () => new TestObservableObject {Age = 10};


            Benchmark.TimeOperation(Repeats, "[MarkRx2]", sutFactory, sut =>
            {
                var cnt = 0;

                for (var i = 0; i < iterations; ++i)
                {
                    new Observer<TestObservableObject, int>(sut, x => x.Age, "Age", v => { ++cnt; });
                }

                Assert.AreEqual(iterations, cnt);
            });

            // MarkRx is faster here as it caches the observables where as we are creating event handler for each item.
            // However we could just subscribe 1000x to same observable as per test below.
            Benchmark.TimeOperation(Repeats, "[DanRx2]", sutFactory, sut =>
            {
                var cnt = 0;

                for (var i = 0; i < iterations; ++i)
                {
                    sut.When("Age", x => x.Age).Subscribe(_ => { ++cnt; });
                }
                Assert.AreEqual(iterations, cnt);
            });

            // WHY Is this slower than the previous test???
            Benchmark.TimeOperation(Repeats, "[DanRx2-Reuse observable]", sutFactory, sut =>
            {
                var cnt = 0;
                var obs = sut.When("Age", x => x.Age);

                for (var i = 0; i < iterations; ++i)
                {
                    obs.Subscribe(_ => { ++cnt; });
                }
                Assert.AreEqual(iterations, cnt);
            });

            //Expression<Func<TestObservableObject, int>> cached = x => x.Child.Age§;

            Benchmark.TimeOperation(Repeats, "[DanRx2-Expression]", sutFactory, sut =>
            {
                var cnt = 0;

                for (var i = 0; i < iterations; ++i)
                {
                    sut.When(x => x.Age).Subscribe(_ => { ++cnt; });
                    //Opticon.When(sut, cached).Subscribe(_ => { ++cnt; });
                }
                Assert.AreEqual(iterations, cnt);
            });
        }
    }
}