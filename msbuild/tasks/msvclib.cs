// HSBuild.Tasks - MSVCLib
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
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace HSBuild.Tasks
{
  public sealed class MSVCLib : ToolTask
  {
    private const string MSVCLibToolName = "lib.exe";
    private string ltcg;

    #region Tool properties

    [Required]
    public ITaskItem[] Sources { get; set; }
    [Required]
    public string OutputFile { get; set; }

    public string LinkTimeCodeGeneration
    {
        get { return this.ltcg; }
        set { this.ltcg = value == null ? null : value.ToUpper(); }
    }

    public ITaskItem[] AdditionalLibraryDirectories { get; set; }
    public ITaskItem[] AdditionalDependencies { get; set; }
    public ITaskItem[] AdditionalOptions { get; set; }

    public string ErrorReporting { get; set; }
    public ITaskItem[] ForceSymbolReferences { get; set; }
    public bool IgnoreAllDefaultLibraries { get; set; }
    public ITaskItem[] IgnoreSpecificDefaultLibraries { get; set; }
    public string ModuleDefinitionFile { get; set; }
    public string Name { get; set; }
    public string TargetMachine { get; set; }
    public string SubSystem { get; set; }
    public bool SuppressStartupBanner { get; set; }
    public bool TreatLibWarningAsErrors { get; set; }
    public bool Verbose { get; set; }

    #endregion

    private string ParseTargetMachine()
    {
      if (string.IsNullOrEmpty(TargetMachine))
        return null;

      string m = TargetMachine.ToUpper();
      if (m.StartsWith("MACHINE"))
        m = m.Substring("MACHINE".Length);

      return m;
    }

    private string ParseErrorReporting()
    {
      if (string.IsNullOrEmpty(ErrorReporting))
        return null;

      string m = ErrorReporting.ToUpper();

      if (m == "NOERRORREPORT")
        return null;
      else if (m == "PROMPTIMMEDIATELY")
        m = "PROMPT";
      else if (m == "QUEUEFORNEXTLOGIN")
        m = "QUEUE";
      else if (m == "SENDERRORREPORT")
        m = "SEND";

        return m;
    }

    protected override string ToolName
    {
      get { return MSVCLibToolName; }
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
      if (SuppressStartupBanner)
        builder.AppendSwitch("/NOLOGO");

      builder.AppendSwitchIfNotNull("/ERRORREPORT:", ParseErrorReporting());

      return builder.ToString();
    }

    protected override string GenerateResponseFileCommands()
    {
      CommandLineBuilder builder = new CommandLineBuilder();

      builder.AppendSwitchIfNotNull("/OUT:", OutputFile);
      builder.AppendSwitchIfNotNull("/NAME:", Name);

      if (TreatLibWarningAsErrors)
          builder.AppendSwitch("/WX");
      if (Verbose)
          builder.AppendSwitch("/VERBOSE");
      builder.AppendSwitchIfNotNull("/MACHINE:", ParseTargetMachine());
      builder.AppendSwitchIfNotNull("/SUBSYSTEM:", SubSystem);
      builder.AppendSwitchIfNotNull("/LTCG:", LinkTimeCodeGeneration);
      builder.AppendSwitchIfNotNull("/DEF:", ModuleDefinitionFile);

      if (IgnoreAllDefaultLibraries)
        builder.AppendSwitch("/NODEFAULTLIB");
      else if (IgnoreSpecificDefaultLibraries != null)
      {
        foreach (ITaskItem lib in IgnoreSpecificDefaultLibraries)
          builder.AppendSwitchUnquotedIfNotNull("/NODEFAULTLIB:", lib);
      }

      if (ForceSymbolReferences != null)
      {
        foreach (ITaskItem symref in ForceSymbolReferences)
          builder.AppendSwitchUnquotedIfNotNull("/INCLUDE:", symref);
      }

      if (AdditionalLibraryDirectories != null)
      {
        foreach (ITaskItem dir in AdditionalLibraryDirectories)
          builder.AppendSwitchUnquotedIfNotNull("/LIBPATH:", dir);
      }

      builder.AppendFileNamesIfNotNull(AdditionalOptions, " ");
      builder.AppendFileNamesIfNotNull(AdditionalDependencies, " ");
      builder.AppendFileNamesIfNotNull(Sources, " ");

      return builder.ToString();
    }
  }
}
