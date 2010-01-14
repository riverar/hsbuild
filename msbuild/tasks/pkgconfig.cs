// HSBuild.Tasks - PkgConfig
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace HSBuild.Tasks
{
  public abstract class PkgConfigToolTask : ToolTask
  {
    private ITaskItem[] pkgcfgPaths;

    public ITaskItem[] PkgConfigPaths
    {
      get { return this.pkgcfgPaths; }
      set { this.pkgcfgPaths = value; }
    }

    protected override int ExecuteTool(string pathToTool, string responseFileCommands, string commandLineCommands)
    {
      if (this.pkgcfgPaths != null)
      {
        string oldEnv = Environment.GetEnvironmentVariable("PKG_CONFIG_PATH");

        List<string> lst = new List<string>(this.pkgcfgPaths.Length + 1);

        foreach (ITaskItem item in this.pkgcfgPaths)
        {
          if (!string.IsNullOrEmpty(item.ItemSpec))
            lst.Add(item.ItemSpec);
        }

        // search environment last
        if (!string.IsNullOrEmpty(oldEnv))
            lst.Add(oldEnv);

        string newEnv = string.Join(";", lst.ToArray());
        Log.LogMessage(MessageImportance.Low, "Set PKG_CONFIG_PATH from \"{0}\" to \"{1}\"", oldEnv, newEnv);
        Environment.SetEnvironmentVariable("PKG_CONFIG_PATH", newEnv);
        int ret = base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands);
        Environment.SetEnvironmentVariable("PKG_CONFIG_PATH", oldEnv);
        return ret;
      }

      return base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands);
    }
  }

  public sealed class PkgConfig : PkgConfigToolTask
  {
    private const string PkgConfigToolName = "pkg-config.exe";

    private ITaskItem[] packages;

    private List<ITaskItem> cIncludes;
    private List<ITaskItem> libs;
    private List<ITaskItem> libPaths;
    private string outputFlags;

    #region Tool properties

    [Required]
    public ITaskItem[] Packages
    {
      get { return this.packages; }
      set { this.packages = value; }
    }

    [Output]
    public ITaskItem[] CIncludes
    {
      get { return (this.cIncludes == null) ? null : this.cIncludes.ToArray(); }
    }

    [Output]
    public ITaskItem[] Libs
    {
      get { return (this.libs == null) ? null : this.libs.ToArray(); }
    }

    [Output]
    public ITaskItem[] LibPaths
    {
      get { return (this.libPaths == null) ? null : this.libPaths.ToArray(); }
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
      builder.AppendSwitch("--cflags");
      builder.AppendSwitch("--libs");

      builder.AppendFileNamesIfNotNull(Packages, " ");

      return builder.ToString();
    }

    protected override int ExecuteTool(string pathToTool, string responseFileCommands, string commandLineCommands)
    {
      this.cIncludes = new List<ITaskItem>();
      this.libs = new List<ITaskItem>();
      this.libPaths = new List<ITaskItem>();
      int ret = base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands);

      if (ret != 0)
      {
        this.cIncludes = null;
        this.libs = null;
        this.libPaths = null;
      }
      else
      {
        foreach (string arg in SplitPkgConfigOutput(outputFlags))
        {
          if (arg.StartsWith("-I"))
            AddIncludePath(arg.Substring(2));
          else if (arg.StartsWith("-L"))
            AddLibraryPath(arg.Substring(2));
          else if (arg.StartsWith("-l"))
            AddLibrary(arg.Substring(2));
        }
      }

      return ret;
    }

    private List<string> SplitPkgConfigOutput(string singleLine)
    {
      List<string> lst = new List<string>();

      for (int i = 0; i < singleLine.Length; )
      {
        while (i < singleLine.Length && char.IsWhiteSpace(singleLine[i])) i++;
        if (i >= singleLine.Length) break;

        int start = singleLine.IndexOfAny(new char[] {' ', '"'}, i);
        string item;

        if (start > i)
        {
          if (singleLine[start] == '"')
          {
            start = singleLine.IndexOf('"', start + 1);
            if (start <= i)
              start = singleLine.Length - 1;
          }

          item = singleLine.Substring(i, start - i + 1);
        }
        else
        {
          item = singleLine.Substring(i);
        }

        i += item.Length;
        lst.Add(item.Trim());
      }

      return lst;
    }

    private void AddIncludePath(string path)
    {
      this.cIncludes.Add(new TaskItem(path));
    }

    private void AddLibraryPath(string path)
    {
      this.libPaths.Add(new TaskItem(path));
    }

    private void AddLibrary(string lib)
    {
      if (Environment.OSVersion.Platform == PlatformID.Win32NT)
      {
        if (lib.Trim() == "m")
          return;
      }

      this.libs.Add(new TaskItem(lib));
    }

    protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
    {
      outputFlags += singleLine;

      base.LogEventsFromTextOutput(singleLine, messageImportance);
    }
  }
}
