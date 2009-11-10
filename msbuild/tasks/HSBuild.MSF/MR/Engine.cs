//
// Copyright (c) 2009 Haakon Sporsheim <haakon.sporsheim@gmail.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.IO;

namespace HSBuild.MSF.MR
{
    public class Engine : IDisposable
    {
        public Engine(string file)
        {
            m_parser = HSBuild.MSF.Parser.CreateFromFile(file);
            m_index = m_parser.GetIndexDataStream();

            m_names = m_index.GetNamesStream(m_parser);
            m_filesInfo = m_index.GetFileInfoStream(m_parser);
        }

        public List<KeyValuePair<string, FileInfo>> GetFiles(FileInfo.Type types)
        {
            var ret = new List<KeyValuePair<string, FileInfo>>();

            foreach (var p in m_filesInfo.List)
            {
                if ((p.Value.FileType & types) > 0)
                {
                    var fi = p.Value;
                    string name = m_names.FindByKey(p.Key);

                    fi.SetFilename(name);
                    ret.Add(new KeyValuePair<string, FileInfo>(name, fi));
                }
            }

            return ret;
        }

        public void FilterFilesUpToDate(List<string> files)
        {
            string dueTo;
            for (int i = 0; i < files.Count; i++)
            {
                while (i < files.Count && IsFileUpToDate(files[i], out dueTo))
                    files.RemoveAt(i);
            }
        }

        public bool IsFileUpToDate(string filename, out string dueToFile)
        {
            FileInfo obj;
            List<FileInfo> deps;
            FileInfo file = QueryFileInfoDeps(Path.GetFullPath(filename), out obj, out deps);

            if (file == null)
            {
                dueToFile = filename;
                return false;
            }

            if (!IsFileUpToDate(file, out dueToFile))
                return false;

            if (!IsFileUpToDate(obj, out dueToFile))
                return false;

            if (!IsFilesUpToDate(deps, out dueToFile))
                return false;

            return true;
        }

        public FileInfo QueryFileInfoDeps(string filename, out FileInfo obj, out List<FileInfo> deps)
        {
            return QueryFileInfoDeps(m_names.FindByName(filename), out obj, out deps);
        }

        public FileInfo QueryFileInfoDeps(uint namesIdx, out FileInfo obj, out List<FileInfo> deps)
        {
            FileInfo ret = null;
            obj = null;
            deps = null;
            ret = null;

            if (namesIdx > 0)
            {
                ret = GetFileInfo(namesIdx);

                if (ret != null)
                    QueryFileInfoDeps(ret, out obj, out deps);
            }

            return ret;
        }

        public void QueryFileInfoDeps(FileInfo file, out FileInfo obj, out List<FileInfo> deps)
        {
            if (string.IsNullOrEmpty(file.FileName))
                file.SetFilename(m_names.FindByKey(file.ID));

            var nfo = m_index.GetFileStream(file.ID, m_parser);

            obj = GetFileInfo(nfo.OBJFile);
            deps = GetFiles(nfo.FileDeps.List);
        }

        private bool IsFilesUpToDate(List<FileInfo> deps, out string dueToFile)
        {
            foreach (var f in deps)
            {
                if (!IsFileUpToDate(f, out dueToFile))
                    return false;
            }

            dueToFile = null;
            return true;
        }

        private bool IsFileUpToDate(FileInfo file, out string dueToFile)
        {
            if (file == null)
                throw new ArgumentException("Argument can't be null!", "file");

            bool ret;
            try
            {
                ret = file.Date.CompareTo(File.GetLastWriteTime(file.FileName)) == 0;
            }
            catch
            {
                ret = false;
            }

            dueToFile = ret ? null : file.FileName;
            return ret;
        }

        private List<FileInfo> GetFiles(List<UInt32> deps)
        {
            var ret = new List<FileInfo>();

            foreach (var d in deps)
                ret.Add(GetFileInfo(d));

            return ret;
        }

        private FileInfo GetFileInfo(uint d)
        {
            FileInfo fi = m_filesInfo.GetInfo(d);
            fi.SetFilename(m_names.FindByKey(d));
            return fi;
        }

        private uint GetStreamData(string name, out byte[] data)
        {
            int i = m_index.GetStreamID(name);

            if (i < 0)
                data = null;
            else
                data = m_parser.GetStreamBytes((uint)i);

            return (uint)i;
        }

        private HSBuild.MSF.Parser m_parser;
        private IndexDataStream m_index;
        private NamesDataStream m_names;
        private FileInfoDataStream m_filesInfo;

        #region IDisposable Members

        public void Dispose()
        {
            m_parser.Dispose();
        }

        #endregion
    }
}
