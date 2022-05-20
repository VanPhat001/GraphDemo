using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.Structures
{
    public enum FlowLableStatus
    {
        Decrease = -1,
        Nothing,
        Increase,
    }

    public class FlowLabel
    {
        public FlowLableStatus Dir { get; set; }
        public int Parent { get; set; }
        public double Flow { get; set; }

        public FlowLabel()
        {
            SetValue(FlowLableStatus.Nothing, -1, 0);
        }

        public FlowLabel(FlowLableStatus dir, int parent, double flow)
        {
            SetValue(dir, parent, flow);
        }

        public FlowLabel(FlowLabel flowLabel)
        {
            SetValue(flowLabel.Dir, flowLabel.Parent, flowLabel.Flow);
        }

        public void SetValue(FlowLableStatus dir, int parent, double flow)
        {
            this.Dir = dir;
            this.Parent = parent;
            this.Flow = flow;
        }

        public override string ToString()
        {
            return $"dir = {this.Dir}\nparent = {this.Parent}\nflow = {this.Flow}";
        }
    }
}
