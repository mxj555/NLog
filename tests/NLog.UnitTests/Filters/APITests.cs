// 
// Copyright (c) 2004-2010 Jaroslaw Kowalski <jaak@jkowalski.net>
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

using System;
using System.Xml;
using System.Reflection;
using System.IO;

using NLog;
using NLog.Config;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.Layouts;
using NLog.Filters;

namespace NLog.UnitTests.Filters
{
    [TestClass]
    public class APITests : NLogTestBase
    {
        [TestMethod]
        public void APITest()
        {
            // this is mostly to make Clover happy

            LogManager.Configuration = CreateConfigurationFromString(@"
            <nlog>
                <targets><target name='debug' type='Debug' layout='${basedir} ${message}' /></targets>
                <rules>
                    <logger name='*' minlevel='Debug' writeTo='debug'>
                    <filters>
                        <whenContains layout='${message}' substring='zzz' action='Ignore' />
                    </filters>
                    </logger>
                </rules>
            </nlog>");

            Assert.IsTrue(LogManager.Configuration.LoggingRules[0].Filters[0] is WhenContainsFilter);
            var wcf = (WhenContainsFilter)LogManager.Configuration.LoggingRules[0].Filters[0];
            Assert.IsInstanceOfType(wcf.Layout, typeof(SimpleLayout));
            Assert.AreEqual(((SimpleLayout)wcf.Layout).Text, "${message}");
            Assert.AreEqual(wcf.Substring, "zzz");
            Assert.AreEqual(FilterResult.Ignore, wcf.Action);
        }
    }
}