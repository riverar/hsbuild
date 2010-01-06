using System;
using System.Collections.Generic;
using HSBuild.Core;
using HSBuild.Commands;
using NUnit.Framework;

namespace HSBuild.Core.Tests
{
    [TestFixture]
    public class ConfigTests
    {
        [Test]
        public void LoadConfig()
        {
            Config config = Config.LoadFromTextReader(new System.IO.StringReader(), string.Empty);
        }
    }
}
