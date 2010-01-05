// HSBuild.Tasks - Set/GetEnv
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
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace HSBuild.Tasks
{
    public class SetEnv : Task
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
        [Output]
        public string OutputEnvironmentVariable
        {
            get { return Environment.GetEnvironmentVariable(Name); }
        }

        public override bool Execute()
        {
            Environment.SetEnvironmentVariable(Name, Value);
            return true;
        }
    }

    public class GetEnv : Task
    {
        [Required]
        public string Name { get; set; }
        [Required, Output]
        public string Value { get; set; }

        public override bool Execute()
        {
            Value = Environment.GetEnvironmentVariable(Name);
            return true;
        }
    }
}
