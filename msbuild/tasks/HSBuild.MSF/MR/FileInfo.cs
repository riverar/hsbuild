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

namespace HSBuild.MSF.MR
{
    public class FileInfo
    {
        public const uint SIZE = 40;

        [Flags()]
        public enum Type
        {
            None                = 0x0000,
            SpecialFile         = 0x0001,
            CompiledFile        = 0x0002,
            LinkerFile          = 0x0004,
            IncludedFile        = 0x0008,
        }

        internal FileInfo(BitHelper bits)
        {
            UInt32 tmpType;

            bits.ReadUInt32(out m_id);
            bits.ReadUInt32(out tmpType);
            m_type = (Type)tmpType;
            m_date = bits.ReadFileTime();

            bits.ReadUInt64(out m_size);

            bits.ReadUInt32(out m_switchID);
            bits.ReadUInt32(out m_relID);

            bits.ReadUInt32(out m_unknown1);
            bits.ReadUInt32(out m_unknown2);
        }

        public UInt32 ID            { get { return m_id; } }
        public Type FileType        { get { return m_type; } }
        public string FileName      { get { return m_filename; } }
        public DateTime Date        { get { return m_date; } }
        public UInt64 Size          { get { return m_size; } }

        private UInt32 m_id;        // 0..3
        private Type m_type;        // 4..7
        private DateTime m_date;    // 8..15
        private UInt64 m_size;      // 16..23
        private UInt32 m_switchID;  // 24..27
        private UInt32 m_relID;  // 28..31
        private UInt32 m_unknown1;  // 32..35
        private UInt32 m_unknown2;  // 36..39
        private string m_filename;

        public override string ToString()
        {
            return string.Format("ID: 0x{0:x8}, {1}, Date: {2} Size: {3} Switches: 0x{4:x8} Rel: 0x{5:x8} Uknown: 0x{6:x8} 0x{7:x8}",
                m_id, m_type.ToString(), m_date, m_size, m_switchID, m_relID,
                m_unknown1, m_unknown2);
        }

        internal void SetFilename(string filename)
        {
            m_filename = filename;
        }
    }
}
