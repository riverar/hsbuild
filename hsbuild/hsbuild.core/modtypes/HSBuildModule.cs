// HSBuild.Modules - HSBuildModule
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
using System.Xml;
using System.Collections.Generic;
using HSBuild.Core;
using HSBuild.Tasks;

namespace HSBuild.Modules
{
    public class HSBuildModule : Module, IBuildableModule
    {
        internal static HSBuildModule ParseModule(string id, string[] deps, XmlElement xmlModule)
        {
            XmlNodeList branchList = xmlModule.GetElementsByTagName("branch");

            if (branchList == null || branchList.Count < 1)
                throw new NotImplementedException("TODO: Error.. missing branch tag in module.");
            else if (branchList.Count > 1)
                throw new NotImplementedException("TODO: Error.. only one branch is allowed for each module.");

            return new HSBuildModule(id, deps, xmlModule, ModuleBranch.ParseBranch(branchList[0] as XmlElement, id));
        }

        private HSBuildModule(string id, string[] deps, XmlElement xmlModule, ModuleBranch branch)
            : base(id, deps)
        {
            XmlAttribute proj = xmlModule.Attributes["projects"];
            if (proj != null)
                m_proj = proj.Value;

            m_branch = branch;
        }

        internal override bool BindRepository(Dictionary<string, Repository> repos)
        {
            return repos.TryGetValue(m_branch.Repository, out m_repository);
        }

        public void Update(ITaskQueue taskQueue, IOutputEngine output, Config config, bool onlyFirstTime)
        {
            Branch branch = Repository.FindBranch(Branch, Id, config.CheckoutRoot);
            if (branch == null)
                throw new NullReferenceException();

            if (!onlyFirstTime || !branch.Exists(false))
                branch.SyncBranch(taskQueue, Branch.PatchQueue, output);
        }

        public void Build(ITaskQueue taskQueue, IOutputEngine output, Config config, Dictionary<string, object> buildArgs)
        {
            Branch branch = Repository.FindBranch(Branch, Id, config.CheckoutRoot);
            if (string.IsNullOrEmpty(Projects))
                taskQueue.QueueTask(new MSBuildTask(buildArgs, config, null, branch.BranchRoot));
            else
                foreach (string proj in Projects.Split(';'))
                    taskQueue.QueueTask(new MSBuildTask(buildArgs, config, proj, branch.BranchRoot));
        }

        public Repository Repository
        {
            get
            {
                return m_repository;
            }
        }

        public ModuleBranch Branch
        {
            get
            {
                return m_branch;
            }
        }

        public string Projects
        {
            get { return m_proj; }
        }

        protected Repository m_repository;
        protected ModuleBranch m_branch;
        private string m_proj = null;
    }
}
