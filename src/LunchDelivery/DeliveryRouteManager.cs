using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LunchDelivery
{
    public class DeliveryRouteManager
    {
        private ICollection<ScheduledDelivery> _scheduledDeliveries;
        private readonly List<ConfirmedDelivery> _confirmedDeliveries = new List<ConfirmedDelivery>();
        private readonly Drone _assignedDrone;

        public DeliveryRouteManager(ICollection<ScheduledDelivery> scheduledDeliveries, Drone assignedDrone)
        {
            _scheduledDeliveries = scheduledDeliveries;
            _assignedDrone = assignedDrone;
        }

        public IResult StartOperation() => 
            new DeliveryTripRequest {ScheduledDeliveries = TakeFollowingDeliveries()};

        public IResult Confirm(DeliveryTripConfirmed deliveryTripConfirmed)
        {
            _confirmedDeliveries.AddRange(deliveryTripConfirmed.ConfirmedDeliveries);
            _scheduledDeliveries = GetRemainingNotConfirmedDeliveriesYet(deliveryTripConfirmed);
            var followingDeliveries = TakeFollowingDeliveries();
            if (followingDeliveries.Any())
            {
                return new DeliveryTripRequest {ScheduledDeliveries = followingDeliveries};
            }

            return new DeliveryRouteResult {ConfirmedDeliveries = _confirmedDeliveries};
        }

        private ImmutableList<ScheduledDelivery> GetRemainingNotConfirmedDeliveriesYet(DeliveryTripConfirmed deliveryTripConfirmed) => 
            _scheduledDeliveries.Where(x => !deliveryTripConfirmed.ConfirmedDeliveries.Select(x => x.Id).Contains(x.Id)).ToImmutableList();

        private ICollection<ScheduledDelivery> TakeFollowingDeliveries() => 
            _scheduledDeliveries.Take(_assignedDrone.MaximumDeliveryCapacityPerTrip).ToImmutableList();
    }
}