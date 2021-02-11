using System.Collections.Generic;
using System.Linq;

namespace LunchDelivery.Schedule
{
    public abstract record Position
    {
        public int CoordinateX;
        public int CoordinateY;
        public CardinalPoint CardinalPoint;

        protected Position(int coordinateX, int coordinateY, CardinalPoint cardinalPoint)
        {
            CoordinateX = coordinateX;
            CoordinateY = coordinateY;
            CardinalPoint = cardinalPoint;
        }

        public static Position CreateStartingPosition() =>
            new PositionPointingNorth();

        public Position GetToNewPositionFollowing(ICollection<DroneOperation> droneOperations) => 
            droneOperations.Aggregate(this, (current, droneOperation) => current.GetNewPositionFollowing(droneOperation));

        private protected abstract Position GetNewPositionFollowing(DroneOperation droneOperation);


        private record PositionPointingNorth : Position
        {
            internal PositionPointingNorth() 
                : base(0, 0, CardinalPoint.North) { }

            internal PositionPointingNorth(Position previousPosition) 
                : base(previousPosition.CoordinateX, previousPosition.CoordinateY, CardinalPoint.North) { }

            private protected override Position GetNewPositionFollowing(DroneOperation droneOperation) =>
                droneOperation switch
                {
                    DroneOperation.Advance => this with {CoordinateY = CoordinateY + 1},
                    DroneOperation.TurnLeft => new PositionPointingWest(this),
                    DroneOperation.TurnRight => new PositionPointingEast(this)
                };
        }

        private record PositionPointingWest : Position
        {
            internal PositionPointingWest(Position previousPosition) 
                : base(previousPosition.CoordinateX, previousPosition.CoordinateY, CardinalPoint.West) { }

            private protected override Position GetNewPositionFollowing(DroneOperation droneOperation) =>
                droneOperation switch
                {
                    DroneOperation.Advance => this with {CoordinateX = CoordinateX - 1},
                    DroneOperation.TurnLeft => new PositionPointingSouth(this),
                    DroneOperation.TurnRight => new PositionPointingNorth(this)
                };
        }

        private record PositionPointingEast : Position
        {
            internal PositionPointingEast(Position previousPosition) 
                : base(previousPosition.CoordinateX, previousPosition.CoordinateY, CardinalPoint.East) { }

            private protected override Position GetNewPositionFollowing(DroneOperation droneOperation) =>
                droneOperation switch
                {
                    DroneOperation.Advance => this with {CoordinateX = CoordinateX + 1},
                    DroneOperation.TurnLeft => new PositionPointingNorth(this),
                    DroneOperation.TurnRight => new PositionPointingSouth(this)
                };
        }

        private record PositionPointingSouth : Position
        {
            internal PositionPointingSouth(Position previousPosition) 
                : base(previousPosition.CoordinateX, previousPosition.CoordinateY, CardinalPoint.South) { }

            private protected override Position GetNewPositionFollowing(DroneOperation droneOperation) =>
                droneOperation switch
                {
                    DroneOperation.Advance => this with {CoordinateY = CoordinateY - 1},
                    DroneOperation.TurnLeft => new PositionPointingEast(this),
                    DroneOperation.TurnRight => new PositionPointingWest(this)
                };
        }
    }
}