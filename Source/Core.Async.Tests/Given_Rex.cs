// <copyright file="Given_Rex.cs" company="million miles per hour ltd">
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
using KodeKandy.Threading;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy
{
    [TestFixture]
    public class Given_Rex : ReactiveTest
    {
        [Test]
        public void When_Completed_Task_ToRex_Then_Projects_Results_Into_Rex()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<Rex<int>>();

            // Act
            scheduler.AdvanceTo(10);
            TaskEx.FromResult(10).ToRex().Subscribe(observer);
            scheduler.Start();

            // Assert
            observer.Messages.AssertEqual(
                OnNext(10, Rex.Result(10)),
                OnCompleted<Rex<int>>(10)
                );
        }

        [Test]
        public void When_Creating_Error_Then_Error_Created()
        {
            // Arrange
            var expected = new Exception();
            var sut = Rex.Error<int>(expected);

            // Assert
            Assert.IsTrue(sut.IsError);
            Assert.AreEqual(expected, sut.Exception);
        }

        [Test]
        public void When_Creating_Result_Then_Result_Created()
        {
            // Arrange
            var sut = Rex.Result(10);

            // Assert
            Assert.IsFalse(sut.IsError);
            Assert.AreEqual(10, sut.Value);
        }

        [Test]
        public void When_Errored_Observable_ToRex_Then_Projects_Error_Into_Rex()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var expectedException = new Exception();
            var observable = scheduler.CreateColdObservable(
                OnNext(10, 10),
                OnError<int>(30, expectedException)
                );
            var observer = scheduler.CreateObserver<Rex<int>>();

            // Act
            observable.ToRex().Subscribe(observer);
            scheduler.Start();

            // Assert
            observer.Messages.AssertEqual(
                OnNext(10, Rex.Result(10)),
                OnNext(30, Rex.Error<int>(expectedException)),
                OnCompleted<Rex<int>>(30)
                );
        }

        [Test]
        public void When_Faulted_Task_ToRex_Then_Projects_Exception_Into_Rex()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var expectedException = new Exception();
            var observer = scheduler.CreateObserver<Rex<int>>();

            // Act
            scheduler.AdvanceTo(10);
            TaskEx.FromException<int>(expectedException).ToRex().Subscribe(observer);
            scheduler.Start();

            // Assert
            observer.Messages.AssertEqual(
                OnNext(10, Rex.Error<int>(expectedException)),
                OnCompleted<Rex<int>>(10));
        }

        [Test]
        public void When_Observable_ToRex_Then_Projects_Results()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var observable = scheduler.CreateColdObservable(
                OnNext(10, 10),
                OnNext(30, 20),
                OnCompleted<int>(40)
                );
            var observer = scheduler.CreateObserver<Rex<int>>();

            // Act
            observable.ToRex().Subscribe(observer);
            scheduler.Start();

            // Assert
            observer.Messages.AssertEqual(
                OnNext(10, Rex.Result(10)),
                OnNext(30, Rex.Result(20)),
                OnCompleted<Rex<int>>(40)
                );
        }

        [Test]
        public void When_SelectResult_Selector_Throws_Then_Captured_As_Rex_Error()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var expectedExceptions = new Exception();
            var observable = scheduler.CreateColdObservable(
                OnNext(10, Rex.Result(10)),
                OnNext(30, Rex.Result(20)),
                OnCompleted<Rex<int>>(40)
                );
            var observer = scheduler.CreateObserver<Rex<int>>();

            // Act
            observable.SelectResult((Func<int, int>) (_ => { throw expectedExceptions; })).Subscribe(observer);
            scheduler.Start();

            // Assert
            observer.Messages.AssertEqual(
                OnNext(10, Rex.Error<int>(expectedExceptions)),
                OnNext(30, Rex.Error<int>(expectedExceptions)),
                OnCompleted<Rex<int>>(40)
                );
        }

        [Test]
        public void When_SelectResult_Then_Projects_Results()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var expectedExceptions = new[] {new Exception(), new Exception()};
            var observable = scheduler.CreateColdObservable(
                OnNext(10, Rex.Result(10)),
                OnNext(20, Rex.Error<int>(expectedExceptions[0])),
                OnNext(30, Rex.Result(20)),
                OnNext(40, Rex.Error<int>(expectedExceptions[1])),
                OnCompleted<Rex<int>>(40)
                );
            var observer = scheduler.CreateObserver<Rex<int>>();

            // Act
            observable.SelectResult(r => r * 10).Subscribe(observer);
            scheduler.Start();

            // Assert
            observer.Messages.AssertEqual(
                OnNext(10, Rex.Result(100)),
                OnNext(20, Rex.Error<int>(expectedExceptions[0])),
                OnNext(30, Rex.Result(200)),
                OnNext(40, Rex.Error<int>(expectedExceptions[1])),
                OnCompleted<Rex<int>>(40)
                );
        }

        [Test]
        public void When_SubscribeRex_Then_Applies_OnNexts()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var expectedExceptions = new[] {new Exception(), new Exception()};
            var observable = scheduler.CreateColdObservable(
                OnNext(10, Rex.Result(10)),
                OnNext(20, Rex.Error<int>(expectedExceptions[0])),
                OnNext(30, Rex.Result(20)),
                OnNext(40, Rex.Error<int>(expectedExceptions[1])),
                OnCompleted<Rex<int>>(40)
                );
            var resultList = new List<int>();
            var errorList = new List<Exception>();

            // Act
            observable.SubscribeRex(resultList.Add, errorList.Add);
            scheduler.Start();

            // Assert
            CollectionAssert.AreEqual(new[] {10, 20}, resultList);
            CollectionAssert.AreEqual(expectedExceptions, errorList);
        }

        [Test]
        public void When_TakeErrors_Then_Only_Errors_Emitted()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var expectedExceptions = new[] {new Exception(), new Exception()};
            var observable = scheduler.CreateColdObservable(
                OnNext(10, Rex.Result(10)),
                OnNext(20, Rex.Error<int>(expectedExceptions[0])),
                OnNext(30, Rex.Result(10)),
                OnNext(40, Rex.Error<int>(expectedExceptions[0])),
                OnCompleted<Rex<int>>(40)
                );
            var observer = scheduler.CreateObserver<Rex<int>>();

            // Act
            observable.TakeErrors().Subscribe(observer);
            scheduler.Start();

            // Assert
            observer.Messages.AssertEqual(
                OnNext(20, Rex.Error<int>(expectedExceptions[0])),
                OnNext(40, Rex.Error<int>(expectedExceptions[0])),
                OnCompleted<Rex<int>>(40)
                );
        }

        [Test]
        public void When_TakeResults_Then_Only_Results_Emitted()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var expectedExceptions = new[] {new Exception(), new Exception()};
            var observable = scheduler.CreateColdObservable(
                OnNext(10, Rex.Result(10)),
                OnNext(20, Rex.Error<int>(expectedExceptions[0])),
                OnNext(30, Rex.Result(10)),
                OnNext(40, Rex.Error<int>(expectedExceptions[0])),
                OnCompleted<Rex<int>>(40)
                );
            var observer = scheduler.CreateObserver<Rex<int>>();

            // Act
            observable.TakeResults().Subscribe(observer);
            scheduler.Start();

            // Assert
            observer.Messages.AssertEqual(
                OnNext(10, Rex.Result(10)),
                OnNext(30, Rex.Result(10)),
                OnCompleted<Rex<int>>(40)
                );
        }
    }
}