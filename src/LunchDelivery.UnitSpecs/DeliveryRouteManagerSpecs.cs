﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        private IResult _result;
        private Exception _exception;

        public override void Given() => 
            _assignedDrone = new Drone {MaximumDeliveryCapacityPerTrip = 3, Id = Guid.NewGuid()};

        public class When_creating_the_manager_without_any_Scheduled_Delivery : DeliveryRouteManagerSpecs
        {
            public override void Given() =>
                _scheduledDeliveries = Enumerable.Empty<ScheduledDelivery>().ToList();

            public override void When() =>
                _exception = Catch.Exception(() =>
                    new DeliveryRouteManager(_scheduledDeliveries, _assignedDrone)
                );

            [Test]
            public void Should_fail() =>
                _exception.Should().NotBeNull();

            [Test]
            public void Should_fail_with_the_type_ArgumentException() =>
                _exception.Should().BeOfType<ArgumentException>();

            [Test]
            public void Should_fail_telling_the_name_of_the_erroneous_parameter() =>
                ((ArgumentException)_exception).ParamName.Should().Be("scheduledDeliveries");

            [Test]
            public void Should_fail_telling_the_the_reason_of_the_failure() =>
                _exception.Message.Should().StartWith($"The manager for delivering the route assigned to the Drone '{_assignedDrone.Id}' cannot be created without providing any scheduled delivery.");
        }

        public class When_starting_the_operation : DeliveryRouteManagerSpecs
        {
            public override void When() =>
                _result = new DeliveryRouteManager(_scheduledDeliveries, _assignedDrone).StartOperation();

            public class When_the_route_has_only_1_deliver : When_starting_the_operation
            {
                public override void Given() => 
                    _scheduledDeliveries = new[] {CreateSomeScheduledDelivery()};

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_request_a_delivery_trip() =>
                    _result.Should().BeOfType<DeliveryTripRequest>();

                [Test]
                public void Should_request_the_delivery_trip_to_the_assigned_drone() =>
                    ResultAsRequest.ResponsibleDroneId.Should().Be(_assignedDrone.Id);

                [Test]
                public void Should_request_the_delivery_trip_with_something_to_deliver() =>
                    ResultAsRequest.ScheduledDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_request_the_delivery_trip_with_only_1_deliver() =>
                    ResultAsRequest.ScheduledDeliveries.Count.Should().Be(1);

                [Test]
                public void Should_request_the_delivery_trip_with_the_same_original_scheduled_deliver() =>
                    ResultAsRequest.ScheduledDeliveries.Should().Contain(_scheduledDeliveries);
            }

            public class When_the_number_of_delivers_is_exactly_the_same_as_the_maximum_capacity_of_the_drone : When_starting_the_operation
            {
                public override void Given() => 
                    _scheduledDeliveries = CreateMultipleScheduledDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip);

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_request_a_delivery_trip() =>
                    _result.Should().BeOfType<DeliveryTripRequest>();

                [Test]
                public void Should_request_the_delivery_trip_to_the_assigned_drone() =>
                    ResultAsRequest.ResponsibleDroneId.Should().Be(_assignedDrone.Id);

                [Test]
                public void Should_request_the_delivery_trip_with_something_to_deliver() =>
                    ResultAsRequest.ScheduledDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_request_a_deliver_with_only_the_number_of_deliveries_allowed_by_the_drone() =>
                    ResultAsRequest.ScheduledDeliveries.Count.Should().Be(_assignedDrone.MaximumDeliveryCapacityPerTrip);

                [Test]
                public void Should_request_the_delivery_trip_with_the_same_original_scheduled_deliveries() =>
                    ResultAsRequest.ScheduledDeliveries.Should().Contain(_scheduledDeliveries);
            }

            public class When_the_number_of_delivers_exceeds_the_maximum_capacity_of_the_drone : When_starting_the_operation
            {
                public override void Given() => 
                    _scheduledDeliveries = CreateMultipleScheduledDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip + 1);

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_request_a_delivery_trip() =>
                    _result.Should().BeOfType<DeliveryTripRequest>();

                [Test]
                public void Should_request_the_delivery_trip_to_the_assigned_drone() =>
                    ResultAsRequest.ResponsibleDroneId.Should().Be(_assignedDrone.Id);

                [Test]
                public void Should_request_the_delivery_trip_with_something_to_deliver() =>
                    ResultAsRequest.ScheduledDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_request_a_deliver_with_only_the_number_of_deliveries_allowed_by_the_drone() =>
                    ResultAsRequest.ScheduledDeliveries.Count.Should().Be(_assignedDrone.MaximumDeliveryCapacityPerTrip);

                [Test]
                public void Should_request_the_delivery_trip_with_the_first_original_scheduled_deliveries() =>
                    ResultAsRequest.ScheduledDeliveries.Should().Contain(new[] {_scheduledDeliveries[0], _scheduledDeliveries[1], _scheduledDeliveries[2]});

                [Test]
                public void Should_request_the_delivery_trip_without_the_original_scheduled_deliveries_exceeding_the_capacity() =>
                    ResultAsRequest.ScheduledDeliveries.Should().NotContain(new[] {_scheduledDeliveries[3]});
            }
        }

        public class When_confirming_deliveries_for_a_delivery_trip : DeliveryRouteManagerSpecs
        {
            private DeliveryRouteManager _deliveryRouteManager;
            private DeliveryTripConfirmed _deliveryTripConfirmed;

            public override void When() =>
                _exception = Catch.Exception(() =>
                    _result = _deliveryRouteManager.Confirm(_deliveryTripConfirmed)
                );

            public class When_there_are_still_the_last_deliveries : When_confirming_deliveries_for_a_delivery_trip
            {
                private int _numberOfSecondSetDeliveries;

                public override void Given()
                {
                    _numberOfSecondSetDeliveries = _assignedDrone.MaximumDeliveryCapacityPerTrip - 1;
                    var firstTripDeliveries = SetManagerGettingTheFirstDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip + _numberOfSecondSetDeliveries);
                    _deliveryTripConfirmed = CreatePerfectDeliveryTripConfirmedFrom(firstTripDeliveries);
                }

                [Test]
                public void Should_not_fail() =>
                    _exception.Should().BeNull();

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_request_a_delivery_trip() =>
                    _result.Should().BeOfType<DeliveryTripRequest>();

                [Test]
                public void Should_request_the_delivery_trip_to_the_assigned_drone() =>
                    ResultAsRequest.ResponsibleDroneId.Should().Be(_assignedDrone.Id);

                [Test]
                public void Should_request_the_delivery_trip_with_something_to_deliver() =>
                    ResultAsRequest.ScheduledDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_request_a_deliver_with_only_the_number_of_remaining_deliveries() =>
                    ResultAsRequest.ScheduledDeliveries.Count.Should().Be(_numberOfSecondSetDeliveries);

                [Test]
                public void Should_request_the_delivery_trip_without_the_first_original_scheduled_deliveries() =>
                    ResultAsRequest.ScheduledDeliveries.Should().NotContain(new[] {_scheduledDeliveries[0], _scheduledDeliveries[1], _scheduledDeliveries[2]});

                [Test]
                public void Should_request_the_delivery_trip_with_the_original_scheduled_deliveries_exceeding_the_capacity() =>
                    ResultAsRequest.ScheduledDeliveries.Should().Contain(_scheduledDeliveries.TakeLast(_numberOfSecondSetDeliveries));
            }

            public class When_there_are_more_and_more_deliveries : When_confirming_deliveries_for_a_delivery_trip
            {
                public override void Given()
                {
                    var firstTripDeliveries = SetManagerGettingTheFirstDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip * 3);
                    _deliveryTripConfirmed = CreatePerfectDeliveryTripConfirmedFrom(firstTripDeliveries);
                }

                [Test]
                public void Should_not_fail() =>
                    _exception.Should().BeNull();

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_request_a_delivery_trip() =>
                    _result.Should().BeOfType<DeliveryTripRequest>();

                [Test]
                public void Should_request_the_delivery_trip_to_the_assigned_drone() =>
                    ResultAsRequest.ResponsibleDroneId.Should().Be(_assignedDrone.Id);

                [Test]
                public void Should_request_the_delivery_trip_with_something_to_deliver() =>
                    ResultAsRequest.ScheduledDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_request_a_deliver_with_only_the_number_of_deliveries_allowed_by_the_drone() =>
                    ResultAsRequest.ScheduledDeliveries.Count.Should().Be(_assignedDrone.MaximumDeliveryCapacityPerTrip);

                [Test]
                public void Should_request_the_delivery_trip_without_the_first_original_scheduled_deliveries() =>
                    ResultAsRequest.ScheduledDeliveries.Should().NotContain(_scheduledDeliveries.Take(3));

                [Test]
                public void Should_request_the_delivery_trip_with_the_original_scheduled_deliveries_exceeding_the_capacity() =>
                    ResultAsRequest.ScheduledDeliveries.Should().Contain(((List<ScheduledDelivery>)_scheduledDeliveries).GetRange(3 , 3));

                [Test]
                public void Should_request_the_delivery_trip_without_the_last_original_scheduled_deliveries() =>
                    ResultAsRequest.ScheduledDeliveries.Should().NotContain(_scheduledDeliveries.TakeLast(3));
            }

            public class When_there_are_no_more_deliveries : When_confirming_deliveries_for_a_delivery_trip
            {
                private ICollection<ConfirmedDelivery> _allConfirmedDeliveries;

                public override void Given()
                {
                    var firstTripDeliveries = SetManagerGettingTheFirstDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip * 2);
                    var firstDeliveryTripConfirmed = CreatePerfectDeliveryTripConfirmedFrom(firstTripDeliveries);
                    var secondTripDeliveries = _deliveryRouteManager.Confirm(firstDeliveryTripConfirmed) as DeliveryTripRequest;
                    _deliveryTripConfirmed = CreatePerfectDeliveryTripConfirmedFrom(secondTripDeliveries);
                    _allConfirmedDeliveries = firstDeliveryTripConfirmed.ConfirmedDeliveries.Concat(_deliveryTripConfirmed.ConfirmedDeliveries).ToList();
                }

                [Test]
                public void Should_not_fail() =>
                    _exception.Should().BeNull();

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_report_the_delivery_route_result() =>
                    _result.Should().BeOfType<DeliveryRouteResult>();

                [Test]
                public void Should_request_the_delivery_trip_to_the_assigned_drone() =>
                    ResultAsReport.PerformerDroneId.Should().Be(_assignedDrone.Id);

                [Test]
                public void Should_tell_something_about_the_confirmed_deliveries() =>
                    ResultAsReport.ConfirmedDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_have_the_exact_number_of_confirmed_deliveries_in_the_route() =>
                    ResultAsReport.ConfirmedDeliveries.Count.Should().Be(_assignedDrone.MaximumDeliveryCapacityPerTrip * 2);

                [Test]
                public void Should_have_all_confirmed_deliveries_in_the_route() =>
                    ResultAsReport.ConfirmedDeliveries.Should().Contain(_allConfirmedDeliveries);
            }

            public class When_the_drone_which_did_the_delivery_is_not_the_same_managed : When_confirming_deliveries_for_a_delivery_trip
            {
                private Guid _unknownDroneId = Guid.NewGuid();

                public override void Given() => 
                    _deliveryTripConfirmed = CreateAnyPerfectDeliveryTripConfirmed() with { PerformerDroneId = _unknownDroneId };

                [Test]
                public void Should_fail() =>
                    _exception.Should().NotBeNull();

                [Test]
                public void Should_fail_with_the_type_ArgumentException() =>
                    _exception.Should().BeOfType<ArgumentException>();

                [Test]
                public void Should_fail_telling_the_name_of_the_erroneous_parameter() =>
                    ((ArgumentException)_exception).ParamName.Should().Be("PerformerDroneId");

                [Test]
                public void Should_fail_telling_the_the_reason_of_the_failure() =>
                    _exception.Message.Should().StartWith($"The delivery trip was confirmed as performed by the Drone '{_unknownDroneId}', however, the actual Drone assigned to this manager is '{_assignedDrone.Id}'.");
            }

            public class When_a_confirmed_delivery_is_unknown_by_the_manager : When_confirming_deliveries_for_a_delivery_trip
            {
                private Guid _unknownDeliveryId = Guid.NewGuid();

                public override void Given()
                {
                    var deliveryTripConfirmed = CreateAnyPerfectDeliveryTripConfirmed();
                    var havingUnknownDelivery = deliveryTripConfirmed.ConfirmedDeliveries.Concat(new[]
                    {
                        new ConfirmedDelivery {Id = _unknownDeliveryId}
                    }).ToList();
                    _deliveryTripConfirmed = deliveryTripConfirmed with {ConfirmedDeliveries = havingUnknownDelivery};
                }

                [Test]
                public void Should_fail() =>
                    _exception.Should().NotBeNull();

                [Test]
                public void Should_fail_with_the_type_ArgumentException() =>
                    _exception.Should().BeOfType<ArgumentException>();

                [Test]
                public void Should_fail_telling_the_name_of_the_erroneous_parameter() =>
                    ((ArgumentException)_exception).ParamName.Should().Be("Id");

                [Test]
                public void Should_fail_telling_the_the_reason_of_the_failure() =>
                    _exception.Message.Should().StartWith($"The delivery trip was confirmed including the delivery '{_unknownDeliveryId}', however, that delivery was not requested by the manager operating the Drone '{_assignedDrone.Id}'.");
            }


            private DeliveryTripConfirmed CreateAnyPerfectDeliveryTripConfirmed()
            {
                var firstTripDeliveries = SetManagerGettingTheFirstDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip);
                return CreatePerfectDeliveryTripConfirmedFrom(firstTripDeliveries);
            }

            private DeliveryTripRequest SetManagerGettingTheFirstDeliveries(int deliveriesAmount)
            {
                _scheduledDeliveries = CreateMultipleScheduledDeliveries(deliveriesAmount);
                _deliveryRouteManager = new DeliveryRouteManager(_scheduledDeliveries, _assignedDrone);
                var tripDeliveries = _deliveryRouteManager.StartOperation();
                return tripDeliveries as DeliveryTripRequest;
            }

            private static DeliveryTripConfirmed CreatePerfectDeliveryTripConfirmedFrom(DeliveryTripRequest tripRequest)
            {
                var confirmedDeliveries = tripRequest.ScheduledDeliveries.Select(x => new ConfirmedDelivery {Id = x.Id}).ToList();
                return new DeliveryTripConfirmed {PerformerDroneId = tripRequest.ResponsibleDroneId, ConfirmedDeliveries = confirmedDeliveries};
            }
        }


        private DeliveryTripRequest ResultAsRequest => _result as DeliveryTripRequest;

        private DeliveryRouteResult ResultAsReport => _result as DeliveryRouteResult;

        private static ScheduledDelivery CreateSomeScheduledDelivery()
        {
            return new() {Id = Guid.NewGuid()};
        }

        private static List<ScheduledDelivery> CreateMultipleScheduledDeliveries(int amount)
        {
            var deliveries = new List<ScheduledDelivery>();
            for (var i = 0; i < amount; i++)
                deliveries.Add(CreateSomeScheduledDelivery());
            return deliveries;
        }
    }
}