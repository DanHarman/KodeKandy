// <copyright file="Given_Observing_PropertyChanged_Events.cs" company="million miles per hour ltd">
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
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.ObservableObjectTests
{
    [TestFixture]
    public class Given_Observing_PropertyChanged_Events
    {
        [Test]
        public void When_SetValue_With_New_Values_Then_Event_Raised()
        {
            // Arrange
            var sut = new TestObservableObject {Age = 10};
            var results = new List<Tuple<string, int>>();
            var expected = new[]
            {
                Tuple.Create("Age", 17),
                Tuple.Create("Age", 70)
            };
            sut.PropertyChanged += (o, pc) => results.Add(Tuple.Create(pc.PropertyName, sut.Age));

            // Act
            sut.Age = 17;
            sut.Age = 70;

            // Assert
            CollectionAssert.AreEqual(expected, results);
        }

        public void TimeOperation(int repeats, string label, Action action)
        {
            var sw = new Stopwatch();
            var res = new List<long>();
            for (var j = 0; j < repeats + 1; ++j)
            {
                sw.Reset();
                sw.Start();
                action();
                sw.Stop();

              //  Console.Error.WriteLine("Elapsed : {0}ms", sw.ElapsedMilliseconds);
                res.Add(sw.ElapsedMilliseconds);
            }

            Console.Error.WriteLine("{0} - Average : {1}ms", label, res.Skip(1).Average()); 
        }

        [Test]
        public void When_V1()
        {
            // Arrange
//            var sut = new TestObservableObject { Age = 10 };
//            var results = new List<Tuple<string, int>>(10000000);
//
//            sut.PropertyChanged += (o, pc) => { }; //results.Add(Tuple.Create(pc.PropertyName, sut.Age));
//            sut.PropertyChanges.Subscribe(_ => { });

            TimeOperation(5, "1 - No subscribers lots of events", () =>
            {
                var sut = new TestObservableObject { Age = 10 };

                for (var i = 0; i < 10000000; ++i)
                    sut.Age = i;
            });

            TimeOperation(5, "1 - Making lots of subscriptions", () =>
            {
                var sut = new TestObservableObject { Age = 10 };
                var zz = sut.WhenPropertyChanged(x => x.Age).Publish().RefCount();
                for (var i = 0; i < 10000; ++i)
                    zz.Subscribe(_ => { });
            });

            TimeOperation(5, "1 - Lots of refcount subscriptions on one object with lots of notifications", () =>
            {
                var sut = new TestObservableObject { Age = 10 };
                var zz = sut.WhenPropertyChanged3(x => x.Age).Publish().RefCount();

                for (var i = 0; i < 10000; ++i)
                   zz.Subscribe(_ => { });

                for (var i = 0; i < 10000; ++i)
                    sut.Age = i;
            });

            TimeOperation(5, "1 - Lots of subscriptions on one object with lots of notifications", () =>
            {
                var sut = new TestObservableObject {Age = 10};
                ;

                for (var i = 0; i < 10000; ++i)
                    sut.WhenPropertyChanged3(x => x.Age).Subscribe(_ => { });

                for (var i = 0; i < 10000; ++i)
                    sut.Age = i;
            });

            TimeOperation(5, "1 - Lots of objects with 1 sub and lots of events", () =>
            {
                for (var i = 0; i < 10000; ++i)
                {
                    var sut = new TestObservableObject {Age = 10};
                    var zz = sut.WhenPropertyChanged(x => x.Age).Subscribe(_ => { });
                    
                    for (var j = 0; j < 10000; ++j)
                        sut.Age = i;
                }
            });
        }

        [Test]
        public void When_V3()
        {
            // Arrange
            //            var sut = new TestObservableObject2 { Age = 10 };
            //            var results = new List<Tuple<string, int>>(10000000);
            //
            //            sut.PropertyChanged += (o, pc) => { }; //results.Add(Tuple.Create(pc.PropertyName, sut.Age));
            //    sut.PropertyChanges.Subscribe(_ => { });

            // sut.WhenPropertyChanged(x => x.Age);


            // Act
//            TimeOperation(5, "3 - No subscribers lots of events", () =>
//            {
//                var sut = new TestObservableObject3 { Age = 10 };
//
//                for (var i = 0; i < 10000000; ++i)
//                    sut.Age = i;
//            });
//
//            TimeOperation(5, "3 - Making lots of subscriptions", () =>
//            {
//                var sut = new TestObservableObject3 { Age = 10 };
//
//                for (var i = 0; i < 10000; ++i)
//                    sut.WhenPropertyChanged3(x => x.Age).Subscribe(_ => { });
//            });
//
//            TimeOperation(5, "1 - Lots of refcount subscriptions on one object with lots of notifications", () =>
//            {
//                var sut = new TestObservableObject3 { Age = 10 };
//                var zz = sut.WhenPropertyChanged3(x => x.Age).Publish().RefCount();
//
//                for (var i = 0; i < 10000; ++i)
//                    zz.Subscribe(_ => { });
//
//                for (var i = 0; i < 10000; ++i)
//                    sut.Age = i;
//            });
//
//            TimeOperation(5, "3 - Lots of subscriptions on on e object and lots of notifications.", () =>
//            {
//                var sut = new TestObservableObject3 { Age = 10 };
//                for (var i = 0; i < 10000; ++i)
//                    sut.WhenPropertyChanged3(x => x.Age).Subscribe(_ => { });
//
//                for (var i = 0; i < 10000; ++i)
//                    sut.Age = i;
//            });
//
//
//            TimeOperation(5, "3 - Lots of direct where subscriptions on one object and lots of notifications.", () =>
//            {
//                var sut = new TestObservableObject3 { Age = 10 };
//                for (var i = 0; i < 10000; ++i)
//                    sut.PropertyChanges.Where(x => x.PropertyName == "Age").Subscribe(_ => { });
//
//                for (var i = 0; i < 10000; ++i)
//                    sut.Age = i;
//            });

//            TimeOperation(5, "3 - Lots of subscriptions on on e object and lots of notifications string prop name.", () =>
//            {
//                for (var o = 0; o < 10000; ++o)
//                {
//                    var sut = new TestObservableObject3 {Age = 10};
//                    for (var i = 0; i < 10; ++i)
//                        sut.WhenPropertyChanged3<TestObservableObject3, int>("Age").Subscribe(_ => { });
//
//                    for (var i = 0; i < 10000; ++i)
//                        sut.Age = i;
//                }
//            });

            TimeOperation(5, "1 rx Subcriber 10k notifications", () =>
            {
                for (var i = 0; i < 10000; ++i)
                {
                    var sut = new TestObservableObject3 { Age = 10 };
                    var zz = sut.WhenPropertyChanged3(x => x.Age).Subscribe(_ => { });

                    for (var j = 0; j < 10000; ++j)
                        sut.Age = i;
                }
            });

            TimeOperation(5, "1 event Subscriber 10k notifications", () =>
            {
                for (var i = 0; i < 10000; ++i)
                {
                    var sut = new TestObservableObject3 { Age = 10 };
                    sut.PropertyChanged += (sender, args) => { };

                    for (var j = 0; j < 10000; ++j)
                        sut.Age = i;
                }
            });

            TimeOperation(5, "3 - 10k RX subcriber & 10k notification.", () =>
            {
                var cnt = 0;
                var sut = new TestObservableObject3 { Age = 10 };
                for (var i = 0; i < 10000; ++i)
                    sut.PropertyChanges/*.Where(x => x.PropertyName == "Age")*/.Subscribe(_ => { ++ cnt; });

                for (var i = 0; i < 10000; ++i)
                    sut.Age = i;

          //      Debug.WriteLine(cnt);
            });

            TimeOperation(5, "3 - 10k RX subscriber & 10k notification filtered to prop.", () =>
            {
                var cnt = 0;
                var sut = new TestObservableObject3 { Age = 10 };
                for (var i = 0; i < 10000; ++i)
                    sut.WhenPropertyChanged3(x => x.Age).Subscribe(_ => { ++cnt; });

                for (var i = 0; i < 10000; ++i)
                    sut.Age = i;

                //      Debug.WriteLine(cnt);
            });

            TimeOperation(5, "3 - 10k event sub & 10k notification.", () =>
            {
                var cnt = 0;

                var sut = new TestObservableObject3 { Age = 10 };
                for (var i = 0; i < 10000; ++i)
                    sut.PropertyChanged += (sender, args) => { ++cnt; };

                for (var i = 0; i < 10000; ++i)
                    sut.Age = i;

            //    Debug.WriteLine(cnt);

            });

            TimeOperation(5, "3 - 10k RX subcribe calls.", () =>
            {
                var cnt = 0;
                var sut = new TestObservableObject3 { Age = 10 };
                for (var i = 0; i < 10000; ++i)
                    sut.PropertyChanges/*.Where(x => x.PropertyName == "Age")*/.Subscribe(_ => { ++cnt; });
            });

            TimeOperation(5, "3 - 10k event sub subscribe calls.", () =>
            {
                var cnt = 0;

                var sut = new TestObservableObject3 { Age = 10 };
                for (var i = 0; i < 10000; ++i)
                    sut.PropertyChanged += (sender, args) => { ++cnt; };
            });
        }

        [Test]
        public void When_SetValue_With_Same_Values_Then_Event_Raised_Once_Per_Value()
        {
            // Arrange
            var sut = new TestObservableObject {Age = 10};
            var results = new List<Tuple<string, int>>();
            var expected = new[]
            {
                Tuple.Create("Age", 70)
            };
            sut.PropertyChanged += (o, pc) => results.Add(Tuple.Create(pc.PropertyName, sut.Age));

            // Act
            sut.Age = 10;
            sut.Age = 10;
            sut.Age = 70;
            sut.Age = 70;

            // Assert
            CollectionAssert.AreEqual(expected, results);
        }
    }
}