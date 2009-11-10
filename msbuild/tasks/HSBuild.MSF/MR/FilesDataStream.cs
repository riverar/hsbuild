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
using System.Text;

namespace HSBuild.MSF.MR
{
    public class FilesDataStream : DataStream
    {
        public const UInt32 MAGIC = 0x20737264; // "drs "

        internal FilesDataStream(uint idx, BitHelper data)
            : base(idx, (uint)data.Length)
        {
            UInt32 type = 0;
            data.ReadUInt32(out type);

            if (type != MAGIC)
                throw new ApplicationException(string.Format("Expected 0x{0:8x}", MAGIC));

            UInt32 size = 0;
            data.ReadUInt32(out size);

            if (size != data.Length)
                throw new ApplicationException(string.Format("Expected correct {0} size ({1} != {2})", Name, size, data.Length));

            data.ReadUInt32(out m_unknown1);
            data.ReadUInt32(out m_unknown2);
            data.ReadUInt32(out m_cfile);
            data.ReadUInt32(out m_objfile);

            UInt32 offClassdeps, offFiledeps, offBoringClasses;
            data.ReadUInt32(out offClassdeps);
            data.ReadUInt32(out offFiledeps);
            data.ReadUInt32(out offBoringClasses);

            if (offFiledeps > 0 && offFiledeps < data.Length)
            {
                data.Position = (int)offFiledeps;
                m_depsFile = new FileDeps(data);
            }

            if (offClassdeps > 0 && offClassdeps < data.Length)
            {
                data.Position = (int)offClassdeps;
                m_depsClass = new FileDeps(data);
            }

            // TODO offBoringClasses
        }

        public override string Name     { get { return "FilesDataStream"; } }
        public FileDeps FileDeps        { get { return m_depsFile; } }
        public FileDeps ClassDeps       { get { return m_depsClass; } }
        public UInt32 CFile             { get { return m_cfile; } }
        public UInt32 OBJFile           { get { return m_objfile; } }

        public override void Print(TextWriter writer)
        {
            base.Print(writer);
            writer.WriteLine("Uknown: 0x{0:x8} 0x{1:x8} .C: 0x{2:x8}, .OBJ: 0x{3:x8}",
                m_unknown1, m_unknown2, m_cfile, m_objfile);
            if (m_depsFile != null && m_depsFile.Count > 0)
                writer.WriteLine(" File: " + m_depsFile.ToString());
            if (m_depsClass != null && m_depsClass.Count > 0)
                writer.WriteLine(" Class: " + m_depsClass.ToString());
        }

        private UInt32 m_unknown1;
        private UInt32 m_unknown2;
        private UInt32 m_cfile;
        private UInt32 m_objfile;

        private FileDeps m_depsClass;
        private FileDeps m_depsFile;

    }
}
