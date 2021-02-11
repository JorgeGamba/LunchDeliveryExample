using System;

namespace LunchDelivery
{
    public record ScheduledDelivery
    {
        public Guid Id;
        public Position TargetPosition;
    }
}