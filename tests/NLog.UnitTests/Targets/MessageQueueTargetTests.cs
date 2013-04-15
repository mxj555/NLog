// 
// Copyright (c) 2004-2011 Jaroslaw Kowalski <jaak@jkowalski.net>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

#if !NET_CF && !SILVERLIGHT && !MONO

namespace NLog.UnitTests.Targets
{
    using System.Collections.Generic;
    using System.Messaging;
    using NUnit.Framework;

#if !NUNIT
    using SetUp = Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute;
    using TestFixture = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
    using Test = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
    using TearDown =  Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute;
#endif
    using NLog.Targets;

    [TestFixture]
    public class MessageQueueTargetTests : NLogTestBase
    {
        [Test]
        public void QueueExists_Write_MessageIsWritten()
        {
            var messageQueueTestProxy = new MessageQueueTestProxy
                                        {
                                            QueueExists = true,
                                        };
            var target = CreateTarget(messageQueueTestProxy, false);

            target.WriteAsyncLogEvent(new LogEventInfo().WithContinuation(_ => {}));

            Assert.AreEqual(1, messageQueueTestProxy.SentMessages.Count);
        }

        [Test]
        public void QueueDoesNotExistsAndDoNotCreate_Write_NothingIsWritten()
        {
            var messageQueueTestProxy = new MessageQueueTestProxy
                                        {
                                            QueueExists = false,
                                        };
            var target = CreateTarget(messageQueueTestProxy, false);

            target.WriteAsyncLogEvent(new LogEventInfo().WithContinuation(_ => {}));

            Assert.AreEqual(0, messageQueueTestProxy.SentMessages.Count);
        }

        [Test]
        public void QueueDoesNotExistsAndCreatedQueue_Write_QueueIsCreated()
        {
            var messageQueueTestProxy = new MessageQueueTestProxy
                                        {
                                            QueueExists = false,
                                        };
            var target = CreateTarget(messageQueueTestProxy, true);

            target.WriteAsyncLogEvent(new LogEventInfo().WithContinuation(_ => {}));

            Assert.IsTrue(messageQueueTestProxy.QueueCreated);
        }

        [Test]
        public void QueueDoesNotExistsAndCreatedQueue_Write_MessageIsWritten()
        {
            var messageQueueTestProxy = new MessageQueueTestProxy
                                        {
                                            QueueExists = false,
                                        };
            var target = CreateTarget(messageQueueTestProxy, true);

            target.WriteAsyncLogEvent(new LogEventInfo().WithContinuation(_ => {}));

            Assert.AreEqual(1, messageQueueTestProxy.SentMessages.Count);
        }

        [Test]
        public void FormatQueueName_Write_DoesNotCheckIfQueueExists()
        {
            var messageQueueTestProxy = new MessageQueueTestProxy();
            var target = CreateTarget(messageQueueTestProxy, false, "DIRECT=http://test.com/MSMQ/queue");
            
            target.WriteAsyncLogEvent(new LogEventInfo().WithContinuation(_ => {}));

            Assert.IsFalse(messageQueueTestProxy.QueueExistsCalled);
        }

        [Test]
        public void DoNotCheckIfQueueExists_Write_DoesNotCheckIfQueueExists()
        {
            var messageQueueTestProxy = new MessageQueueTestProxy();
            var target = CreateTarget(messageQueueTestProxy, false, checkIfQueueExists: false);

            target.WriteAsyncLogEvent(new LogEventInfo().WithContinuation(_ => {}));

            Assert.IsFalse(messageQueueTestProxy.QueueExistsCalled);
        }

        private static MessageQueueTarget CreateTarget(MessageQueueProxy messageQueueTestProxy, bool createQueue, string queueName = "Test", bool checkIfQueueExists = true)
        {
            var target = new MessageQueueTarget
                         {
                             MessageQueueProxy = messageQueueTestProxy,
                             Queue = queueName,
                             CreateQueueIfNotExists = createQueue,
                             CheckIfQueueExists = checkIfQueueExists,
                         };
            target.Initialize(null);
            return target;
        }
    }

    internal class MessageQueueTestProxy : MessageQueueProxy
    {
        public IList<Message> SentMessages { get; private set; }

        public bool QueueExists { get; set; }

        public bool QueueCreated { get; private set; }

        public bool QueueExistsCalled { get; private set; }

        public MessageQueueTestProxy()
        {
            this.SentMessages = new List<Message>();
        }

        public override bool Exists(string queue)
        {
            this.QueueExistsCalled = true;
            return this.QueueExists;
        }

        public override void Create(string queue)
        {
            this.QueueCreated = true;
        }

        public override void Send(string queue, Message message)
        {
            SentMessages.Add(message);
        }
    }
}

#endif