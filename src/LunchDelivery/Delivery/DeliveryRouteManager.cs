using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LunchDelivery.Schedule;

namespace LunchDelivery.Delivery
{
    public class DeliveryRouteManager
    {
        private readonly ICollection<ScheduledDelivery> _allScheduledDeliveries;
        private ICollection<ScheduledDelivery> _remainingScheduledDeliveries;
        private readonly List<ConfirmedDelivery> _confirmedDeliveries = new();
        private readonly List<IFailedDelivery> _allFailedDeliveries = new List<IFailedDelivery>();
        private readonly Drone _assignedDrone;

        public DeliveryRouteManager(ICollection<ScheduledDelivery> scheduledDeliveries, Drone assignedDrone)
        {
            GuardAgainstNoDeliveries(scheduledDeliveries, assignedDrone);

            _allScheduledDeliveries = scheduledDeliveries;
            _remainingScheduledDeliveries = scheduledDeliveries;
            _assignedDrone = assignedDrone;
        }

        public DeliveryTripRequest StartOperation() => 
            CreateDeliveryTripRequestWith(TakeFollowingDeliveries());

        public ICheckResult Check(DeliveryTripConfirmed deliveryTripConfirmed)
        {
            GuardAgainstUnknownDroneOf(deliveryTripConfirmed);
            GuardAgainstUnknownDeliveryIn(deliveryTripConfirmed);

            var failedDeliveries = GetFailedByWrongPositionDeliveriesGiven(deliveryTripConfirmed.ConfirmedDeliveries)
                .Concat(GetFailedDeliveriesByAlreadyConfirmedGiven(deliveryTripConfirmed.ConfirmedDeliveries))
                .ToImmutableList();
            _allFailedDeliveries.AddRange(failedDeliveries);
            _confirmedDeliveries.AddRange(deliveryTripConfirmed.ConfirmedDeliveries);
            _remainingScheduledDeliveries = GetRemainingNotConfirmedDeliveriesYet(deliveryTripConfirmed);
            var followingDeliveries = TakeFollowingDeliveries();
            if (followingDeliveries.Any())
            {
                return new WorkOrder
                {
                    Request = CreateDeliveryTripRequestWith(followingDeliveries),
                    FailedDeliveries = failedDeliveries
                };
            }

            return new DeliveryRouteReport
            {
                PerformerDroneId = _assignedDrone.Id,
                ScheduledDeliveries = _allScheduledDeliveries,
                ConfirmedDeliveries = _confirmedDeliveries,
                FailedDeliveries = _allFailedDeliveries,
                LastFailedDeliveries = failedDeliveries
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
                if (!_allScheduledDeliveries.Select(x => x.Id).Contains(confirmedDelivery.Id))
                    // TODO: Create more specific Exception types in case the client for this object decides it's going to manage this kind of inconsistency
                    throw new ArgumentException(
                        $"The delivery trip was confirmed including the delivery '{confirmedDelivery.Id}', however, that delivery was not requested by the manager operating the Drone '{_assignedDrone.Id}'.",
                        nameof(confirmedDelivery.Id));
        }

        private DeliveryTripRequest CreateDeliveryTripRequestWith(ICollection<ScheduledDelivery> scheduledDeliveries) =>
            new() {ResponsibleDroneId = _assignedDrone.Id, ScheduledDeliveries = scheduledDeliveries};

        private ImmutableList<ScheduledDelivery> GetRemainingNotConfirmedDeliveriesYet(DeliveryTripConfirmed deliveryTripConfirmed) =>
            _remainingScheduledDeliveries.Where(x => !deliveryTripConfirmed.ConfirmedDeliveries.Select(x => x.Id).Contains(x.Id)).ToImmutableList();

        private ICollection<ScheduledDelivery> TakeFollowingDeliveries() => 
            _remainingScheduledDeliveries.Take(_assignedDrone.MaximumDeliveryCapacityPerTrip).ToImmutableList();

        private IEnumerable<IFailedDelivery> GetFailedByWrongPositionDeliveriesGiven(IEnumerable<ConfirmedDelivery> confirmedDeliveries) =>
            from confirmedDelivery in confirmedDeliveries
            let scheduledDelivery = FindMatchingScheduledDeliveryFor(confirmedDelivery)
            where scheduledDelivery is not null && !confirmedDelivery.DischargePosition.Equals(scheduledDelivery.FinalTargetPosition)
            select new FailedByWrongPositionDelivery
            {
                ScheduledDelivery = scheduledDelivery,
                ConfirmedDelivery = confirmedDelivery,
                Reason = "The delivery was done at a wrong position"
            };

        private IEnumerable<IFailedDelivery> GetFailedDeliveriesByAlreadyConfirmedGiven(IEnumerable<ConfirmedDelivery> confirmedDeliveries) =>
            from confirmedDelivery in confirmedDeliveries
            let alreadyConfirmedDelivery = FindMatchingAlreadyConfirmedDeliveryFor(confirmedDelivery)
            where alreadyConfirmedDelivery is not null
            select new FailedByAlreadyConfirmedDelivery
            {
                AlreadyConfirmedDelivery = alreadyConfirmedDelivery,
                ConfirmedDelivery = confirmedDelivery,
                Reason = "The delivery had already been confirmed"
            };

        private ScheduledDelivery FindMatchingScheduledDeliveryFor(ConfirmedDelivery confirmedDelivery) => 
            _remainingScheduledDeliveries.FirstOrDefault(x => x.Id.Equals(confirmedDelivery.Id));

        private ConfirmedDelivery FindMatchingAlreadyConfirmedDeliveryFor(ConfirmedDelivery confirmedDelivery) => 
            _confirmedDeliveries.FirstOrDefault(x => x.Id.Equals(confirmedDelivery.Id));
    }
}