using System.Collections.Generic;

namespace LunchDelivery
{
    public record DeliveryRouteResult : IResult
    {
        public ICollection<ConfirmedDelivery> ConfirmedDeliveries;
    }
}