using System;
using System.Collections.Generic;
using LunchDelivery.Schedule;

namespace LunchDelivery.Delivery
{
    public record DeliveryRouteReport : ICheckResult
    {
        public Guid PerformerDroneId;
        public ICollection<ScheduledDelivery> ScheduledDeliveries;
        public ICollection<ConfirmedDelivery> ConfirmedDeliveries;
        public ICollection<IFailedDelivery> FailedDeliveries;
        public ICollection<IFailedDelivery> LastFailedDeliveries;
    }
}