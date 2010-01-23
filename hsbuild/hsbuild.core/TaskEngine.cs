// HSBuild.Core - Task/TaskEngine interfaces
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
}
