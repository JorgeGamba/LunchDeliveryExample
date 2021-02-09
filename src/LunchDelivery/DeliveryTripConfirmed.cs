using System.Collections.Generic;

namespace LunchDelivery
{
    public record DeliveryTripConfirmed
    {
        public ICollection<ConfirmedDelivery> ConfirmedDeliveries;
    }
}