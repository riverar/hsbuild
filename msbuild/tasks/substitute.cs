using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Oah.Tasks
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
            Log.LogMessage(MessageImportance.Normal, "Skip Substitute {0} -> {1}", file.ItemSpec, outName);
            continue;
          }

          outFile.Delete();
        }

        tmpFile.MoveTo(outName);
        File.SetLastWriteTimeUtc(outName, lastWrite);

        Log.LogMessage(MessageImportance.Normal, "Substitute {0} -> {1}", file.ItemSpec, outName);
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

      StringBuilder buffer = new StringBuilder(input.ReadToEnd());

      input.Close();

      foreach (ITaskItem exp in expressions)
        ExecuteExpression(buffer, exp.ItemSpec);

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

    private void ExecuteExpression(StringBuilder buffer, string exp)
    {
      switch (exp[0])
      {
        case 's':
          ExecuteSubstitution(buffer, exp);
          break;
        default:
          Log.LogErrorFromException(new NotImplementedException(),false, true, exp);
          break;
      }
    }

    private void ExecuteSubstitution(StringBuilder buffer, string exp)
    {
      string replacement = null;
      string pattern = null;

      ParseSubstitution(exp, ref pattern, ref replacement);

      if (pattern == null)
        Log.LogError("Couldn't find pattern in expression", exp);
      else if (replacement == null)
        Log.LogError("Couldn't find replacement in expression", exp);
      else
        buffer.Replace(pattern, replacement);
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
