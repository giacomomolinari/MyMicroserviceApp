using SubsManagerInterface;

namespace SubsManagerImplementation;

public class SubsManagerStub : ISubsManager
{
    // for each Event Type name, store list of EventHandler Type names
    // It would be good to make this an independent data store, because we don't want it to be lost in
    // case the service stops running...
    private Dictionary<string, List<string>> _subscriptionDict = new Dictionary<string, List<string>>();

    public void addSubscription<T, TH>()
    {
        string eventName = typeof(T).Name;
        string eventHandlerName = typeof(TH).Name;

        List<string>? handlerTypeList;
        if (_subscriptionDict.TryGetValue(eventName, out handlerTypeList)) // if there is already a list of handlers associated to this event...
        {
            handlerTypeList.Add(eventHandlerName);                         // ..add to the list
        }
        else                                                               // Otherwise...      
        {
            handlerTypeList = new List<string> { eventHandlerName };
            _subscriptionDict.Add(eventName, handlerTypeList);             // ..add new entry to dictionary
        }


    }

    public List<string>? getHandlerTypeIfSubscribed(string eventName)
    {

        List<string>? handlerTypeList = (_subscriptionDict.TryGetValue(eventName, out handlerTypeList)) ? handlerTypeList : null;

        return handlerTypeList;

    }

    public void removeSubscription<T, TH>()
    {
        throw new NotImplementedException();
    }
}
