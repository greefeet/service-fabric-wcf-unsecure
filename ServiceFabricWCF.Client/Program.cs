using ServiceFabricWCF.Shared;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace ServiceFabricWCF.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //In fabric use reverse proxy endpoint
            //string ExactEndpoint = "http://localhost:30002/250d8bdf-6b44-4da6-b8d6-24a8159060d6/9e8c478f-7cff-4774-94a4-cf90e410442b-131472564957114134";
            string ReverseProxyEndpoint = "http://localhost:19081/ServiceFabricWCF/Calculator";


            ICalculator CalculatorService = FactoryBasicHttpBinding<ICalculator>(ReverseProxyEndpoint);
            Console.WriteLine($"Service Result : {await CalculatorService.Add(2,3)}");
        }

        static o FactoryBasicHttpBinding<o>(string EndpointAddressUri)
        {
            Binding binding = new BasicHttpBinding();
            ChannelFactory<o> Factory = new ChannelFactory<o>(new BasicHttpBinding());
            return Factory.CreateChannel(new EndpointAddress(EndpointAddressUri));
        }
    }
}
