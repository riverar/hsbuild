using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Oah.Tasks
{
  public sealed class PkgConfigQueryVariable : PkgConfigToolTask
  {
    private const string PkgConfigToolName = "pkg-config.exe";

    private ITaskItem[] packages;

    private string key;
    private string output;

    #region Tool properties

    [Required]
    public string Key
    {
      get { return this.key; }
      set { this.key = value; }
    }

    [Required]
    public ITaskItem[] Packages
    {
      get { return this.packages; }
      set { this.packages = value; }
    }

    [Output]
    public ITaskItem Value
    {
      get { return new TaskItem(output); }
    }

    #endregion

    protected override string ToolName
    {
      get { return PkgConfigToolName; }
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
      builder.AppendSwitchIfNotNull("--variable=", key);

      builder.AppendFileNamesIfNotNull(Packages, " ");

      return builder.ToString();
    }

    protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
    {
      output += singleLine;

      base.LogEventsFromTextOutput(singleLine, messageImportance);
    }
  }
}
