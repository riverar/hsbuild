// HSBuild.Commands - Help command
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
using System.Reflection;
using System.Collections.Generic;
using HSBuild.Core;

namespace HSBuild.Commands
{
    public class HelpCommand : Command
    {
        public HelpCommand(Config config)
            : base(config)
        {
        }

        public HelpCommand(Command cmd)
            : base(null)
        {
            m_cmd = cmd;
        }

        public HelpCommand(Exception ex)
            : base(null)
        {
            m_ex = ex;
        }

        public static string Name { get { return "help"; } }
        public static string Description { get { return "Prints this help information."; } }

        public override void PrintHelp(IOutputEngine output)
        {
            output.WriteOutput(OutputType.Heading, "\nCommands:");
            output.WriteOutput(OutputType.Info, "  for individual command help run hsbuild <command> --help");
            PrintCommandShortHelp(output, ListCommand.Name, ListCommand.Description);
            PrintCommandShortHelp(output, UpdateCommand.Name, UpdateCommand.Description);
            PrintCommandShortHelp(output, BuildCommand.Name, BuildCommand.Description);
            PrintCommandShortHelp(output, HelpCommand.Name, HelpCommand.Description);
            PrintCommandShortHelp(output, ShellCommand.Name, ShellCommand.Description);

            //output.WriteOutput(OutputType.Normal, "\tclean:");
            //output.WriteOutput(OutputType.Normal, "\tinfo:");
        }

        protected override void Execute(ITaskQueue taskQueue, IModuleSetLoader loader, IOutputEngine output)
        {
            if (ErrorMsg != null)
                output.WriteOutput(OutputType.Error, ErrorMsg);

            output.WriteOutput(OutputType.Normal, "HSBuild " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            output.WriteOutput(OutputType.Info, "  by Haakon Sporsheim <haakon.sporsheim@gmail.com>");

            if (m_cmd != null)
                m_cmd.PrintHelp(output);
            else
                this.PrintHelp(output);
        }

        private Command m_cmd = null;
        private Exception m_ex = null;
        public string ErrorMsg { get { return m_ex == null ? null : m_ex.Message; } }
    }
}
