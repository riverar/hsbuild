// HSBuild.Core.Tests - ModuleSet tests
//
// Copyright (C) 2009-2010 Haakon Sporsheim
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
using System.IO;
using System.Xml;
using HSBuild.Core;
using HSBuild.Modules;
using HSBuild.VCS;
using NUnit.Framework;

namespace HSBuild.Core.Tests
{
    [TestFixture]
    public class ModuleSetTests
    {
        [Test]
        public void ParseEmptyModuleSet()
        {
            StringReader reader = new StringReader("<?xml version=\"1.0\"?><moduleset></moduleset>");

            ModuleSet set = new ModuleSet(reader);

            Assert.AreEqual(0, set.Repositories.Count);
            Assert.AreEqual(0, set.Modules.Count);
        }

        [Test]
        public void ParseGitRepository()
        {
            StringReader reader = new StringReader("<?xml version=\"1.0\"?><moduleset>" +
                "<repository type=\"git\" name=\"git.test.tpl\" href=\"git://annongit.test.tpl/git\" />" +
                "</moduleset>");

            ModuleSet set = new ModuleSet(reader);

            Assert.AreEqual(1, set.Repositories.Count);
            Repository repo = set.Repositories["git.test.tpl"];
            Assert.IsNotNull(repo);
            Assert.IsInstanceOf(typeof(GitRepository), repo);

            Assert.AreEqual("git.test.tpl", repo.Name);
            Assert.AreEqual("git://annongit.test.tpl/git", ((GitRepository)repo).HRef);
        }

        [Test]
        public void ParseBzrRepository()
        {
            StringReader reader = new StringReader("<?xml version=\"1.0\"?><moduleset>" +
                "<repository type=\"bzr\" name=\"bzzzr.test.tpl\" href=\"bzr://test.tpl/bzr\" />" +
                "</moduleset>");

            ModuleSet set = new ModuleSet(reader);

            Assert.AreEqual(1, set.Repositories.Count);
            Repository repo = set.Repositories["bzzzr.test.tpl"];
            Assert.IsInstanceOf(typeof(BzrRepository), repo);

            Assert.AreEqual("bzzzr.test.tpl", repo.Name);
            Assert.AreEqual("bzr://test.tpl/bzr", ((BzrRepository)repo).HRef);
        }

        [Test]
        public void ParseSimpleHSBuildModule()
        {
            StringReader reader = new StringReader("<?xml version=\"1.0\"?>" +
                "<moduleset>" +
                    "<repository type=\"git\" name=\"git.test.tpl\" href=\"git://annongit.test.tpl/git\" />" +
                    "<hsbuildmodule id=\"glib\" projects=\"glib.sln\">" +
                        "<branch repo=\"git.test.tpl\" module=\"glib\" />" +
                    "</hsbuildmodule>" +
                "</moduleset>");

            ModuleSet set = new ModuleSet(reader);

            Assert.AreEqual(1, set.Modules.Count);

            Module mod = set.Modules["glib"];

            Assert.IsNotNull(mod);
            Assert.IsInstanceOf(typeof(HSBuildModule), mod);
            Assert.AreEqual("glib.sln", ((HSBuildModule)mod).Projects);

            Assert.IsNotNull(mod.Branch);
            Assert.AreEqual("glib", mod.Branch.Module);
            Assert.AreEqual("git.test.tpl", mod.Branch.Repository);
            Assert.IsNullOrEmpty(mod.Branch.CheckoutDir);
            Assert.IsNullOrEmpty(mod.Branch.Revision);

            Assert.AreEqual(0, mod.Dependencies.Length);
        }

        [Test]
        public void ParseSimpleHSBuildModuleWithCheckoutDir()
        {
            StringReader reader = new StringReader("<?xml version=\"1.0\"?>" +
                "<moduleset>" +
                    "<repository type=\"git\" name=\"git.test.tpl\" href=\"git://annongit.test.tpl/git\" />" +
                    "<hsbuildmodule id=\"gstreamer\">" +
                        "<branch repo=\"git.test.tpl\" module=\"gstreamer/gstreamer\" checkoutdir=\"gstreamer\" />" +
                    "</hsbuildmodule>" +
                "</moduleset>");

            ModuleSet set = new ModuleSet(reader);

            Assert.AreEqual(1, set.Modules.Count);

            Module mod = set.Modules["gstreamer"];

            Assert.IsNotNull(mod);
            Assert.IsInstanceOf(typeof(HSBuildModule), mod);

            Assert.IsNotNull(mod.Branch);
            Assert.AreEqual("gstreamer/gstreamer", mod.Branch.Module);
            Assert.AreEqual("git.test.tpl", mod.Branch.Repository);
            Assert.AreEqual("gstreamer", mod.Branch.CheckoutDir);
            Assert.IsNullOrEmpty(mod.Branch.Revision);

            Assert.AreEqual(0, mod.Dependencies.Length);
        }

        [Test]
        public void ParseModuleDependencies()
        {
            StringReader reader = new StringReader("<?xml version=\"1.0\"?>" +
                "<moduleset>" +
                    "<repository type=\"git\" name=\"git.test.tpl\" href=\"git://annongit.test.tpl/git\" />" +
                    "<hsbuildmodule id=\"glib\">" +
                        "<branch repo=\"git.test.tpl\" module=\"glib\" />" +
                        "<dependencies>" +
                            "<dep package=\"mod1\" />" +
                            "<dep package=\"mod2\" />" +
                            "<dep package=\"mod3\" />" +
                        "</dependencies>" +
                    "</hsbuildmodule>" +
                "</moduleset>");

            ModuleSet set = new ModuleSet(reader);
            Module mod = set.Modules["glib"];

            Assert.IsNotNull(mod);
            Assert.IsInstanceOf(typeof(HSBuildModule), mod);

            Assert.AreEqual(3, mod.Dependencies.Length);
            Assert.Contains("mod1", mod.Dependencies);
            Assert.Contains("mod2", mod.Dependencies);
            Assert.Contains("mod3", mod.Dependencies);
        }

        [Test]
        public void ParseModuleBranchPatches()
        {
            StringReader reader = new StringReader("<?xml version=\"1.0\"?>" +
                "<moduleset>" +
                    "<repository type=\"git\" name=\"git.test.tpl\" href=\"git://annongit.test.tpl/git\" />" +
                    "<hsbuildmodule id=\"glib\">" +
                        "<branch repo=\"git.test.tpl\" module=\"glib\">" +
                            "<patches>" +
                                "<patch file=\"fix1.patch\" strip=\"1\" />" +
                                "<patch uri=\"http://patches.org/fix2.patch\" strip=\"1\" />" +
                                "<patch uri=\"https://patches.org/fix3.patch\" strip=\"0\" />" +
                            "</patches>" +
                        "</branch>" +
                    "</hsbuildmodule>" +
                "</moduleset>");

            ModuleSet set = new ModuleSet(reader);
            Module mod = set.Modules["glib"];

            Assert.IsNotNull(mod);
            Assert.IsInstanceOf(typeof(HSBuildModule), mod);

            Patch patch = mod.Branch.PatchQueue;
            Assert.IsNotNull(patch);
            Assert.IsTrue(patch.Uri.IsFile);
            Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "fix1.patch"), patch.Uri.LocalPath);
            Assert.AreEqual(1, patch.Strip);

            patch = patch.Next;
            Assert.IsNotNull(patch);
            Assert.AreEqual(Uri.UriSchemeHttp, patch.Uri.Scheme);
            Assert.AreEqual("patches.org", patch.Uri.Host);
            Assert.AreEqual("/fix2.patch", patch.Uri.PathAndQuery);
            Assert.AreEqual(1, patch.Strip);

            patch = patch.Next;
            Assert.IsNotNull(patch);
            Assert.AreEqual(Uri.UriSchemeHttps, patch.Uri.Scheme);
            Assert.AreEqual("patches.org", patch.Uri.Host);
            Assert.AreEqual("/fix3.patch", patch.Uri.PathAndQuery);
            Assert.AreEqual(0, patch.Strip);

            patch = patch.Next;
            Assert.IsNull(patch);
        }
    }
}
