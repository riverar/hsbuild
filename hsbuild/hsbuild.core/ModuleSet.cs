// HSBuild.Core - ModuleSet
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
        public ModuleSet(string filename)
            : this()
        {
            if (!Path.IsPathRooted(filename))
                filename = Path.Combine(Environment.CurrentDirectory, filename);

            included.Add(new Uri(filename));
            ReadModuleSet(new StreamReader(filename), Path.GetDirectoryName(filename));
        }

        public ModuleSet(TextReader reader)
            : this()
        {
            ReadModuleSet(reader, Environment.CurrentDirectory);
        }

        protected ModuleSet()
        {
            repos = new Dictionary<string, Repository>();
            included = new List<Uri>();
            modules = new Dictionary<string, Module>();
        }

        protected void ReadModuleSet(TextReader reader, string basedir)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            AddModuleSetXmlDocument(doc, basedir);

            // Bind repository for each module, if needed
            foreach (Module m in modules.Values)
            {
                if (!m.BindRepository(repos))
                    throw new Exception(string.Format("Unable to bind repository for module {0}.", m.Id));
            }
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

        private void ParseInclude(XmlNode include, string basedir)
        {
            XmlAttribute href = include.Attributes["href"];
            if (href == null || string.IsNullOrEmpty(href.Value))
            {
                // TODO: implement ModuleSetIncludeException
                throw new Exception("Include missing href attribute.");
            }

            Uri incUri;
            try
            {
                incUri = new Uri(href.Value);
            }
            catch
            {
                incUri = new Uri(Path.Combine(basedir, href.Value));
            }

            if (!incUri.IsFile)
            {
                throw new NotImplementedException("Support for System.Net.WebXXX not implemented yet.");
            }
            else if (!incUri.IsAbsoluteUri)
            {
                // TODO: implement ModuleSetIncludeException
                throw new Exception(string.Format("Non absolute Uri {0} not supported.", incUri.AbsolutePath));
            }

            // Detect loop
            if (included.Contains(incUri))
            {
                // TODO: Error/Warning
                return;
            }

            Stream stream;
            try
            {
                stream = new FileStream(incUri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch
            {
                // TODO: Error/Warning
                return;
            }

            included.Add(incUri);

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(stream);
            }
            finally
            {
                stream.Close();
            }

            AddModuleSetXmlDocument(doc, basedir);
        }

        private void AddModuleSetXmlDocument(XmlDocument doc, string basedir)
        {
            Dictionary<string, Repository> incRepos;
            Dictionary<string, Module> incModules;
            XmlNodeList incIncludes;

            try
            {
                incRepos = ParseRepositories(doc.GetElementsByTagName("repository"));
                incModules = ParseModules(GetModuleElements(doc));
                incIncludes = doc.GetElementsByTagName("include");
            }
            catch
            {
                // TODO: Error/warning
                return;
            }

            // Module set loaded, add it to repo and module lists
            //
            foreach (var r in incRepos)
            {
                if (!repos.ContainsKey(r.Key))
                    repos.Add(r.Key, r.Value);
                else
                {
                    // TODO: implement check whether or not they are equal. They should be!
                    //       If not, there will be a problem.
                }
            }

            if (incIncludes != null)
            {
                foreach (XmlNode inc in incIncludes)
                    ParseInclude(inc, basedir);
            }

            foreach (var m in incModules)
            {
                if (!modules.ContainsKey(m.Key))
                    modules.Add(m.Key, m.Value);
                else
                {
                    // TODO: Error!
                }
            }
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

        private List<Uri> included;
    }
}
