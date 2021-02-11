using System;

namespace LunchDelivery.Schedule
{
    public record ScheduledDelivery
    {
        public Guid Id;
        public Position TargetPosition;
    }
}