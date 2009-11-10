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
    public class IndexDataStream : DataStream
    {
        public const UInt32 MAGIC = 0x01312E94;
        public const UInt32 StreamIndex = 1;

        internal IndexDataStream(BitHelper data)
            : base(StreamIndex, (uint)data.Length)
        {
            UInt32 type = 0;
            data.ReadUInt32(out type);

            if (type != MAGIC)
                throw new ApplicationException(string.Format("Expected 0x{0:8x}", MAGIC));

            data.ReadInt32(out m_sig);
            data.ReadInt32(out m_age);
            data.ReadGuid(out m_guid);
            Int32 size = 0;
            data.ReadInt32(out size);

            m_indexs = new Dictionary<string, Int32>();

            int stringsOffset = data.Position;
            data.Position += size;

            int count = 0, max = 0;
            data.ReadInt32(out count);
            data.ReadInt32(out max);

            BitFields present = new BitFields(data);
            BitFields deleted = new BitFields(data);

            if (deleted.Length != 0)
                throw new ApplicationException("Unsupported IndexDataStream");

            for (int i = 0; i < max; i++)
            {
                if (present.IsSet(i))
                {
                    string key;
                    Int32 val, offset;

                    data.ReadInt32(out offset);
                    data.ReadInt32(out val);

                    int pos = data.Position;
                    data.Position = stringsOffset + offset;
                    data.ReadCString(out key);
                    data.Position = pos;

                    m_indexs.Add(key.ToLowerInvariant(), val);
                }
            }

            if (count != m_indexs.Count)
                throw new ApplicationException();
        }

        public override string Name { get { return "IndexDataStream"; } }
        public override void Print(TextWriter writer)
        {
            base.Print(writer);
            foreach (KeyValuePair<string, Int32> p in m_indexs)
                writer.WriteLine("  {0:x8} {1}", p.Value, p.Key);
        }

        public Int32 GetStreamID(string name)
        {
            Int32 ret = -1;
            if (!m_indexs.TryGetValue(name.ToLowerInvariant(), out ret))
                return -1;

            return ret;
        }

        public byte[] GetStreamDataByID(Int32 id, Parser parser)
        {
            uint size = 0;
            var pages = parser.GetStreamPages((uint)id, out size);
            if (pages == null)
                return null;

            return parser.GetPageBytes(pages.ToArray(), size);
        }

        public byte[] GetStreamDataByName(string name, Parser parser, out Int32 id)
        {
            id = GetStreamID(name);
            return (id < 0) ? null : GetStreamDataByID(id, parser);
        }

        public NamesDataStream GetNamesStream(Parser parser)
        {
            uint i = (uint)GetStreamID("/names");
            return new NamesDataStream(i, new BitHelper(parser.GetStreamBytes(i)));
        }

        public HSBuild.MSF.MR.FileInfoDataStream GetFileInfoStream(Parser parser)
        {
            uint i = (uint)GetStreamID("/mr/files/info");
            return new HSBuild.MSF.MR.FileInfoDataStream(i, new BitHelper(parser.GetStreamBytes(i)));
        }

        public HSBuild.MSF.MR.FilesDataStream GetFileStream(uint idxFile, Parser parser)
        {
            uint i = (uint)GetStreamID(string.Format("/mr/files/{0:x8}", idxFile));
            return new HSBuild.MSF.MR.FilesDataStream(i, new BitHelper(parser.GetStreamBytes(i)));
        }

        private Int32 m_sig;
        private Int32 m_age;
        private Guid m_guid;
        private Dictionary<string, Int32> m_indexs;
    }
}
