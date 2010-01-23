// HSBuild.Core - ModuleBranch
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

namespace HSBuild.Core
{
    public class ModuleBranch
    {
        internal static ModuleBranch ParseBranch(XmlElement branch, string moduleFallback)
        {
            if (branch == null)
                throw new ArgumentException("branch can not be null", "branch");
            if (!branch.Name.Equals("branch", StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("branch must have name: \"branch\"", "branch");

            XmlAttribute repo = branch.Attributes["repo"];
            if (repo == null)
                throw new NotImplementedException("TODO: Error.. branch must have repo attribute!");

            XmlAttribute module = branch.Attributes["module"];
            XmlAttribute checkoutdir = branch.Attributes["checkoutdir"];
            XmlAttribute revision = branch.Attributes["revision"];
            XmlAttribute version = branch.Attributes["version"];
            XmlNodeList patches = branch.SelectNodes("patches/patch");
            return new ModuleBranch(repo.Value,
                module == null ? moduleFallback : module.Value,
                checkoutdir == null ? null : checkoutdir.Value,
                revision == null ? null : revision.Value,
                version == null ? null : version.Value,
                ParsePatches(patches));
        }

        private static Patch ParsePatches(XmlNodeList patches)
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

        public ModuleBranch(string repo, string module, string checkoutdir, string revision, string version, Patch patchQueue)
        {
            m_repo = repo;
            m_module = module;
            m_checkoutDir = checkoutdir;
            m_revision = revision;
            m_version = version;
            m_patch = patchQueue;
        }

        private string m_repo;
        private string m_module;
        private string m_checkoutDir;
        private string m_revision;
        private string m_version;
        private Patch m_patch;

        public string Repository
        {
            get
            {
                return m_repo;
            }
        }

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

        public Patch PatchQueue
        {
            get
            {
                return m_patch;
            }
        }
    }
}
