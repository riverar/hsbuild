using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Oah.Tasks
{
  public sealed class MSManifest : ToolTask
  {
    private const string MSManifestToolName = "mt.exe";

    #region Tool properties
    [Required]
    public ITaskItem[] Sources { get; set; }

    [Required]
    public ITaskItem OutputManifestFile { get; set; }

    public ITaskItem[] AdditionalOptions { get; set; }
    public string AssemblyIdentity { get; set; }
    public bool GenerateCategoryTags { get; set; }
    public bool GenerateCatalogFiles { get; set; }
    public bool SuppressDependencyElement { get; set; }
    public bool SuppressStartupBanner { get; set; }
    public bool Verbose { get; set; }

    public bool UpdateFileHashes { get; set; }
    public ITaskItem UpdateFileHashesSearchPath { get; set; }

    public ITaskItem InputResourceManifests { get; set; }
    public ITaskItem OutputResourceManifests { get; set; }
    public ITaskItem ManifestFromManagedAssembly { get; set; }

    public ITaskItem TypeLibraryFile { get; set; }
    public ITaskItem RegistrarScriptFile { get; set; }
    public ITaskItem ComponentFileName { get; set; }
    public ITaskItem ReplacementsFile { get; set; }

    #endregion

    protected override string ToolName
    {
      get { return MSManifestToolName; }
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
          builder.AppendSwitch("/nologo");

      return builder.ToString();
    }

    protected override string GenerateResponseFileCommands()
    {
      CommandLineBuilder builder = new CommandLineBuilder();

      builder.AppendSwitchIfNotNull("/out:", OutputManifestFile);
      if (Verbose)
        builder.AppendSwitch("verbose");
      if (SuppressDependencyElement)
        builder.AppendSwitch("/nodependency");
      if (GenerateCategoryTags)
        builder.AppendSwitch("/category");
      if (GenerateCatalogFiles)
        builder.AppendSwitch("/makecdfs");

      if (UpdateFileHashes)
        builder.AppendSwitchIfNotNull("/hashupdate:", UpdateFileHashesSearchPath);

      builder.AppendSwitchIfNotNull("/inputresource:", InputResourceManifests);
      builder.AppendSwitchIfNotNull("/outputresource:", OutputResourceManifests);
      builder.AppendSwitchIfNotNull("/managedassemblyname:", ManifestFromManagedAssembly);
      builder.AppendSwitchIfNotNull("/identity:", AssemblyIdentity);
      builder.AppendSwitchIfNotNull("/rgs:", RegistrarScriptFile);
      builder.AppendSwitchIfNotNull("/tlb:", TypeLibraryFile);
      builder.AppendSwitchIfNotNull("/dll:", ComponentFileName);
      builder.AppendSwitchIfNotNull("/replacements:", ReplacementsFile);

      // Use notify_update and check return value if update needed.
      //builder.AppendSwitch("/notify_update");

      builder.AppendFileNamesIfNotNull(AdditionalOptions, " ");
      builder.AppendSwitchIfNotNull("/manifest ", Sources, " ");

      return builder.ToString();
    }
  }
}
