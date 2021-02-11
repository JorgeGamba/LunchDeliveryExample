using System;
using System.Collections.Generic;
using LunchDelivery.Schedule;

namespace LunchDelivery.Delivery
{
    public record DeliveryTripRequest
    {
        public Guid ResponsibleDroneId;
        public ICollection<ScheduledDelivery> ScheduledDeliveries;
    }
}