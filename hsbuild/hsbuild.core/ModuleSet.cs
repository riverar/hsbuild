// HSBuild.Core - ModuleSet
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
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace HSBuild.Core
{
    public class ModuleNotFoundException : Exception
    {
        public ModuleNotFoundException(string name)
            : base("Module '" + name + "' not found")
        {
        }
    }

    public class ModuleSet
    {
        public ModuleSet(TextReader reader)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            repos = ParseRepositories(doc.GetElementsByTagName("repository"));
            modules = ParseModules(GetModuleElements(doc));

            ParseIncludes(doc.GetElementsByTagName("include"));
        }

        private List<XmlElement> GetModuleElements(XmlDocument doc)
        {
            List<XmlElement> ret = new List<XmlElement>();
            XmlNode it = doc.DocumentElement.FirstChild;
            while (it != null)
            {
                if (it is XmlElement && it.Name.EndsWith("module", StringComparison.InvariantCultureIgnoreCase))
                    ret.Add(it as XmlElement);

                it = it.NextSibling;
            }
            return ret;
        }

        private Dictionary<string, Module> ParseModules(List<XmlElement> moduleList)
        {
            Dictionary<string, Module> ret = new Dictionary<string, Module>();

            if (moduleList != null)
            {
                foreach (XmlElement element in moduleList)
                {
                    Module mod = Module.ParseModule(element);
                    if (mod != null)
                    {
                        if (string.IsNullOrEmpty(mod.Id))
                            throw new NotImplementedException("TODO: Error.. module without id attribute.");
                        else if (ret.ContainsKey(mod.Id))
                            throw new NotImplementedException("TODO: Error.. module alrady present.");
                        else
                            ret.Add(mod.Id, mod);
                    }
                }
            }

            return ret;
        }

        private Dictionary<string, Repository> ParseRepositories(XmlNodeList repoList)
        {
            Dictionary<string, Repository> ret = new Dictionary<string, Repository>();

            if (repoList != null)
            {
                foreach (XmlNode node in repoList)
                {
                    Repository repo = Repository.ParseRepository(node);
                    if (repo != null)
                        ret.Add(repo.Name, repo);
                }
            }

            return ret;
        }

        private void ParseIncludes(XmlNodeList xmlNodeList)
        {
            //throw new NotImplementedException("TODO: Parse <include />");
        }

        #region Properties
        private Dictionary<string, Repository> repos;
        private Dictionary<string, Module> modules;

        public Dictionary<string, Repository> Repositories
        {
            get
            {
                return repos;
            }
        }

        public Dictionary<string, Module> Modules
        {
            get
            {
                return modules;
            }
        }

        #endregion
    }
}
