// HSBuild.Tasks - Substitute
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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HSBuild.Tasks
{
  public sealed class Substitute : Task
  {
    private ITaskItem[] sourceFiles;
    private ITaskItem[] expressions;
    private List<ITaskItem> outputFiles;
    private List<ITaskItem> writtenFiles;
    private string outdir;
    private string outfile;

    #region Tool properties
    [Required]
    public ITaskItem[] SourceFiles
    {
      get { return this.sourceFiles; }
      set { this.sourceFiles = value; }
    }

    [Required]
    public ITaskItem[] Expressions
    {
      get { return this.expressions; }
      set { this.expressions = value; }
    }

    [Output]
    public ITaskItem[] DestinationFiles
    {
      get { return this.outputFiles.ToArray(); }
    }

    [Output]
    public ITaskItem[] WrittenFiles
    {
      get { return this.writtenFiles.ToArray(); }
    }

    public string OutputDirectory
    {
      get { return this.outdir; }
      set { this.outdir = value; }
    }

    public string OutputFileName
    {
      get { return this.outfile; }
      set { this.outfile = value; }
    }

    #endregion

    public override bool Execute()
    {
      bool ret = true;

      outputFiles = new List<ITaskItem>(sourceFiles.Length);
      writtenFiles = new List<ITaskItem>(sourceFiles.Length);

      if (sourceFiles.Length > 1 && !string.IsNullOrEmpty(OutputFileName))
      {
        Log.LogWarning("Multiple files ({0}) and OutputFileName ({1}) set.", sourceFiles.Length, OutputFileName);
      }

      foreach (ITaskItem file in sourceFiles)
      {
        string outName = GetOutputFile(file.ItemSpec);
        if (string.IsNullOrEmpty(outName))
          continue;

        outputFiles.Add(new TaskItem(outName, file.CloneCustomMetadata()));

        DateTime lastWrite = File.GetLastAccessTimeUtc(file.ItemSpec);
        FileInfo tmpFile = SubstituteFile(file.ItemSpec);

        FileInfo outFile = new FileInfo(outName);
        if (outFile.Exists)
        {
          if (outFile.Length == tmpFile.Length && outFile.LastWriteTimeUtc == lastWrite)
          {
            Log.LogMessage(MessageImportance.Low, "Skip Substitute {0} -> {1}", file.ItemSpec, outName);
            continue;
          }

          outFile.Delete();
        }

        tmpFile.MoveTo(outName);
        File.SetLastWriteTimeUtc(outName, lastWrite);

        Log.LogMessage(MessageImportance.Low, "Substitute {0} -> {1}", file.ItemSpec, outName);
        writtenFiles.Add(new TaskItem(outName, file.CloneCustomMetadata()));
      }

      return ret;
    }

    public FileInfo SubstituteFile(string inFile)
    {
      StreamReader input;
      try
      {
        input = new StreamReader(inFile, true);
      }
      catch (Exception ex)
      {
        Log.LogErrorFromException(ex, false, true, inFile);
        return null;
      }

      var ret = new FileInfo(Path.GetTempFileName());

      StreamWriter output;
      try
      {
        Encoding enc = input.CurrentEncoding;
        if (enc == Encoding.UTF8)
          enc = Encoding.Default;

        output = new StreamWriter(ret.FullName, false, enc);
      }
      catch (Exception ex)
      {
        Log.LogErrorFromException(ex, false, true, ret.FullName);
        return null;
      }

      string buffer = input.ReadToEnd();

      input.Close();

      foreach (ITaskItem exp in expressions)
        buffer = ExecuteExpression(buffer, exp.ItemSpec);

      output.Write(buffer);
      output.Close();

      return ret;
    }

    private string GetOutputFile(string file)
    {
      if (!string.IsNullOrEmpty(OutputFileName) && string.Compare(Path.GetFileName(file), OutputFileName, true) != 0)
      {
        return (outdir == null) ? OutputFileName : Path.Combine(outdir, OutputFileName);
      }

      if (string.Compare(Path.GetExtension(file), ".in", true) == 0)
      {
        string filename = Path.GetFileNameWithoutExtension(file);

        return (outdir == null) ? filename : Path.Combine(outdir, filename);
      }

      if (outdir == null)
      {
        Log.LogError("File: {0} does not have .in extension, which requires OutputDirectory to be set.", file);
        return null;
      }

      return Path.Combine(outdir, Path.GetFileName(file));
    }

    private string ExecuteExpression(string buffer, string exp)
    {
      switch (exp[0])
      {
        case 's':
          return ExecuteSubstitution(buffer, exp);
        default:
          Log.LogErrorFromException(new NotImplementedException(),false, true, exp);
          return buffer;
      }
    }

    private string ExecuteSubstitution(string buffer, string exp)
    {
      string replacement = null;
      string pattern = null;

      ParseSubstitution(exp, ref pattern, ref replacement);

      if (pattern == null)
        Log.LogError("Couldn't find pattern in expression", exp);
      else if (replacement == null)
        Log.LogError("Couldn't find replacement in expression", exp);
      else
        return Regex.Replace(buffer, pattern, replacement);

      return buffer;
    }

    private void ParseSubstitution(string exp, ref string pattern, ref string replacement)
    {
      if (exp.Length < 5) // Must contain "s/.//"
        return;

      if (exp[0] != 's')
        return;

      if (exp[1] == '\\')
        return;

      const int firstIdx = 1;
      char divider = exp[firstIdx];
      int lastIdx = exp.LastIndexOf(divider);

      if (lastIdx <= 1)
        return;

      int midIdx = 1;
      while (midIdx < lastIdx)
      {
        midIdx = exp.IndexOf(divider, midIdx + 1);
        if (midIdx <= firstIdx)
          return;

        if (exp[midIdx - 1] != '\\')
          break;
      }

      pattern =     exp.Substring(firstIdx  + 1, midIdx - (firstIdx + 1));
      replacement = exp.Substring(midIdx    + 1, lastIdx - (midIdx  + 1));

      pattern.Replace("\\" + divider, divider.ToString());
      replacement.Replace("\\" + divider, divider.ToString());
    }

  }
}
