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
        public HelpCommand()
            : base(null)
        {
        }

        public HelpCommand(Exception ex)
            : base(null)
        {
            m_ex = ex;
        }

        protected override void Execute(ITaskQueue taskQueue, IModuleSetLoader loader, IOutputEngine output)
        {
            if (ErrorMsg != null)
                output.WriteOutput(OutputType.Error, ErrorMsg);

            output.WriteOutput(OutputType.Normal, "HSBuild " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            output.WriteOutput(OutputType.Info, "  by Haakon Sporsheim <haakon.sporsheim@gmail.com>");
            output.WriteOutput(OutputType.Heading, "\nCommands:");
            output.WriteOutput(OutputType.Heading, "\tupdate");
            output.WriteOutput(OutputType.Normal, "\t\tUpdates specified branches and dependencies.");
            output.WriteOutput(OutputType.Heading, "\tlist");
            output.WriteOutput(OutputType.Normal, "\t\tLists modules found in module set file.");
            output.WriteOutput(OutputType.Heading, "\tbuild");
            output.WriteOutput(OutputType.Normal, "\t\tBuilds specified branches and dependencies.");
            output.WriteOutput(OutputType.Normal, "\t\tModules will also be fetched if they are nowhere to be found.");
            output.WriteOutput(OutputType.Heading, "\thelp");
            output.WriteOutput(OutputType.Normal, "\t\tPrints this help information.");
            output.WriteOutput(OutputType.Heading, "\tshell");
            output.WriteOutput(OutputType.Normal, "\t\tStarts a shell with environment configured by configuration used.");

            //output.WriteOutput(OutputType.Normal, "\tclean:");
            //output.WriteOutput(OutputType.Normal, "\tinfo:");
        }

        private Exception m_ex = null;
        public string ErrorMsg { get { return m_ex == null ? null : m_ex.Message; } }
    }
}
