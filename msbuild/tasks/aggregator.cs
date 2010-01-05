// HSBuild.Tasks - Aggregator
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
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HSBuild.Tasks
{
  public sealed class Aggregator : Task
  {
    private ITaskItem[] sourceFiles;
    private ITaskItem outFile;
    private ITaskItem[] headLines;
    private ITaskItem[] separatorLines;
    private ITaskItem[] tailLines;

    #region Tool properties
    [Required]
    public ITaskItem[] SourceFiles
    {
      get { return this.sourceFiles; }
      set { this.sourceFiles = value; }
    }

    [Required]
    public ITaskItem OutputFile
    {
      get { return this.outFile; }
      set { this.outFile = value; }
    }

    public ITaskItem[] HeadLines
    {
      get { return this.headLines; }
      set { this.headLines = value; }
    }

    public ITaskItem[] SeparatorLines
    {
      get { return this.separatorLines; }
      set { this.separatorLines = value; }
    }

    public ITaskItem[] TailLines
    {
      get { return this.tailLines; }
      set { this.tailLines = value; }
    }

    #endregion

    public override bool Execute()
    {
      StreamWriter output;

      try
      {
        output = new StreamWriter(outFile.ItemSpec, false);
      }
      catch (Exception ex)
      {
        Log.LogErrorFromException(ex, false, true, outFile.ItemSpec);
        return false;
      }

      if (HeadLines != null)
      {
        foreach (ITaskItem line in HeadLines)
          output.WriteLine(line.ItemSpec);
      }

      bool ret = true, first = true;
      foreach (ITaskItem file in sourceFiles)
      {
        if (!first)
        {
          if (SeparatorLines != null)
          {
            foreach (ITaskItem line in SeparatorLines)
              output.WriteLine(line.ItemSpec);
          }
        }
        else
        {
          first = false;
        }

        if (!AggregateFile(file, output))
          ret = false;
      }

      if (TailLines != null)
      {
        foreach (ITaskItem line in TailLines)
          output.WriteLine(line.ItemSpec);
      }

      output.Close();
      return ret;
    }

    private bool AggregateFile(ITaskItem file, StreamWriter output)
    {
      if (file == null || string.IsNullOrEmpty(file.ItemSpec))
      {
        Log.LogMessage("Null or empty specified.");
        return true;
      }
      else if (!File.Exists(file.ItemSpec))
      {
        Log.LogError("File ({0}) doesn't exist.", file.ItemSpec);
        return false;
      }

      StreamReader input;
      try
      {
        input = new StreamReader(file.ItemSpec, true);
      }
      catch (Exception ex)
      {
        Log.LogErrorFromException(ex, false, true, file.ItemSpec);
        return false;
      }

      while (!input.EndOfStream)
        output.WriteLine(input.ReadLine());

      input.Close();
      return true;
    }
  }
}
