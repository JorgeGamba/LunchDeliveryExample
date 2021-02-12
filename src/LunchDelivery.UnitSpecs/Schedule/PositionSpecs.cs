using System.Collections.Generic;
using Doing.BDDExtensions;
using FluentAssertions;
using LunchDelivery.Schedule;
using NUnit.Framework;

namespace LunchDelivery.UnitSpecs.Schedule
{
    [TestFixture]
    public class PositionSpecs : FeatureSpecifications
    {
        private Position _originalPosition;
        private ICollection<DroneOperation> _droneOperations;
        private Position _result;

        public override void Given() =>
            _originalPosition = Position.CreateStartingPosition();

        public override void When() => 
            _result = _originalPosition.GetToNewPositionFollowing(_droneOperations);

        public class When_It_is_just_a_simple_advance : PositionSpecs
        {
            public override void Given() =>
                _droneOperations = new[] {DroneOperation.Advance};
            
            [Test]
            public void Should_get_a_target_position() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_stay_in_the_same_coordinate_x_as_0() =>
                _result.CoordinateX.Should().Be(0);

            [Test]
            public void Should_move_forward_a_position_in_the_coordinate_y() =>
                _result.CoordinateY.Should().Be(1);

            [Test]
            public void Should_point_North() =>
                _result.CardinalPoint.Should().Be(CardinalPoint.North);
        }

        public class When_It_is_just_a_turn_left : PositionSpecs
        {
            public override void Given() =>
                _droneOperations = new[] {DroneOperation.TurnLeft};
            
            [Test]
            public void Should_get_a_target_position() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_stay_in_the_same_coordinate_x_as_0() =>
                _result.CoordinateX.Should().Be(0);

            [Test]
            public void Should_stay_in_the_same_coordinate_y_0() =>
                _result.CoordinateY.Should().Be(0);

            [Test]
            public void Should_point_West() =>
                _result.CardinalPoint.Should().Be(CardinalPoint.West);
        }

        public class When_It_is_just_a_turn_right : PositionSpecs
        {
            public override void Given() =>
                _droneOperations = new[] {DroneOperation.TurnRight};
            
            [Test]
            public void Should_get_a_target_position() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_stay_in_the_same_coordinate_x_as_0() =>
                _result.CoordinateX.Should().Be(0);

            [Test]
            public void Should_stay_in_the_same_coordinate_y_0() =>
                _result.CoordinateY.Should().Be(0);

            [Test]
            public void Should_point_East() =>
                _result.CardinalPoint.Should().Be(CardinalPoint.East);
        }

        public class When_being_pointing_north_have_only_multiple_advance_operations : PositionSpecs
        {
            public override void Given() =>
                _droneOperations = new[]
                {
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                };
            
            [Test]
            public void Should_get_a_target_position() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_stay_in_the_same_coordinate_x_as_0() =>
                _result.CoordinateX.Should().Be(0);

            [Test]
            public void Should_move_north_the_same_number_of_positions_as_provided_operations_in_the_coordinate_y() =>
                _result.CoordinateY.Should().Be(3);

            [Test]
            public void Should_point_North() =>
                _result.CardinalPoint.Should().Be(CardinalPoint.North);
        }

        public class When_after_turning_right_have_only_multiple_advance_operations : PositionSpecs
        {
            public override void Given() =>
                _droneOperations = new[]
                {
                    DroneOperation.TurnRight,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                };
            
            [Test]
            public void Should_get_a_target_position() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_move_west_the_same_number_of_positions_as_provided_operations_minus_1_in_the_coordinate_x() =>
                _result.CoordinateX.Should().Be(3);

            [Test]
            public void Should_stay_in_the_same_coordinate_y_as_0() =>
                _result.CoordinateY.Should().Be(0);

            [Test]
            public void Should_point_East() =>
                _result.CardinalPoint.Should().Be(CardinalPoint.East);
        }

        public class When_after_turning_left_have_only_multiple_advance_operations : PositionSpecs
        {
            public override void Given() =>
                _droneOperations = new[]
                {
                    DroneOperation.TurnLeft,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                };
            
            [Test]
            public void Should_get_a_target_position() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_move_west_the_same_number_of_positions_as_provided_operations_minus_1_in_the_coordinate_x() =>
                _result.CoordinateX.Should().Be(-3);

            [Test]
            public void Should_stay_in_the_same_coordinate_y_as_0() =>
                _result.CoordinateY.Should().Be(0);

            [Test]
            public void Should_point_West() =>
                _result.CardinalPoint.Should().Be(CardinalPoint.West);
        }

        public class When_after_turning_twice_right_have_only_multiple_advance_operations : PositionSpecs
        {
            public override void Given() =>
                _droneOperations = new[]
                {
                    DroneOperation.TurnRight,
                    DroneOperation.TurnRight,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                };
            
            [Test]
            public void Should_get_a_target_position() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_stay_in_the_same_coordinate_x_as_0() =>
                _result.CoordinateX.Should().Be(0);

            [Test]
            public void Should_move_south_the_same_number_of_positions_as_provided_operations_minus_2_in_the_coordinate_y() =>
                _result.CoordinateY.Should().Be(-3);

            [Test]
            public void Should_point_East() =>
                _result.CardinalPoint.Should().Be(CardinalPoint.South);
        }

        public class When_after_turning_twice_left_have_only_multiple_advance_operations : PositionSpecs
        {
            public override void Given() =>
                _droneOperations = new[]
                {
                    DroneOperation.TurnRight,
                    DroneOperation.TurnRight,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                };
            
            [Test]
            public void Should_get_a_target_position() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_stay_in_the_same_coordinate_x_as_0() =>
                _result.CoordinateX.Should().Be(0);

            [Test]
            public void Should_move_south_the_same_number_of_positions_as_provided_operations_minus_2_in_the_coordinate_y() =>
                _result.CoordinateY.Should().Be(-3);

            [Test]
            public void Should_point_East() =>
                _result.CardinalPoint.Should().Be(CardinalPoint.South);
        }

        public class When_after_turning_right_advance_turn_left_and_advance_again : PositionSpecs
        {
            public override void Given() =>
                _droneOperations = new[]
                {
                    DroneOperation.TurnRight,
                    DroneOperation.Advance,
                    DroneOperation.TurnLeft,
                    DroneOperation.Advance
                };
            
            [Test]
            public void Should_get_a_target_position() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_move_west_a_position_in_the_coordinate_x() =>
                _result.CoordinateX.Should().Be(1);

            [Test]
            public void Should_move_north_a_position_in_the_coordinate_y() =>
                _result.CoordinateY.Should().Be(1);

            [Test]
            public void Should_point_North() =>
                _result.CardinalPoint.Should().Be(CardinalPoint.North);
        }

        public class When_after_turning_right_twice_advance_turn_left_and_advance_again : PositionSpecs
        {
            public override void Given() =>
                _droneOperations = new[]
                {
                    DroneOperation.TurnRight,
                    DroneOperation.TurnRight,
                    DroneOperation.Advance,
                    DroneOperation.TurnLeft,
                    DroneOperation.Advance
                };
            
            [Test]
            public void Should_get_a_target_position() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_move_west_a_position_in_the_coordinate_x() =>
                _result.CoordinateX.Should().Be(1);

            [Test]
            public void Should_move_south_a_position_in_the_coordinate_y() =>
                _result.CoordinateY.Should().Be(-1);

            [Test]
            public void Should_point_East() =>
                _result.CardinalPoint.Should().Be(CardinalPoint.East);
        }

        public class When_after_turning_left_advance_turn_right_and_advance_again : PositionSpecs
        {
            public override void Given() =>
                _droneOperations = new[]
                {
                    DroneOperation.TurnLeft,
                    DroneOperation.Advance,
                    DroneOperation.TurnRight,
                    DroneOperation.Advance
                };
            
            [Test]
            public void Should_get_a_target_position() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_move_west_a_position_in_the_coordinate_x() =>
                _result.CoordinateX.Should().Be(-1);

            [Test]
            public void Should_move_north_a_position_in_the_coordinate_y() =>
                _result.CoordinateY.Should().Be(1);

            [Test]
            public void Should_point_North() =>
                _result.CardinalPoint.Should().Be(CardinalPoint.North);
        }

        public class When_after_turning_left_twice_advance_turn_right_and_advance_again : PositionSpecs
        {
            public override void Given() =>
                _droneOperations = new[]
                {
                    DroneOperation.TurnLeft,
                    DroneOperation.TurnLeft,
                    DroneOperation.Advance,
                    DroneOperation.TurnRight,
                    DroneOperation.Advance
                };
            
            [Test]
            public void Should_get_a_target_position() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_move_west_a_position_in_the_coordinate_x() =>
                _result.CoordinateX.Should().Be(-1);

            [Test]
            public void Should_move_south_a_position_in_the_coordinate_y() =>
                _result.CoordinateY.Should().Be(-1);

            [Test]
            public void Should_point_West() =>
                _result.CardinalPoint.Should().Be(CardinalPoint.West);
        }
    }
}