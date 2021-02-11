using System;
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
            public void Should_break() =>
                _exception.Should().NotBeNull();

            [Test]
            public void Should_break_with_the_type_ArgumentException() =>
                _exception.Should().BeOfType<ArgumentException>();

            [Test]
            public void Should_break_telling_the_name_of_the_erroneous_parameter() =>
                ((ArgumentException)_exception).ParamName.Should().Be("scheduledDeliveries");

            [Test]
            public void Should_break_telling_the_the_reason_of_the_failure() =>
                _exception.Message.Should().StartWith($"The manager for delivering the route assigned to the Drone '{_assignedDrone.Id}' cannot be created without providing any scheduled delivery.");
        }

        public class When_starting_the_operation : DeliveryRouteManagerSpecs
        {
            private DeliveryTripRequest _result;

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
                public void Should_request_the_delivery_trip_to_the_assigned_drone() =>
                    _result.ResponsibleDroneId.Should().Be(_assignedDrone.Id);

                [Test]
                public void Should_request_the_delivery_trip_with_something_to_deliver() =>
                    _result.ScheduledDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_request_the_delivery_trip_with_only_1_deliver() =>
                    _result.ScheduledDeliveries.Count.Should().Be(1);

                [Test]
                public void Should_request_the_delivery_trip_with_the_same_original_scheduled_deliver() =>
                    _result.ScheduledDeliveries.Should().Contain(_scheduledDeliveries);
            }

            public class When_the_number_of_delivers_is_exactly_the_same_as_the_maximum_capacity_of_the_drone : When_starting_the_operation
            {
                public override void Given() => 
                    _scheduledDeliveries = CreateMultipleScheduledDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip);

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_request_the_delivery_trip_to_the_assigned_drone() =>
                    _result.ResponsibleDroneId.Should().Be(_assignedDrone.Id);

                [Test]
                public void Should_request_the_delivery_trip_with_something_to_deliver() =>
                    _result.ScheduledDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_request_a_deliver_with_only_the_number_of_deliveries_allowed_by_the_drone() =>
                    _result.ScheduledDeliveries.Count.Should().Be(_assignedDrone.MaximumDeliveryCapacityPerTrip);

                [Test]
                public void Should_request_the_delivery_trip_with_the_same_original_scheduled_deliveries() =>
                    _result.ScheduledDeliveries.Should().Contain(_scheduledDeliveries);
            }

            public class When_the_number_of_delivers_exceeds_the_maximum_capacity_of_the_drone : When_starting_the_operation
            {
                public override void Given() => 
                    _scheduledDeliveries = CreateMultipleScheduledDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip + 1);

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_request_the_delivery_trip_to_the_assigned_drone() =>
                    _result.ResponsibleDroneId.Should().Be(_assignedDrone.Id);

                [Test]
                public void Should_request_the_delivery_trip_with_something_to_deliver() =>
                    _result.ScheduledDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_request_a_deliver_with_only_the_number_of_deliveries_allowed_by_the_drone() =>
                    _result.ScheduledDeliveries.Count.Should().Be(_assignedDrone.MaximumDeliveryCapacityPerTrip);

                [Test]
                public void Should_request_the_delivery_trip_with_the_first_original_scheduled_deliveries() =>
                    _result.ScheduledDeliveries.Should().Contain(new[] {_scheduledDeliveries[0], _scheduledDeliveries[1], _scheduledDeliveries[2]});

                [Test]
                public void Should_request_the_delivery_trip_without_the_original_scheduled_deliveries_exceeding_the_capacity() =>
                    _result.ScheduledDeliveries.Should().NotContain(new[] {_scheduledDeliveries[3]});
            }
        }

        public class When_checking_deliveries_for_a_delivery_trip : DeliveryRouteManagerSpecs
        {
            private DeliveryRouteManager _deliveryRouteManager;
            private DeliveryTripConfirmed _deliveryTripConfirmed;
            private ICheckResult _result;

            public override void When() =>
                _exception = Catch.Exception(() =>
                    _result = _deliveryRouteManager.Check(_deliveryTripConfirmed)
                );

            public class When_there_are_still_the_last_deliveries : When_checking_deliveries_for_a_delivery_trip
            {
                private int _numberOfSecondSetDeliveries;

                public override void Given()
                {
                    _numberOfSecondSetDeliveries = _assignedDrone.MaximumDeliveryCapacityPerTrip - 1;
                    var firstDeliveryTripRequest = SetManagerGettingTheFirstDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip + _numberOfSecondSetDeliveries);
                    _deliveryTripConfirmed = CreatePerfectDeliveryTripConfirmedFrom(firstDeliveryTripRequest);
                }

                [Test]
                public void Should_not_break() =>
                    _exception.Should().BeNull();

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_get_a_result_as_a_WorkOrder() =>
                    _result.Should().BeOfType<WorkOrder>();

                [Test]
                public void Should_have_no_failed_Deliveries() =>
                    ResultAsWorkOrder.FailedDeliveries.Should().BeEmpty();

                [Test]
                public void Should_have_a_request() =>
                    ResultAsWorkOrder.Request.Should().NotBeNull();
                
                [Test]
                public void Should_request_the_delivery_trip_to_the_assigned_drone() =>
                    ResultAsWorkOrder.Request.ResponsibleDroneId.Should().Be(_assignedDrone.Id);

                [Test]
                public void Should_request_the_delivery_trip_with_something_to_deliver() =>
                    ResultAsWorkOrder.Request.ScheduledDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_request_a_deliver_with_only_the_number_of_remaining_deliveries() =>
                    ResultAsWorkOrder.Request.ScheduledDeliveries.Count.Should().Be(_numberOfSecondSetDeliveries);

                [Test]
                public void Should_request_the_delivery_trip_without_the_first_original_scheduled_deliveries() =>
                    ResultAsWorkOrder.Request.ScheduledDeliveries.Should().NotContain(new[] {_scheduledDeliveries[0], _scheduledDeliveries[1], _scheduledDeliveries[2]});

                [Test]
                public void Should_request_the_delivery_trip_with_the_original_scheduled_deliveries_exceeding_the_capacity() =>
                    ResultAsWorkOrder.Request.ScheduledDeliveries.Should().Contain(_scheduledDeliveries.TakeLast(_numberOfSecondSetDeliveries));
            }

            public class When_there_are_more_and_more_deliveries : When_checking_deliveries_for_a_delivery_trip
            {
                public override void Given()
                {
                    var firstDeliveryTripRequest = SetManagerGettingTheFirstDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip * 3);
                    _deliveryTripConfirmed = CreatePerfectDeliveryTripConfirmedFrom(firstDeliveryTripRequest);
                }

                [Test]
                public void Should_not_break() =>
                    _exception.Should().BeNull();

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_get_a_result_as_a_WorkOrder() =>
                    _result.Should().BeOfType<WorkOrder>();

                [Test]
                public void Should_have_no_failed_Deliveries() =>
                    ResultAsWorkOrder.FailedDeliveries.Should().BeEmpty();

                [Test]
                public void Should_have_a_request() =>
                    ResultAsWorkOrder.Request.Should().NotBeNull();

                [Test]
                public void Should_request_the_delivery_trip_to_the_assigned_drone() =>
                    ResultAsWorkOrder.Request.ResponsibleDroneId.Should().Be(_assignedDrone.Id);

                [Test]
                public void Should_request_the_delivery_trip_with_something_to_deliver() =>
                    ResultAsWorkOrder.Request.ScheduledDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_request_a_deliver_with_only_the_number_of_deliveries_allowed_by_the_drone() =>
                    ResultAsWorkOrder.Request.ScheduledDeliveries.Count.Should().Be(_assignedDrone.MaximumDeliveryCapacityPerTrip);

                [Test]
                public void Should_request_the_delivery_trip_without_the_first_original_scheduled_deliveries() =>
                    ResultAsWorkOrder.Request.ScheduledDeliveries.Should().NotContain(_scheduledDeliveries.Take(3));

                [Test]
                public void Should_request_the_delivery_trip_with_the_original_scheduled_deliveries_exceeding_the_capacity() =>
                    ResultAsWorkOrder.Request.ScheduledDeliveries.Should().Contain(((List<ScheduledDelivery>)_scheduledDeliveries).GetRange(3 , 3));

                [Test]
                public void Should_request_the_delivery_trip_without_the_last_original_scheduled_deliveries() =>
                    ResultAsWorkOrder.Request.ScheduledDeliveries.Should().NotContain(_scheduledDeliveries.TakeLast(3));
            }

            public class When_there_are_no_more_deliveries : When_checking_deliveries_for_a_delivery_trip
            {
                private ICollection<ConfirmedDelivery> _allConfirmedDeliveries;

                public override void Given()
                {
                    var firstTripRequest = SetManagerGettingTheFirstDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip * 2);
                    var firstDeliveryTripConfirmed = CreatePerfectDeliveryTripConfirmedFrom(firstTripRequest);
                    var secondTripRequest = ((WorkOrder) _deliveryRouteManager.Check(firstDeliveryTripConfirmed)).Request;
                    _deliveryTripConfirmed = CreatePerfectDeliveryTripConfirmedFrom(secondTripRequest);
                    _allConfirmedDeliveries = firstDeliveryTripConfirmed.ConfirmedDeliveries.Concat(_deliveryTripConfirmed.ConfirmedDeliveries).ToList();
                }

                [Test]
                public void Should_not_break() =>
                    _exception.Should().BeNull();

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_report_the_delivery_route_result() =>
                    _result.Should().BeOfType<DeliveryRouteReport>();

                [Test]
                public void Should_have_no_failed_Deliveries() =>
                    ResultAsReport.FailedDeliveries.Should().BeEmpty();

                [Test]
                public void Should_have_no_last_failed_Deliveries() =>
                    ResultAsReport.LastFailedDeliveries.Should().BeEmpty();

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

            public class When_the_drone_which_did_the_delivery_is_not_the_same_managed : When_checking_deliveries_for_a_delivery_trip
            {
                private Guid _unknownDroneId = Guid.NewGuid();

                public override void Given() => 
                    _deliveryTripConfirmed = CreateAnyPerfectDeliveryTripConfirmed() with { PerformerDroneId = _unknownDroneId };

                [Test]
                public void Should_break() =>
                    _exception.Should().NotBeNull();

                [Test]
                public void Should_break_with_the_type_ArgumentException() =>
                    _exception.Should().BeOfType<ArgumentException>();

                [Test]
                public void Should_break_telling_the_name_of_the_erroneous_parameter() =>
                    ((ArgumentException)_exception).ParamName.Should().Be("PerformerDroneId");

                [Test]
                public void Should_break_telling_the_the_reason_of_the_failure() =>
                    _exception.Message.Should().StartWith($"The delivery trip was confirmed as performed by the Drone '{_unknownDroneId}', however, the actual Drone assigned to this manager is '{_assignedDrone.Id}'.");
            }

            public class When_a_confirmed_delivery_is_unknown_by_the_manager : When_checking_deliveries_for_a_delivery_trip
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
                public void Should_break() =>
                    _exception.Should().NotBeNull();

                [Test]
                public void Should_break_with_the_type_ArgumentException() =>
                    _exception.Should().BeOfType<ArgumentException>();

                [Test]
                public void Should_break_telling_the_name_of_the_erroneous_parameter() =>
                    ((ArgumentException)_exception).ParamName.Should().Be("Id");

                [Test]
                public void Should_break_telling_the_the_reason_of_the_failure() =>
                    _exception.Message.Should().StartWith($"The delivery trip was confirmed including the delivery '{_unknownDeliveryId}', however, that delivery was not requested by the manager operating the Drone '{_assignedDrone.Id}'.");
            }

            public class When_a_delivery_was_made_at_the_wrong_position : When_checking_deliveries_for_a_delivery_trip
            {
                private ScheduledDelivery _targetScheduledDelivery;
                private ConfirmedDelivery _targetConfirmedDelivery;

                public override void Given()
                {
                    SetManagerGettingTheFirstDeliveries(5);
                    _targetScheduledDelivery = _scheduledDeliveries[1];
                    _targetConfirmedDelivery = new ConfirmedDelivery {Id = _targetScheduledDelivery.Id, DischargePosition = CreateAnyPosition()};
                    _deliveryTripConfirmed = new DeliveryTripConfirmed
                    {
                        PerformerDroneId = _assignedDrone.Id, 
                        ConfirmedDeliveries = new[]
                        {
                            CreateConfirmedDeliveryFor(_scheduledDeliveries[0]),
                            _targetConfirmedDelivery,
                            CreateConfirmedDeliveryFor(_scheduledDeliveries[2])
                        }
                    };
                }

                [Test]
                public void Should_not_break() =>
                    _exception.Should().BeNull();

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_get_a_result_as_a_WorkOrder() =>
                    _result.Should().BeOfType<WorkOrder>();

                [Test]
                public void Should_request_the_delivery_trip_for_any_remaining_deliveries() =>
                    ResultAsWorkOrder.Request.ScheduledDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_have_failed_Deliveries() =>
                    ResultAsWorkOrder.FailedDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_have_failed_with_only_the_wrong_delivery() =>
                    ResultAsWorkOrder.FailedDeliveries.Count.Should().Be(1);

                [Test]
                public void Should_have_failed_as_FailedWrongPositionDelivery() =>
                    FirstFailedDelivery.Should().BeOfType<FailedByWrongPositionDelivery>();

                [Test]
                public void Should_have_failed_telling_the_actually_requested_delivery() =>
                    FirstFailedDeliveryByWrongPosition.ScheduledDelivery.Should().Be(_targetScheduledDelivery);

                [Test]
                public void Should_have_failed_telling_the_finally_confirmed_delivery() =>
                    FirstFailedDeliveryByWrongPosition.ConfirmedDelivery.Should().Be(_targetConfirmedDelivery);

                [Test]
                public void Should_have_failed_telling_the_reason_for_that() =>
                    FirstFailedDeliveryByWrongPosition.Reason.Should().Be("The delivery was done at a wrong position");
            }

            public class When_a_delivery_made_had_already_been_confirmed : When_checking_deliveries_for_a_delivery_trip
            {
                private ConfirmedDelivery _alreadyConfirmedDelivery;
                private ConfirmedDelivery _targetConfirmedDelivery;

                public override void Given()
                {
                    var firstDeliveryTripRequest = SetManagerGettingTheFirstDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip * 3);
                    var firstDeliveryTripConfirmed = CreatePerfectDeliveryTripConfirmedFrom(firstDeliveryTripRequest);
                    _alreadyConfirmedDelivery = firstDeliveryTripConfirmed.ConfirmedDeliveries.Last();
                    _deliveryRouteManager.Check(firstDeliveryTripConfirmed);
                    _targetConfirmedDelivery = _alreadyConfirmedDelivery;
                    _deliveryTripConfirmed = new DeliveryTripConfirmed
                    {
                        PerformerDroneId = _assignedDrone.Id, 
                        ConfirmedDeliveries = new[]
                        {
                            CreateConfirmedDeliveryFor(_scheduledDeliveries[3]),
                            CreateConfirmedDeliveryFor(_scheduledDeliveries[4]),
                            _targetConfirmedDelivery,
                            CreateConfirmedDeliveryFor(_scheduledDeliveries[5])
                        }
                    };
                }

                [Test]
                public void Should_not_break() =>
                    _exception.Should().BeNull();

                [Test]
                public void Should_get_a_result() =>
                    _result.Should().NotBeNull();

                [Test]
                public void Should_get_a_result_as_a_WorkOrder() =>
                    _result.Should().BeOfType<WorkOrder>();

                [Test]
                public void Should_request_the_delivery_trip_for_any_remaining_deliveries() =>
                    ResultAsWorkOrder.Request.ScheduledDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_have_failed_Deliveries() =>
                    ResultAsWorkOrder.FailedDeliveries.Should().NotBeEmpty();

                [Test]
                public void Should_have_failed_with_only_the_wrong_delivery() =>
                    ResultAsWorkOrder.FailedDeliveries.Count.Should().Be(1);

                [Test]
                public void Should_have_failed_as_FailedWrongPositionDelivery() =>
                    FirstFailedDelivery.Should().BeOfType<FailedByAlreadyConfirmedDelivery>();

                [Test]
                public void Should_have_failed_telling_the_actually_requested_delivery() =>
                    FirstFailedDeliveryByAlreadyConfirmed.AlreadyConfirmedDelivery.Should().Be(_alreadyConfirmedDelivery);

                [Test]
                public void Should_have_failed_telling_the_finally_confirmed_delivery() =>
                    FirstFailedDeliveryByAlreadyConfirmed.ConfirmedDelivery.Should().Be(_targetConfirmedDelivery);

                [Test]
                public void Should_have_failed_telling_the_reason_for_that() =>
                    FirstFailedDeliveryByAlreadyConfirmed.Reason.Should().Be("The delivery had already been confirmed");
            }

            public class When_a_previously_requested_delivery_is_not_confirmed_in_the_next_check : When_checking_deliveries_for_a_delivery_trip
            {
                [Test]
                [Ignore("Not yet implemented")]
                public void Should_not_break() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_get_a_result() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_get_a_result_as_a_WorkOrder() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_request_the_delivery_trip_for_any_remaining_deliveries() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_have_failed_Deliveries() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_have_failed_with_only_the_wrong_delivery() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_have_failed_as_FailedByNotMadeDelivery() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_have_failed_telling_the_actually_requested_delivery() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_have_failed_telling_the_delivery_trip_where_it_was_expected_the_delivery() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_have_failed_telling_the_reason_for_that() {}
            }

            public class When_multiple_deliveries_fail_in_the_same_confirmed_delivery_trip : When_checking_deliveries_for_a_delivery_trip
            {
                [Test]
                [Ignore("Not yet implemented")]
                public void Should_not_break() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_get_a_result() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_get_a_result_as_a_WorkOrder() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_request_the_delivery_trip_for_any_remaining_deliveries() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_have_failed_Deliveries() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_have_failed_containing_all_wrong_deliveries() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_have_failed_with_only_the_wrong_deliveries() {}
            }

            public class When_all_delivery_trip_end_with_failed_deliveries_in_some_of_them_even_from_the_last_one : When_checking_deliveries_for_a_delivery_trip
            {
                [Test]
                [Ignore("Not yet implemented")]
                public void Should_not_break() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_get_a_result() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_get_a_result_as_a_DeliveryRouteReport() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_report_the_same_performer_drone_as_the_one_formerly_assigned_to_the_delivery() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_report_all_formerly_scheduled_deliveries_to_do() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_report_all_confirmed_deliveries_made_by_the_drone() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_report_all_failed_deliveries_identified_for_all_the_checks() {}

                [Test]
                [Ignore("Not yet implemented")]
                public void Should_report_all_failed_deliveries_identified_for_only_the_last_check() {}
            }


            private WorkOrder ResultAsWorkOrder => _result as WorkOrder;

            private DeliveryRouteReport ResultAsReport => _result as DeliveryRouteReport;

            private IFailedDelivery FirstFailedDelivery => ResultAsWorkOrder.FailedDeliveries.First();

            private FailedByWrongPositionDelivery FirstFailedDeliveryByWrongPosition => FirstFailedDelivery as FailedByWrongPositionDelivery;

            private FailedByAlreadyConfirmedDelivery FirstFailedDeliveryByAlreadyConfirmed => FirstFailedDelivery as FailedByAlreadyConfirmedDelivery;

            private DeliveryTripConfirmed CreateAnyPerfectDeliveryTripConfirmed()
            {
                var firstDeliveryTripRequest = SetManagerGettingTheFirstDeliveries(_assignedDrone.MaximumDeliveryCapacityPerTrip);
                return CreatePerfectDeliveryTripConfirmedFrom(firstDeliveryTripRequest);
            }

            private DeliveryTripRequest SetManagerGettingTheFirstDeliveries(int deliveriesAmount)
            {
                _scheduledDeliveries = CreateMultipleScheduledDeliveries(deliveriesAmount);
                _deliveryRouteManager = new DeliveryRouteManager(_scheduledDeliveries, _assignedDrone);
                var tripDeliveries = _deliveryRouteManager.StartOperation();
                return tripDeliveries;
            }

            private static DeliveryTripConfirmed CreatePerfectDeliveryTripConfirmedFrom(DeliveryTripRequest tripRequest)
            {
                var confirmedDeliveries = tripRequest.ScheduledDeliveries.Select(x => CreateConfirmedDeliveryFor(x)).ToList();
                return new DeliveryTripConfirmed {PerformerDroneId = tripRequest.ResponsibleDroneId, ConfirmedDeliveries = confirmedDeliveries};
            }

            private static ConfirmedDelivery CreateConfirmedDeliveryFor(ScheduledDelivery sourceDelivery)
            {
                return new()
                {
                    Id = sourceDelivery.Id,
                    DischargePosition = sourceDelivery.TargetPosition
                };
            }
        }


        private static List<ScheduledDelivery> CreateMultipleScheduledDeliveries(int amount)
        {
            var deliveries = new List<ScheduledDelivery>();
            for (var i = 0; i < amount; i++)
                deliveries.Add(CreateSomeScheduledDelivery());
            return deliveries;
        }

        private static int _positionNumerator;

        private static ScheduledDelivery CreateSomeScheduledDelivery()
        {
            return new()
            {
                Id = Guid.NewGuid(), TargetPosition = CreateAnyPosition()
            };
        }

        private static Position CreateAnyPosition()
        {
            return new()
            {
                CoordinateX = ++_positionNumerator,
                CoordinateY = ++_positionNumerator,
                CardinalPoint = (CardinalPoint) (_positionNumerator / 2 % 4)
            };
        }
    }
}