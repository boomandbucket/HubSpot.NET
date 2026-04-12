using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.CustomEvent.Dto;
using HubSpot.NET.Api.Schemas;
using HubSpot.NET.Core;
using System.Net;

namespace HubSpot.NET.IntegrationTests.Api.CustomEvent
{
    public class HubSpotCustomEventApiAsyncIntegrationTests : HubSpotAsyncIntegrationTestBase
    {
        private async Task<EventDefinition> CreateUniqueEventDefinitionAsync(string primaryObjectId = HubSpotObjectTypes.CONTACT)
        {
            var eventDefinition = new EventDefinition
            {
                Name = "test_event_" + Guid.NewGuid().ToString("N"),
                Label = "Test Event",
                Labels = new SchemasLabelsModel { Singular = "Test Event" },
                Description = "Test event description",
                PrimaryObjectId = primaryObjectId,
                TrackingType = "MANUAL"
            };

            var createdEvent = await CustomEventApi.CreateEventDefinitionAsync(eventDefinition);
            CustomEventsToCleanup.Add(createdEvent.Name);
            return createdEvent;
        }

        [Fact]
        public async Task CreateEventDefinitionAsync_WhenValidEvent_ShouldCreateEvent()
        {
            // Arrange
            var eventDefinition = new EventDefinition
            {
                Name = "test_event_" + Guid.NewGuid().ToString("N"),
                Label = "Test Event",
                Labels = new SchemasLabelsModel { Singular = "Test Event" },
                Description = "Test event description",
                PrimaryObjectId = HubSpotObjectTypes.CONTACT,
                TrackingType = "MANUAL"
            };

            // Act
            var result = await CustomEventApi.CreateEventDefinitionAsync(eventDefinition);
            CustomEventsToCleanup.Add(result.Name);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Name.Should().Be(eventDefinition.Name);
                result.Labels.Singular.Should().Be(eventDefinition.Labels.Singular);
                result.Description.Should().Be(eventDefinition.Description);
                result.PrimaryObjectId.Should().Be(eventDefinition.PrimaryObjectId);
                result.TrackingType.Should().Be(eventDefinition.TrackingType);
                result.Archived.Should().BeFalse();
            }
        }

        [Fact]
        public async Task DeleteEventDefinitionAsync_WhenValidEvent_ShouldDeleteEvent()
        {
            // Arrange
            var eventDefinition = await CreateUniqueEventDefinitionAsync();

            // Act
            await CustomEventApi.DeleteEventDefinitionAsync(eventDefinition.Name);
            CustomEventsToCleanup.Remove(eventDefinition.Name);

            // Assert
            Func<Task> act = async () => await CustomEventApi.GetByNameAsync<EventDefinition>(eventDefinition.Name);
            await act.Should().ThrowAsync<HubSpotException>()
                .Where(e => e.ReturnedError.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteEventDefinitionAsync_WhenNonExistentEvent_ShouldThrowException()
        {
            // Arrange
            var nonExistentEventName = "test_event_" + Guid.NewGuid().ToString("N");

            // Act & Assert
            Func<Task> act = async () => await CustomEventApi.DeleteEventDefinitionAsync(nonExistentEventName);
            await act.Should().ThrowAsync<HubSpotException>()
                .Where(e => e.ReturnedError.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task SendEventTrackingDataForContact_WhenValidData_ShouldSucceedWithNoException()
        {
            var eventDefinition = await CreateUniqueEventDefinitionAsync();
            var contact = await RecreateTestContactAsync();

            var eventTracking = CreateTestEventTracking(contact.Email, eventDefinition.FullyQualifiedName);

            Func<Task> act = async () => await CustomEventApi.SendEventTrackingData(eventTracking);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task SendEventTrackingDataForContact_WhenInvalidEvent_ShouldThrowException()
        {
            var contact = await RecreateTestContactAsync();
            var eventTracking = CreateTestEventTracking(contact.Email, "nonexisting_event");

            Func<Task> act = async () => await CustomEventApi.SendEventTrackingData(eventTracking);

            await act.Should().ThrowAsync<HubSpotException>();
        }

        [Fact]
        public async Task SendEventTrackingDataForContact_WhenInvalidEmail_ShouldThrowException()
        {
            var eventDefinition = await CreateUniqueEventDefinitionAsync();

            var eventTracking = CreateTestEventTracking("invalid_email", eventDefinition.FullyQualifiedName);

            Func<Task> act = async () => await CustomEventApi.SendEventTrackingData(eventTracking);

            await act.Should().ThrowAsync<HubSpotException>();
        }

        [Fact]
        public async Task GetByNameAsync_WhenValidEventName_ShouldReturnEvent()
        {
            var eventDefinition = await CreateUniqueEventDefinitionAsync();

            var result = await CustomEventApi.GetByNameAsync<EventDefinition>(eventDefinition.Name);

            var label = "Test Event";
            result.Should().BeEquivalentTo(new EventDefinition
            {
                Name = eventDefinition.Name,
                Labels = new SchemasLabelsModel
                {
                    Singular = label,
                    Plural = label
                }
            }, options =>
            options
                .Excluding(e => e.Description)
                .Excluding(e => e.FullyQualifiedName)
                .Excluding(e => e.Id)
                .Excluding(e => e.CreatedAt)
                .Excluding(e => e.UpdatedAt)
                .Excluding(e => e.Archived)
                .Excluding(e => e.TrackingType)
                .Excluding(e => e.PrimaryObjectId));
        }

        [Fact]
        public async Task SendEventTrackingDataForCompany_WhenValidData_ShouldSucceedWithNoException()
        {
            var company = await RecreateTestCompanyAsync();
            var eventDefinition = await CreateUniqueEventDefinitionAsync(HubSpotObjectTypes.COMPANY);

            var eventTracking = CreateTestEventTracking(company.Id.Value, eventDefinition.FullyQualifiedName);

            Func<Task> act = async () => await CustomEventApi.SendEventTrackingData(eventTracking);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task SendEventTrackingDataForCompany_WhenInvalidObjectId_ShouldNotThrowException()
        {
            long randomNonExistingCompanyId = 10000234;
            var eventDefinition = await CreateUniqueEventDefinitionAsync(HubSpotObjectTypes.COMPANY);

            var eventTracking = CreateTestEventTracking(randomNonExistingCompanyId, eventDefinition.FullyQualifiedName);

            Func<Task> act = async () => await CustomEventApi.SendEventTrackingData(eventTracking);

            await act.Should().NotThrowAsync();
        }

        private EventTracking CreateTestEventTracking(string email, string eventName)
        {
            return new EventTracking
            {
                EventName = eventName,
                OccurredAt = DateTime.UtcNow.ToString("o"),
                Email = email
            };
        }

        private EventTracking CreateTestEventTracking(long id, string eventName)
        {
            return new EventTracking
            {
                EventName = eventName,
                OccurredAt = DateTime.UtcNow.ToString("o"),
                ObjectId = id.ToString()
            };
        }
    }
}
