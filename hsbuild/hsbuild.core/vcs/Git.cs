// HSBuild.VCS - Git support
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
using System.IO;
using HSBuild.Core;
using HSBuild.Tasks;

namespace HSBuild.VCS
{
    public class GitRepository : Repository
    {
        internal GitRepository(string name, string href)
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

        internal override Branch GetRemoteBranch(ModuleVCSBranch branch, string checkoutroot)
        {
            return new GitBranch(this, branch.Module, branch.Revision, checkoutroot, branch.CheckoutDir);
        }
    }

    public class GitBranch : Branch
    {
        public GitBranch(GitRepository repo, string mod, string rev, string root, string dir)
            : base(mod, rev, root, dir)
        {
            m_repo = repo;

            m_gitEXE = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), "Git\\bin\\git.exe");
        }

        public override bool Exists(bool remote)
        {
            if (remote)
                throw new NotImplementedException("Remote exists");
            else
                return Directory.Exists(Path.Combine(BranchRoot, ".git"));
        }

        protected override void Checkout(ITaskQueue taskQueue)
        {
            Uri uri = new Uri(new Uri(m_repo.HRef), m_module);
            taskQueue.QueueTask(new ConsoleTask(m_gitEXE, new string[] { "clone", uri.AbsoluteUri, m_checkoutDir }, m_checkoutRoot));
        }

        protected override void Update(ITaskQueue taskQueue)
        {
            taskQueue.QueueTask(new ConsoleTask(m_gitEXE, new string[] { "pull", "--rebase" }, BranchRoot));
            taskQueue.QueueTask(new ConsoleTask(m_gitEXE, new string[] { "checkout", m_revision }, BranchRoot));
        }

        protected override void ApplyPatch(ITaskQueue taskQueue, Patch patch, string local_patch)
        {
            taskQueue.QueueTask(new ConsoleTask(m_gitEXE, new string[] { "apply", "--whitespace=nowarn -p" + patch.Strip.ToString(), "\"" + local_patch + "\"" }, BranchRoot));
        }

        private GitRepository m_repo;
        private string m_gitEXE;
    }


}
