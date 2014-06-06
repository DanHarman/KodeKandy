// <copyright file="Class1.cs" company="million miles per hour ltd">
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
using System.Linq;
using KodeKandy.Panopticon;
using NUnit.Framework;
using ReactiveUI;

namespace Panopticon.PerformanceTests
{
    internal class TestObservableObject : ObservableObject
    {
        private int age;
        public int Age
        {
            get { return age; }
            set { SetValue(ref age, value); }
        }
    }

    public class TestReactiveUiObject : ReactiveObject
    {
        private int age;
        public int Age
        {
            get { return age; }
            set { this.RaiseAndSetIfChanged(ref age, value); }
        }
    }

    [TestFixture]
    public class Class1
    {
        [Test]
        public void When_PropObs()
        {
            Console.Error.WriteLine("Subscribing to 1 property, notifying it 10000x, and repeating this 100x");
            Benchmark.TimeOperation(5, "[MarkRx]", () =>
            {
                var cnt = 0;

                for (var i = 0; i < 100; ++i)
                {
                    var sut = new TestObservableObject {Age = 10};
                    PropertyObserver.Observe<int>(sut, "Age", v => { ++cnt; });

                    for (var j = 0; j < 10000; ++j)
                        sut.Age = j;
                    ;
                }
                // Console.Error.WriteLine("  Count: {0}", cnt);
            });

            Benchmark.TimeOperation(5, "[DanRx-coop]", () =>
            {
                var cnt = 0;
                for (var i = 0; i < 100; ++i)
                {
                    var sut = new TestObservableObject {Age = 10};
                    //  var zz = sut.WhenPropertyChanged3(x => x.Age).Subscribe(_ => { ++cnt; });
                    var zz = sut.WhenPropertyChanged3("Age").Subscribe(_ => { ++cnt; });

                    for (var j = 0; j < 10000; ++j)
                        sut.Age = j;
                }
                // Console.Error.WriteLine("  Count: {0}", cnt);
            });

            Benchmark.TimeOperation(5, "[DanRx-non-coop]", () =>
            {
                var cnt = 0;
                for (var i = 0; i < 100; ++i)
                {
                    var sut = new TestObservableObject {Age = 10};
                    var zz = sut.WhenPropertyChangedNu<TestObservableObject, int>("Age").Subscribe(_ => { ++cnt; });

                    for (var j = 0; j < 10000; ++j)
                        sut.Age = j;
                }
                // Console.Error.WriteLine("  Count: {0}", cnt);
            });

            Console.Error.WriteLine("Subscribing 10000x to 1 property:");

            Benchmark.TimeOperation(5, "[MarkRx]", () =>
            {
                var cnt = 0;
                var sut = new TestObservableObject {Age = 10};
                for (var i = 0; i < 1000; ++i)
                {
                    PropertyObserver.Observe<int>(sut, "Age", v => { ++cnt; });
                }
            });

            Benchmark.TimeOperation(5, "[DanRx-coop]", () =>
            {
                var cnt = 0;
                var sut = new TestObservableObject {Age = 10};
                for (var i = 0; i < 1000; ++i)
                {
                    sut.WhenPropertyChanged3("Age").Subscribe(_ => { ++cnt; });
                }
            });

            Benchmark.TimeOperation(5, "[DanRx-noncoop]", () =>
            {
                var cnt = 0;
                var sut = new TestObservableObject {Age = 10};
                for (var i = 0; i < 1000; ++i)
                {
                    sut.WhenPropertyChangedNu<TestObservableObject, int>("Age").Subscribe(_ => { ++cnt; });
                }
            });

            Console.Error.WriteLine("Subscribing to 1 property on 10000 obj:");

            Func<TestObservableObject[]> sutFactory =
                () => Enumerable.Repeat(default(object), 10000).Select(_ => new TestObservableObject()).ToArray();

            Benchmark.TimeOperation(5, "[MarkRx]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    PropertyObserver.Observe<int>(sut, "Age", v => { ++cnt; });
                }
            });


            Benchmark.TimeOperation(5, "[DanRx-coop]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    sut.WhenPropertyChanged3("Age").Subscribe(_ => { ++cnt; });
                }
            });

            Benchmark.TimeOperation(5, "[DanRx-noncoop]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    sut.WhenPropertyChangedNu<TestObservableObject, int>("Age").Subscribe(_ => { ++cnt; });
                }
            });

            Console.Error.WriteLine("Disposing of a subscription on each of 10000 obj:");

            Benchmark.TimeOperation(5, "[MarkRx]", () => Enumerable.Repeat(default(object), 10000)
                                                                   .Select(x => new TestObservableObject())
                                                                   .Select(x => PropertyObserver.Observe<int>(x, "Age", v => { }))
                                                                   .ToArray(), suts =>
                                                                   {
                                                                       foreach (var sut in suts)
                                                                       {
                                                                           sut.Dispose();
                                                                       }
                                                                   });


            Benchmark.TimeOperation(5, "[DanRx-coop]", () => Enumerable.Repeat(default(object), 10000)
                                                                       .Select(x => new TestObservableObject())
                                                                       .Select(x => x.WhenPropertyChanged3(p => p.Age).Subscribe(_ => { }))
                                                                       .ToArray(), suts =>
                                                                       {
                                                                           foreach (var sut in suts)
                                                                           {
                                                                               sut.Dispose();
                                                                           }
                                                                       });

            Benchmark.TimeOperation(5, "[DanRx-noncoop]", () => Enumerable.Repeat(default(object), 10000)
                                                                          .Select(x => new TestObservableObject())
                                                                          .Select(x => x.WhenPropertyChangedNu(p => p.Age).Subscribe(_ => { }))
                                                                          .ToArray(), suts =>
                                                                          {
                                                                              foreach (var sut in suts)
                                                                              {
                                                                                  sut.Dispose();
                                                                              }
                                                                          });

            Console.Error.WriteLine("Disposing of 10000 subscriptions to 1 property on 1 object:");

            Benchmark.TimeOperation(5, "[MarkRx]",
                () => Enumerable.Repeat(new TestObservableObject(), 10000).Select(x => PropertyObserver.Observe<int>(x, "Age", v => { })).ToArray(),
                suts =>
                {
                    foreach (var sut in suts)
                    {
                        sut.Dispose();
                    }
                });

            Benchmark.TimeOperation(5, "[DanRx-coop]",
                () =>
                    Enumerable.Repeat(new TestObservableObject(), 10000).Select(x => x.WhenPropertyChanged3(p => p.Age).Subscribe(_ => { })).ToArray(),
                suts =>
                {
                    foreach (var sut in suts)
                    {
                        sut.Dispose();
                    }
                });

            Benchmark.TimeOperation(5, "[DanRx-noncoop]",
                () =>
                    Enumerable.Repeat(new TestObservableObject(), 10000).Select(x => x.WhenPropertyChangedNu<TestObservableObject, int>("Age").Subscribe(_ => { })).ToArray(),
                suts =>
                {
                    foreach (var sut in suts)
                    {
                        sut.Dispose();
                    }
                });


//            Benchmark.TimeOperation(5, "[ReactiveUI] 1 Subcriber 10k notifications", () =>
//            {
//                var cnt = 0;
//                for (var i = 0; i < 100; ++i)
//                {
//                    var sut = new TestReactiveUiObject { Age = 10 };
//                    var zz = sut.WhenAnyValue(x => x.Age).Subscribe(_ => { ++cnt; });
//
//                    for (var j = 0; j < 10000; ++j)
//                        sut.Age = j;
//                }
//            });
        }
    }
}