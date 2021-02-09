using System.Collections.Generic;

namespace LunchDelivery
{
    public record DeliveryTripRequest : IResult
    {
        public ICollection<ScheduledDelivery> ScheduledDeliveries;
    }
}