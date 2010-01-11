// HSBuild.VCS - Tarball
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
    public class TarballRepository : Repository
    {
        internal TarballRepository(string name, string href)
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
            return new TarballBranch(this, branch.Module, modName, branch.Version, checkoutroot);
        }
    }

    public class TarballBranch : Branch
    {
        public TarballBranch(TarballRepository repo, string mod, string id, string ver, string root)
            : base(GetTarballName(id, ver), null, root)
        {
            if (mod == null)
                throw new ArgumentNullException("mod");
            if (id == null)
                throw new ArgumentNullException("id");

            m_repo = repo;
            m_uri = new Uri(new Uri(m_repo.HRef), mod);
        }

        private static string GetTarballName(string name, string version)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return (version == null) ? name : string.Format("{0}-{1}", name, version);
        }

        public override bool Exists(bool remote)
        {
            if (remote)
                throw new NotImplementedException("Remote exists");
            else
                return Directory.Exists(BranchRoot);
        }

        protected override void Checkout(ITaskQueue taskQueue)
        {
            string local;
            if (m_uri.IsFile)
            {
                local = m_uri.LocalPath;
            }
            else
            {
                HttpTask task = new HttpTask(m_uri);
                taskQueue.QueueTask(task);
                local = task.LocalFile;
            }

            taskQueue.QueueTask(new ArchiveTask(local, m_uri.AbsolutePath, m_checkoutRoot));
        }

        protected override void Update(ITaskQueue taskQueue)
        {
            if (!Exists(false))
                Checkout(taskQueue);
        }

        protected override void ApplyPatch(ITaskQueue taskQueue, Patch patch, string local_patch)
        {
            // FIXME: change Tarball::ApplyPatch to NOT use git apply!
            string gitEXE = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), "Git\\bin\\git.exe");
            taskQueue.QueueTask(new ConsoleTask(gitEXE, new string[] { "apply", "--whitespace=nowarn -p" + patch.Strip.ToString(), "\"" + local_patch + "\"" }, BranchRoot));
        }

        private TarballRepository m_repo;
        private Uri m_uri;
    }
}
