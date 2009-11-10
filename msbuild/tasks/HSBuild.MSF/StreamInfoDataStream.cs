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
    public class StreamInfoDataStream : DataStream
    {
        public const UInt32 StreamIndex = 0;

        internal StreamInfoDataStream(BitHelper data)
            : base(StreamIndex, (uint)data.Length)
        {
            UInt32 count;
            data.ReadUInt32(out count);

            if ((count + 1) * sizeof(UInt32) > data.Length)
                throw new ApplicationException("Unable to parse StreamInfoDataStream");

            m_streamSize = new UInt32[(int)count];

            for (uint i = 0; i < count; i++)
                data.ReadUInt32(out m_streamSize[i]);

            m_unknown = new byte[data.Length - data.Position];
            data.ReadBytes(m_unknown);
        }

        public override string Name { get { return "StreamInfoDataStream"; } }
        public override void Print(TextWriter writer)
        {
            base.Print(writer);

            writer.WriteLine(" unknown 0x{0:x8} bytes:", m_unknown.Length);
            PrintBytes(writer, m_unknown);
        }

        private UInt32[] m_streamSize;
        private byte[] m_unknown;
    }
}
