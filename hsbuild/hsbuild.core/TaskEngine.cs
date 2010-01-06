// HSBuild.Core - Task/TaskEngine interfaces
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
using System.Diagnostics;
using System.Collections.Generic;

namespace HSBuild.Core
{
    public interface ITask
    {
        int Execute(IOutputEngine output);
    }

    public interface ITaskQueue
    {
        ITask[] TaskQueue { get; }
        void QueueTask(ITask task);
    }

    public interface ITaskEngine
    {
        bool ExecuteTaskQueue(ITask[] queue, IOutputEngine output);
    }

    public class ConsoleTask : ITask
    {
        public ConsoleTask(string command, string[] arguments, string cwd)
        {
            m_info = new ProcessStartInfo(command);
            m_info.WorkingDirectory = cwd;
            m_info.Arguments = arguments == null ? null : string.Join(" ", arguments);
            m_info.UseShellExecute = false;
        }

        #region ITask Members

        public int Execute(IOutputEngine output)
        {
            Process p = Process.Start(m_info);
            p.WaitForExit();

            return p.ExitCode;
        }

        #endregion

        #region Properties

        public ProcessStartInfo StartInfo
        {
            get
            {
                return m_info;
            }
        }

        #endregion

        private ProcessStartInfo m_info;
    }

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
            WebClient client = new WebClient();
            Stream stream_in = client.OpenRead(m_uri);
            Stream stream_out = File.Create(m_local);

            byte[] buffer = new byte[1024];
            int len;

            while ((len = stream_in.Read(buffer, 0, buffer.Length)) != 0)
                stream_out.Write(buffer, 0, len);

            stream_in.Close();
            stream_out.Close();
            return len;
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
