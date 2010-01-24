// HSBuild.Core - ModuleSourceLocation
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

namespace HSBuild.Core
{
    public class ModuleVCSBranch : ModuleSourceLocation
    {
        internal static ModuleVCSBranch ParseModuleVCSBranch(XmlNode branch, string moduleFallback)
        {
            XmlAttribute repo = branch.Attributes["repo"];
            if (repo == null)
                throw new NotImplementedException("TODO: Error.. branch must have repo attribute!");

            XmlAttribute module = branch.Attributes["module"];
            XmlAttribute checkoutdir = branch.Attributes["checkoutdir"];
            XmlAttribute revision = branch.Attributes["revision"];
            XmlAttribute version = branch.Attributes["version"];
            XmlNodeList patches = branch.SelectNodes("patches/patch");
            return new ModuleVCSBranch(repo.Value,
                module == null ? moduleFallback : module.Value,
                checkoutdir == null ? null : checkoutdir.Value,
                revision == null ? null : revision.Value,
                version == null ? null : version.Value,
                ParsePatches(patches));
        }

        private ModuleVCSBranch(string repo, string module, string checkoutdir, string revision, string version, Patch patchQueue)
            : base(module, checkoutdir != null ? checkoutdir : module.Replace(".tar.gz", ""), patchQueue)
        {
            m_repoName = repo;
            m_revision = revision;
            m_version = version;
        }

        internal override bool BindRepository(Dictionary<string, Repository> repos)
        {
            return repos.TryGetValue(Repository, out m_repository);
        }

        internal override void Update(ITaskQueue taskQueue, IOutputEngine output, Config config, bool onlyFirstTime)
        {
            Branch branch = m_repository.GetRemoteBranch(this, config.CheckoutRoot);

            if (!onlyFirstTime || !branch.Exists(false))
                branch.SyncBranch(taskQueue, PatchQueue, output);
        }

        public string Repository
        {
            get
            {
                return m_repoName;
            }
        }

        public string Revision
        {
            get
            {
                return m_revision;
            }
        }

        public string Version
        {
            get
            {
                return m_version;
            }
        }

        private string m_repoName;
        private string m_revision;
        private string m_version;
        private Repository m_repository;
    }

    public abstract class ModuleSourceLocation
    {
        internal static ModuleSourceLocation ParseSourceLocation(XmlElement xmlModule, string moduleFallback)
        {
            foreach (XmlNode node in xmlModule.ChildNodes)
            {
                if (node.Name.Equals("branch", StringComparison.OrdinalIgnoreCase))
                    return ModuleVCSBranch.ParseModuleVCSBranch(node, moduleFallback);
            }

            return null;
        }

        internal abstract bool BindRepository(Dictionary<string, Repository> repos);
        internal abstract void Update(ITaskQueue taskQueue, IOutputEngine output, Config config, bool onlyFirstTime);

        protected static Patch ParsePatches(XmlNodeList patches)
        {
            if (patches == null || patches.Count <= 0)
                return null;

            Patch ret = null;
            for (int i = patches.Count - 1; i >= 0; --i)
            {
                XmlNode p = patches[i];
                XmlAttribute f = p.Attributes["file"];
                XmlAttribute u = p.Attributes["uri"];

                Uri uri;
                if (u != null)
                    uri = new Uri(u.Value);
                else if (f != null)
                {
                    if (System.IO.Path.IsPathRooted(f.Value))
                        uri = new Uri(f.Value, UriKind.Absolute);
                    else
                        uri = new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, f.Value), UriKind.Absolute);
                }
                else
                    throw new NotImplementedException("TODO: Error.. patch is missing uri/file attribute.");

                XmlAttribute s = p.Attributes["strip"];
                ret = new Patch(uri, s == null ? null : s.Value, ret);
            }

            return ret;
        }

        protected ModuleSourceLocation(string module, string checkoutdir, Patch patchQueue)
        {
            m_module = module;
            m_checkoutDir = checkoutdir;
            m_patch = patchQueue;
        }

        private string m_module;
        private string m_checkoutDir;
        private Patch m_patch;

        public string Module
        {
            get
            {
                return m_module;
            }
        }

        public string CheckoutDir
        {
            get
            {
                return m_checkoutDir;
            }
        }

        public Patch PatchQueue
        {
            get
            {
                return m_patch;
            }
        }
    }
}
