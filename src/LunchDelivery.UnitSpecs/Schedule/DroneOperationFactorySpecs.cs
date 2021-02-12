using Doing.BDDExtensions;
using FluentAssertions;
using LunchDelivery.Schedule;
using NUnit.Framework;

namespace LunchDelivery.UnitSpecs.Schedule
{
    [TestFixture]
    public class DroneOperationFactorySpecs : FeatureSpecifications
    {
        private char _source;
        private DroneOperation _result;
        private string _failureReason;
        private bool _worked;

        public override void When() =>
            _worked = DroneOperationFactory.TryCreateFrom(_source, out _result, out _failureReason);

        public class When_the_source_movement_description_is_a_character_A : DroneOperationFactorySpecs
        {
            public override void Given() => 
                _source = 'A';

            [Test]
            public void Should_work() =>
                _worked.Should().BeTrue();

            [Test]
            public void Should_get_some_operation() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_get_a_drone_movement_for_advancing() =>
                _result.Should().Be(DroneOperation.Advance);

            [Test]
            public void Should_not_get_any_failure_reason() =>
                _failureReason.Should().BeNull();
        }

        public class When_the_source_movement_description_is_a_character_a : DroneOperationFactorySpecs
        {
            public override void Given() => 
                _source = 'a';

            [Test]
            public void Should_work() =>
                _worked.Should().BeTrue();

            [Test]
            public void Should_get_some_operation() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_get_a_drone_movement_for_advancing() =>
                _result.Should().Be(DroneOperation.Advance);

            [Test]
            public void Should_not_get_any_failure_reason() =>
                _failureReason.Should().BeNull();
        }

        public class When_the_source_movement_description_is_a_character_I : DroneOperationFactorySpecs
        {
            public override void Given() => 
                _source = 'I';

            [Test]
            public void Should_work() =>
                _worked.Should().BeTrue();

            [Test]
            public void Should_get_some_operation() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_get_a_drone_movement_for_turning_left() =>
                _result.Should().Be(DroneOperation.TurnLeft);

            [Test]
            public void Should_not_get_any_failure_reason() =>
                _failureReason.Should().BeNull();
        }

        public class When_the_source_movement_description_is_a_character_i : DroneOperationFactorySpecs
        {
            public override void Given() => 
                _source = 'i';

            [Test]
            public void Should_work() =>
                _worked.Should().BeTrue();

            [Test]
            public void Should_get_some_operation() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_get_a_drone_movement_for_turning_left() =>
                _result.Should().Be(DroneOperation.TurnLeft);

            [Test]
            public void Should_not_get_any_failure_reason() =>
                _failureReason.Should().BeNull();
        }

        public class When_the_source_movement_description_is_a_character_D : DroneOperationFactorySpecs
        {
            public override void Given() => 
                _source = 'D';

            [Test]
            public void Should_work() =>
                _worked.Should().BeTrue();

            [Test]
            public void Should_get_some_operation() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_get_a_drone_movement_for_turning_right() =>
                _result.Should().Be(DroneOperation.TurnRight);

            [Test]
            public void Should_not_get_any_failure_reason() =>
                _failureReason.Should().BeNull();
        }

        public class When_the_source_movement_description_is_a_character_d : DroneOperationFactorySpecs
        {
            public override void Given() => 
                _source = 'd';

            [Test]
            public void Should_work() =>
                _worked.Should().BeTrue();

            [Test]
            public void Should_get_some_operation() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_get_a_drone_movement_for_turning_right() =>
                _result.Should().Be(DroneOperation.TurnRight);

            [Test]
            public void Should_not_get_any_failure_reason() =>
                _failureReason.Should().BeNull();
        }

        public class When_the_source_movement_description_is_an_unknown_character : DroneOperationFactorySpecs
        {
            public override void Given() => 
                _source = '^';

            [Test]
            public void Should_not_work() =>
                _worked.Should().BeFalse();

            [Test]
            public void Should_not_get_anything() =>
                _result.Should().Be(0);

            [Test]
            public void Should_get_a_failure_reason() =>
                _failureReason.Should().NotBeNull();

            [Test]
            public void Should_get_a_failure_reason_telling_the_detail() =>
                _failureReason.Should().Be("The character '^' is unknown.");
        }
    }
}