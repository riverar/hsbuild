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
using System.IO;
using System.Text;

namespace HSBuild.MSF
{
    internal class BitHelper
    {
        internal BitHelper(int capacity)
        {
            m_buffer = new byte[capacity];
        }

        internal BitHelper(byte[] bytes)
        {
            m_buffer = bytes;
        }

        internal byte[] Buffer
        {
            get { return m_buffer; }
        }

        private byte[] m_buffer;

        internal void FillBuffer(Stream stream, int capacity)
        {
            MinCapacity(capacity);
            stream.Read(m_buffer, 0, capacity);
            m_offset = 0;
        }

        internal int Length
        {
            get { return m_buffer.Length; }
        }

        internal int Position
        {
            get { return m_offset; }
            set { m_offset = value; }
        }
        private int m_offset;


        internal void MinCapacity(int capacity)
        {
            if (m_buffer.Length < capacity)
                m_buffer = new byte[capacity];

            m_offset = 0;
        }

        internal void Align(int alignment)
        {
            while ((m_offset % alignment) != 0)
                m_offset++;
        }

        internal void ReadInt16(out short value)
        {
            value = BitConverter.ToInt16(m_buffer, m_offset);
            m_offset += 2;
        }

        internal void ReadInt32(out int value)
        {
            value = BitConverter.ToInt32(m_buffer, m_offset);
            m_offset += 4;
        }

        internal void ReadInt64(out long value)
        {
            value = BitConverter.ToInt64(m_buffer, m_offset);
            m_offset += 8;
        }

        internal void ReadUInt8(out byte value)
        {
            value = m_buffer[m_offset];
            m_offset += 1;
        }

        internal void ReadUInt16(out ushort value)
        {
            value = BitConverter.ToUInt16(m_buffer, m_offset);
            m_offset += 2;
        }

        internal void ReadUInt32(out uint value)
        {
            value = BitConverter.ToUInt32(m_buffer, m_offset);
            m_offset += 4;
        }

        internal void ReadUInt64(out ulong value)
        {
            value = BitConverter.ToUInt64(m_buffer, m_offset);
            m_offset += 8;
        }

        internal void ReadInt32(int[] values)
        {
            for (int i = 0; i < values.Length; i++)
                ReadInt32(out values[i]);
        }

        internal void ReadUInt32(uint[] values)
        {
            for (int i = 0; i < values.Length; i++)
                ReadUInt32(out values[i]);
        }

        internal void ReadBytes(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = m_buffer[m_offset++];
        }

        internal float ReadFloat()
        {
          float result = BitConverter.ToSingle(m_buffer, m_offset);
          m_offset += 4;
          return result;
        }

        internal double ReadDouble()
        {
            double result = BitConverter.ToDouble(m_buffer, m_offset);
            m_offset += 8;
            return result;
        }

        internal decimal ReadDecimal()
        {
            int[] bits = new int[4];
            ReadInt32(bits);
            return new decimal(bits);
        }

        internal void ReadBString(out string value)
        {
            ushort len;
            ReadUInt16(out len);
            value = Encoding.UTF8.GetString(m_buffer, m_offset, len);
            m_offset += len;
        }

        internal void ReadCString(out string value)
        {
            int len = 0;
            while (m_offset + len < m_buffer.Length && m_buffer[m_offset + len] != 0)
                len++;

            value = Encoding.UTF8.GetString(m_buffer, m_offset, len);
            m_offset += len + 1;
        }

        internal void SkipCString(out string value)
        {
          int len = 0;
          while (m_offset + len < m_buffer.Length && m_buffer[m_offset + len] != 0)
            len++;

          m_offset += len + 1;
          value= null;
        }

        internal void ReadGuid(out Guid guid)
        {
            int a;
            short b, c;
            byte[] d = new byte[8];

            ReadInt32(out a);
            ReadInt16(out b);
            ReadInt16(out c);
            ReadBytes(d);

            guid = new Guid(a, b, c, d);
        }

        internal string ReadString()
        {
            int len = 0;
            while (m_offset + len < m_buffer.Length && m_buffer[m_offset + len] != 0)
                len+=2;

            string result = Encoding.Unicode.GetString(m_buffer, m_offset, len);
            m_offset += len + 2;
            return result;
        }

        internal DateTime ReadFileTime()
        {
            Int64 filetime = 0;
            ReadInt64(out filetime);
            return DateTime.FromFileTime(filetime);
        }
    }
}
