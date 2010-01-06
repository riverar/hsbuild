// HSBuild.Core - Branch
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

namespace HSBuild.Core
{
    public abstract class Branch
    {
        protected Branch(string mod, string rev, string root)
        {
            m_module = mod;
            m_revision = rev;
            m_checkoutRoot = root;
        }

        public void SyncBranch(ITaskQueue taskQueue, Patch patchQueue, IOutputEngine output)
        {
            DirectoryInfo info = new DirectoryInfo(Path.Combine(m_checkoutRoot, m_module));

            if (info.Exists && this.Exists(false))
                this.Update(taskQueue);
            else
                this.Checkout(taskQueue);

            Patch patch = patchQueue;
            while (patch != null)
            {
                string local;
                if (patch.Uri.IsFile)
                {
                    local = patch.Uri.LocalPath;
                }
                else
                {
                    HttpTask task = new HttpTask(patch.Uri);
                    taskQueue.QueueTask(task);
                    local = task.LocalFile;
                }

                this.ApplyPatch(taskQueue, patch, local);
                patch = patch.Next;
            }
        }

        public virtual string BranchRoot { get { return Path.Combine(m_checkoutRoot, m_module); } }
        public abstract bool Exists(bool remote);

        protected abstract void Checkout(ITaskQueue taskQueue);
        protected abstract void Update(ITaskQueue taskQueue);
        protected abstract void ApplyPatch(ITaskQueue taskQueue, Patch patch, string local_patch);

        #region Properties
        #endregion

        protected string m_module;
        protected string m_revision;
        protected string m_checkoutRoot;
    }
}
