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
    public class NamesDataStream : DataStream
    {
        public const UInt32 MAGIC = 0xEFFEEFFE;

        internal NamesDataStream(uint idx, BitHelper data)
            : base(idx, (uint)data.Length)
        {
            UInt32 type = 0;
            data.ReadUInt32(out type);

            if (type != MAGIC)
                throw new ApplicationException(string.Format("Expected 0x{0:8x}", MAGIC));

            data.ReadInt32(out m_sig);
            Int32 size = 0;
            data.ReadInt32(out size);

            m_names = new Dictionary<string, UInt32>();

            int stringsOffset = data.Position;
            data.Position += size;

            int len = 0;
            data.ReadInt32(out len);

            for (int i = 0; i < len; i++)
            {
                UInt32 n;
                data.ReadUInt32(out n);

                if (n > 0)
                {
                    string k;

                    int pos = data.Position;
                    data.Position = stringsOffset + (int)n;
                    data.ReadCString(out k);
                    data.Position = pos;

                    m_names.Add(k, n);
                }
            }

            if (data.Position + sizeof(UInt32) == data.Length)
            {
                uint names = 0;
                data.ReadUInt32(out names);
                if (names != m_names.Count)
                    throw new ApplicationException();
            }
        }

        public UInt32 FindByName(string name)
        {
            foreach (var p in m_names)
            {
                if (string.Compare(p.Key, name, true) == 0)
                    return p.Value;
            }

            return 0;
        }

        public string FindByKey(UInt32 d)
        {
            foreach (var p in m_names)
            {
                if (p.Value == d)
                    return p.Key;
            }

            return null;
        }

        public override string Name { get { return "NamesDataStream"; } }
        public override void Print(TextWriter writer)
        {
            base.Print(writer);
            foreach (KeyValuePair<string, UInt32> p in m_names)
                writer.WriteLine("{0:x4}:  {1}", p.Value, p.Key);
        }

        private Int32 m_sig;
        private Dictionary<string, UInt32> m_names;
    }
}
