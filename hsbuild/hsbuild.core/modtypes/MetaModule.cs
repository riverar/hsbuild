// HSBuild.Modules - MetaModule
//
// Copyright (C) 2010 Haakon Sporsheim <haakon.sporsheim@gmail.com>
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
using HSBuild.Core;

namespace HSBuild.Modules
{
    public class MetaModule : Module
    {
        public MetaModule(string id, string[] deps)
            : base(id, deps)
        {
        }

        internal override bool BindRepository(System.Collections.Generic.Dictionary<string, Repository> repos)
        {
            return true;
        }
    }
}
