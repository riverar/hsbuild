// HSBuild.Tasks - MSVCCompiler
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
  public sealed class MSVCCompiler : ToolTask
  {
    private const string MSVCCompilerToolName = "cl.exe";

    private ITaskItem[] sourceFiles;
    private ITaskItem[] inputfiles;
    private ITaskItem[] includeDirectories;
    private ITaskItem[] preprocessors;
    private ITaskItem[] forceIncludes;
    private ITaskItem[] forceUsing;
    private ITaskItem objectFileName;
    private ITaskItem programDataBaseFileName;
    private ITaskItem precompiledHeaderOutputFile;
    private ITaskItem assemblerListingLocation;
    private string[] options;
    private string errorReport;
    private string warningLevel;
    private bool treatWarningAsError = false;
    private string runtimeLibrary;
    private string optimization;
    private string favorSizeOrSpeed;
    private string inlineFunctionExpansion;
    private bool intrinsicFunctions = false;
    private bool minimalRebuild = false;
    private string debugInformationFormat;
    private string basicRuntimeChecks;
    private bool smallerTypeCheck = false;
    private bool bufferSecurityCheck = true;
    private bool runtimeTypeInfo = true;
    private bool stringPooling = false;
    private bool minimalRebuildFromTracking = true;

    #region Tool properties
    [Required]
    public ITaskItem[] SourceFiles
    {
      get { return this.sourceFiles; }
      set { this.sourceFiles = value; }
    }

    public ITaskItem[] AdditionalIncludeDirectories
    {
      get { return this.includeDirectories; }
      set { this.includeDirectories = value; }
    }

    public ITaskItem[] PreprocessorDefinitions
    {
      get { return this.preprocessors; }
      set { this.preprocessors = value; }
    }

    public ITaskItem[] ForceIncludes
    {
      get { return this.forceIncludes; }
      set { this.forceIncludes = value; }
    }

    public ITaskItem[] ForceUsing
    {
      get { return this.forceUsing; }
      set { this.forceUsing = value; }
    }

    public ITaskItem ObjectFileName
    {
      get { return this.objectFileName; }
      set { this.objectFileName = value; }
    }

    public ITaskItem ProgramDataBaseFileName
    {
      get { return this.programDataBaseFileName; }
      set { this.programDataBaseFileName = value; }
    }

    public ITaskItem PrecompiledHeaderOutputFile
    {
      get { return this.precompiledHeaderOutputFile; }
      set { this.precompiledHeaderOutputFile = value; }
    }

    public ITaskItem AssemblerListingLocation
    {
      get { return this.assemblerListingLocation; }
      set { this.assemblerListingLocation = value; }
    }

    public string[] AdditionalOptions
    {
      get { return this.options; }
      set { this.options = value; }
    }

    public string ErrorReport
    {
      get { return this.errorReport; }
      set { this.errorReport = value == null ? null : value.ToUpper(); }
    }

    public string WarningLevel
    {
      get { return this.warningLevel; }
      set { this.warningLevel = value; }
    }

    public bool TreatWarningAsError
    {
      get { return this.treatWarningAsError; }
      set { this.treatWarningAsError = value; }
    }

    public string RuntimeLibrary
    {
      get { return this.runtimeLibrary; }
      set { this.runtimeLibrary = value; }
    }

    public string Optimization
    {
      get { return this.optimization; }
      set { this.optimization = value; }
    }

    public string FavorSizeOrSpeed
    {
      get { return this.favorSizeOrSpeed; }
      set { this.favorSizeOrSpeed = value; }
    }

    public string InlineFunctionExpansion
    {
      get { return this.inlineFunctionExpansion; }
      set { this.inlineFunctionExpansion = value; }
    }

    public bool IntrinsicFunctions
    {
      get { return this.intrinsicFunctions; }
      set { this.intrinsicFunctions = value; }
    }

    public bool MinimalRebuild
    {
      get { return this.minimalRebuild; }
      set { this.minimalRebuild = value; }
    }

    public string DebugInformationFormat
    {
      get { return this.debugInformationFormat; }
      set { this.debugInformationFormat = value; }
    }

    public string BasicRuntimeChecks
    {
      get { return this.basicRuntimeChecks; }
      set { this.basicRuntimeChecks = value; }
    }

    public bool SmallerTypeCheck
    {
      get { return this.smallerTypeCheck; }
      set { this.smallerTypeCheck = value; }
    }

    public bool BufferSecurityCheck
    {
      get { return this.bufferSecurityCheck; }
      set { this.bufferSecurityCheck = value; }
    }

    public bool RuntimeTypeInfo
    {
      get { return this.runtimeTypeInfo; }
      set { this.runtimeTypeInfo = value; }
    }

    public bool StringPooling
    {
      get { return this.stringPooling; }
      set { this.stringPooling = value; }
    }

    public bool MinimalRebuildFromTracking
    {
      get { return this.minimalRebuildFromTracking; }
      set { this.minimalRebuildFromTracking = value; }
    }

    #endregion

    protected override string ToolName
    {
      get { return MSVCCompilerToolName; }
    }

    protected override string GenerateFullPathToTool()
    {
      if (String.IsNullOrEmpty(ToolPath))
      {
        return ToolName;
      }

      return Path.Combine(Path.GetFullPath(ToolPath), ToolName);
    }

    public override bool Execute()
    {
      inputfiles = GetFilteredSourceFiles();
      return base.Execute();
    }

    protected override bool SkipTaskExecution()
    {
     if (inputfiles == null || inputfiles.Length == 0)
     {
        if (sourceFiles != null && sourceFiles.Length > 0)
          Log.LogMessage("Skip due to Minimal Rebuild tracking");

        return true;
      }

      return base.SkipTaskExecution();
    }

    protected override string GenerateCommandLineCommands()
    {
      CommandLineBuilder builder = new CommandLineBuilder();
      builder.AppendSwitch("/nologo");
      builder.AppendSwitch("/c");
      builder.AppendSwitchIfNotNull("/errorReport:", ErrorReport);

      return builder.ToString();
    }

    protected override string GenerateResponseFileCommands()
    {
      CommandLineBuilder builder = new CommandLineBuilder();

      builder.AppendSwitch(GetWarningLevelSwitch());
      if (TreatWarningAsError)
        builder.AppendSwitch("/WX");

      if (PreprocessorDefinitions!= null)
      {
        foreach (ITaskItem dir in PreprocessorDefinitions)
          builder.AppendSwitchIfNotNull("/D", dir);
      }

      builder.AppendSwitch(GetOptimizationSwitch());
      builder.AppendSwitch(GetFavorSizeOrSpeedSwitch());
      builder.AppendSwitch(GetInlineFunctionExpansionSwitch());
      if (IntrinsicFunctions)
        builder.AppendSwitch("/Oi");

      builder.AppendSwitch(GetRunTimeLibrarySwitch());

      if (MinimalRebuild)
        builder.AppendSwitch("/Gm");

      if (StringPooling)
        builder.AppendSwitch("/GF");

      if (!BufferSecurityCheck)
        builder.AppendSwitch("/GS-");

      if (!RuntimeTypeInfo)
        builder.AppendSwitch("/GR-");

      if (SmallerTypeCheck)
        builder.AppendSwitch("/RTCc");
      builder.AppendSwitch(GetBasicRuntimeChecksSwitch());
      builder.AppendSwitch(GetDebugInformationFormatSwitch());
      builder.AppendSwitchIfNotNull("", AdditionalOptions, " ");

      builder.AppendSwitchIfNotNull("/Fo", ObjectFileName);
      builder.AppendSwitchIfNotNull("/Fd", ProgramDataBaseFileName);
      builder.AppendSwitchIfNotNull("/Fp", PrecompiledHeaderOutputFile);
      builder.AppendSwitchIfNotNull("/Fa", AssemblerListingLocation);

      if (ForceIncludes != null)
      {
        foreach (ITaskItem inc in ForceIncludes)
          builder.AppendSwitchIfNotNull("/FI", inc);
      }

      if (ForceUsing != null)
      {
        foreach (ITaskItem _using in ForceUsing)
          builder.AppendSwitchIfNotNull("/FU", _using);
      }

      if (AdditionalIncludeDirectories != null)
      {
        foreach (ITaskItem dir in AdditionalIncludeDirectories)
          builder.AppendSwitchUnquotedIfNotNull("/I", dir);
      }

      builder.AppendFileNamesIfNotNull(inputfiles, " ");

      return builder.ToString();
    }

    private ITaskItem[] GetFilteredSourceFiles()
    {
      if (MinimalRebuildFromTracking)
      {
        List<ITaskItem> ret = new List<ITaskItem>();
        HSBuild.MSF.MR.Engine engine;

        try
        {
          FileInfo mrdbfile = FindMrDatabaseFile();
          if (mrdbfile == null || !mrdbfile.Exists)
            return sourceFiles;

          Log.LogMessage(MessageImportance.Low, "Using Minimal Rebuild Database {0}", mrdbfile.FullName);
          engine = new HSBuild.MSF.MR.Engine(mrdbfile.FullName);
        }
        catch (Exception ex)
        {
          Log.LogWarningFromException(ex);
          return sourceFiles;
        }

        string dueTo;
        foreach (var item in sourceFiles)
        {
          if (!engine.IsFileUpToDate(item.ItemSpec, out dueTo))
          {
            ret.Add(item);
            Log.LogMessage(MessageImportance.Low, "Compile {0} due to {1}", item.ItemSpec, dueTo);
          }
          else
            Log.LogMessage(MessageImportance.Low, "Skip due to MR {0}", item.ItemSpec);
        }

        engine.Dispose();
        return ret.ToArray();
      }

      return sourceFiles;
    }

    private FileInfo FindMrDatabaseFile()
    {
      DirectoryInfo mrdbdir;

      if (Directory.Exists(ObjectFileName.ItemSpec))
        mrdbdir = new DirectoryInfo(ObjectFileName.ItemSpec);
      else
      {
        mrdbdir = new DirectoryInfo(Path.GetDirectoryName(ObjectFileName.ItemSpec));
        if (!mrdbdir.Exists)
          mrdbdir = new DirectoryInfo(Environment.CurrentDirectory);
      }

      var files = mrdbdir.GetFiles("*.idb");
      if (files.Length == 0)
      {
        Log.LogMessage(MessageImportance.Normal,"Could not find Minimal Rebuild Database in {0}", mrdbdir.FullName);
        return null;
      }

      if (files.Length > 1)
        Log.LogWarning("Found more than one .idb file in {0}", mrdbdir.FullName);

      return files[0];
    }

    private string GetWarningLevelSwitch()
    {
      if (string.IsNullOrEmpty(WarningLevel))
        return "";

      switch (WarningLevel.ToLower())
      {
        case "level0":
          return "/W0";
        case "level1":
          return "/W1";
        case "level2":
          return "/W2";
        case "level3":
          return "/W3";
        case "level4":
          return "/W4";
        case "all":
          return "/Wall";
        default:
          Log.LogError("WarningLevel not recognized: {0}", WarningLevel);
          break;
      }

      return "";
    }

    private string GetOptimizationSwitch()
    {
      if (string.IsNullOrEmpty(Optimization))
        return "";

      switch (Optimization.ToLower())
      {
        case "disabled":
          return "/Od";
        case "full":
          return "/Ox";
        case "minsize":
          return "/O1";
        case "maxspeed":
          return "/O2";
        default:
          Log.LogError("Optimization not recognized: {0}", Optimization);
          break;
      }

      return "";
    }

    private string GetFavorSizeOrSpeedSwitch()
    {
      if (string.IsNullOrEmpty(FavorSizeOrSpeed))
        return "";

      switch (FavorSizeOrSpeed.ToLower())
      {
        case "neither":
          break;
        case "size":
          return "/Os";
        case "speed":
          return "/Ot";
        default:
          Log.LogError("FavorSizeOrSpeed not recognized: {0}", FavorSizeOrSpeed);
          break;
      }

      return "";
    }

    private string GetInlineFunctionExpansionSwitch()
    {
      if (string.IsNullOrEmpty(InlineFunctionExpansion))
        return "";

      switch (InlineFunctionExpansion.ToLower())
      {
        case "default":
          return "";
        case "disabled":
          return "/Ob0";
        case "onlyexplicitinline":
          return "/Ob1";
        case "anysuitable":
          return "/Ob2";
        default:
          Log.LogError("InlineFunctionExpansion not recognized: {0}", InlineFunctionExpansion);
          break;
      }

      return "";
    }

    private string GetRunTimeLibrarySwitch()
    {
      if (string.IsNullOrEmpty(RuntimeLibrary))
        return "";

      switch (RuntimeLibrary.ToLower())
      {
        case "multithreadeddll":
          return "/MD";
        case "multithreadeddebugdll":
          return "/MDd";
        case "multithreaded":
          return "/MT";
        case "multithreadeddebug":
          return "/MTd";
        default:
          Log.LogError("RuntimeLibrary not recognized: {0}", RuntimeLibrary);
          break;
      }

      return "";
    }

    private string GetBasicRuntimeChecksSwitch()
    {
      if (string.IsNullOrEmpty(BasicRuntimeChecks))
        return "";

      switch (BasicRuntimeChecks.ToLower())
      {
        case "disabled":
          break;
        case "uninitvariablesruntimecheck":
          return "/RTCu";
        case "stackframeruntimecheck":
          return "/RTCs";
        case "enablefastchecks":
          return "/RTC1";
        default:
          Log.LogError("BasicRuntimeChecks not recognized: {0}", BasicRuntimeChecks);
          break;
      }

      return "";
    }

    private string GetDebugInformationFormatSwitch()
    {
      if (string.IsNullOrEmpty(DebugInformationFormat))
        return "";

      switch (DebugInformationFormat.ToLower())
      {
        case "disabled":
          break;
        case "oldstyleinfo":
        case "symbolic":
          return "/Z7";
        case "programdatabase":
          return "/Zi";
        case "editandcontinue":
          return "/ZI";
        default:
          Log.LogError("DebugInformationFormat not recognized: {0}", DebugInformationFormat);
          break;
      }

      return "";
    }

  }
}
