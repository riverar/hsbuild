using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Oah.Tasks
{
  public sealed class MSManifestResource : ToolTask
  {
    private const string MSResourceCompilerToolName = "rc.exe";

    private ITaskItem inputManifest;
    private ITaskItem outputResource;
    private bool dll = false;

    #region Tool properties
    [Required]
    public ITaskItem Manifest
    {
      get { return this.inputManifest; }
      set { this.inputManifest = value; }
    }

    [Required]
    public ITaskItem OutputResource
    {
      get { return this.outputResource; }
      set { this.outputResource = value; }
    }

    public bool Dll
    {
      get { return this.dll; }
      set { this.dll = value; }
    }
    #endregion

    protected override string ToolName
    {
      get { return MSResourceCompilerToolName; }
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
      builder.AppendSwitchIfNotNull("/fo", OutputResource);

      string tmpRc = CreateTemporaryRCFile();

      builder.AppendFileNameIfNotNull(tmpRc);

      return builder.ToString();
    }

    private string CreateTemporaryRCFile()
    {
      string file = Path.GetTempFileName();
      StreamWriter writer = null;

      try
      {
        writer = new StreamWriter(file, false, Encoding.Default);
        writer.Write(string.Format("{0} {1} \"{2}\"\n",
          (Dll ? "2" : "1"),
          "24",
          Manifest.ItemSpec.Replace("\\","\\\\")));
      }
      finally
      {
        if (writer != null)
          writer.Close();
      }

      return file;
    }
  }
}
