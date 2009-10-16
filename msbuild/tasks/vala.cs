using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Oah.Tasks
{
  public sealed class Vala : PkgConfigToolTask
  {
    private const string ValaToolName = "valac.exe";

    private ITaskItem[] sourceFiles;
    private List<ITaskItem> outputFiles;
    private ITaskItem[] packages;
    private ITaskItem[] vapidirs;
    private string basedir;
    private string outdir;
    private string library;
    private string headerFileName;
    private string vapiFileName;
    private bool quietMode = false;
    private bool verboseOutput = false;
    private bool debug = false;
    private bool keepTemp = false;
    private bool multithreadSupport = false;

    #region Tool properties
    [Required]
    public ITaskItem[] Sources
    {
      get { return this.sourceFiles; }
      set { this.sourceFiles = value; }
    }

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

        return new TaskItem(Path.Combine(outdir, HeaderFileName));
      }
    }

    [Output]
    public ITaskItem SymbolsFile
    {
      get
      {
        if (string.IsNullOrEmpty(Library))
          return null;

        return new TaskItem(Path.Combine(outdir, Library) + ".symbols");
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

    public ITaskItem[] Packages
    {
      get { return this.packages; }
      set { this.packages = value; }
    }

    public string BaseDirectory
    {
      get { return this.basedir; }
      set { this.basedir = value; }
    }

    public string OutputDirectory
    {
      get { return this.outdir; }
      set { this.outdir = value; }
    }

    public string Library
    {
      get { return this.library; }
      set { this.library = value; }
    }

    public ITaskItem[] VapiDirectories
    {
      get { return this.vapidirs; }
      set { this.vapidirs = value; }
    }

    public bool Quiet
    {
      get { return this.quietMode; }
      set { this.quietMode = value; }
    }

    public bool Debug
    {
      get { return this.debug; }
      set { this.debug = value; }
    }

    public bool Verbose
    {
      get { return this.verboseOutput; }
      set { this.verboseOutput = value; }
    }

    public bool KeepTemporaryFiles
    {
      get { return this.keepTemp; }
      set { this.keepTemp = value; }
    }

    public bool Thread
    {
      get { return this.multithreadSupport; }
      set { this.multithreadSupport = value; }
    }

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

      builder.AppendFileNamesIfNotNull(Sources, " ");

      return builder.ToString();
    }

    private void CreateOutputDirectoriesAndFiles()
    {
      outputFiles = new List<ITaskItem>(sourceFiles.Length);

      foreach (ITaskItem item in sourceFiles)
      {
        string spec = item.ItemSpec;

        if (Path.GetExtension(spec).ToLower() == ".vapi")
          continue;

        if (basedir != null)
        {
          if (spec.StartsWith(basedir))
          {
            spec = spec.Remove(0, basedir.Length).TrimStart(Path.DirectorySeparatorChar);
          }
          else
          {
            Log.LogWarning("Found vala file ({1}) outside basedir ({0})", basedir, spec);
          }
        }

        string filePath = outdir != null ? Path.Combine(outdir, spec) : spec;

        string cfile = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath)) + ".c";
        outputFiles.Add(new TaskItem(cfile));

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
