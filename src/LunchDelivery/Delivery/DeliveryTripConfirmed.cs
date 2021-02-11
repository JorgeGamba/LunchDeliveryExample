using System;
using System.Collections.Generic;

namespace LunchDelivery.Delivery
{
    public record DeliveryTripConfirmed
    {
        public Guid PerformerDroneId;
        public ICollection<ConfirmedDelivery> ConfirmedDeliveries;
    }
}