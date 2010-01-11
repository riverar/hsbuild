// HSBuild.Tasks - HttpTask
//
// Copyright (C) 2009-2010 Haakon Sporsheim
//
// HSBuild is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// HSBuild is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with HSBuild.  If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.IO;
using System.Net;
using HSBuild.Core;

namespace HSBuild.Tasks
{
    public class HttpTask : ITask
    {
        public HttpTask(Uri uri)
        {
            m_uri = uri;
            m_local = Path.Combine(Path.GetTempPath(), m_uri.Segments[m_uri.Segments.Length - 1]);
        }

        public HttpTask(Uri uri, string local)
        {
            m_uri = uri;
            if (string.IsNullOrEmpty(local))
                local = Path.Combine(Path.GetTempPath(), m_uri.Segments[m_uri.Segments.Length - 1]);

            m_local = local;
        }

        #region ITask Members

        public int Execute(IOutputEngine output)
        {
            output.WriteOutput(OutputType.Heading, "Download: " + m_uri.ToString());

            WebClient client = new WebClient();
            Stream stream_in = client.OpenRead(m_uri);
            if (!stream_in.CanRead)
                return 1;

            long lenStream = -1;
            string lenContent = client.ResponseHeaders.Get("Content-Length");
            if (!string.IsNullOrEmpty(lenContent))
            {
                output.WriteOutput(OutputType.Info, string.Format("Size: {0} bytes", lenContent));
                lenStream = long.Parse(lenContent);
            }

            Stream stream_out = File.Create(m_local);
            int lenDownloaded = 0;

            try
            {
                byte[] buffer = new byte[1024];
                int len;

                if (lenStream > 0)
                {
                    float lastoutput = 0;

                    while ((len = stream_in.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        lenDownloaded += len;
                        stream_out.Write(buffer, 0, len);

                        float per = (float)lenDownloaded / (float)lenStream;
                        if (per > lastoutput + 0.1 || per >= 1)
                        {
                            lastoutput = per;
                            output.WriteOutput(OutputType.Info, string.Format("Downloaded: {0} bytes ({1})", lenDownloaded, per.ToString("P0")));
                        }
                    }
                }
                else
                {
                    while ((len = stream_in.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        lenDownloaded += len;
                        stream_out.Write(buffer, 0, len);
                        output.WriteOutput(OutputType.Debug, string.Format("Downloaded: {0} bytes", lenDownloaded));
                    }
                }
            }
            finally
            {
                stream_in.Close();
                stream_out.Close();
            }

            return lenDownloaded == 0 ? -1 : 0;
        }

        #endregion

        #region Properties

        public Uri Uri
        {
            get
            {
                return m_uri;
            }
        }

        public string LocalFile
        {
            get
            {
                return m_local;
            }
        }

        #endregion

        private Uri m_uri;
        private string m_local;
    }
}
