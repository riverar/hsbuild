// HSBuild.Core - Patch
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

namespace HSBuild.Core
{
    public class Patch
    {
        public Patch(Uri uri, string strip, Patch next)
        {
            m_uri = uri;
            m_next = next;

            try
            {
                m_strip = uint.Parse(strip);
            }
            catch
            {
                m_strip = 0;
            }
        }

        #region Properties

        public Uri Uri
        {
            get { return m_uri; }
        }

        public uint Strip
        {
            get { return m_strip; }
        }

        public Patch Next
        {
            get { return m_next; }
        }

        #endregion

        private Uri m_uri;
        private uint m_strip;
        private Patch m_next;
    }
}
