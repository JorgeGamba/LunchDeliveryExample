using System.Linq;
using Doing.BDDExtensions;
using FluentAssertions;
using LunchDelivery.Schedule;
using NUnit.Framework;
using static LunchDelivery.UnitSpecs.ObjectMother;

namespace LunchDelivery.UnitSpecs.Schedule
{
    [TestFixture]
    public class DroneMovementSpecs : FeatureSpecifications
    {
        private MovementDescription _source;
        private Position _startingPosition;
        private DroneMovement _result;
        private string _failureReason;
        private bool _worked;

        public override void Given() =>
            _startingPosition = Position.CreateStartingPosition();

        public override void When() =>
            _worked = DroneMovement.TryCreateFrom(_source, _startingPosition, out _result, out _failureReason);

        public class When_the_source_have_only_a_character_for_advancing : DroneMovementSpecs
        {
            public override void Given() => 
                _source = CreateMovementDescriptionFrom("A");

            [Test]
            public void Should_work() =>
                _worked.Should().BeTrue();

            [Test]
            public void Should_get_something() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_get_a_drone_operation() =>
                _result.DroneOperations.Count.Should().Be(1);

            [Test]
            public void Should_get_a_drone_operation_for_advancing() =>
                _result.DroneOperations.First().Should().Be(DroneOperation.Advance);

            [Test]
            public void Should_get_a_target_position() =>
                _result.TargetPosition.Should().NotBeNull();

            [Test]
            public void Should_get_a_target_position_different_to_the_starting_one() =>
                _result.TargetPosition.Should().NotBe(_startingPosition);

            [Test]
            public void Should_get_a_target_position_with_the_expected_coordinate_x() =>
                _result.TargetPosition.CoordinateX.Should().Be(0);

            [Test]
            public void Should_get_a_target_position_with_the_expected_coordinate_y() =>
                _result.TargetPosition.CoordinateY.Should().Be(1);

            [Test]
            public void Should_get_a_target_position_with_the_expected_cardinal_point() =>
                _result.TargetPosition.CardinalPoint.Should().Be(CardinalPoint.North);

            [Test]
            public void Should_not_get_any_failure_reason() =>
                _failureReason.Should().BeNull();
        }

        public class When_the_source_have_only_a_character_for_turning_left : DroneMovementSpecs
        {
            public override void Given() => 
                _source = CreateMovementDescriptionFrom("i");

            [Test]
            public void Should_not_work() =>
                _worked.Should().BeFalse();

            [Test]
            public void Should_not_get_anything() =>
                _result.Should().BeNull();

            [Test]
            public void Should_get_a_failure_reason() =>
                _failureReason.Should().NotBeNull();

            [Test]
            public void Should_get_a_failure_reason_telling_the_detail() =>
                _failureReason.Should().Be($"The movement description '{_source.Value}' has no one advance move.");
        }

        public class When_the_source_have_only_a_character_for_turning_right : DroneMovementSpecs
        {
            public override void Given() => 
                _source = CreateMovementDescriptionFrom("D");

            [Test]
            public void Should_not_work() =>
                _worked.Should().BeFalse();

            [Test]
            public void Should_not_get_anything() =>
                _result.Should().BeNull();

            [Test]
            public void Should_get_a_failure_reason() =>
                _failureReason.Should().NotBeNull();

            [Test]
            public void Should_get_a_failure_reason_telling_the_detail() =>
                _failureReason.Should().Be($"The movement description '{_source.Value}' has no one advance move.");
        }

        public class When_the_source_have_multiple_characters : DroneMovementSpecs
        {
            public override void Given() => 
                _source = CreateMovementDescriptionFrom("aIdAdia");

            [Test]
            public void Should_work() =>
                _worked.Should().BeTrue();

            [Test]
            public void Should_get_something() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_get_a_drone_operation_for_each_character() =>
                _result.DroneOperations.Count.Should().Be(7);

            [Test]
            public void Should_get_the_right_drone_operations_for_all_characters() =>
                _result.DroneOperations.Should().Contain(new[] {DroneOperation.Advance, DroneOperation.TurnLeft, DroneOperation.TurnRight});

            [Test]
            public void Should_get_a_target_position() =>
                _result.TargetPosition.Should().NotBeNull();

            [Test]
            public void Should_get_a_target_position_different_to_the_starting_one() =>
                _result.TargetPosition.Should().NotBe(_startingPosition);

            [Test]
            public void Should_get_a_target_position_with_the_expected_coordinate_x() =>
                _result.TargetPosition.CoordinateX.Should().Be(0);

            [Test]
            public void Should_get_a_target_position_with_the_expected_coordinate_y() =>
                _result.TargetPosition.CoordinateY.Should().Be(3);

            [Test]
            public void Should_get_a_target_position_with_the_expected_cardinal_point() =>
                _result.TargetPosition.CardinalPoint.Should().Be(CardinalPoint.North);

            [Test]
            public void Should_not_get_any_failure_reason() =>
                _failureReason.Should().BeNull();
        }

        public class When_the_source_have_multiple_characters_but_no_one_is_for_advancing : DroneMovementSpecs
        {
            public override void Given() => 
                _source = CreateMovementDescriptionFrom("DidI");

            [Test]
            public void Should_not_work() =>
                _worked.Should().BeFalse();

            [Test]
            public void Should_not_get_anything() =>
                _result.Should().BeNull();

            [Test]
            public void Should_get_a_failure_reason() =>
                _failureReason.Should().NotBeNull();

            [Test]
            public void Should_get_a_failure_reason_telling_the_detail() =>
                _failureReason.Should().Be($"The movement description '{_source.Value}' has no one advance move.");
        }

        public class When_the_source_have_an_unknown_character : DroneMovementSpecs
        {
            public override void Given() =>
                _source = CreateMovementDescriptionFrom("A*i");

            [Test]
            public void Should_not_work() =>
                _worked.Should().BeFalse();

            [Test]
            public void Should_not_get_anything() =>
                _result.Should().BeNull();

            [Test]
            public void Should_get_a_failure_reason() =>
                _failureReason.Should().NotBeNull();

            [Test]
            public void Should_get_a_failure_reason_telling_the_detail() =>
                _failureReason.Should().StartWith($"The movement description '{_source.Value}' contain unknown characters.");

            [Test]
            public void Should_get_a_failure_reason_telling_the_detailed_reason_from_the_specific_operation() =>
                _failureReason.Should().Contain("The character '*' is unknown.");
        }
    }
}