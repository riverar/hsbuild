// HSBuild.Tasks - MSVCLinker
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
  public sealed class MSVCLinker : ToolTask
  {
    private const string MSVCLinkerToolName = "link.exe";
    private string ltcg;

    #region Tool properties

    [Required]
    public ITaskItem[] Sources { get; set; }

    [Required]
    public string OutputFile { get; set; }

    public string LinkTimeCodeGeneration
    {
      get { return this.ltcg; }
      set { this.ltcg = value == null ? null : value.ToUpper(); }
    }

    public ITaskItem[] AdditionalLibraryDirectories { get; set; }
    public ITaskItem[] AdditionalDependencies { get; set; }
    public ITaskItem[] AdditionalOptions { get; set; }
    public string LinkErrorReporting { get; set; }
    public string ModuleDefinitionFile { get; set; }
    public string ImportLibrary { get; set; }
    public bool GenerateManifest { get; set; }
    public string MSDOSStubFileName { get; set; }
    public bool GenerateMapFile { get; set; }
    public string MapFileName { get; set; }
    public bool MapExports { get; set; }
    public string ProgramDatabaseFile { get; set; }
    public string StripPrivateSymbols { get; set; }
    public string TargetMachine { get; set; }
    public string SubSystem { get; set; }
    public bool SuppressStartupBanner { get; set; }
    public bool TreatLinkerWarningAsErrors { get; set; }
    public bool ImageHasSafeExceptionHandlers { get; set; }
    public bool LinkIncremental { get; set; }
    public bool LinkDll { get; set; }
    public bool GenerateDebugInformation { get; set; }
    public bool SetChecksum { get; set; }
    public bool? OptimizeReferences { get; set; }
    public bool? EnableCOMDATFolding { get; set; }
    public bool PreventDllBinding { get; set; }
    public bool AllowIsolation { get; set; }
    public bool LargeAddressAware { get; set; }
    public bool DataExecutionPrevention { get; set; }
    public string BaseAddress { get; set; }
    public bool FixedBaseAddress { get; set; }
    public bool RandomizedBaseAddress { get; set; }
    public string EntryPointSymbol { get; set; }
    public bool NoEntryPoint { get; set; }
    public bool Profile { get; set; }
    public string ProfileGuidedDatabase { get; set; }
    public bool SwapRunFromCD { get; set; }
    public bool SwapRunFromNET { get; set; }
    public bool TerminalServerAware { get; set; }
    public bool IgnoreAllDefaultLibraries { get; set; }
    public ITaskItem[] IgnoreSpecificDefaultLibraries { get; set; }
    public int HeapReserveSize { get; set; }
    public int HeapCommitSize { get; set; }
    public int StackReserveSize { get; set; }
    public int StackCommitSize { get; set; }
    public int SectionAlignment { get; set; }
    public string SpecifySectionAttributes { get; set; }
    public ITaskItem[] MergeSections { get; set; }
    public bool EnableUAC { get; set; }
    public bool UACUIAccess { get; set; }
    public string UACExecutionLevel { get; set; }
    public bool SupportUnloadOfDelayLoadedDLL { get; set; }
    public bool SupportNobindOfDelayLoadedDLL { get; set; }
    public ITaskItem[] DelayLoadDLLs { get; set; }
    public bool DelaySign { get; set; }
    public string KeyContainer { get; set; }
    public string KeyFile { get; set; }
    public string Version { get; set; }
    public string Driver { get; set; }
    public int TypeLibraryResourceID { get; set; }
    public string TypeLibraryFile { get; set; }
    public bool IgnoreEmbeddedIDL { get; set; }
    public string MergedIDLBaseFileName { get; set; }
    public string MidlCommandFile { get; set; }
    public ITaskItem[] ForceSymbolReferences { get; set; }

//Skip??:
//  MinimalRebuildFromTracking
//  AcceptableNonZeroExitCodes
//  MinimumRequiredVersion
//  CreateHotPatchableImage

//Missing:
//  ForceFileOutput
//  FunctionOrder
//  LinkStatus
//  ShowProgress
//  TurnOffAssemblyGeneration
//  TrackerLogDirectory
//
//  TLogReadFiles
//  TLogWriteFiles
//  TrackFileAccess
//
//Missing - CLR:
//  AddModuleNamesToAssembly (/ASSEMBLYMODULE)
//  AssemblyDebug            (/ASSEMBLYDEBUG)
//  AssemblyLinkResource     (/ASSEMBLYLINKRESOURCE)
//  EmbedManagedResourceFile (/ASSEMBLYRESOURCE)
//  CLRImageType
//  CLRSupportLastError
//  CLRThreadAttribute
//  CLRUnmanagedCodeCheck


    #endregion

    private string HeapString
    {
      get
      {
        if (HeapReserveSize <= 0)
          return null;

        if (HeapCommitSize > 0)
          return string.Format("{0},{1}", HeapReserveSize, HeapCommitSize);
        else
          return string.Format("{0}", HeapReserveSize);
      }
    }
    private string StackString
    {
      get
      {
        if (StackReserveSize <= 0)
          return null;

        if (StackCommitSize > 0)
          return string.Format("{0},{1}", StackReserveSize, StackCommitSize);
        else
          return string.Format("{0}", StackReserveSize);
      }
    }
    private string ManifestUACString
    {
      get
      {
        if (EnableUAC)
        {
          string uiaccess = string.Format("uiAccess='{0}'", UACUIAccess);

          if (!string.IsNullOrEmpty(UACExecutionLevel))
              return string.Format("level='{0}' {1}", UACExecutionLevel, uiaccess);

          return uiaccess;
        }
        else
          return "NO";
      }
    }

    private string ParseDriverString()
    {
      if (Driver == null)
        return null;

      string drv = Driver.Trim().ToUpper();

      if (drv.Length == 0)
        return " ";
      else if (string.Compare(Driver, "NOTSET", true) == 0)
        return null;

      if (string.Compare(Driver, "UPONLY", true) != 0 &&
          string.Compare(Driver, "WDM", true) != 0)
        Log.LogWarning("/DRIVER must be set to either UPONLY or WDM (but is '{0}')", Driver);

      return string.Format(":{0}", drv);
    }

    private string ParseTargetMachine()
    {
        if (string.IsNullOrEmpty(TargetMachine))
            return null;

        string m = TargetMachine.ToUpper();
        if (m.StartsWith("MACHINE"))
            m = m.Substring("MACHINE".Length);

        return m;
    }

    private string ParseErrorReporting()
    {
        if (string.IsNullOrEmpty(LinkErrorReporting))
            return null;

        string m = LinkErrorReporting.ToUpper();

        if (m == "NOERRORREPORT")
          return null;
        else if (m == "PROMPTIMMEDIATELY")
          m = "PROMPT";
        else if (m == "QUEUEFORNEXTLOGIN")
          m = "QUEUE";
        else if (m == "SENDERRORREPORT")
          m = "SEND";

        return m;
    }

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
      if (SuppressStartupBanner)
        builder.AppendSwitch("/NOLOGO");

      builder.AppendSwitchIfNotNull("/ERRORREPORT:", ParseErrorReporting());

      return builder.ToString();
    }

    protected override string GenerateResponseFileCommands()
    {
      CommandLineBuilder builder = new CommandLineBuilder();

      builder.AppendSwitchIfNotNull("/OUT:", OutputFile);

      if (TreatLinkerWarningAsErrors)
        builder.AppendSwitch("/WX");
      if (LinkIncremental)
        builder.AppendSwitch("/INCREMENTAL");
      if (LinkDll)
        builder.AppendSwitch("/DLL");
      if (GenerateDebugInformation)
        builder.AppendSwitch("/DEBUG");
      if (SetChecksum)
        builder.AppendSwitch("/RELEASE");
      if (PreventDllBinding)
        builder.AppendSwitch("/ALLOWBIND:NO");
      if (!AllowIsolation)
        builder.AppendSwitch("/ALLOWISOLATION:NO");
      if (GenerateMapFile)
      {
        builder.AppendSwitch("/MAP");
        if (MapExports)
          builder.AppendSwitch("/MAPINFO:EXPORTS");
        builder.AppendSwitchIfNotNull("/MAPFILE:", MapFileName);
      }
      if (GenerateManifest)
      {
        builder.AppendSwitch("/MANIFEST:EMBED");
      }
      builder.AppendSwitchIfNotNull("/MANIFESTUAC:", ManifestUACString);
      builder.AppendSwitchIfNotNull("/PDB:", ProgramDatabaseFile);
      builder.AppendSwitchIfNotNull("/PDBSTRIPPED:", StripPrivateSymbols);
      builder.AppendSwitchIfNotNull("/LTCG:", LinkTimeCodeGeneration);
      builder.AppendSwitchIfNotNull("/DRIVER", ParseDriverString());

      builder.AppendSwitchIfNotNull("/ENTRY:", EntryPointSymbol);
      if (NoEntryPoint)
        builder.AppendSwitch("/NOENTRY");
      builder.AppendSwitchIfNotNull("/BASE:", BaseAddress);
      if (FixedBaseAddress)
        builder.AppendSwitch("/FIXED");
      if (RandomizedBaseAddress)
        builder.AppendSwitch("/DYNAMICBASE");
      if (LargeAddressAware)
        builder.AppendSwitch("/LARGEADDRESSAWARE");
      if (DataExecutionPrevention)
        builder.AppendSwitch("/NXCOMPAT");
      if (Profile)
        builder.AppendSwitch("/PROFILE");
      builder.AppendSwitchIfNotNull("/PGD:", ProfileGuidedDatabase);
      if (SwapRunFromCD)
        builder.AppendSwitch("/SWAPRUN:CD");
      if (SwapRunFromNET)
        builder.AppendSwitch("/SWAPRUN:NET");
      if (TerminalServerAware)
        builder.AppendSwitch("/TSAWARE");
      if (ImageHasSafeExceptionHandlers)
        builder.AppendSwitch("/SAFESEH");
      if (OptimizeReferences != null)
        builder.AppendSwitch("/OPT:" + ((bool)OptimizeReferences ? "NOREF" : "REF"));
      if (EnableCOMDATFolding != null)
        builder.AppendSwitch("/OPT:" + ((bool)EnableCOMDATFolding ? "NOICF" : "ICF"));

      if (SectionAlignment > 0)
        builder.AppendSwitch("/ALIGN:" + SectionAlignment.ToString());
      builder.AppendSwitchIfNotNull("/SECTION:", SpecifySectionAttributes);
      if (MergeSections != null)
      {
        foreach (ITaskItem secToFrom in MergeSections)
          builder.AppendSwitchUnquotedIfNotNull("/MERGE:", secToFrom);
      }

      if (DelayLoadDLLs != null)
      {
        foreach (ITaskItem dll in DelayLoadDLLs)
          builder.AppendSwitchUnquotedIfNotNull("/DELAYLOAD:", dll);
      }
      if (SupportUnloadOfDelayLoadedDLL)
        builder.AppendSwitch("/DELAY:UNLOAD");
      if (SupportNobindOfDelayLoadedDLL)
        builder.AppendSwitch("/DELAY:NOBIND");
      if (DelaySign)
        builder.AppendSwitch("/DELAYSIGN");
      builder.AppendSwitchIfNotNull("/KEYFILE:", KeyFile);
      builder.AppendSwitchIfNotNull("/KEYCONTAINER:", KeyContainer);

      builder.AppendSwitchIfNotNull("/VERSION:", Version);
      builder.AppendSwitchIfNotNull("/HEAP:", HeapString);
      builder.AppendSwitchIfNotNull("/STACK:", StackString);
      builder.AppendSwitchIfNotNull("/MACHINE:", ParseTargetMachine());
      if (SubSystem != null)
        builder.AppendSwitch("/SUBSYSTEM:" + SubSystem + "\",5.01\"");
      builder.AppendSwitchIfNotNull("/DEF:", ModuleDefinitionFile);
      builder.AppendSwitchIfNotNull("/IMPLIB:", ImportLibrary);
      builder.AppendSwitchIfNotNull("/STUB:", MSDOSStubFileName);

      builder.AppendSwitchIfNotNull("/TLBOUT:", TypeLibraryFile);
      if (TypeLibraryResourceID > 1)
        builder.AppendSwitch("/TLBID:" + TypeLibraryResourceID.ToString());

      if (IgnoreEmbeddedIDL)
        builder.AppendSwitch("/IGNOREIDL");
      builder.AppendSwitchIfNotNull("/IDLOUT:", MergedIDLBaseFileName);
      builder.AppendSwitchIfNotNull("/MIDL:@", MidlCommandFile);

      if (IgnoreAllDefaultLibraries)
        builder.AppendSwitch("/NODEFAULTLIB");
      else if (IgnoreSpecificDefaultLibraries != null)
      {
        foreach (ITaskItem lib in IgnoreSpecificDefaultLibraries)
          builder.AppendSwitchUnquotedIfNotNull("/NODEFAULTLIB:", lib);
      }

      if (ForceSymbolReferences != null)
      {
        foreach (ITaskItem symref in ForceSymbolReferences)
          builder.AppendSwitchUnquotedIfNotNull("/INCLUDE:", symref);
      }

      if (AdditionalLibraryDirectories != null)
      {
        foreach (ITaskItem dir in AdditionalLibraryDirectories)
          builder.AppendSwitchUnquotedIfNotNull("/LIBPATH:", dir);
      }

      builder.AppendFileNamesIfNotNull(AdditionalOptions, " ");
      builder.AppendFileNamesIfNotNull(AdditionalDependencies, " ");
      builder.AppendFileNamesIfNotNull(Sources, " ");

      return builder.ToString();
    }
  }
}
