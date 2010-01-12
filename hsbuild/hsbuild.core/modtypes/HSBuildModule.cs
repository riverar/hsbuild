// HSBuild.Modules - HSBuildModule
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
using System.Xml;
using System.Collections.Generic;
using HSBuild.Core;
using HSBuild.Tasks;

namespace HSBuild.Modules
{
    public class HSBuildModule : Module
    {
        internal HSBuildModule(string id, ModuleBranch branch, string[] deps, XmlAttributeCollection attribs)
            : base(id, branch, deps)
        {
            XmlAttribute proj = attribs["projects"];
            if (proj != null)
                m_proj = proj.Value;
        }

        public string Projects
        {
            get { return m_proj; }
        }

        public override void Build(ITaskQueue taskQueue, IOutputEngine output, Branch branch, Dictionary<string, object> buildArgs, Config cfg)
        {
            if (string.IsNullOrEmpty(Projects))
                taskQueue.QueueTask(new MSBuildTask(buildArgs, cfg, null, branch.BranchRoot));
            else
                foreach (string proj in Projects.Split(';'))
                    taskQueue.QueueTask(new MSBuildTask(buildArgs, cfg, proj, branch.BranchRoot));
        }

        private string m_proj = null;
    }
}
