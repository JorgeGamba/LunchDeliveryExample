using System;
using System.Collections.Generic;

namespace LunchDelivery
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