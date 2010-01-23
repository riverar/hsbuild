// HSBuild.Commands - Build command
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

namespace HSBuild.Commands
{
    public class BuildCommand : Command
    {
        public BuildCommand(Config cfg)
            : base(cfg)
        {
        }

        private static OptionEntrySpec[] BuildCommandOptionEntries =
        {
            new OptionEntrySpec("Configuration", "-c", "--configuration", OptionEntrySpec.OptionType.String),
            new OptionEntrySpec("Platform", "-p", "--platform", OptionEntrySpec.OptionType.String),
        };

        internal override OptionEntrySpec[] GetOptionEntrySpecs()
        {
            return BuildCommandOptionEntries;
        }

        protected override void Execute(ITaskQueue taskqueue, IModuleSetLoader loader, IOutputEngine output)
        {
            if (!LoadModuleSetAndSetupModules(loader, output))
                return;

            LinkedListNode<Module> node = m_modulesList.First;
            Dictionary<string, object> buildArgs = Arguments;

            while (node != null)
            {
                if (node.Value is IBuildableModule)
                {
                    IBuildableModule bm = node.Value as IBuildableModule;
                    bm.Update(taskqueue, output, Config, true);
                    bm.Build(taskqueue, output, Config, buildArgs);
                }

                node = node.Next;
            }
        }
    }
}
