---
services: service-fabric
platforms: dotnet
Fabric Cluster Version: 5.7.198.9494
Fabric Nuget Version: 5.7.198, 2.7.198
---

# Service Fabric WCF Unsecure

This sample project demonstrates Client and WCF Service in azure service fabric.

## Getting Start

1. Install [Microsoft Azure Service Fabric SDK](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started)
2. Change Build Profile to "x64"
3. Build Solutions
4. Publish "ServiceFabricWCF" to local cluster - [Deploy and remove applications using Visual Studio](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-publish-app-remote-cluster)
5. Run "ServiceFabricWCF.Client". Client Result :
```cmd
Service Result : 5
```

## Client WCF with BasicHttpBinding

```cs
//Program.cs
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
```

## Stateless Service with WCF+BasicHttpBinding

```cs
//1. Regsiter Service - Program.cs
internal static class Program
{
    private static void Main()
    {
        try
        {
            ServiceRuntime.RegisterServiceAsync("CalculatorType", context => new Calculator(context)).GetAwaiter().GetResult();
                
            ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Calculator).Name);
            
            // Prevents this host process from terminating so services keep running.
            Thread.Sleep(Timeout.Infinite);
        }
        catch (Exception e)
        {
            ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
            throw;
        }
    }
}
```

```cs
//2. Create communication instant - Calculator.cs
internal sealed class Calculator : StatelessService
{
    protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
    {
        return new[] {
            new ServiceInstanceListener(context => new WcfCommunicationListener<ICalculator>(
                context, 
                new CalculatorServiceSimple(),
                new BasicHttpBinding(),
                endpointResourceName: "ServiceEndpoint"
                ))
        };
    }
}
```

```cs
//3. Implement service - CalculatorService.cs
public class CalculatorServiceSimple : ICalculator
{
    public Task<double> Add(double n1, double n2)
    {
        return Task.FromResult(n1 + n2);
    }
}
```

```xml
<!--ServiceManifest.xml-->
<ServiceManifest>
  <Resources>
    <Endpoints>
      <Endpoint Name="ServiceEndpoint" Protocol="http" />
    </Endpoints>
  </Resources>
</ServiceManifest>
```

```xml
<!--http://localhost:19080/Explorer/index.html#/tab/manifest-->
<ClusterManifest>
     <NodeTypes>
        <NodeType Name="NodeType0">
            <Endpoints>
                <!--1908 is use to be Reverse Proxy Endpoint at http://localhost:19081/ServiceFabricWCF/Calculator -->
                <HttpApplicationGatewayEndpoint Port="19081" Protocol="http" />
            </Endpoints>
        </NodeTypes>
     </NodeTypes>
</ClusterManifest>
```

