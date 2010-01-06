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
using HSBuild.Core;

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

        internal override Branch FindBranch(ModuleBranch branch, string checkoutroot)
        {
            throw new NotImplementedException();
        }
    }

    public class TarballBranch : Branch
    {
        public TarballBranch(TarballRepository repo, string mod, string rev, string root)
            : base(mod, rev, root)
        {
            m_repo = repo;
        }

        public override bool Exists(bool remote)
        {
            throw new NotImplementedException();
        }

        protected override void Checkout(ITaskQueue taskQueue)
        {
            throw new NotImplementedException();
        }

        protected override void Update(ITaskQueue taskQueue)
        {
            throw new NotImplementedException();
        }

        protected override void ApplyPatch(ITaskQueue taskQueue, Patch patch, string local_patch)
        {
            throw new NotImplementedException();
        }

        private TarballRepository m_repo;
    }
}
