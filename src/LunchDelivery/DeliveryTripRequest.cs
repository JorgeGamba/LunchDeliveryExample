using System;
using System.Collections.Generic;

namespace LunchDelivery
{
    public record DeliveryTripRequest : IResult
    {
        public Guid ResponsibleDroneId;
        public ICollection<ScheduledDelivery> ScheduledDeliveries;
    }
}