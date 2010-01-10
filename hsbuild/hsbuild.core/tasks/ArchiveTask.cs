// HSBuild.Tasks - ArchiveTask
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
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using HSBuild.Core;

namespace HSBuild.Tasks
{
    public class ArchiveTask : ITask
    {
        enum Compression
        {
            None,
            Compress,
            Zip,
            Gzip,
            Bzip2,
        }

        public ArchiveTask(string tarball, string originalFilename, string targetDir)
        {
            m_tarball = tarball;
            m_targetDir = targetDir;

            if (originalFilename.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                m_compression = Compression.Zip;
            else if (originalFilename.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase))
                m_compression = Compression.Gzip;
            else if (originalFilename.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
                m_compression = Compression.Gzip;
            else if (originalFilename.EndsWith(".tar.bz2", StringComparison.OrdinalIgnoreCase))
                m_compression = Compression.Bzip2;
            else
                m_compression = Compression.None;
        }

        private static Stream OpenCompressedStream(Compression comp, string filename)
        {
            FileStream fileStream = File.OpenRead(filename);
            switch (comp)
            {
                case Compression.Gzip:
                    return new GZipInputStream(fileStream);
                case Compression.Bzip2:
                    return new BZip2InputStream(fileStream);
                default:
                    return fileStream;
            }
        }

        #region ITask Members

        public int Execute(IOutputEngine output)
        {
            if (m_compression == Compression.Zip)
            {
                FastZip zip = new FastZip();
                zip.ExtractZip(m_tarball, m_tarball, null);
            }
            else
            {
                TarArchive archive = null;
                try
                {
                    Stream s = OpenCompressedStream(m_compression, m_tarball);
                    archive = TarArchive.CreateInputTarArchive(s);
                    archive.ExtractContents(m_targetDir);
                }
                finally
                {
                    if (archive != null)
                        archive.Close();
                }
            }

            return 0;
        }

        #endregion

        private string m_tarball;
        private string m_targetDir;
        private Compression m_compression;
    }
}
