using System;
using System.Collections.Generic;

namespace LunchDelivery.Schedule
{
    public record ScheduledDelivery
    {
        public Guid Id;
        public ICollection<DroneMovement> DroneMovements;
        public Position TargetPosition;
    }
}