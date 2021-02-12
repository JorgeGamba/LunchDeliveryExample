using Doing.BDDExtensions;
using FluentAssertions;
using LunchDelivery.Schedule;
using NUnit.Framework;

namespace LunchDelivery.UnitSpecs.Schedule
{
    [TestFixture]
    public class MovementDescriptionSpecs : FeatureSpecifications
    {
        private string _source;
        private MovementDescription _result;
        private string _failureReason;
        private bool _worked;

        public override void When() =>
            _worked = MovementDescription.TryCreateFrom(_source, out _result, out _failureReason);

        public class When_the_source_string_is_a_valid_text : MovementDescriptionSpecs
        {
            public override void Given() => 
                _source = "X";

            [Test]
            public void Should_work() =>
                _worked.Should().BeTrue();

            [Test]
            public void Should_get_something() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_get_the_original_value() =>
                _result.Value.Should().Be(_source);

            [Test]
            public void Should_not_get_any_failure_reason() =>
                _failureReason.Should().BeNull();
        }

        public class When_the_source_string_is_null : MovementDescriptionSpecs
        {
            public override void Given() => 
                _source = null;

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
                _failureReason.Should().Be("It is required to provide a text.");
        }

        public class When_the_source_string_is_empty : MovementDescriptionSpecs
        {
            public override void Given() => 
                _source = string.Empty;

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
                _failureReason.Should().Be("It is required to provide a text.");
        }

        public class When_the_source_string_is_just_spaces : MovementDescriptionSpecs
        {
            public override void Given() => 
                _source = "   ";

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
                _failureReason.Should().Be("It is required to provide a text.");
        }

        public class When_the_source_string_has_spaces_at_the_beginning : MovementDescriptionSpecs
        {
            public override void Given() => 
                _source = "  Foo";

            [Test]
            public void Should_work() =>
                _worked.Should().BeTrue();

            [Test]
            public void Should_get_something() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_get_the_original_value_without_those_spaces() =>
                _result.Value.Should().Be("Foo");

            [Test]
            public void Should_not_get_any_failure_reason() =>
                _failureReason.Should().BeNull();
        }

        public class When_the_source_string_has_spaces_at_the_end : MovementDescriptionSpecs
        {
            public override void Given() => 
                _source = "Foo  ";

            [Test]
            public void Should_work() =>
                _worked.Should().BeTrue();

            [Test]
            public void Should_get_something() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_get_the_original_value_without_those_spaces() =>
                _result.Value.Should().Be("Foo");

            [Test]
            public void Should_not_get_any_failure_reason() =>
                _failureReason.Should().BeNull();
        }

        public class When_the_source_string_has_spaces_inside : MovementDescriptionSpecs
        {
            public override void Given() => 
                _source = "  F o  o ";

            [Test]
            public void Should_work() =>
                _worked.Should().BeTrue();

            [Test]
            public void Should_get_something() =>
                _result.Should().NotBeNull();

            [Test]
            public void Should_get_the_original_value_without_those_spaces() =>
                _result.Value.Should().Be("Foo");

            [Test]
            public void Should_not_get_any_failure_reason() =>
                _failureReason.Should().BeNull();
        }
    }
}