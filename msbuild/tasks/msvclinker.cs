using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using
 Microsoft.Build.Framework;

namespace Oah.Tasks
{
  public sealed class MSVCLinker : ToolTask
  {
    private const string MSVCLinkerToolName = "link.exe";

    private ITaskItem[] sourceFiles;
    private ITaskItem[] libPaths;
    private ITaskItem[] libs;

    private string outputFile;
    private string defFile;
    private string impFile;
    private string manifestFile;

    private string machine;
    private string subSystem;    private string errorReport;

    private bool incremental = false;
    private bool dll = false;
    private bool debug = true;
    private bool pdb = true;
    private bool map = false;
    private bool checksum = false;

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

    public string ImpLibFile
    {
      get { return this.impFile; }
      set { this.impFile = value; }
    }

    public string ManifestFile
    {
      get { return this.manifestFile; }
      set { this.manifestFile = value; }
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

    public bool Incremental
    {
      get { return this.incremental; }
      set { this.incremental = value; }
    }

    public bool Dll
    {
      get { return this.dll; }
      set { this.dll = value; }
    }

    public bool Debug
    {
      get { return this.debug; }
      set { this.debug = value; }
    }

    public bool Pdb
    {
      get { return this.pdb; }
      set { this.pdb = value; }
    }

    public bool Map
    {
      get { return this.map; }
      set { this.map = value; }
    }

    public bool SetChecksum
    {
      get { return this.checksum; }
      set { this.checksum = value; }
    }

    #endregion

    protected override string ToolName
    {
      get { return MSVCLinkerToolName; }
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

      string filePathNoExt = OutputFile;
      if (filePathNoExt.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
          filePathNoExt.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
      {
        filePathNoExt = filePathNoExt.Substring(0, filePathNoExt.Length - 4);
      }

      if (Incremental)
        builder.AppendSwitch("/INCREMENTAL");
      if (Dll)
        builder.AppendSwitch("/DLL");
      if (Debug)
        builder.AppendSwitch("/DEBUG");
      if (SetChecksum)
        builder.AppendSwitch("/RELEASE");
      if (Map)
        builder.AppendSwitchIfNotNull("/MAP /MAPFILE:", filePathNoExt + ".map");
      if (Pdb)
      {
        if (!Debug)
          Log.LogWarning("Pdb enabled without required Debug switch.");
        builder.AppendSwitchIfNotNull("/PDB:", filePathNoExt + ".pdb");
      }

      builder.AppendSwitchIfNotNull("/MANIFEST /MANIFESTFILE:", ManifestFile);

      builder.AppendSwitchIfNotNull("/MACHINE:", Machine);
      builder.AppendSwitchIfNotNull("/SUBSYSTEM:", SubSystem);
      builder.AppendSwitchIfNotNull("/DEF:", DefFile);
      builder.AppendSwitchIfNotNull("/IMPLIB:", ImpLibFile);

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
