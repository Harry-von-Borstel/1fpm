using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controls
{
    public interface ISimpleNode: IEnumerable<ISimpleNode>
    {
        ISimpleNode Parent { get; }
        string Name { get; }
        bool IsEqual(object oNode);
    }
}
