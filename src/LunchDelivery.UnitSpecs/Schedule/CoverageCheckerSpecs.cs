using Doing.BDDExtensions;
using FluentAssertions;
using LunchDelivery.Schedule;
using NUnit.Framework;

namespace LunchDelivery.UnitSpecs.Schedule
{
    [TestFixture]
    public class CoverageCheckerSpecs : FeatureSpecifications
    {
        private Position _targetPosition;
        private int _allowedBlocksAround;
        private CoverageChecker _coverageChecker;
        private bool _result;

        public override void Given()
        {
            var districtArea = new DistrictArea
            {
                BlocksWide = 3,
                BlocksHigh = 2
            };
            _allowedBlocksAround = 2;
            _coverageChecker = new CoverageChecker(districtArea, _allowedBlocksAround);
        }

        public override void When() => 
            _result = _coverageChecker.CanDeliverAt(_targetPosition);

        public class When_the_checked_position_is_the_initial_one : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = Position.CreateStartingPosition();

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_below_the_top_limit_of_the_district : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(DroneOperation.Advance);

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_just_at_the_top_limit_of_the_district : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.Advance,
                    DroneOperation.Advance
                    );

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_between_the_top_limit_of_the_district_and_the_final_top_perimeter : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                    );

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_just_at_the_final_top_perimeter : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                );

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_above_the_final_top_perimeter : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                );

            [Test]
            public void Should_not_allow_to_deliver_there() =>
                _result.Should().BeFalse();
        }

        public class When_the_checked_position_is_just_above_the_final_bottom_perimeter : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.TurnRight,
                    DroneOperation.TurnRight,
                    DroneOperation.Advance
                    );

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_just_at_the_final_bottom_perimeter : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.TurnRight,
                    DroneOperation.TurnRight,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                );

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_below_the_final_bottom_perimeter : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.TurnRight,
                    DroneOperation.TurnRight,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                );

            [Test]
            public void Should_not_allow_to_deliver_there() =>
                _result.Should().BeFalse();
        }

        public class When_the_checked_position_is_before_the_right_limit_of_the_district : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.TurnRight,
                    DroneOperation.Advance
                    );

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_just_at_the_right_limit_of_the_district : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.TurnRight,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                    );

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_between_the_right_limit_of_the_district_and_the_final_right_perimeter : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.TurnRight,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                    );

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_just_at_the_final_right_perimeter : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.TurnRight,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                );

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_after_the_final_right_perimeter : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.TurnRight,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                );

            [Test]
            public void Should_not_allow_to_deliver_there() =>
                _result.Should().BeFalse();
        }

        public class When_the_checked_position_is_just_after_the_final_left_perimeter : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.TurnLeft,
                    DroneOperation.Advance
                    );

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_just_at_the_final_left_perimeter : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.TurnLeft,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                );

            [Test]
            public void Should_allow_to_deliver_there() =>
                _result.Should().BeTrue();
        }

        public class When_the_checked_position_is_before_the_final_left_perimeter : CoverageCheckerSpecs
        {
            public override void Given() => 
                _targetPosition = CreatePositionFollowing(
                    DroneOperation.TurnLeft,
                    DroneOperation.Advance,
                    DroneOperation.Advance,
                    DroneOperation.Advance
                );

            [Test]
            public void Should_not_allow_to_deliver_there() =>
                _result.Should().BeFalse();
        }

    
        private static Position CreatePositionFollowing(params DroneOperation[] droneOperations)
        {
            return Position.CreateStartingPosition()
                .GetToNewPositionFollowing(droneOperations);
        }
    }
}