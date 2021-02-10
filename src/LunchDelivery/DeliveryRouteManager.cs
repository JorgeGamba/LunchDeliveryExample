using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LunchDelivery
{
    public class DeliveryRouteManager
    {
        private ICollection<ScheduledDelivery> _scheduledDeliveries;
        private readonly List<ConfirmedDelivery> _confirmedDeliveries = new();
        private readonly Drone _assignedDrone;

        public DeliveryRouteManager(ICollection<ScheduledDelivery> scheduledDeliveries, Drone assignedDrone)
        {
            GuardAgainstNoDeliveries(scheduledDeliveries, assignedDrone);

            _scheduledDeliveries = scheduledDeliveries;
            _assignedDrone = assignedDrone;
        }

        public IResult StartOperation() => 
            CreateDeliveryTripRequestWith(TakeFollowingDeliveries());

        public IResult Confirm(DeliveryTripConfirmed deliveryTripConfirmed)
        {
            GuardAgainstUnknownDroneOf(deliveryTripConfirmed);
            GuardAgainstUnknownDeliveryIn(deliveryTripConfirmed);

            _confirmedDeliveries.AddRange(deliveryTripConfirmed.ConfirmedDeliveries);
            _scheduledDeliveries = GetRemainingNotConfirmedDeliveriesYet(deliveryTripConfirmed);
            var followingDeliveries = TakeFollowingDeliveries();
            if (followingDeliveries.Any())
                return CreateDeliveryTripRequestWith(followingDeliveries);

            return new DeliveryRouteResult
            {
                PerformerDroneId = _assignedDrone.Id,
                ConfirmedDeliveries = _confirmedDeliveries
            };
        }


        private static void GuardAgainstNoDeliveries(ICollection<ScheduledDelivery> scheduledDeliveries, Drone assignedDrone)
        {
            if (!scheduledDeliveries.Any())
                throw new ArgumentException(
                    $"The manager for delivering the route assigned to the Drone '{assignedDrone.Id}' cannot be created without providing any scheduled delivery.",
                    nameof(scheduledDeliveries));
        }
        
        private void GuardAgainstUnknownDroneOf(DeliveryTripConfirmed deliveryTripConfirmed)
        {
            if (!deliveryTripConfirmed.PerformerDroneId.Equals(_assignedDrone.Id))
                // TODO: Create more specific Exception types in case the client for this object decides it's going to manage this kind of inconsistency
                throw new ArgumentException(
                    $"The delivery trip was confirmed as performed by the Drone '{deliveryTripConfirmed.PerformerDroneId}', however, the actual Drone assigned to this manager is '{_assignedDrone.Id}'.",
                    nameof(deliveryTripConfirmed.PerformerDroneId));
        }
        
        private void GuardAgainstUnknownDeliveryIn(DeliveryTripConfirmed deliveryTripConfirmed)
        {
            foreach (var confirmedDelivery in deliveryTripConfirmed.ConfirmedDeliveries)
                if (!_scheduledDeliveries.Select(x => x.Id).Contains(confirmedDelivery.Id))
                    // TODO: Create more specific Exception types in case the client for this object decides it's going to manage this kind of inconsistency
                    throw new ArgumentException(
                        $"The delivery trip was confirmed including the delivery '{confirmedDelivery.Id}', however, that delivery was not requested by the manager operating the Drone '{_assignedDrone.Id}'.",
                        nameof(confirmedDelivery.Id));
        }

        private DeliveryTripRequest CreateDeliveryTripRequestWith(ICollection<ScheduledDelivery> scheduledDeliveries) =>
            new() {ResponsibleDroneId = _assignedDrone.Id, ScheduledDeliveries = scheduledDeliveries};

        private ImmutableList<ScheduledDelivery> GetRemainingNotConfirmedDeliveriesYet(DeliveryTripConfirmed deliveryTripConfirmed) =>
            _scheduledDeliveries.Where(x => !deliveryTripConfirmed.ConfirmedDeliveries.Select(x => x.Id).Contains(x.Id)).ToImmutableList();

        private ICollection<ScheduledDelivery> TakeFollowingDeliveries() => 
            _scheduledDeliveries.Take(_assignedDrone.MaximumDeliveryCapacityPerTrip).ToImmutableList();
    }
}