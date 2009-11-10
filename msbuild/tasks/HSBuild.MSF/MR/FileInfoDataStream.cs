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
    public class FileInfoDataStream : DataStream
    {
        public const UInt32 MAGIC = 0x20535244; // "DRS "
        public const UInt32 VerSize = 0x20;

        internal FileInfoDataStream(uint idx, BitHelper data)
            : base(idx, (uint)data.Length)
        {
            UInt32 tmp = 0;

            data.ReadUInt32(out tmp);
            if (tmp != MAGIC)
                throw new ApplicationException(string.Format("Expected 0x{0:8x}", MAGIC));

            data.ReadUInt32(out tmp);
            if (tmp != data.Length)
                throw new ApplicationException(string.Format("Expected correct {0} size ({1} != {2})", Name, tmp, data.Length));

            data.ReadUInt32(out tmp);
            if (tmp != 0xFFFFFFFB)
                throw new ApplicationException(string.Format("Expected 0xFFFFFB but got 0x{0:x8} in {1}", tmp, Name));

            data.ReadUInt32(out tmp);
            if (tmp != 0)
                throw new ApplicationException(string.Format("Expected 0x000000 but got 0x{0:x8} in {1}", tmp, Name));

            data.ReadUInt32(out tmp);
            if (tmp != VerSize)
                throw new ApplicationException(string.Format("Expected VerSize of 0x{0:x4} but got 0x{1:x8} in {2}", VerSize, tmp, Name));

            m_ver = new byte[tmp];
            data.ReadBytes(m_ver);

            int count = 0, max = 0;
            data.ReadInt32(out count);
            data.ReadInt32(out max);

            m_present = new BitFields(data);
            m_deleted = new BitFields(data);
            m_listFI = new List<KeyValuePair<UInt32, FileInfo>>(count);

            //if (m_deleted.Length != 0)
            //    throw new ApplicationException("Unsupported MRFileInfoDataStream");

            for (int i = 0; i < max; i++)
            {
                if (m_present.IsSet(i))
                {
                    data.ReadUInt32(out tmp);
                    m_listFI.Add(new KeyValuePair<UInt32, FileInfo>(tmp, new FileInfo(data)));
                }
            }

            if (count != m_listFI.Count)
                throw new ApplicationException();
        }

        public override string Name { get { return "MRFileInfoDataStream"; } }
        public List<KeyValuePair<UInt32, FileInfo>> List { get { return m_listFI; } }
        public override void Print(TextWriter writer)
        {
            base.Print(writer);
            writer.WriteLine("VER section:");
            PrintBytes(writer, m_ver);

            foreach (KeyValuePair<UInt32, FileInfo> fi in m_listFI)
                writer.WriteLine(fi.Value);
        }

        internal FileInfo GetInfo(UInt32 i)
        {
            foreach (var p in m_listFI)
            {
                if (p.Key == i)
                    return p.Value;
            }

            return null;
        }

        private byte[] m_ver;
        private List<KeyValuePair<UInt32, FileInfo>> m_listFI;
        BitFields m_present;
        BitFields m_deleted;
    }

}
