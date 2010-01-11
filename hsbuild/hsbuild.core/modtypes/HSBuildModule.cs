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
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32;
using HSBuild.Core;
using HSBuild.Tasks;

namespace HSBuild.Modules
{
    public class HSBuildModule : Module
    {
        internal HSBuildModule(string id, ModuleBranch branch, string[] deps, XmlAttributeCollection attribs)
            : base(id, branch, deps)
        {
            try
            {
                RegistryKey msbuildKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions\3.5");
                m_msbuildEXE = msbuildKey.GetValue("MSBuildToolsPath").ToString();
            }
            catch
            {
                m_msbuildEXE = "";
            }

            m_msbuildEXE = Path.Combine(m_msbuildEXE, "msbuild.exe");

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
            List<string> args = new List<string>();

            object platform, conf;
            buildArgs.TryGetValue("Platform", out platform);
            buildArgs.TryGetValue("Configuration", out conf);

            if (platform != null && !string.IsNullOrEmpty(platform.ToString()))
                args.Add("/p:Platform=" + platform.ToString());
            if (conf != null && !string.IsNullOrEmpty(conf.ToString()))
                args.Add("/p:Configuration=" + conf);
            // FIXME: add opt more switches...

            if (string.IsNullOrEmpty(Projects))
                taskQueue.QueueTask(new ConsoleTask(m_msbuildEXE, args.ToArray(), branch.BranchRoot));
            else
            {
                foreach (string proj in Projects.Split(';'))
                {
                    args.Add(proj);
                    taskQueue.QueueTask(new ConsoleTask(m_msbuildEXE, args.ToArray(), branch.BranchRoot));
                    args.RemoveAt(args.Count - 1);
                }
            }
        }

        private string m_msbuildEXE;
        private string m_proj = null;
    }
}
