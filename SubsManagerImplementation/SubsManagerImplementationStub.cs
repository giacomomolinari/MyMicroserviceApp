using SubsManagerInterface;

namespace SubsManagerImplementation;

public class SubsManagerStub : ISubsManager
{
    // for each Event Type name, store list of concrete EventHandler Type objects
    // It would be good to make this an independent data store, because we don't want it to be lost in
    // case the service stops running...
    private Dictionary<string, List<Type>> _subscriptionDict = new Dictionary<string, List<Type>>();

    public void addSubscription<T, TH>()
    {
        string eventName = typeof(T).Name;
        Type eventHandlerType = typeof(TH);

        List<Type>? handlerTypeList;
        if (_subscriptionDict.TryGetValue(eventName, out handlerTypeList)) // if there is already a list of handlers associated to this event...
        {
            handlerTypeList.Add(eventHandlerType);                         // ..add to the list
        }
        else                                                               // Otherwise...      
        {
            handlerTypeList = new List<Type> { eventHandlerType };
            _subscriptionDict.Add(eventName, handlerTypeList);             // ..add new list containing entry to dictionary
        }

    }

    /// <summary>
    /// Given event name, return list of handlers for that event.
    /// Return null if there is no such handler registered.
    /// </summary>
    /// <param name="eventName"> Name of event </param>
    /// <returns></returns>
    public List<Type>? getHandlerTypeIfSubscribed(string eventName)
    {

        List<Type>? handlerTypeList = _subscriptionDict.TryGetValue(eventName, out handlerTypeList) ? handlerTypeList : null;

        return handlerTypeList;

    }

    public void removeSubscription<T, TH>()
    {
        throw new NotImplementedException();
    }
}
