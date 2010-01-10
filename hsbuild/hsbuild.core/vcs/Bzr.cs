// HSBuild.VCS - Bazaar support
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
using HSBuild.Core;
using HSBuild.Tasks;

namespace HSBuild.VCS
{
    public class BzrRepository : Repository
    {
        internal BzrRepository(string name, string href)
            : base(name)
        {
            m_href = href;
        }

        private string m_href;
        public string HRef
        {
            get
            {
                return m_href;
            }
        }

        internal override Branch FindBranch(ModuleBranch branch, string modName, string checkoutroot)
        {
            return new BzrBranch(this, branch.Module, branch.Revision, checkoutroot);
        }
    }

    public class BzrBranch : Branch
    {
        public BzrBranch(BzrRepository repo, string mod, string rev, string root)
            : base(mod, rev, root)
        {
            m_repo = repo;
        }

        public override bool Exists(bool remote)
        {
            if (remote)
                throw new NotImplementedException("Remote exists");
            else
                return Directory.Exists(Path.Combine(BranchRoot, ".bzr"));
        }

        protected override void Checkout(ITaskQueue taskQueue)
        {
            Uri uri = new Uri(new Uri(m_repo.HRef), m_module);
            taskQueue.QueueTask(new ConsoleTask("bzr", new string[] { "branch", uri.AbsoluteUri }, m_checkoutRoot));
        }

        protected override void Update(ITaskQueue taskQueue)
        {
            taskQueue.QueueTask(new ConsoleTask("bzr", new string[] { "pull", "--overwrite", "-r", m_revision }, BranchRoot));
        }

        protected override void ApplyPatch(ITaskQueue taskQueue, Patch patch, string local_patch)
        {
            taskQueue.QueueTask(new ConsoleTask("bzr", new string[] { "patch", "--strip=" + patch.Strip.ToString(), local_patch }, BranchRoot));
        }

        private BzrRepository m_repo;
    }
}
