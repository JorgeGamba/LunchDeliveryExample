using System.Collections.Generic;

namespace LunchDelivery
{
    public record DeliveryTripRequest
    {
        public ICollection<ScheduledDelivery> ScheduledDeliveries;
    }
}