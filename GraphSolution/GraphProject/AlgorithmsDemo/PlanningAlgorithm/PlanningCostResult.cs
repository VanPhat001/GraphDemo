using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProject.AlgorithmsDemo.PlanningAlgorithm
{
    public class PlanningCostResult
    {
        public PlanningCostDigits Digit { get; set; }
        public List<double> Results { get; set; }

        public PlanningCostResult(PlanningCostDigits digit, List<double> results = null)
        {
            Digit = digit;
            Results = results;
        }
    }
}
