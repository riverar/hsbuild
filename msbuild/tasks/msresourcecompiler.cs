// HSBuild.Tasks - MSResourceCompiler
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
  public sealed class MSResourceCompiler : ToolTask
  {
    private const string MSResourceCompilerToolName = "rc.exe";

    private ITaskItem sourceFile;
    private ITaskItem[] includeDirectories;
    private string[] preprocessors;
    private string[] options;
    private string outputFile;
    private bool verbose = false;

    #region Tool properties
    [Required]
    public ITaskItem SourceFile
    {
      get { return this.sourceFile; }
      set { this.sourceFile = value; }
    }

    public ITaskItem[] AdditionalIncludeDirectories
    {
      get { return this.includeDirectories; }
      set { this.includeDirectories = value; }
    }

    public string[] PreprocessorDefinitions
    {
      get { return this.preprocessors; }
      set { this.preprocessors = value; }
    }

    public string[] AdditionalOptions
    {
      get { return this.options; }
      set { this.options = value; }
    }

    [Required]
    public string OutputFile
    {
      get { return this.outputFile; }
      set { this.outputFile = value; }
    }

    public bool Verbose
    {
      get { return this.verbose; }
      set { this.verbose = value; }
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
      if (Verbose)
        builder.AppendSwitch("/v");

      if (PreprocessorDefinitions != null)
      {
        foreach (string dir in PreprocessorDefinitions)
          builder.AppendSwitchIfNotNull("/d", dir);
      }

      builder.AppendSwitchIfNotNull("", AdditionalOptions, " ");

      if (AdditionalIncludeDirectories != null)
      {
        foreach (ITaskItem dir in AdditionalIncludeDirectories)
          builder.AppendSwitchUnquotedIfNotNull("/i", dir);
      }

      builder.AppendSwitchIfNotNull("/fo", OutputFile);

      builder.AppendFileNameIfNotNull(SourceFile);

      return builder.ToString();
    }
  }
}
