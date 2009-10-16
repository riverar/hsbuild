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

    private ITaskItem[] inputManifests;
    private ITaskItem outputManifest;

    #region Tool properties
    [Required]
    public ITaskItem[] InputManifest
    {
      get { return this.inputManifests; }
      set { this.inputManifests = value; }
    }

    [Required]
    public ITaskItem OutputManifest
    {
      get { return this.outputManifest; }
      set { this.outputManifest = value; }
    }
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
      builder.AppendSwitch("/nologo");

      return builder.ToString();
    }

    protected override string GenerateResponseFileCommands()
    {
      CommandLineBuilder builder = new CommandLineBuilder();

      builder.AppendSwitchIfNotNull("/out:", OutputManifest);
      // Use notify_update and check return value if update needed.
      //builder.AppendSwitch("/notify_update");
      builder.AppendSwitch("/manifest");
      builder.AppendFileNamesIfNotNull(InputManifest, " ");

      return builder.ToString();
    }
  }
}
