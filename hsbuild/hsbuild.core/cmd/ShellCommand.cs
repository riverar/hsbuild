﻿// HSBuild.Commands - Shell command
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
using HSBuild.Core;

namespace HSBuild.Commands
{
    public class ShellCommand : Command
    {
        public ShellCommand(Config cfg)
            : base(cfg)
        {
        }

        private static OptionEntrySpec[] ShellCommandOptionEntries =
        {
        };

        internal override OptionEntrySpec[] GetOptionEntrySpecs()
        {
            return ShellCommandOptionEntries;
        }

        public override void Execute(ITaskQueue taskqueue, IModuleSetLoader loader, IOutputEngine output)
        {
            output.WriteOutput(OutputType.Heading, "Starting HSBuild shell...");
            taskqueue.QueueTask(new ConsoleTask("cmd.exe", null, null));
        }
    }
}