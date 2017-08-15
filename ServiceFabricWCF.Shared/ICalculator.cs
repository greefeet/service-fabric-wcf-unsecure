using System.ServiceModel;
using System.Threading.Tasks;

namespace ServiceFabricWCF.Shared
{
    [ServiceContract]
    public interface ICalculator
    {
        [OperationContract]
        Task<double> Add(double n1, double n2);
    }
}