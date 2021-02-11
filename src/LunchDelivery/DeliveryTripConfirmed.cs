using System;
using System.Collections.Generic;

namespace LunchDelivery
{
    public record DeliveryTripConfirmed
    {
        public Guid PerformerDroneId;
        public ICollection<ConfirmedDelivery> ConfirmedDeliveries;
    }
}