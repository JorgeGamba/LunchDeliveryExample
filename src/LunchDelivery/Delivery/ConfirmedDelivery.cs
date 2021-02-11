using System;

namespace LunchDelivery.Delivery
{
    public record ConfirmedDelivery
    {
        public Guid Id;
        public Position DischargePosition;
    }
}