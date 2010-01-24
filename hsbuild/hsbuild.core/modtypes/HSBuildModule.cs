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
using System.IO;
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
            ModuleSourceLocation msl = ModuleSourceLocation.ParseSourceLocation(xmlModule, id);

            return new HSBuildModule(id, deps, xmlModule, msl);
        }

        private HSBuildModule(string id, string[] deps, XmlElement xmlModule, ModuleSourceLocation location)
            : base(id, deps)
        {
            XmlAttribute proj = xmlModule.Attributes["projects"];
            if (proj != null)
                m_proj = proj.Value;

            m_location = location;
        }

        internal override bool BindRepository(Dictionary<string, Repository> repos)
        {
            return Location.BindRepository(repos);
        }

        public void Update(ITaskQueue taskQueue, IOutputEngine output, Config config, bool onlyFirstTime)
        {
            Location.Update(taskQueue, output, config, onlyFirstTime);
        }

        public void Build(ITaskQueue taskQueue, IOutputEngine output, Config config, Dictionary<string, object> buildArgs)
        {
            string localLocation = Path.Combine(config.CheckoutRoot, Location.CheckoutDir);

            if (string.IsNullOrEmpty(Projects))
                taskQueue.QueueTask(new MSBuildTask(buildArgs, config, null, localLocation));
            else
                foreach (string proj in Projects.Split(';'))
                    taskQueue.QueueTask(new MSBuildTask(buildArgs, config, proj, localLocation));
        }

        public ModuleSourceLocation Location
        {
            get
            {
                return m_location;
            }
        }

        public string Projects
        {
            get { return m_proj; }
        }

        protected ModuleSourceLocation m_location;
        private string m_proj = null;
    }
}
