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

namespace HSBuild.MSF
{
    internal class BitFields
    {
        internal BitFields(BitHelper bits)
        {
            int size;
            bits.ReadInt32(out size);
            m_dwords = new UInt32[size];
            bits.ReadUInt32(m_dwords);
        }

        internal BitFields(int size)
        {
            m_dwords = new uint[size];
        }

        internal int Length
        {
            get { return m_dwords.Length; }
        }

        internal bool IsSet(int index)
        {
            int word = index / 32;
            if (word >= m_dwords.Length) return false;
            return ((m_dwords[word] & GetBit(index)) != 0);
        }

        private static uint GetBit(int index)
        {
            return ((uint)1 << (index % 32));
        }

        private UInt32[] m_dwords;
    }
}
