/*
    1fpm -- One File Per Message - File oriented mail system
    Copyright (C) 2015  blueshell Software Engineering Harry von Borstel 
    (http://www.blueshell.com)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

 */
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controls
{
    public class ShellMockFolder : ISimpleNode
    {
        public ShellMockFolder Parent { get; set; }
        public string Name { get; set; }

        public List<ShellMockFolder> Children { get; set; }

        public ShellMockFolder(string name, params ShellMockFolder[] children)
        {
            this.Name = name;
            this.Children = new List<ShellMockFolder>();
            foreach(var child in children)
            {
                child.Parent = this;
                this.Children.Add(child);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        #region IEnumerable<ShellMockFolder> Members

        /// <summary>
        /// Enumerates through contents of the ShellObjectContainer
        /// </summary>
        /// <returns>Enumerated contents</returns>
        public IEnumerator<ISimpleNode> GetEnumerator()
        {
            return (IEnumerator<ShellMockFolder>)Children.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion


        ISimpleNode ISimpleNode.Parent
        {
            get { return this.Parent; }
        }



        public bool IsEqual(object oNode)
        {
            return this == oNode;
        }
    }
}
