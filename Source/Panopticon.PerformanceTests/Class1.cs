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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using KodeKandy;
using KodeKandy.Panopticon;
using KodeKandy.Panopticon.Linq;
using KodeKandy.Panopticon.Linq.ObservableImpl;
using KodeKandy.Panopticon.Properties;
using NUnit.Framework;
using ReactiveUI;

namespace Panopticon.PerformanceTests
{

    public class ObservableObjectL : INotifyPropertyChanged
    {
        private readonly PropertyChangeHelper propertyChangeHelper;

        public ObservableObjectL()
        {
            propertyChangeHelper = new PropertyChangeHelper(this);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChangeHelper.PropertyChanged += value; }
            remove { propertyChangeHelper.PropertyChanged -= value; }
        }

        
        [NotifyPropertyChangedInvocator("propertyName")]
        public void SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            propertyChangeHelper.SetPropertyValue(ref property, value, propertyName);
        }

        [NotifyPropertyChangedInvocator("propertyName")]
        public void SetValue<TVal>(ref TVal property, TVal value, object userData, [CallerMemberName] string propertyName = null)
        {
            propertyChangeHelper.SetPropertyValue(ref property, value, propertyName, userData);
        }
    }

    internal class TestObservableObject : ObservableObject
    {
        private int age;
        private TestObservableObject child;
        public int Age
        {
            get { return age; }
            set { SetValue(ref age, value); }
        }

        public TestObservableObject Child
        {
            get { return child; }
            set { SetValue(ref child, value); }
        }
    }


    internal class TestObservableObjectL : ObservableObjectL
    {
        private int age;
        private TestObservableObjectL child;
        public int Age
        {
            get { return age; }
            set { SetValue(ref age, value); }
        }

        public TestObservableObjectL Child
        {
            get { return child; }
            set { SetValue(ref child, value); }
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
        public void When_Nested_Perf()
        {
            Console.Error.WriteLine("Subscribing 10000x to 1 property:");

            Benchmark.TimeOperation(5, "[DanRx-coop]", () =>
            {
                var cnt = 0;
                var sut = new TestObservableObject {Age = 10, Child = new TestObservableObject {Age = 20}};
                for (var i = 0; i < 1000; ++i)
                {
                    sut.WhenPropertyChangedChain(x => x.Child).Subscribe(_ => { ++cnt; });
                }
            });

            Benchmark.TimeOperation(5, "[MarkRx2]", () =>
            {
                var cnt = 0;

                var sut = new TestObservableObject {Age = 10, Child = new TestObservableObject {Age = 20}};
                for (var i = 0; i < 1000; ++i)
                {
                    new Observer<TestObservableObject, TestObservableObject>(sut, "Child").Chain<TestObservableObject, TestObservableObject, int>(
                        "Age", v => { ++cnt; });
                    //  PropertyObserver.Observe<int>(sut, "Child.Age", v => { ++cnt; });
                }
            });

            Console.Error.WriteLine("Subscribing to 1 child property on 10000 obj:");

            Func<TestObservableObject[]> sutFactory =
                () =>
                    Enumerable.Repeat(default(object), 10000).Select(
                        _ => new TestObservableObject {Age = 10, Child = new TestObservableObject {Age = 20}}).ToArray();


            Benchmark.TimeOperation(5, "[MarkRx]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    PropertyObserver.Observe<int>(sut, "Child.Age", v => { ++cnt; });
                }
            });

            Benchmark.TimeOperation(5, "[MarkRx2]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    // new Observer<TestObservableObject, TestObservableObject>(sut, "Child").Chain<TestObservableObject, TestObservableObject, int>("Age", v => { ++cnt; });
                    new Observer<TestObservableObject, TestObservableObject>(sut, x => x.Child, "Child")
                        .Chain<TestObservableObject, TestObservableObject, int>(x => x.Age, "Age", v => { ++cnt; });
                    //  PropertyObserver.Observe<int>(sut, "Child.Age", v => { ++cnt; });
                }

                Assert.AreEqual(10000, cnt);
            });

            var memberInfos = ExpressionHelpers.GetMemberInfos<TestObservableObject, int>(x => x.Child.Age);

            Benchmark.TimeOperation(5, "[DanRx]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    //   sut.WhenPropertyChangedChain<TestObservableObject, int>(memberInfos).Subscribe(_ => { ++cnt; });
                    sut.WhenPropertyChangedChain(x => x.Child.Age).Subscribe(_ => { ++cnt; });
                }
            });
        }

        [Test]
        public void When_Nested_Perf_Comparison()
        {
            Console.Error.WriteLine("Subscribing 1000x to 1 property:");

            Benchmark.TimeOperation(5, "[DanRx-chain]", () =>
            {
                var cnt = 0;
                var sut = new TestObservableObject {Age = 10}; //, Child = new TestObservableObject {Age = 20}};
                for (var i = 0; i < 1000; ++i)
                {
                    sut.WhenPropertyChangedChain(x => x.Age).Subscribe(_ => { ++cnt; });
                }

                Assert.AreEqual(1000, cnt);
            });
        }


        [Test]
        public void When_Nested_Perf_Comparison_Fast()
        {
            Console.Error.WriteLine("Subscribing 10000x to 1 property:");

            Benchmark.TimeOperation(5, "[DanRx-noncoop]", () =>
            {
                var cnt = 0;
                var sut = new TestObservableObject {Age = 10};
                for (var i = 0; i < 1000; ++i)
                {
                    sut.WhenPropertyChangedNu<TestObservableObject, int>("Age").Subscribe(_ => { ++cnt; });
                }
                Assert.AreEqual(1000, cnt);
            });
        }

        class FastReturn<T> : IObservable<T>
        {
            private T val;
            public FastReturn(T value)
            {
                val = value;
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                observer.OnNext(val);
                return Disposable.Empty;
            }
        }


        [Test]
        public void When_Nested_Perf_Comparison_Extreme()
        {
            Console.Error.WriteLine("Subscribing 10000x to 1 nested property:");
            var iter = 10000;
            var outter = 5;
            Benchmark.TimeOperation(outter, "[MarkRx2]", () =>
            {
                var cnt = 0;

                var sut = new TestObservableObjectL { Age = 10, Child = new TestObservableObjectL { Age = 20 } };
                for (var i = 0; i < iter; ++i)
                {
                    new Observer<TestObservableObjectL, TestObservableObjectL>(sut, x => x.Child, "Child")
                        .Chain<TestObservableObjectL, TestObservableObjectL, int>(x => x.Age, "Age", v => { ++cnt; });
                }
                Assert.AreEqual(iter, cnt);
            });

            Benchmark.TimeOperation(outter, "[DanRx2]", () =>
            {
                var cnt = 0;
                var sut = new TestObservableObjectL { Age = 10, Child = new TestObservableObjectL { Age = 20 } };

                for (var i = 0; i < iter; ++i)
                {
                    Opticon.Observe(sut).When("Child", x => x.Child).When("Age", x => x.Age).Subscribe(_ => { ++cnt; });
                }
                Assert.AreEqual(iter, cnt);
            });

            Benchmark.TimeOperation(outter, "[DanRx2-slacker]", () =>
            {
                var cnt = 0;
                var sut = new TestObservableObjectL { Age = 10, Child = new TestObservableObjectL { Age = 20 } };

                for (var i = 0; i < iter; ++i)
                {
                    Opticon.Observe(sut, x => x.Child.Age).Subscribe(_ => { ++cnt; });
                }
                Assert.AreEqual(iter, cnt);
            });
        }

        [Test]
        public void When_Nested_Perf_Comparison_Extreme_Lotsa_Obj()
        {
            Console.Error.WriteLine("Subscribing to 1 child property on 10000 obj:");

            Func<TestObservableObject[]> sutFactory =
                () =>
                    Enumerable.Repeat(default(object), 10000).Select(
                        _ => new TestObservableObject { Age = 10, Child = new TestObservableObject { Age = 20 } }).ToArray();


            Benchmark.TimeOperation(5, "[MarkRx2]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    new Observer<TestObservableObject, TestObservableObject>(sut, x => x.Child, "Child")
                        .Chain<TestObservableObject, TestObservableObject, int>(x => x.Age, "Age", v => { ++cnt; });
                }

                Assert.AreEqual(10000, cnt);
            });

            Benchmark.TimeOperation(5, "[DanRx2]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    Opticon.Observe(sut).When("Child", x => x.Child).When("Age", x => x.Age).Subscribe(_ => { ++cnt; });
                }
                Assert.AreEqual(10000, cnt);
            });

            Benchmark.TimeOperation(5, "[DanRx2-slack]", sutFactory, suts =>
            {
                var cnt = 0;

                foreach (var sut in suts)
                {
                    Opticon.Observe(sut, x => x.Child.Age).Subscribe(_ => { ++cnt; });
                }
                Assert.AreEqual(10000, cnt);
            });
        }

        [Test]
        public void When_PropChain()
        {
            // Arrange
            int res = 0;
            var sut = new TestObservableObject {Age = 10, Child = new TestObservableObject {Age = 20}};

            // Act
            PropertyObservableFactory.Create(sut, x => x.Child.Age).Subscribe(x => res = x);
            //sut.WhenPropertyChangedChain(x => x.Child.Age).Subscribe(x => res = x);

            // Assert
            Assert.AreEqual(20, res);
        }

        [Test]
        public void When_PropChain_Leaf_Updates()
        {
            // Arrange
            var res = new List<int>();
            var sut = new TestObservableObject {Age = 10, Child = new TestObservableObject {Age = 20}};

            // Act
            PropertyObservableFactory.Create(sut, x => x.Child.Age).Subscribe(res.Add);
            //sut.WhenPropertyChangedChain(x => x.Child.Age).Subscribe(res.Add);
            sut.Child.Age = 30;
            sut.Child.Age = 70;

            // Assert
            Assert.AreEqual(new object[] {20, 30, 70}, res);
        }

        [Test]
        public void When_PropChain_Node_Updates()
        {
            // Arrange
            var res = new List<int>();
            var sut = new TestObservableObject {Age = 10, Child = new TestObservableObject {Age = 20}};
            var updates = Enumerable.Range(1, 5).Select(count => new TestObservableObject {Age = count});

            // Act
            PropertyObservableFactory.Create(sut, x => x.Child.Age).Subscribe(res.Add);

//            sut.WhenPropertyChangedChain(x => x.Child.Age).Subscribe(res.Add);
            foreach (var update in updates)
                sut.Child = update;

            // Assert
            Assert.AreEqual(new object[] {20, 1, 2, 3, 4, 5}, res);
        }

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

            Benchmark.TimeOperation(5, "[MarkRx2]", () =>
            {
                var cnt = 0;

                for (var i = 0; i < 100; ++i)
                {
                    var sut = new TestObservableObject { Age = 10 };
                    new Observer<TestObservableObject, int>(sut, x => x.Age, "Age", v => { ++cnt; });
                      //.Chain<TestObservableObject, TestObservableObject, int>(x => x.Age, "Age", v => { ++cnt; });

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

            Benchmark.TimeOperation(5, "[DanRx2-Explicit]", () =>
            {
                var cnt = 0;
                for (var i = 0; i < 100; ++i)
                {
                    var sut = new TestObservableObject { Age = 10 };
                    (new FastReturn<TestObservableObject>(sut)).When("Age", x => x.Age).Subscribe(_ => { ++cnt; });

                    for (var j = 0; j < 10000; ++j)
                        sut.Age = j;
                }
                // Console.Error.WriteLine("  Count: {0}", cnt);
            });

            Benchmark.TimeOperation(5, "[DanRx2-Slack]", () =>
            {
                var cnt = 0;
                for (var i = 0; i < 100; ++i)
                {
                    var sut = new TestObservableObject { Age = 10 };
                    PropertyObservableFactory.Create(sut, x => x.Age).Subscribe(_ => { ++cnt; });

                    for (var j = 0; j < 10000; ++j)
                        sut.Age = j;
                }
                // Console.Error.WriteLine("  Count: {0}", cnt);
            });

            Console.Error.WriteLine("Subscribing 1000x to 1 property:");

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
                                                                          .Select(x => x.WhenPropertyChangedChain(p => p.Age).Subscribe(_ => { }))
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
                    Enumerable.Repeat(new TestObservableObject(), 10000).Select(
                        x => x.WhenPropertyChangedNu<TestObservableObject, int>("Age").Subscribe(_ => { })).ToArray(),
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