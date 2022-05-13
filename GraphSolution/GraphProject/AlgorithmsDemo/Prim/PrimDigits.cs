using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.AlgorithmsDemo.Prim
{
    public enum PrimDigits
    {
        Init,
        SelectNode,
        UpdateMinCost,
        CancelNode,
        Marked,
        SelectEdge,
        UpdateCost,
        CancelEdge,
        SpanningTree
    }
}
