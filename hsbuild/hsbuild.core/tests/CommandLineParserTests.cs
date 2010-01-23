// HSBuild.Core.Tests - CommandLineParser tests
//
// Copyright (C) 2009-2010 Haakon Sporsheim <haakon.sporsheim@gmail.com>
//
// HSBuild is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// HSBuild is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with HSBuild.  If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Collections.Generic;
using HSBuild.Core;
using HSBuild.Commands;
using NUnit.Framework;

namespace HSBuild.Core.Tests
{
    [TestFixture]
    public class CommandLineParserTests
    {
        [Test]
        public void ParseMinimalCommandLine()
        {
            Command cmd = CommandLineParser.Parse(null);

            Assert.IsNotNull(cmd);
            Assert.IsInstanceOf(typeof(BuildCommand), cmd);
            Assert.AreEqual(0, cmd.Arguments.Count);
        }

        [Test]
        public void ParseCommandWithOneModule()
        {
            Command cmd = CommandLineParser.Parse("build mymodule1".Split(' '));

            Assert.IsNotNull(cmd);
            Assert.IsInstanceOf(typeof(BuildCommand), cmd);
            Assert.AreEqual(1, cmd.Config.Modules.Length);
            Assert.Contains("mymodule1", cmd.Config.Modules);
        }

        [Test]
        public void ParseCommandWithMultipleModules()
        {
            Command cmd = CommandLineParser.Parse("build mymodule1 mymodule2,mymodule3".Split(' '));

            Assert.IsNotNull(cmd);
            Assert.IsInstanceOf(typeof(BuildCommand), cmd);
            Assert.AreEqual(3, cmd.Config.Modules.Length);
            Assert.Contains("mymodule1", cmd.Config.Modules);
            Assert.Contains("mymodule2", cmd.Config.Modules);
            Assert.Contains("mymodule3", cmd.Config.Modules);
        }

        [TestFixture]
        public class GlobalOptions
        {
            [Test]
            public void ParseModuleSetLongName()
            {
                Command cmd = CommandLineParser.Parse("--moduleset my.modules".Split(' '));

                Assert.IsNotNull(cmd);
                Assert.AreEqual("my.modules", cmd.Config.ModuleSet);
            }

            [Test]
            public void ParseEscapedFilename()
            {
                Command cmd = CommandLineParser.Parse("--moduleset=\"my first.modules\"".Split(' '));

                Assert.IsNotNull(cmd);
                Assert.AreEqual("my first.modules", cmd.Config.ModuleSet);
            }

            [Test]
            public void ParseHelpOption()
            {
                Command cmd = CommandLineParser.Parse("--help".Split(' '));

                Assert.IsNotNull(cmd);
                Assert.IsInstanceOf(typeof(HelpCommand), cmd);
                Assert.IsNull((cmd as HelpCommand).ErrorMsg);
            }

            [Test]
            public void ParseInvalid()
            {
                Command cmd = CommandLineParser.Parse("--stupid".Split(' '));

                Assert.IsNotNull(cmd);
                Assert.IsInstanceOf(typeof(HelpCommand), cmd);
                Assert.AreEqual("Command word --stupid not recognized.", (cmd as HelpCommand).ErrorMsg);
            }
        }
    }
}
