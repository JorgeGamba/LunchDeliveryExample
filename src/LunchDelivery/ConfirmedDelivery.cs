using System;

namespace LunchDelivery
{
    public record ConfirmedDelivery
    {
        public Guid Id;
        public Position DischargePosition;
    }
}