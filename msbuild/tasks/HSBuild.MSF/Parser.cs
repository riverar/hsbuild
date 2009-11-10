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
    public class Parser : IDisposable
    {
        public struct Header
        {
            public const string IDENT =     "Microsoft C/C++ MSF 7.00\r\n\x001ADS\0\0";
            public const int IDENTsize = 0x20;

            public UInt32                   dPageBytes;         // 0x0400
            public UInt32                   dFlagPage;          // 0x0002
            public UInt32                   dFilePages;         // file size / dPageBytes
            public UInt32                   dRootBytes;         // stream directory size
            public UInt32                   dReserved;          // 0
            public UInt32[]                 adIndexPages;       // root page index pages

            public uint GetNoOfPages(uint bytes)
            {
                if (bytes == 0 || dPageBytes == 0)
                    return 0;

                return ((bytes - 1) / dPageBytes) + 1;
            }

            public bool Deserialize(BinaryReader reader)
            {
                byte[] ident = reader.ReadBytes(IDENTsize);
                string sIdent = ASCIIEncoding.ASCII.GetString(ident);

                if (sIdent.CompareTo(IDENT) != 0)
                    return false;

                dPageBytes = reader.ReadUInt32();
                dFlagPage = reader.ReadUInt32();
                dFilePages = reader.ReadUInt32();
                dRootBytes = reader.ReadUInt32();
                dReserved = reader.ReadUInt32();

                uint rootPagesLen = GetNoOfPages(dRootBytes);
                uint indexPagesLen = GetNoOfPages(rootPagesLen * sizeof(UInt32));

                adIndexPages = new UInt32[indexPagesLen];
                for (uint i = 0; i < indexPagesLen; i++)
                    adIndexPages[i] = reader.ReadUInt32();

                return true;
            }
        }

        private Parser() {}

        public static Parser CreateFromFile(string file)
        {
            BinaryReader br = null;
            try
            {
                br = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                var header = new Header();

                var ret = new Parser();
                if (!header.Deserialize(br))
                {
                    br.Close();
                    return null;
                }

                ret.m_br = br;
                ret.m_header = header;

                UInt32[] rootPages = ret.GetPageIndexes(header.adIndexPages, ret.GetNoOfPages(header.dRootBytes));
                byte[] rootStream = ret.GetPageBytes(rootPages, header.dRootBytes);
                ret.m_streams = ret.GetStreamList(rootStream);

                return ret;
            }
            catch (IOException)
            {
                if (br != null)
                    br.Close();
                return null;
            }
        }

        private UInt32[] GetPageIndexes(UInt32[] pages, uint indexes)
        {
            uint indexesLeft = indexes;
            List<UInt32> ret = new List<UInt32>((int)indexes);

            foreach (UInt32 p in pages)
            {
                m_br.BaseStream.Seek(p * m_header.dPageBytes, SeekOrigin.Begin);
                for (int i = 0; i < m_header.dPageBytes / 4 && indexesLeft > 0; i++, indexesLeft--)
                {
                    ret.Add(m_br.ReadUInt32());
                }
            }

            return ret.ToArray();
        }

        private List<KeyValuePair<List<UInt32>, UInt32>> GetStreamList(byte[] rootStream)
        {
            BitHelper bits = new BitHelper(rootStream);
            Int32 streams;

            bits.ReadInt32(out streams);
            List<KeyValuePair<List<UInt32>, UInt32>> ret = new List<KeyValuePair<List<UInt32>, UInt32>>(streams);

            int pageOffset = bits.Position + (streams * sizeof(UInt32));
            for (int i = 0; i < streams; i++)
            {
                UInt32 size = 0;
                bits.ReadUInt32(out size);
                int pageLen = (int)GetNoOfPages(size);
                List<UInt32> pages = new List<UInt32>(pageLen);

                int pos = bits.Position;
                while (pageLen-- > 0)
                {
                    UInt32 page = 0;

                    bits.Position = pageOffset;
                    bits.ReadUInt32(out page);
                    pageOffset = bits.Position;

                    pages.Add(page);
                }
                bits.Position = pos;

                ret.Add(new KeyValuePair<List<UInt32>, UInt32>(pages, size));
            }

            return ret;
        }

        private void ReadPage(uint page, int size, byte[] buffer, int offset)
        {
            int i = offset, len = (int)size;

            m_br.BaseStream.Seek(page * m_header.dPageBytes, SeekOrigin.Begin);

            while (len > 0)
            {
                int tmp = m_br.Read(buffer, i, len);
                i += tmp;
                len -= tmp;
            }
        }

        public uint GetNoOfPages(uint bytes)
        {
            return m_header.GetNoOfPages(bytes);
        }

        public byte[] GetPageBytes(uint page)
        {
            return GetPageBytes(page, m_header.dPageBytes);
        }

        public byte[] GetPageBytes(uint page, uint size)
        {
            byte[] ret = new byte[size];
            ReadPage(page, (int)size, ret, 0);
            return ret;
        }

        public byte[] GetPageBytes(uint[] pages)
        {
            return GetPageBytes(pages, m_header.dPageBytes * (uint)pages.Length);
        }

        public byte[] GetPageBytes(uint[] pages, uint size)
        {
            byte[] ret = new byte[size];
            int offset = 0;
            int len = (int)size;
            int pageSize = (int)m_header.dPageBytes;

            foreach (uint p in pages)
            {
                ReadPage(p, Math.Min(pageSize, len), ret, offset);

                offset += pageSize;
                len -= pageSize;
            }

            return ret;
        }

        private BinaryReader m_br;
        private Header m_header;

        private List<KeyValuePair<List<UInt32>, UInt32>> m_streams;

        public DataStream[] GetStreams()
        {
            List<DataStream> ret = new List<DataStream>(m_streams.Count);
            ret.Add(GetStreamInfoDataStream());
            ret.Add(GetIndexDataStream());

            for (int i = 2; i < m_streams.Count; i++)
            {
                var p = m_streams[i];
                if (p.Value > 0)
                    ret.Add(new UnknownDataStream((uint)i, new BitHelper(GetPageBytes(p.Key.ToArray(), p.Value))));
                else
                    ret.Add(new EmptyDataStream((uint)i));
            }

            return ret.ToArray();
        }

        public List<UInt32> GetStreamPages(uint index, out UInt32 size)
        {
            KeyValuePair<List<UInt32>, UInt32> p = m_streams[(int)index];
            size = p.Value;
            return p.Key;
        }

        public byte[] GetStreamBytes(uint index)
        {
            uint size = 0;
            return GetPageBytes(GetStreamPages(index, out size).ToArray(), size);
        }

        public StreamInfoDataStream GetStreamInfoDataStream()
        {
            return new StreamInfoDataStream(new BitHelper(GetStreamBytes(StreamInfoDataStream.StreamIndex)));
        }

        public IndexDataStream GetIndexDataStream()
        {
            return new IndexDataStream(new BitHelper(GetStreamBytes(IndexDataStream.StreamIndex)));
        }

        #region IDisposable Members

        public void Dispose()
        {
            m_br.Close();
            m_streams = null;
        }

        #endregion
    }
}
