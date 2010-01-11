// HSBuild - Console
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HSBuild.Core;

namespace HSBuild.Console
{
    public class ConsoleTaskQueue : ITaskQueue, ITaskEngine
    {
        private List<ITask> m_queue = new List<ITask>();

        #region ITaskQueue Members

        public ITask[] TaskQueue
        {
            get { return m_queue.ToArray(); }
        }

        public void QueueTask(ITask task)
        {
            m_queue.Add(task);
        }

        #endregion

        #region ITaskEngine Members

        public bool ExecuteTaskQueue(ITask[] queue, IOutputEngine output)
        {
            if (queue == null)
                return false;

            foreach (ITask t in queue)
            {
                int ret = t.Execute(output);

                output.WriteOutput(ret == 0 ? OutputType.Debug : OutputType.Error,
                    string.Format("Task '{0}' executed with error code {1}.", t.ToString(), ret));
            }

            return true;
        }

        #endregion

    }

    class FileModuleSetLoader : IModuleSetLoader
    {
        #region IModuleSetLoader Members

        public ModuleSet LoadModuleSet(Config config)
        {
            System.IO.StreamReader sr;
            try
            {
                sr = new System.IO.StreamReader(config.ModuleSet);
            }
            catch
            {
                return null;
            }

            return new ModuleSet(sr);
        }

        #endregion
    }

    class ConsoleOutputEngine : IOutputEngine
    {
        #region IOutputEngine Members

        public void WriteOutput(OutputType type, string line)
        {
            ConsoleColor clrBgr = System.Console.BackgroundColor;
            ConsoleColor clrFgr = System.Console.ForegroundColor;
            try
            {
                if (type >= OutputType.Info)
                {
                    if (type == OutputType.Heading)
                        System.Console.ForegroundColor = ConsoleColor.Cyan;
                    else if (type == OutputType.Info)
                        System.Console.ForegroundColor = ConsoleColor.DarkGray;
                    else if (type == OutputType.Error)
                        System.Console.ForegroundColor = ConsoleColor.Red;

                    System.Console.WriteLine(line);
                }
            }
            finally
            {
                System.Console.ForegroundColor = clrFgr;
                System.Console.BackgroundColor = clrBgr;
            }
        }

        #endregion
    }

    class Program
    {
        static void Main(string[] args)
        {
            Command cmd = CommandLineParser.Parse(args);
            if (cmd != null)
            {
                ConsoleTaskQueue queueEngine = new ConsoleTaskQueue();
                FileModuleSetLoader loader = new FileModuleSetLoader();
                ConsoleOutputEngine output = new ConsoleOutputEngine();

                if (cmd.Config != null)
                    cmd.Config.SetupEnvironment();
                cmd.Execute(queueEngine, loader, output);
                queueEngine.ExecuteTaskQueue(queueEngine.TaskQueue, output);
            }
        }
    }
}
