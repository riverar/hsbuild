// HSBuild.Tasks - Copy2
//
// Copyright (C) 2010 Haakon Sporsheim <haakon.sporsheim@gmail.com>
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
  public sealed class Copy2 : Task
  {
    #region Tool properties
    [Required]
    public ITaskItem[] SourceFiles { get; set; }
    [Output]
    public ITaskItem[] CopiedFiles { get; private set; }
    public ITaskItem[] DestinationFiles { get; set; }
    public ITaskItem DestinationFolder { get; set; }
    #endregion

    const int BYTES_TO_READ = sizeof(Int64);

    static bool FilesAreEqual(FileInfo file1, FileInfo file2)
    {
      if (!file1.Exists)
        throw new FileNotFoundException("Source file not found", file1.FullName);
      if (!file2.Exists)
        return false;
      if (file1.Length != file2.Length)
        return false;

      UInt64 iterations = (UInt64)Math.Ceiling((double)file1.Length / BYTES_TO_READ);

      using (FileStream fs1 = file1.OpenRead())
      using (FileStream fs2 = file2.OpenRead())
      {
        byte[] bytes1 = new byte[BYTES_TO_READ];
        byte[] bytes2 = new byte[BYTES_TO_READ];

        for (UInt64 i = 0; i < iterations; i++)
        {
          fs1.Read(bytes1, 0, BYTES_TO_READ);
          fs2.Read(bytes2, 0, BYTES_TO_READ);

          if (BitConverter.ToInt64(bytes1, 0) != BitConverter.ToInt64(bytes2, 0))
            return false;
        }
      }

      return true;
    }

    public override bool Execute()
    {
      if (DestinationFolder != null && DestinationFiles != null)
        throw new NotSupportedException("DestinationFolder and DestinationFiles are mutual exclusive properties.");

      ITaskItem[] src = SourceFiles;
      ITaskItem[] dst = DestinationFiles != null ? DestinationFiles : CreateDestinationFiles();

      if (src.Length != dst.Length)
          throw new NotSupportedException("DestinationFiles must have equal amount of items as SourceFiles.");

      List<ITaskItem> copied = new List<ITaskItem>(SourceFiles.Length);
      for (int i = 0; i < src.Length; i++)
      {
        FileInfo srcfi = new FileInfo(src[i].ItemSpec);
        FileInfo dstfi = new FileInfo(dst[i].ItemSpec);
        if (!FilesAreEqual(srcfi, dstfi))
        {
          if (!dstfi.Directory.Exists)
            dstfi.Directory.Create();

          Log.LogMessage(MessageImportance.Low, "Copying file from {0} to {1}", src[i].ItemSpec, dst[i].ItemSpec);
          srcfi.CopyTo(dstfi.FullName, true);
          copied.Add(dst[i]);
        }
      }

      CopiedFiles = copied.ToArray();

      return true;
    }

    private ITaskItem[] CreateDestinationFiles()
    {
      List<ITaskItem> files = new List<ITaskItem>(SourceFiles.Length);

      foreach (ITaskItem item in SourceFiles)
        files.Add(new TaskItem(Path.Combine(DestinationFolder.ItemSpec, Path.GetFileName(item.ItemSpec))));

      return files.ToArray();
    }
  }
}
