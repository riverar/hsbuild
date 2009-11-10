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
    public class FileDeps
    {
        public const UInt32 MAGIC = 0x20535244; // "DRS "

        internal FileDeps(BitHelper bits)
        {
            UInt32 tmp = 0;
            bits.ReadUInt32(out tmp);

            if (tmp != MAGIC)
                throw new ApplicationException(string.Format("Expected 0x{0:8x}", MAGIC));

            bits.ReadUInt32(out m_size);

            bits.ReadUInt32(out tmp);
            if (tmp != 0xFFFFFFFF)
                throw new ApplicationException(string.Format("Expected 0xFFFFFF but got 0x{0:x8} in {1}", tmp, Name));

            bits.ReadUInt32(out tmp);
            if (tmp != 0)
                throw new ApplicationException(string.Format("Expected 0x000000 but got 0x{0:x8} in {1}", tmp, Name));

            bits.ReadUInt32(out tmp);
            if (tmp != 0x0C)
                throw new ApplicationException(string.Format("Expected size of 0x0C but got 0x{0:x8} in {1}", tmp, Name));

            m_ver = new byte[tmp];
            bits.ReadBytes(m_ver);

            int count = 0, max = 0;
            bits.ReadInt32(out count);
            bits.ReadInt32(out max);

            m_present = new BitFields(bits);
            m_deleted = new BitFields(bits);
            m_deps = new List<UInt32>(count);

            //if (m_deleted.Length != 0)
            //    throw new ApplicationException("Unsupported MRFileInfoDataStream");

            for (int i = 0; i < max; i++)
            {
                if (m_present.IsSet(i))
                {
                    bits.ReadUInt32(out tmp);
                    m_deps.Add(tmp);
                }
            }

            if (count != m_deps.Count)
                throw new ApplicationException();
        }

        public string Name          { get { return "MRFileDeps"; } }
        public List<UInt32> List    { get { return m_deps; } }
        public int Count            { get { return m_deps.Count; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(m_deps.Count * 10);

            for (int i = 0; i < m_deps.Count; i++)
            {
                if ((i + 1) % 32 == 0)
                    sb.Append("\n");

                sb.Append(string.Format(" 0x{0:x8}", m_deps[i]));
            }

            return sb.ToString();
        }

        private UInt32 m_size;
        private byte[] m_ver;
        private List<UInt32> m_deps;
        BitFields m_present;
        BitFields m_deleted;
    }
}
