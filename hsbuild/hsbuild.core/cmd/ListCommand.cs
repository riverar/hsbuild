// HSBuild.Commands - List command
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
    public class ListCommand : Command
    {
        public ListCommand(Config cfg)
            : base(cfg)
        {
        }

        private static OptionEntrySpec[] ListCommandOptionEntries =
        {
            new OptionEntrySpec("all", "-a", "--all-modules", OptionEntrySpec.OptionType.None),
        };

        internal override OptionEntrySpec[] GetOptionEntrySpecs()
        {
            return ListCommandOptionEntries;
        }

        public override void Execute(ITaskQueue taskQueue, IModuleSetLoader loader, IOutputEngine output)
        {
            if (Arguments.ContainsKey(ListCommandOptionEntries[0].Specifier) ||
                Config.Modules == null || Config.Modules.Length <= 0)
            {
                ModuleSet moduleset = LoadModuleSet(loader, output);
                if (moduleset != null)
                {
                    output.WriteOutput(OutputType.Heading, "Modules:");
                    foreach (KeyValuePair<string, Module> kv in moduleset.Modules)
                        output.WriteOutput(OutputType.Normal, "\t" + kv.Key);
                }
            }
            else
            {
                if (LoadModuleSetAndSetupModules(loader, output))
                {
                    output.WriteOutput(OutputType.Heading, "Modules:");
                    foreach (Module m in m_modulesList)
                        output.WriteOutput(OutputType.Normal, "\t" + m.Id);
                }
            }
        }
    }
}
