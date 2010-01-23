// HSBuild.Core - Command
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
using System.Collections.Generic;

namespace HSBuild.Core
{
    public enum OutputType
    {
        Debug,
        Info,
        Heading,
        Normal,
        Warning,
        Error
    }

    public interface IOutputEngine
    {
        void WriteOutput(OutputType type, string line);
    }

    public interface IModuleSetLoader
    {
        ModuleSet LoadModuleSet(Config config);
    }

    public abstract class Command
    {
        #region Properties
        public Config Config
        {
            get
            {
                return m_config;
            }
        }

        public Dictionary<string, object> Arguments
        {
            get
            {
                if (m_arguments == null || m_arguments.Count <= 0)
                    return new Dictionary<string, object>();

                Dictionary<string, object> ret = new Dictionary<string, object>(m_arguments.Count);

                foreach (KeyValuePair<OptionEntrySpec, object> spec in m_arguments)
                    ret.Add(spec.Key.Specifier, spec.Value);

                return ret;
            }
        }

        protected Config m_config;
        protected LinkedList<Module> m_modulesList;
        internal Dictionary<OptionEntrySpec, object> m_arguments;
        #endregion

        public Command(Config config)
        {
            m_config = config;
        }

        internal void SetParsedArguments(Dictionary<OptionEntrySpec, object> args)
        {
            m_arguments = args;
        }

        protected abstract void Execute(ITaskQueue taskQueue, IModuleSetLoader loader, IOutputEngine output);
        public void ExecuteCommand(ITaskQueue taskQueue, IModuleSetLoader loader, IOutputEngine output)
        {
            if (m_config != null)
            {
                if (string.IsNullOrEmpty(m_config.Filename))
                    output.WriteOutput(OutputType.Info, "Config file not found, using default configuration.");
                else
                    output.WriteOutput(OutputType.Info, string.Format("Using config file: {0}", m_config.Filename));

                output.WriteOutput(OutputType.Info, string.Format("Using module set file: {0}", m_config.ModuleSet));
            }

            Execute(taskQueue, loader, output);
        }

        internal virtual OptionEntrySpec[] GetOptionEntrySpecs()
        {
            return null;
        }

        protected ModuleSet LoadModuleSet(IModuleSetLoader loader, IOutputEngine output)
        {
            output.WriteOutput(OutputType.Debug, "Loading moduleset " + Config.ModuleSet);
            try
            {
                ModuleSet ret = loader.LoadModuleSet(Config);
                if (ret == null)
                    output.WriteOutput(OutputType.Error, string.Format("{0} (moduleset) not found...", Config.ModuleSet));
                return ret;
            }
            catch (Exception ex)
            {
                output.WriteOutput(OutputType.Error, string.Format("Moduleset {0} parse error...", Config.ModuleSet));
                output.WriteOutput(OutputType.Debug, ex.ToString());
                return null;
            }
            finally
            {
                output.WriteOutput(OutputType.Debug, "Loading (" + Config.ModuleSet + ") finished.");
            }
        }

        protected bool LoadModuleSetAndSetupModules(IModuleSetLoader loader, IOutputEngine output)
        {
            ModuleSet moduleSet = LoadModuleSet(loader, output);
            if (moduleSet == null)
                return false;

            m_modulesList = new LinkedList<Module>();
            try
            {
                FilterModuleList(Config.Modules, moduleSet, ref m_modulesList);
            }
            catch (Exception ex)
            {
                output.WriteOutput(OutputType.Error, "Modules not loaded (" + ex.Message + ")");
                output.WriteOutput(OutputType.Debug, ex.ToString());
                return false;
            }
            output.WriteOutput(OutputType.Debug, "Moduleset stack populated (Count: " + m_modulesList.Count + ")");
            return true;
        }

        protected void FilterModuleList(string[] modules, ModuleSet moduleSet, ref LinkedList<Module> modulesList)
        {
            foreach (string module in modules)
            {
                Module m;
                if (!moduleSet.Modules.TryGetValue(module, out m))
                {
                    throw new ModuleNotFoundException(module);
                }
                else if (!modulesList.Contains(m) && FilterModule(m))
                {
                    if (m.Dependencies != null && m.Dependencies.Length > 0)
                        FilterModuleList(m.Dependencies, moduleSet, ref modulesList);

                    m.SetRepository(moduleSet.Repositories[m.Branch.Repository]);
                    modulesList.AddLast(m);
                }
            }
        }

        protected virtual bool FilterModule(Module m)
        {
            // TODO: add skip modules list??
            return true;
        }
    }

}
