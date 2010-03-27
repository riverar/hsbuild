// HSBuild.Core - CommandLineParser
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
using System.Text;
using System.Collections.Generic;
using HSBuild.Commands;

namespace HSBuild.Core
{
    class OptionEntrySpec
    {
        public enum OptionType
        {
            None,
            String,
            Number,
            Filename,
        }

        public OptionEntrySpec(string spec, string shortName, string longName, OptionType type)
        {
            m_spec = spec;
            m_short = shortName;
            m_long = longName;
            m_type = type;
        }

        private string m_spec;
        private string m_short;
        private string m_long;
        private OptionType m_type;

        public string Specifier
        {
            get
            {
                return m_spec;
            }
        }

        public string ShortName
        {
            get
            {
                return m_short;
            }
        }

        public string LongName
        {
            get
            {
                return m_long;
            }
        }

        public OptionType Type
        {
            get
            {
                return m_type;
            }
        }
    }

    public class CommandLineParser
    {
        public static Command Parse(string[] args)
        {
            LinkedList<string> list;

            if (args == null)
                list = new LinkedList<string>();
            else
                list = new LinkedList<string>(args);

            Config cfg = Config.CreateConfig(ParseOptions(globalOptions, ref list));

            try
            {
                return ParseCommand(cfg, ref list);
            }
            catch (Exception ex)
            {
                return new HelpCommand(ex);
            }
        }

        private static OptionEntrySpec[] globalOptions =
        {
            new OptionEntrySpec("file", "-f", "--file", OptionEntrySpec.OptionType.Filename),
            new OptionEntrySpec("moduleset", "-m", "--moduleset", OptionEntrySpec.OptionType.Filename),
            new OptionEntrySpec("no-interact", null, "--no-interact", OptionEntrySpec.OptionType.None)
        };

        private static Dictionary<OptionEntrySpec, object> ParseOptions(OptionEntrySpec[] entries, ref LinkedList<string> list)
        {
            Dictionary<OptionEntrySpec, object> ret = new Dictionary<OptionEntrySpec, object>();

            while (list != null && list.Count > 0 && list.First.Value[0] == '-')
            {
                string option = list.First.Value;
                OptionEntrySpec spec = FindOption(ref option, entries);
                if (spec == null)
                    break;

                list.RemoveFirst();
                if (!string.IsNullOrEmpty(option))
                    list.AddFirst(option);
                ret.Add(spec, ParseOptionPayload(spec, option, ref list));
            }

            return ret;
        }

        private static OptionEntrySpec FindOption(ref string p, OptionEntrySpec[] entries)
        {
            foreach (OptionEntrySpec spec in entries)
            {
                if (!string.IsNullOrEmpty(spec.LongName) && p.StartsWith(spec.LongName, StringComparison.OrdinalIgnoreCase))
                {
                    p = p.Substring(spec.LongName.Length).TrimStart('=');
                    return spec;
                }

                if (!string.IsNullOrEmpty(spec.ShortName) && p.StartsWith(spec.ShortName, StringComparison.OrdinalIgnoreCase))
                {
                    p = p.Substring(spec.ShortName.Length).TrimStart('=');
                    return spec;
                }
            }

            return null;
        }

        private static object ParseOptionPayload(OptionEntrySpec spec, string payload, ref LinkedList<string> list)
        {
            object ret;
            switch (spec.Type)
            {
                case OptionEntrySpec.OptionType.Number:
                    ret = int.Parse(list.First.Value);
                    list.RemoveFirst();
                    break;
                case OptionEntrySpec.OptionType.String:
                case OptionEntrySpec.OptionType.Filename:
                    return ParseEscapedString(ref list);
                case OptionEntrySpec.OptionType.None:
                    return null;
                default:
                    throw new ArgumentException("spec.Type is corrupt??", "spec.Type");
            }

            return ret;
        }

        private static string ParseEscapedString(ref LinkedList<string> list)
        {
            StringBuilder builder = new StringBuilder();

            if (list.First.Value[0] == '"')
            {
                builder.Append(list.First.Value.Substring(1));
                list.RemoveFirst();

                while (list.Count > 0 && list.First.Value[list.First.Value.Length-1] != '"')
                {
                    builder.Append(' ');
                    builder.Append(list.First.Value);
                    list.RemoveFirst();
                }

                if (list.Count > 0)
                {
                    builder.Append(' ');
                    builder.Append(list.First.Value.Substring(0, list.First.Value.Length - 1));
                    list.RemoveFirst();
                }
            }
            else
            {
                builder.Append(list.First.Value);
                list.RemoveFirst();
            }

            return builder.ToString();
        }

        private static Command ParseCommand(Config config, ref LinkedList<string> list)
        {
            if (list.Count <= 0)
            {
                if (config.Modules == null || config.Modules.Length <= 0)
                    return new HelpCommand(config);
                else
                    return new BuildCommand(config);
            }

            Command ret = null;

            string cmdWord = list.First.Value;
            list.RemoveFirst();
            switch (cmdWord.ToLower())
            {
                case "--help":
                case "help":
                case "-h":
                    ret = new HelpCommand(config);
                    break;
                //case "clean":
                //    ret =  new CleanCommand(config);
                //    break;
                //case "info":
                //    ret =  new InfoCommand(config);
                //    break;
                case "list":
                    ret = new ListCommand(config);
                    break;
                case "shell":
                    ret = new ShellCommand(config);
                    break;
                case "update":
                    ret = new UpdateCommand(config);
                    break;
                case "build":
                    ret = new BuildCommand(config);
                    break;
                default:
                    throw new ArgumentException("Command word " + cmdWord + " not recognized.");
            }

            ret.SetParsedArguments(ParseOptions(ret.GetOptionEntrySpecs(), ref list));

            if (list.Count > 0)
            {
                string[] modules = ParseNonOptionWord(ref list);
                if (modules != null && modules.Length > 0)
                    config.OverrideModules(modules);
            }

            if (list.Count > 0)
            {
                // TODO: error reporting!
            }

            return ret;
        }

        private static string[] ParseNonOptionWord(ref LinkedList<string> list)
        {
            List<string> ret = new List<string>();

            while (list.Count > 0 && list.First.Value[0] != '-')
            {
                ret.AddRange(list.First.Value.Split(',', ';'));
                list.RemoveFirst();
            }

            return ret.ToArray();
        }
    }
}
