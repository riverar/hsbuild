// HSBuild.Core.Tests - Command tests
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
using System.Collections.Generic;
using HSBuild.Core;
using HSBuild.Commands;
using HSBuild.Tasks;
using NUnit.Framework;

namespace HSBuild.Core.Tests
{
    internal class TestCommand : Command
    {
        private ModuleSet m_moduleset;
        public TestCommand(ModuleSet moduleset)
            : base(Config.CreateDefaultConfig())
        {
            m_moduleset = moduleset;
        }

        public override void Execute(ITaskQueue taskQueue, IModuleSetLoader loader, IOutputEngine output)
        {
            throw new NotImplementedException();
        }

        public LinkedList<Module> LoadModulesStack()
        {
            LinkedList<Module> modules = new LinkedList<Module>();
            FilterModuleList(Config.Modules, m_moduleset, ref modules);
            return modules;
        }
    }

    public class ModuleSetTestStrings
    {
        public static string Repository_Gnome =
            "<repository type=\"git\" name=\"git.gnome.org\" href=\"git://git.gnome.org/\"/>";
        public static string Repository_Freedesktop =
            "<repository type=\"git\" name=\"git.freedesktop.org\" href=\"git://anongit.freedesktop.org/\"/>";
        public static string SubModule_GLib =
            "<hsbuildmodule id=\"glib\">" +
            "<branch repo=\"git.gnome.org\" module=\"glib\" />" +
            "</hsbuildmodule>";
        public static string SubModule_Pixman =
            "<hsbuildmodule id=\"pixman\">" +
            "<branch repo=\"git.freedesktop.org\" module=\"pixman\">" +
                "<patches>" +
                    "<patch file=\"fix1.patch\" strip=\"1\" />" +
                    "<patch uri=\"http://patches.org/fix2.patch\" strip=\"1\" />" +
                "</patches>" +
            "</branch>" +
            "</hsbuildmodule>";
        public static string SubModule_Cairo =
            "<hsbuildmodule id=\"cairo\">" +
            "<branch repo=\"git.freedesktop.org\" module=\"cairo\" />" +
            "<dependencies><dep package=\"pixman\" /></dependencies>" +
            "</hsbuildmodule>";
        public static string SubModule_Pango =
            "<hsbuildmodule id=\"pango\">" +
            "<branch repo=\"git.gnome.org\" module=\"pango\" />" +
            "<dependencies>" +
                "<dep package=\"glib\" />" +
                "<dep package=\"cairo\" />" +
            "</dependencies>" +
            "</hsbuildmodule>";
        public static string SubModule_ATK =
            "<hsbuildmodule id=\"atk\">" +
            "<branch repo=\"git.gnome.org\" module=\"atk\" />" +
            "<dependencies><dep package=\"glib\" /></dependencies>" +
            "</hsbuildmodule>";
        public static string SubModule_GTK =
            "<hsbuildmodule id=\"gtk+\">" +
            "<branch repo=\"git.gnome.org\" module=\"gtk+\" />" +
            "<dependencies>" +
                "<dep package=\"glib\" />" +
                "<dep package=\"cairo\" />" +
                "<dep package=\"pango\" />" +
                "<dep package=\"atk\" />" +
            "</dependencies>" +
            "</hsbuildmodule>";
    }

    [TestFixture]
    public class LoadModulesTests
    {
        private TestCommand m_cmd;

        [SetUp]
        public void Setup()
        {
            StringReader reader = new StringReader("<?xml version=\"1.0\"?>" +
                "<moduleset>" +
                    ModuleSetTestStrings.Repository_Gnome +
                    ModuleSetTestStrings.Repository_Freedesktop +
                    ModuleSetTestStrings.SubModule_GLib +
                    ModuleSetTestStrings.SubModule_Pixman +
                    ModuleSetTestStrings.SubModule_Cairo +
                    ModuleSetTestStrings.SubModule_Pango +
                    ModuleSetTestStrings.SubModule_ATK +
                    ModuleSetTestStrings.SubModule_GTK +
                "</moduleset>");
            m_cmd = new TestCommand(new ModuleSet(reader));
        }

        [Test]
        public void LoadModulesStackAll()
        {
            m_cmd.Config.OverrideModules(new string[] { "gtk+" });
            LinkedList<Module> list = m_cmd.LoadModulesStack();

            Assert.AreEqual(6, list.Count);

            Assert.AreEqual("glib", list.First.Value.Id);
            list.RemoveFirst();
            Assert.AreEqual("pixman", list.First.Value.Id);
        }

        [Test]
        public void LoadModulesStackPartial()
        {
            m_cmd.Config.OverrideModules(new string[] { "atk" });
            LinkedList<Module> list = m_cmd.LoadModulesStack();

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("glib", list.First.Value.Id);
            list.RemoveFirst();
            Assert.AreEqual("atk", list.First.Value.Id);
        }
    }

    internal class CommandsEngine : ITaskQueue, IModuleSetLoader, IOutputEngine
    {
        static string ModuleSet =
            "<?xml version=\"1.0\"?>" +
            "<moduleset>" +
                ModuleSetTestStrings.Repository_Gnome +
                ModuleSetTestStrings.Repository_Freedesktop +
                ModuleSetTestStrings.SubModule_GLib +
                ModuleSetTestStrings.SubModule_Pixman +
                ModuleSetTestStrings.SubModule_Cairo +
                ModuleSetTestStrings.SubModule_Pango +
                ModuleSetTestStrings.SubModule_ATK +
                ModuleSetTestStrings.SubModule_GTK +
            "</moduleset>";

        #region ITaskQueue Members

        public void QueueTask(ITask task)
        {
            m_tasks.Add(task);
        }

        public ITask[] TaskQueue
        {
            get { return m_tasks.ToArray(); }
        }

        #endregion

        #region IModuleSetLoader Members

        public ModuleSet LoadModuleSet(Config config)
        {
            return new ModuleSet(new StringReader(ModuleSet));
        }

        #endregion

        #region IOutputEngine Members

        public void WriteOutput(OutputType type, string line)
        {
            switch (type)
            {
                case OutputType.Normal:
                    m_outNormal.Add(line);
                    break;
            }
        }

        #endregion

        #region Properties
        internal string[] NormalOutput
        {
            get
            {
                return m_outNormal.ToArray();
            }
        }

        #endregion

        private List<string> m_outNormal = new List<string>();
        private List<ITask> m_tasks = new List<ITask>();
    }

    [TestFixture]
    public class CommandsTests
    {
        CommandsEngine engine;

        [SetUp]
        public void Setup()
        {
            engine = new CommandsEngine();
        }

        [Test]
        public void ExecuteListCommandAll()
        {
            ListCommand cmd = new ListCommand(Config.CreateDefaultConfig());
            cmd.Execute(engine, engine, engine);

            string[] output = engine.NormalOutput;
            Assert.IsNotNull(output);
            Assert.AreEqual(6, output.Length);

            Assert.AreEqual("glib", output[0].Trim());
            Assert.AreEqual("pixman", output[1].Trim());
            Assert.AreEqual("cairo", output[2].Trim());
            Assert.AreEqual("pango", output[3].Trim());
            Assert.AreEqual("atk", output[4].Trim());
            Assert.AreEqual("gtk+", output[5].Trim());
        }

        [Test]
        public void ExecuteListCommandPartial()
        {
            ListCommand cmd = new ListCommand(Config.CreateDefaultConfig());
            cmd.Config.OverrideModules(new string[] { "cairo" });
            cmd.Execute(engine, engine, engine);

            string[] output = engine.NormalOutput;
            Assert.IsNotNull(output);
            Assert.AreEqual(2, output.Length);

            Assert.AreEqual("pixman", output[0].Trim());
            Assert.AreEqual("cairo", output[1].Trim());
        }

        [Test]
        public void ExecuteUpdateCommandSimple()
        {
            UpdateCommand cmd = new UpdateCommand(Config.CreateDefaultConfig());
            cmd.Config.OverrideModules(new string[] { "glib" });
            cmd.Execute(engine, engine, engine);

            ITask[] tasks = engine.TaskQueue;

            Assert.AreEqual(1, tasks.Length);
            Assert.IsInstanceOf(typeof(ConsoleTask), tasks[0]);

            ConsoleTask task = tasks[0] as ConsoleTask;

            StringAssert.Contains("git", task.StartInfo.FileName);
            Assert.AreEqual("clone git://git.gnome.org/glib glib", task.StartInfo.Arguments);
            Assert.AreEqual(Environment.CurrentDirectory, task.StartInfo.WorkingDirectory);
        }

        [Test]
        public void ExecuteUpdateCommandWithPatches()
        {
            UpdateCommand cmd = new UpdateCommand(Config.CreateDefaultConfig());
            cmd.Config.OverrideModules(new string[] { "pixman" });
            cmd.Execute(engine, engine, engine);

            ITask[] tasks = engine.TaskQueue;

            Assert.AreEqual(4, tasks.Length);
            Assert.IsInstanceOf(typeof(ConsoleTask), tasks[0]);
            Assert.IsInstanceOf(typeof(ConsoleTask), tasks[1]);
            Assert.IsInstanceOf(typeof(HttpTask), tasks[2]);
            Assert.IsInstanceOf(typeof(ConsoleTask), tasks[3]);

            ConsoleTask task = tasks[0] as ConsoleTask;

            StringAssert.Contains("git", task.StartInfo.FileName);
            Assert.AreEqual("clone git://anongit.freedesktop.org/pixman pixman", task.StartInfo.Arguments);
            Assert.AreEqual(Environment.CurrentDirectory, task.StartInfo.WorkingDirectory);

            task = tasks[1] as ConsoleTask;
            StringAssert.Contains("git", task.StartInfo.FileName);
            Assert.AreEqual("apply --whitespace=nowarn -p1 \"" + Path.Combine(Environment.CurrentDirectory, "fix1.patch") + "\"", task.StartInfo.Arguments);
            Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "pixman"), task.StartInfo.WorkingDirectory);

            task = tasks[3] as ConsoleTask;
            StringAssert.Contains("git", task.StartInfo.FileName);
            Assert.AreEqual("apply --whitespace=nowarn -p1 \"" + Path.Combine(Path.GetTempPath(), "fix2.patch") + "\"", task.StartInfo.Arguments);
            Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "pixman"), task.StartInfo.WorkingDirectory);
        }
    }
}
