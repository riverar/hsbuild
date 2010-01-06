// HSBuild.Core - Repository
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
using HSBuild.VCS;

namespace HSBuild.Core
{
    public abstract class Repository
    {
        internal static Repository ParseRepository(XmlNode node)
        {
            XmlAttribute name = node.Attributes["name"];
            XmlAttribute type = node.Attributes["type"];

            if (type != null && name != null)
            {
                XmlAttribute href = node.Attributes["href"];
                switch (type.Value.ToLower())
                {
                    case "git":
                        if (href != null)
                            return new GitRepository(name.Value, href.Value);
                        break;
                    case "bzr":
                        if (href != null)
                            return new BzrRepository(name.Value, href.Value);
                        break;
                    case "tarball":
                        if (href != null)
                            return new TarballRepository(name.Value, href.Value);
                        break;
                }
            }

            return null;
        }

        internal abstract Branch FindBranch(ModuleBranch branch, string checkoutroot);

        protected Repository(string name)
        {
            m_name = name;
        }

        public string Name
        {
            get
            {
                return m_name;
            }
        }

        private string m_name;
    }
}
