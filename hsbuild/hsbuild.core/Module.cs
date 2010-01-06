// HSBuild.Core - Module
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
using HSBuild.Modules;

namespace HSBuild.Core
{
    public abstract class Module
    {
        public abstract void Build(ITaskQueue taskQueue, IOutputEngine output, Branch branch, string platform, string configuration);
        internal static Module ParseModule(XmlElement module)
        {
            XmlAttribute id = module.Attributes["id"];
            XmlNodeList branchList = module.GetElementsByTagName("branch");

            if (id == null)
                throw new NotImplementedException("TODO: Error.. missing id attribute in module element");
            if (branchList == null || branchList.Count < 1)
                throw new NotImplementedException("TODO: Error.. missing branch tag in module.");
            else if (branchList.Count > 1)
                throw new NotImplementedException("TODO: Error.. only one branch is allowed for each module.");

            string[] deps = ParseDependencies(module.SelectNodes("dependencies/dep"));
            ModuleBranch branch = ModuleBranch.ParseBranch(branchList[0] as XmlElement, id.Value);
            switch (module.Name.ToLower())
            {
                case "hsbuildmodule":
                    return new HSBuildModule(id.Value, branch, deps, module.Attributes);
                default:
                    break;
            }

            return null;
        }

        internal Module(string id, ModuleBranch branch, string[] deps)
        {
            m_id = id;
            m_branch = branch;
            m_deps = deps != null ? new List<string>(deps) : new List<string>();
        }

        internal void SetRepository(Repository repo)
        {
            m_repository = repo;
        }

        #region Properties

        public string Id
        {
            get
            {
                return m_id;
            }
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

        public string[] Dependencies
        {
            get
            {
                return m_deps.ToArray();
            }
        }

        #endregion

        protected string m_id;
        protected Repository m_repository;
        protected ModuleBranch m_branch;
        protected List<string> m_deps;

        private static string[] ParseDependencies(XmlNodeList deps)
        {
            List<string> ret = new List<string>();
            if (deps != null)
            {
                foreach (XmlNode dep in deps)
                {
                    XmlAttribute package = dep.Attributes["package"];
                    if (package != null && !string.IsNullOrEmpty(package.Value))
                        ret.Add(package.Value);
                }
            }

            return ret.ToArray();
        }
    }
}
