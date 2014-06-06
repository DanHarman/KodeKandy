// <copyright file="Given_WeakEventHandler.cs" company="million miles per hour ltd">
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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KodeKandy.Panopticon.Internal;
using KodeKandy.Panopticon.Properties;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.Internal
{
    [TestFixture]
    public class Given_WeakPropertyChangedEventHandler
    {
        class TestObject : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [Test]
        public void When_Subscribed_To_Event_Then_Does_Not_Prevent_GC_Of_Event_Source()
        {
            // Arrange
            var obj = new TestObject();
            var wr = new WeakReference(obj); // We need this to see if obj is disposed of later...
            var sut = new WeakPropertyChangedEventHandler((sender, args) => { });
            obj.PropertyChanged += sut.Handler;
            Assert.IsTrue(wr.IsAlive);

            // Act
            obj = null;
            GC.Collect();

            // Assert
            Assert.IsFalse(wr.IsAlive);
        }

//        [Test]
//        public void When_Subscribed_To_Event_Directly_Then_Does_Prevent_GC_Of_Event_Source()
//        {
//            // Arrange
//            var obj = new TestObject();
//            var wr = new WeakReference(obj); // We need this to see if obj is disposed of later...
//            PropertyChangedEventHandler handler = (sender, args) => { };
//            obj.PropertyChanged += handler;
//            Assert.IsTrue(wr.IsAlive);
//
//            // Act
//            obj = null;
//            GC.Collect();
//
//            // Assert
//            Assert.IsTrue(wr.IsAlive);
//            Assert.NotNull(handler);
//        }
    }
}