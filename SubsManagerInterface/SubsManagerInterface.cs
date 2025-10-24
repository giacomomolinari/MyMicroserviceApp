using System.Runtime.CompilerServices;

namespace SubsManagerInterface;

public interface SubsManagerInterface
{

    // Add a subscription for events:T to be handled by handler: TH
    void addSubscription<T, TH>();

    // Remove a subscription for events:T to be handled by handler: TH
    void removeSubscription<T, TH>();

    // Return handler type name for events of type T
    // Return null if there is no subscription to events of type T
    string? getHandlerTypeIfSubscribed<T>();

}
