// HSBuild.Tasks - MSBuildTask
//
// Copyright (C) 2010 Haakon Sporsheim <haakon.sporsheim@gmail.com>
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
using System.Collections.Generic;
using Microsoft.Win32;
using HSBuild.Core;

namespace HSBuild.Tasks
{
    public class MSBuildTask : ConsoleTask
    {
        public MSBuildTask(Dictionary<string, object> buildArgs, Config cfg, string project, string cwd)
            : base(m_MSBuildEXE, GenerateArguments(buildArgs, cfg, project), cwd)
        {
            m_project = project;
        }

        static string[] GenerateArguments(Dictionary<string, object> buildArgs, Config cfg, string project)
        {
            List<string> args = new List<string>();

            object platform, conf, none;
            buildArgs.TryGetValue("Platform", out platform);
            buildArgs.TryGetValue("Configuration", out conf);

            if (platform != null && !string.IsNullOrEmpty(platform.ToString()))
                args.Add("/p:Platform=" + platform.ToString());

            if (conf != null && !string.IsNullOrEmpty(conf.ToString()))
                args.Add("/p:Configuration=" + conf);
            else if (!string.IsNullOrEmpty(cfg.Configuration))
                args.Add("/p:Configuration=" + cfg.Configuration);

            if (buildArgs.TryGetValue("Verbose", out none))
                args.Add("/v:d");

            // FIXME: add opt more switches...

            if (!string.IsNullOrEmpty(project))
                args.Add(project);

            return args.ToArray();
        }

        protected override string GetOutputHeader()
        {
            if (string.IsNullOrEmpty(m_project))
                return "Build using MSBuild";
            else
                return string.Format("Build {0} using MSBuild", m_project);
        }

        static MSBuildTask()
        {
            string path;
            try
            {
                RegistryKey msbuildKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions\12.0");
                path = msbuildKey.GetValue("MSBuildToolsPath").ToString();
            }
            catch
            {
                path = "";
            }

            m_MSBuildEXE = Path.Combine(path, "msbuild.exe");
        }

        private static string m_MSBuildEXE;
        private string m_project;
    }
}
