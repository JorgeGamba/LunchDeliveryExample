using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LunchDelivery.Schedule
{
    public record DroneMovement
    {
        public ICollection<DroneOperation> DroneOperations;
        public Position TargetPosition;

        private DroneMovement(ICollection<DroneOperation> droneOperations, Position targetPosition)
        {
            DroneOperations = droneOperations;
            TargetPosition = targetPosition;
        }

        public static bool TryCreateFrom(MovementDescription source, Position startingPosition, out DroneMovement result, out string failureReason)
        {
            failureReason = null;
            var droneOperations = GetValidatedDroneOperations(source, ref failureReason);
            ValidateThereIsAtLeastAnAdvanceIn(source, droneOperations, ref failureReason);
            if (!string.IsNullOrEmpty(failureReason))
            {
                result = null;
                return false;
            }
            var targetPosition = startingPosition.GetToNewPositionFollowing(droneOperations);
            
            result = new DroneMovement(droneOperations, targetPosition);
            failureReason = null;
            
            return true;
        }


        private static ICollection<DroneOperation> GetValidatedDroneOperations(MovementDescription source, ref string failureReason)
        {
            var (droneOperations, failureReasonsByCharacter) = FindDroneOperationsIn(source);
            if (!string.IsNullOrEmpty(failureReasonsByCharacter))
                failureReason = ExtendFailureReasonWith($"The movement description '{source.Value}' contain unknown characters.", failureReasonsByCharacter);
            
            return droneOperations;
        }

        private static (ICollection<DroneOperation> DroneOperations, string FailureReasons) FindDroneOperationsIn(MovementDescription characters)
        {
            var builder = ImmutableList.CreateBuilder<DroneOperation>();
            string allFailureReasons = null;
            foreach (var character in characters.Value)
            {
                if (DroneOperationFactory.TryCreateFrom(character, out var currentDroneOperation, out var failureReason))
                    builder.Add(currentDroneOperation);
                else
                    allFailureReasons = ExtendFailureReasonWith(allFailureReasons, failureReason);
            }

            return (builder.ToImmutable(), allFailureReasons);
        }

        private static void ValidateThereIsAtLeastAnAdvanceIn(MovementDescription source, ICollection<DroneOperation> droneOperations, ref string failureReason)
        {
            if (!droneOperations.Contains(DroneOperation.Advance))
                failureReason = ExtendFailureReasonWith(failureReason, $"The movement description '{source.Value}' has no one advance move.");
        }

        private static string ExtendFailureReasonWith(string formerFailureReason, string additionalFailureReason) =>
            formerFailureReason is null
                ? additionalFailureReason
                : formerFailureReason + Environment.NewLine + additionalFailureReason;
    }
}