using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LunchDelivery
{
    public class DeliveryRouteManager
    {
        private readonly ICollection<ScheduledDelivery> _scheduledDeliveries;
        private readonly Drone _assignedDrone;

        public DeliveryRouteManager(ICollection<ScheduledDelivery> scheduledDeliveries, Drone assignedDrone)
        {
            _scheduledDeliveries = scheduledDeliveries;
            _assignedDrone = assignedDrone;
        }

        public DeliveryTripRequest StartOperation()
        {
            return new DeliveryTripRequest {ScheduledDeliveries = TakeOnlyTheFirstAllowedDeliveries()};
        }

        
        private ICollection<ScheduledDelivery> TakeOnlyTheFirstAllowedDeliveries()
        {
            return _scheduledDeliveries.Take(_assignedDrone.MaximumDeliveryCapacityPerTrip).ToImmutableList();
        }
    }
}