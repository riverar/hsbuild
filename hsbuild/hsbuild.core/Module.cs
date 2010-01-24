// HSBuild.Core - Module
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
using HSBuild.Modules;

namespace HSBuild.Core
{
    public interface IUpdatableModule
    {
        void Update(ITaskQueue taskQueue, IOutputEngine output, Config config, bool onlyFirstTime);
    }

    public interface IBuildableModule : IUpdatableModule
    {
        void Build(ITaskQueue taskQueue, IOutputEngine output, Config config, Dictionary<string, object> args);
    }

    public abstract class Module
    {
        internal abstract bool BindRepository(Dictionary<string, Repository> repos);

        internal static Module ParseModule(XmlElement module)
        {
            XmlAttribute id = module.Attributes["id"];
            if (id == null || string.IsNullOrEmpty(id.Value))
                throw new NotImplementedException("TODO: Error.. missing id attribute in module element");

            string[] deps = ParseDependencies(module.SelectNodes("dependencies/dep"));

            switch (module.Name.ToLower())
            {
                case "module":
                    return new MetaModule(id.Value, deps);
                case "hsbuildmodule":
                    return HSBuildModule.ParseModule(id.Value, deps, module);
                default:
                    break;
            }

            return null;
        }

        internal Module(string id, string[] deps)
        {
            m_id = id;
            m_deps = deps != null ? new List<string>(deps) : new List<string>();
        }

        #region Properties

        public string Id
        {
            get
            {
                return m_id;
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
