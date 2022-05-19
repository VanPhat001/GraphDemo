using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.AlgorithmsDemo.PlanningAlgorithm
{
    public class PlanningAlgorithmMessage : Message<PlanningAlgorithmDigits, int>
    {
        public PlanningAlgorithmMessage(PlanningAlgorithmDigits digit, List<int> dataList = null) : base(digit, dataList)
        {
        }
    }
}
