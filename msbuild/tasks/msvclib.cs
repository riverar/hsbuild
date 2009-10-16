using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Oah.Tasks
{
  public sealed class MSVCLib : ToolTask
  {
    private const string MSVCLibToolName = "lib.exe";

    private ITaskItem[] sourceFiles;
    private ITaskItem[] libPaths;
    private ITaskItem[] libs;

    private string outputFile;
    private string defFile;

    private string machine;
    private string subSystem;
    private string errorReport;

    #region Tool properties

    [Required]
    public ITaskItem[] Sources
    {
      get { return this.sourceFiles; }
      set { this.sourceFiles = value; }
    }

    public ITaskItem[] AdditionalLibPaths
    {
      get { return this.libPaths; }
      set { this.libPaths = value; }
    }

    public ITaskItem[] AdditionalLibraries
    {
      get { return this.libs; }
      set { this.libs = value; }
    }

    [Required]
    public string OutputFile
    {
      get { return this.outputFile; }
      set { this.outputFile = value; }
    }

    public string DefFile
    {
      get { return this.defFile; }
      set { this.defFile = value; }
    }

    public string Machine
    {
      get { return this.machine; }
      set { this.machine = value; }
    }

    public string SubSystem
    {
      get { return this.subSystem; }
      set { this.subSystem = value; }
    }

    public string ErrorReport
    {
      get { return this.errorReport; }
      set { this.errorReport = value == null ? null : value.ToUpper(); }
    }

    #endregion

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
      builder.AppendSwitch("/NOLOGO");

      builder.AppendSwitchIfNotNull("/ERRORREPORT:", ErrorReport);

      return builder.ToString();
    }

    protected override string GenerateResponseFileCommands()
    {
      CommandLineBuilder builder = new CommandLineBuilder();

      builder.AppendSwitchIfNotNull("/OUT:", OutputFile);

      builder.AppendSwitchIfNotNull("/MACHINE:", Machine);
      builder.AppendSwitchIfNotNull("/SUBSYSTEM:", SubSystem);
      builder.AppendSwitchIfNotNull("/DEF:", DefFile);

      if (AdditionalLibPaths != null)
      {
        foreach (ITaskItem dir in AdditionalLibPaths)
          builder.AppendSwitchUnquotedIfNotNull("/LIBPATH:", dir);
      }

      builder.AppendFileNamesIfNotNull(AdditionalLibraries, " ");
      builder.AppendFileNamesIfNotNull(Sources, " ");

      return builder.ToString();
    }
  }
}
