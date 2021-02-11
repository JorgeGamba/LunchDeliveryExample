using System;
using System.Collections.Generic;

namespace LunchDelivery
{
    public record DeliveryTripRequest
    {
        public Guid ResponsibleDroneId;
        public ICollection<ScheduledDelivery> ScheduledDeliveries;
    }
}