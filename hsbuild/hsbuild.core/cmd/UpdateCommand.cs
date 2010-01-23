// HSBuild.Commands - Update command
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
    public class UpdateCommand : Command
    {
        public UpdateCommand(Config cfg)
            : base(cfg)
        {
        }

        internal override OptionEntrySpec[] GetOptionEntrySpecs()
        {
            return null;
        }

        protected override void Execute(ITaskQueue taskQueue, IModuleSetLoader loader, IOutputEngine output)
        {
            if (!LoadModuleSetAndSetupModules(loader, output))
                return;

            LinkedListNode<Module> node = m_modulesList.First;

            while (node != null)
            {
                Module m = node.Value;
                Branch b = m.Repository.FindBranch(m.Branch, m.Id, Config.CheckoutRoot);
                if (b == null)
                    throw new NullReferenceException();
                b.SyncBranch(taskQueue, m.Branch.PatchQueue, output);
                node = node.Next;
            }
        }
    }
}
