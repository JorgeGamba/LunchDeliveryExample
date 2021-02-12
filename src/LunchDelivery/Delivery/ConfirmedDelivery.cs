using System;
using LunchDelivery.Schedule;

namespace LunchDelivery.Delivery
{
    public record ConfirmedDelivery
    {
        public Guid Id;
        public Position DischargePosition;
    }
}