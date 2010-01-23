// HSBuild.Core - Config
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
using System.Collections.Generic;
using Microsoft.Win32;

namespace HSBuild.Core
{
    public class Config
    {
        public static Config LoadFromFile(string filename)
        {
            return LoadFromTextReader(new StreamReader(filename));
        }

        public static Config LoadFromDirectory(string directory)
        {
            return LoadFromFile(Config.FindConfigFile(directory));
        }

        public static Config LoadFromCurrentDirectory()
        {
            return LoadFromDirectory(Environment.CurrentDirectory);
        }

        public static Config LoadFromTextReader(TextReader reader)
        {
            try
            {
                Config ret = new Config();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                }

                return ret;
            }
            catch //(Exception ex)
            {
                // TODO: error/warning
            }

            return null;
        }

        internal static Config CreateConfig(Dictionary<OptionEntrySpec, object> options)
        {
            Config ret = null;
            string cfgFile = GetOptionEntryDictionaryValue(options, "file");

            if (!string.IsNullOrEmpty(cfgFile))
            {
                if (File.Exists(cfgFile))
                {
                    ret = Config.LoadFromFile(cfgFile);
                }
                else
                {
                    // TODO: error/warning?
                }
            }
            else
            {
                List<string> configPossibilities = new List<string>(3);

                configPossibilities.Add(System.IO.Path.Combine(Environment.CurrentDirectory, Config.DefaultConfigFileName));
                configPossibilities.Add(Config.GetDefaultConfigFile());

                foreach (string cfg in configPossibilities)
                {
                    if (File.Exists(cfg))
                    {
                        ret = Config.LoadFromFile(cfg);
                        break;
                    }
                }
            }

            if (ret == null)
            {
                // TODO: warning?
                ret = Config.CreateDefaultConfig();
            }

            string moduleset = GetOptionEntryDictionaryValue(options, "moduleset");
            if (!string.IsNullOrEmpty(moduleset))
                ret.m_moduleSet = moduleset;

            return ret;
        }

        private static string GetOptionEntryDictionaryValue(Dictionary<OptionEntrySpec, object> options, string spec)
        {
            foreach (KeyValuePair<OptionEntrySpec, object> pair in options)
            {
                if (string.Compare(pair.Key.Specifier, spec, true) == 0)
                    return pair.Value.ToString();
            }

            return null;
        }

        public void OverrideModules(string[] modules)
        {
            m_modules = new List<string>(modules);
        }

        #region Properties
        public string ModuleSet
        {
            get
            {
                return m_moduleSet;
            }
        }

        public string[] Modules
        {
            get
            {
                return m_modules.ToArray();
            }
        }

        public string Prefix
        {
            get
            {
                return m_prefix;
            }
        }

        public string CheckoutRoot
        {
            get
            {
                return m_checkoutRoot;
            }
        }

        private string m_moduleSet;
        private List<string> m_modules;
        private string m_checkoutRoot;
        private string m_prefix = "__build__";
        #endregion

        public const string ModuleSetFileExt = ".modules";
        public const string DefaultModuleSetFileName = "hsbuild" + ModuleSetFileExt;
        public const string DefaultConfigFileName = ".hsbuildconf";
        public static string GetDefaultConfigFile()
        {
            return System.IO.Path.Combine(GetHomePath(), DefaultConfigFileName);
        }

        public static string GetHomePath()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                return Environment.GetEnvironmentVariable("HOME");
            else
                return Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
        }

        public static Config CreateDefaultConfig()
        {
            Config ret = new Config();
            ret.m_checkoutRoot = Environment.CurrentDirectory;
            ret.m_moduleSet = System.IO.Path.Combine(Environment.CurrentDirectory, DefaultModuleSetFileName);
            ret.m_modules = new List<string>();

            return ret;
        }

        private static string FindConfigFile(string directory)
        {
            return Path.Combine(directory, DefaultConfigFileName);
        }

        public void SetupEnvironment()
        {
            // Prefix
            string prx = (Path.IsPathRooted(Prefix) ? Prefix : Path.Combine(Environment.CurrentDirectory, Prefix)).Trim();
            if (!prx.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !prx.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                prx += Path.DirectorySeparatorChar.ToString();
            }

            Environment.SetEnvironmentVariable("HSBUILD_PREFIX_PATH", prx);

            // MSBuild in PATH
            var keyMSBuild = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions\3.5");
            if (keyMSBuild != null)
            {
                string frmwkdir = keyMSBuild.GetValue("MSBuildToolsPath").ToString();

                List<string> path = new List<string>(Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator));
                path.Insert(0, frmwkdir);
                Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator.ToString(), path.ToArray()));
            }
        }
    }
}
