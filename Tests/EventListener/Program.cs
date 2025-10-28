using SubsManagerInterface;
using SubsManagerImplementation;
using EventBusInterface;
using EventBusImplementation;



// unlike recipes, which runs in the same Docker virtual network as the rabbitmq 
// host, this listener app runs in localhost and so should access rabbitmq
// via its localhost port
string connectionString = "localhost";

string brokerName = "MyMicroserviceApp";
string serviceName = "eventListener";
ISubsManager subsManager = new SubsManagerStub();