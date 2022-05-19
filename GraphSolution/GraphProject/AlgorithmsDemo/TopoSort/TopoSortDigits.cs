using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.AlgorithmsDemo.TopoSort
{
    public enum TopoSortDigits
    {
        AddNodeIntoRoot,
        PickRoot,
        UpdateNeighbour,
        NewRoots,
        CancelEdge,
        CalcDegree
    }
}
