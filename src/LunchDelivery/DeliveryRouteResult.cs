using System;
using System.Collections.Generic;

namespace LunchDelivery
{
    public record DeliveryRouteResult : IResult
    {
        public Guid PerformerDroneId;
        public ICollection<ConfirmedDelivery> ConfirmedDeliveries;
    }
}