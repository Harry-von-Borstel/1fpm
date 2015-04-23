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
    public interface Adaptor
    {
        object Adaptee { get; set; }
    }

    public class ShellFolderAdaptor: ISimpleNode, Adaptor
    {
        private ShellFolder shellFolder;

        public ShellFolderAdaptor(object shellFolder)
        {
            this.shellFolder = (ShellFolder)shellFolder;
        }

        public object Adaptee
        {
            get { return shellFolder; }
            set { shellFolder = value as ShellFolder; }
        }
        

        public ISimpleNode Parent
        {
            get
            {
                var parent = this.shellFolder.Parent as ShellFolder;
                return parent == null
                    ? null
                    : new ShellFolderAdaptor(parent);
            }
        }

        public string Name
        {
            get { return this.shellFolder.Name; }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return ((IEnumerable<ISimpleNode>)this).GetEnumerator();
        }

        IEnumerator<ISimpleNode> IEnumerable<ISimpleNode>.GetEnumerator()
        {
            foreach (ShellFolder sf in this.shellFolder.Where(so => so is ShellFolder))
                yield return new ShellFolderAdaptor(sf);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public bool IsEqual(object oNode)
        {
            return this.Adaptee.Equals( oNode);
        }
    }
}
