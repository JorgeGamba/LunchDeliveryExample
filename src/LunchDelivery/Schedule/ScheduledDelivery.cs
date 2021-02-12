using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LunchDelivery.Schedule
{
    public record ScheduledDelivery
    {
        public Guid Id;
        public ICollection<DroneMovement> DroneMovements;
        public Position FinalTargetPosition;

        private ScheduledDelivery(Guid id, ICollection<DroneMovement> droneMovements, Position finalTargetPosition)
        {
            Id = id;
            DroneMovements = droneMovements;
            FinalTargetPosition = finalTargetPosition;
        }

        public static bool TryCreateFrom(ICollection<MovementDescription> source, CoverageChecker coverageChecker, out ScheduledDelivery result, out string failureReason)
        {
            var (droneMovements, failureReasonsByMovement) = FindDroneMovementsIn(source);
            var allFailureReasonsByNoCoverage = ValidateAllMovementsAreInTheCoverageArea(droneMovements, coverageChecker);
            failureReason = Helpers.MergeFailureReasons(failureReasonsByMovement, allFailureReasonsByNoCoverage);
            if (!string.IsNullOrEmpty(failureReason))
            {
                result = null;
                return false;
            }
            
            result = new ScheduledDelivery(Guid.NewGuid(), droneMovements, droneMovements.Last().TargetPosition);
            return true;
        }


        private static (ICollection<DroneMovement> DroneMovements, string FailureReasons) FindDroneMovementsIn(ICollection<MovementDescription> movementDescriptions)
        {
            var builder = ImmutableList.CreateBuilder<DroneMovement>();
            string allFailureReasons = null;
            var finalTargetPosition = Position.CreateStartingPosition();
            foreach (var movementDescription in movementDescriptions)
            {
                if (DroneMovement.TryCreateFrom(movementDescription, finalTargetPosition, out var currentDroneMovement, out var failureReason))
                {
                    builder.Add(currentDroneMovement);
                    finalTargetPosition = currentDroneMovement.TargetPosition;
                }
                else
                {
                    allFailureReasons = Helpers.MergeFailureReasons(allFailureReasons, failureReason);
                }
            }

            return (builder.ToImmutable(), allFailureReasons);
        }

        private static string ValidateAllMovementsAreInTheCoverageArea(IEnumerable<DroneMovement> droneMovements, CoverageChecker coverageChecker)
        {
            string allFailureReasons = null;
            foreach (var droneMovement in droneMovements)
            {
                if (!coverageChecker.CanDeliverAt(droneMovement.TargetPosition))
                    allFailureReasons = Helpers.MergeFailureReasons(
                        allFailureReasons, 
                        $"The target position for the movement description '{droneMovement.OriginalMovementDescription.Value}' is out of the coverage area.");
            }

            return allFailureReasons;
        }
    }
}