// HSBuild.Tasks - ConsoleTasks
//
// Copyright (C) 2009-2010 Haakon Sporsheim <haakon.sporsheim@gmail.com>
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
using System.Diagnostics;
using HSBuild.Core;

namespace HSBuild.Tasks
{
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
}
