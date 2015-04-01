// HSBuild.Tasks - Vala
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
  public sealed class Vala : PkgConfigToolTask
  {
    private const string ValaToolName = "valac-0.28.exe";

    private List<ITaskItem> outputFiles;
    private string headerFileName;
    private string vapiFileName;

    #region Tool properties
    [Required]
    public ITaskItem[] Sources { get; set; }

    [Output]
    public ITaskItem[] DestinationFiles
    {
      get { return this.outputFiles.ToArray(); }
    }

    public string HeaderFileName
    {
      set { this.headerFileName = value; }
      get
      {
        if (!string.IsNullOrEmpty(headerFileName))
          return headerFileName;

        if (!string.IsNullOrEmpty(Library))
          return Library + ".h";

        return null;
      }
    }

    public string InternalHeaderFileName { get; set; }

    public string InternalVapiFileName { get; set; }

    public string SymbolsFileName { get; set; }

    public string VapiFileName
    {
      set { this.vapiFileName = value; }
      get
      {
        if (!string.IsNullOrEmpty(vapiFileName))
          return vapiFileName;

        if (!string.IsNullOrEmpty(Library))
          return Library + ".vapi";

        return null;
      }
    }

    [Output]
    public ITaskItem HeaderFile
    {
      get
      {
        if (string.IsNullOrEmpty(HeaderFileName))
          return null;

        return new TaskItem(Path.Combine(OutputDirectory, HeaderFileName));
      }
    }

    [Output]
    public ITaskItem InternalHeaderFile
    {
      get
      {
        if (string.IsNullOrEmpty(InternalHeaderFileName))
          return null;

        return new TaskItem(Path.Combine(OutputDirectory, InternalHeaderFileName));
      }
    }

    [Output]
    public ITaskItem InternalVapiFile
    {
      get
      {
        if (string.IsNullOrEmpty(InternalVapiFileName))
          return null;

        return new TaskItem(Path.Combine(OutputDirectory, InternalVapiFileName));
      }
    }

    [Output]
    public ITaskItem SymbolsFile
    {
      get
      {
        if (string.IsNullOrEmpty(SymbolsFileName))
          return null;

        return new TaskItem(Path.Combine(OutputDirectory, SymbolsFileName));
      }
    }

    [Output]
    public ITaskItem VapiFile
    {
      get
      {
        if (string.IsNullOrEmpty(Library) && string.IsNullOrEmpty(vapiFileName))
          return null;

        string file = (string.IsNullOrEmpty(vapiFileName)) ? (Library + ".vapi") : vapiFileName;
        ITaskItem ret = new TaskItem(Path.Combine(OutputDirectory, file));

        // On output the vapi file will most likely be added to DataFile item group,
        // which by the SubDirectory meta data will be copied to the correct directory!
        ret.SetMetadata("SubDirectory", "vala/vapi");
        return ret;
      }
    }

    public ITaskItem[] Packages { get; set; }
    public string BaseDirectory { get; set; }
    public string OutputDirectory { get; set; }
    public string Library { get; set; }

    public ITaskItem[] VapiDirectories { get; set; }
    public bool Quiet { get; set; }
    public bool Debug { get; set; }
    public bool Verbose { get; set; }
    public bool KeepTemporaryFiles { get; set; }
    public bool Thread { get; set; }
    public ITaskItem[] Defines { get; set; }

    #endregion

    protected override string ToolName
    {
      get { return ValaToolName; }
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
      builder.AppendSwitch("-C");

      builder.AppendSwitchIfNotNull("-b ", BaseDirectory);
      builder.AppendSwitchIfNotNull("-d ", OutputDirectory);

      builder.AppendSwitchUnquotedIfNotNull("--vapi=", VapiFileName);
      builder.AppendSwitchUnquotedIfNotNull("--library=", Library);
      builder.AppendSwitchUnquotedIfNotNull("--header=", HeaderFile);
      builder.AppendSwitchUnquotedIfNotNull("--internal-header=", InternalHeaderFile);
      builder.AppendSwitchUnquotedIfNotNull("--internal-vapi=", InternalVapiFileName);
      builder.AppendSwitchUnquotedIfNotNull("--symbols=", SymbolsFile);

      if (Packages != null)
      {
        foreach (ITaskItem pkg in Packages)
          builder.AppendSwitchIfNotNull("--pkg=", pkg);
      }

      if (VapiDirectories != null)
      {
        foreach (ITaskItem dir in VapiDirectories)
          builder.AppendSwitchIfNotNull("--vapidir=", dir);
      }

      if (Verbose) builder.AppendSwitch("-v");
      if (Quiet) builder.AppendSwitch("-q");
      if (Debug) builder.AppendSwitch("-g");
      if (KeepTemporaryFiles) builder.AppendSwitch("--save-temps");
      if (Thread) builder.AppendSwitch("--thread");

      if (Defines != null)
      {
        foreach (ITaskItem def in Defines)
          builder.AppendSwitchIfNotNull("-D ", def);
      }

      builder.AppendFileNamesIfNotNull(Sources, " ");

      return builder.ToString();
    }

    private void CreateOutputDirectoriesAndFiles()
    {
      outputFiles = new List<ITaskItem>(Sources.Length);

      foreach (ITaskItem item in Sources)
      {
        string spec = item.ItemSpec;

        if (Path.GetExtension(spec).ToLower() == ".vapi")
          continue;

        if (BaseDirectory != null)
        {
          if (spec.StartsWith(BaseDirectory))
          {
              spec = spec.Remove(0, BaseDirectory.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
          }
          else
          {
              Log.LogWarning("Found vala file ({1}) outside basedir ({0})", BaseDirectory, spec);
          }
        }

        string filePath = string.IsNullOrEmpty(OutputDirectory) ? spec : Path.Combine(OutputDirectory, spec);

        string cfile = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath)) + ".c";
        outputFiles.Add(new TaskItem(cfile));

        Log.LogMessage(MessageImportance.Low, "Create {0} from {1}", cfile, spec);
        DirectoryInfo info = new DirectoryInfo(Path.GetDirectoryName(cfile));
        if (!info.Exists) info.Create();
      }
    }

    public override bool Execute()
    {
      CreateOutputDirectoriesAndFiles();

      return base.Execute();
    }
  }
}
