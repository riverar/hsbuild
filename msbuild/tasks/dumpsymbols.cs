// HSBuild.Tasks - DumpSymbols
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
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HSBuild.Tasks
{
    public sealed class DumpSymbols : ToolTask
    {
        private const string DumpBinToolName = "dumpbin.exe";

        private string m_tmpoutput;

        #region Tool properties
        [Required]
        public ITaskItem[] Sources { get; set; }
        [Required]
        public string SymbolsFile { get; set; }

        public bool MatchAllFilterExpressions { get; set; }
        public ITaskItem[] FilterExpressions { get; set; }
        #endregion

        protected override string ToolName
        {
            get { return DumpBinToolName; }
        }

        protected override string GenerateFullPathToTool()
        {
            if (String.IsNullOrEmpty(ToolPath))
            {
                return ToolName;
            }

            return Path.Combine(Path.GetFullPath(ToolPath), ToolName);
        }

        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilder builder = new CommandLineBuilder();
            builder.AppendSwitch("/symbols");
            builder.AppendSwitchIfNotNull("/OUT:", m_tmpoutput);
            builder.AppendFileNamesIfNotNull(Sources, " ");

            return builder.ToString();
        }

        public override bool Execute()
        {
            m_tmpoutput = Path.GetTempFileName();

            if (!base.Execute())
                return false;

            using (StreamReader reader = new StreamReader(m_tmpoutput))
            {
                if (!GenerateOutputFile(reader))
                    return false;
            }

            return true;
        }

        private bool FilterSymbol(string sym)
        {
            if (FilterExpressions == null)
                return true;

            foreach (ITaskItem item in FilterExpressions)
            {
                if (item == null || string.IsNullOrEmpty(item.ItemSpec))
                    continue;

                if (Regex.IsMatch(sym, item.ItemSpec, RegexOptions.Singleline))
                {
                    if (!MatchAllFilterExpressions)
                        return true;
                }
                else if (MatchAllFilterExpressions)
                    return false;
            }

            return MatchAllFilterExpressions;
        }

        private bool GenerateOutputFile(TextReader input)
        {
            string line;
            StringCollection symbols = new StringCollection();

            try
            {
                while ((line = input.ReadLine()) != null)
                {
                    StringCollection coll = new StringCollection();
                    coll.AddRange(line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));

                    int div = coll.IndexOf("|");
                    if (div < 2)
                        continue;

                    // Only read external symbols.
                    if (!coll[div - 1].Trim().Equals("external", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Dont expose imported (__imp__) symbols
                    if (!coll[div - 2].Trim().Equals("()", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Filter out symbols like @_FOO_bar
                    if (coll[div + 1].Trim()[0] != '_')
                        continue;

                    string sym = coll[div + 1].Trim().Substring(1);
                    if (!FilterSymbol(sym))
                    {
                        Log.LogMessage(MessageImportance.Low, "Skipping {0} symbol.", sym);
                        continue;
                    }

                    Log.LogMessage(MessageImportance.Low, "Found symbol: {0}", sym);
                    symbols.Add(sym);
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }

            StreamWriter output;
            try
            {
                output = new StreamWriter(SymbolsFile, false, Encoding.Default);
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, false, true, SymbolsFile);
                return false;
            }

            foreach (string sym in symbols)
                output.WriteLine("\t" + sym);
            output.Close();

            return true;
        }

    }
}
