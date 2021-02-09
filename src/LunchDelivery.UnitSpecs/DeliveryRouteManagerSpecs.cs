using System;
using System.Collections.Generic;
using Doing.BDDExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace LunchDelivery.UnitSpecs
{
    [TestFixture]
    public class DeliveryRouteManagerSpecs : FeatureSpecifications
    {
        private IList<ScheduledDelivery> _scheduledDeliveries;
        private Drone _assignedDrone;
        private DeliveryTripRequest _result;

        public override void Given() => 
            _assignedDrone = new Drone {MaximumDeliveryCapacityPerTrip = 3};

        public override void When() => 
            _result = new DeliveryRouteManager(_scheduledDeliveries, _assignedDrone).StartOperation();

        public class When_the_route_has_only_1_deliver : DeliveryRouteManagerSpecs
        {
            public override void Given() => 
                _scheduledDeliveries = new[] {CreateSomeScheduledDelivery()};

            [Test]
            public void Should_request_a_delivery_trip() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_request_a_delivery_trip_with_something_to_deliver() =>
                _result.ScheduledDeliveries.Should().NotBeEmpty();

            [Test]
            public void Should_request_a_delivery_trip_with_only_1_deliver() =>
                _result.ScheduledDeliveries.Count.Should().Be(1);

            [Test]
            public void Should_request_a_delivery_trip_with_the_same_original_scheduled_deliver() =>
                _result.ScheduledDeliveries.Should().Contain(_scheduledDeliveries);
        }

        public class When_the_number_of_delivers_is_exactly_the_same_as_the_maximum_capacity_of_the_drone : DeliveryRouteManagerSpecs
        {
            public override void Given()
            {
                _scheduledDeliveries = new List<ScheduledDelivery>();
                for (var i = 0; i < _assignedDrone.MaximumDeliveryCapacityPerTrip; i++)
                    _scheduledDeliveries.Add(CreateSomeScheduledDelivery());
            }

            [Test]
            public void Should_request_a_delivery_trip() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_request_a_delivery_trip_with_something_to_deliver() =>
                _result.ScheduledDeliveries.Should().NotBeEmpty();

            [Test]
            public void Should_request_a_deliver_with_only_the_number_of_deliveries_allowed_by_the_drone() =>
                _result.ScheduledDeliveries.Count.Should().Be(_assignedDrone.MaximumDeliveryCapacityPerTrip);

            [Test]
            public void Should_request_a_delivery_trip_with_the_same_original_scheduled_deliveries() =>
                _result.ScheduledDeliveries.Should().Contain(_scheduledDeliveries);
        }

        public class When_the_number_of_delivers_exceeds_the_maximum_capacity_of_the_drone : DeliveryRouteManagerSpecs
        {
            public override void Given()
            {
                _scheduledDeliveries = new List<ScheduledDelivery>();
                for (var i = 0; i < _assignedDrone.MaximumDeliveryCapacityPerTrip + 1; i++)
                    _scheduledDeliveries.Add(CreateSomeScheduledDelivery());
            }

            [Test]
            public void Should_request_a_delivery_trip() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_request_a_delivery_trip_with_something_to_deliver() =>
                _result.ScheduledDeliveries.Should().NotBeEmpty();

            [Test]
            public void Should_request_a_deliver_with_only_the_number_of_deliveries_allowed_by_the_drone() =>
                _result.ScheduledDeliveries.Count.Should().Be(_assignedDrone.MaximumDeliveryCapacityPerTrip);

            [Test]
            public void Should_request_a_delivery_trip_with_the_first_original_scheduled_deliveries() =>
                _result.ScheduledDeliveries.Should().Contain(new[] {_scheduledDeliveries[0], _scheduledDeliveries[1], _scheduledDeliveries[2]});

            [Test]
            public void Should_request_a_delivery_trip_without_the_original_scheduled_deliveries_exceeding_the_capacity() =>
                _result.ScheduledDeliveries.Should().NotContain(new[] {_scheduledDeliveries[3]});
        }


        private static ScheduledDelivery CreateSomeScheduledDelivery()
        {
            return new() {Id = Guid.NewGuid()};
        }
    }
}