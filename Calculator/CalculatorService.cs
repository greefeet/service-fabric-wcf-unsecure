using ServiceFabricWCF.Shared;
using System.Fabric;
using System.Threading.Tasks;

namespace Calculator
{
    public class CalculatorServiceSimple : ICalculator
    {
        public Task<double> Add(double n1, double n2)
        {
            return Task.FromResult(n1 + n2);
        }
    }
    public class CalculatorService : ICalculator
    {
        private readonly ServiceContext context;
        public CalculatorService(ServiceContext context)
        {
            this.context = context;
        }
        public Task<double> Add(double n1, double n2)
        {
            return Task.FromResult(n1 + n2);
        }
    }
    
}
