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

namespace HSBuild.MSF
{
    public abstract class DataStream
    {
        internal DataStream(uint idx, uint size)
        {
            m_index = idx;
            m_size = size;
        }

        public abstract string Name { get; }

        virtual public void Print(TextWriter writer)
        {
            writer.WriteLine("{0} ({1:x4} bytes: {2})", Name, m_index, m_size);
        }

        public static void PrintBytes(TextWriter writer, byte[] buffer)
        {
            if (buffer.Length > 0)
            {
                int i = 0;

                foreach (byte val in buffer)
                {
                    if (i++ % 32 == 0 && i > 1)
                        writer.WriteLine();

                    writer.Write(" {0}", val.ToString("x2"));
                }

                writer.WriteLine();
            }
        }

        protected uint m_index;
        protected uint m_size;
    }

    public class EmptyDataStream : DataStream
    {
        internal EmptyDataStream(uint idx) : base(idx, 0) {}
        public override string Name { get { return "EmptyDataStream"; } }
    }

    public class UnknownDataStream : DataStream
    {
        internal UnknownDataStream(uint idx, BitHelper data)
            : base(idx, (uint)data.Length)
        {
            m_data = new byte[data.Length];
            data.ReadBytes(m_data);
        }

        public override string Name { get { return "UnknownDataStream"; } }

        public override void Print(TextWriter writer)
        {
            base.Print(writer);

            writer.WriteLine(" of {0} bytes (0x{0:x8}):", m_data.Length);
            PrintBytes(writer, m_data);
        }

        byte[] m_data;
    }

}
