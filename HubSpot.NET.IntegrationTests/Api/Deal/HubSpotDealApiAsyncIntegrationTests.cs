﻿using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Deal;
using HubSpot.NET.Api.Deal.Dto;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.Deal;

public sealed class HubSpotDealApiAsyncIntegrationTests : HubSpotAsyncIntegrationTestBase
{
    [Fact]
    public async Task CreateDeal()
    {
        var (newDeal, createdDeal) = await PrepareDeal();
        using (new AssertionScope())
        {
            createdDeal.Id.Should().NotBeNull();
            createdDeal.Name.Should().Be(newDeal.Name);
            DealApi.GetByIdAsync<DealHubSpotModel>(createdDeal.Id.Value).Should().NotBeNull();
        }

        await DealApi.DeleteAsync(createdDeal.Id ?? 0);
    }

    [Fact]
    public async Task CreateDeal_WithAssociation_ShouldBeSuccessful()
    {
        var createdCompany = await RecreateTestCompanyAsync("My Test Company");
        var createdContact = await RecreateTestContactAsync(email: "TestContact@test.tt");

        var newDeal = new DealHubSpotModel
        {
            Name = Guid.NewGuid().ToString(),
            Associations =
            {
                AssociatedCompany = new[] { createdCompany.Id.Value },
                AssociatedContacts = new[] { createdContact.Id.Value }
            }
        };
        var createdDeal = await DealApi.CreateAsync(newDeal);

        await Task.Delay(10000);

        var retrievedDeal = await DealApi.GetByIdAsync<DealHubSpotModel>(createdDeal.Id.Value);

        using (new AssertionScope())
        {
            createdDeal.Associations.AssociatedCompany[0].Should().Be(createdCompany.Id.Value);
            createdDeal.Associations.AssociatedContacts[0].Should().Be(createdContact.Id.Value);
            retrievedDeal.Associations.AssociatedCompany[0].Should().Be(createdCompany.Id.Value);
            retrievedDeal.Associations.AssociatedContacts[0].Should().Be(createdContact.Id.Value);
        }

        await DealApi.DeleteAsync(createdDeal.Id ?? 0);
    }

    [Fact]
    public async Task GetDealById()
    {
        var (_, createdDeal) = await PrepareDeal();

        (await DealApi.GetByIdAsync<DealHubSpotModel>(createdDeal.Id.Value)).Should().NotBeNull();

        await DealApi.DeleteAsync(createdDeal.Id ?? 0);
    }

    [Fact]
    public async Task DeleteDeal()
    {
        var (_, createdDeal) = await PrepareDeal();
        await DealApi.DeleteAsync(createdDeal.Id ?? 0);
        await Task.Delay(5000);

        var act = async () => { await DealApi.GetByIdAsync<DealHubSpotModel>(createdDeal.Id.Value); };

        await act.Should().ThrowAsync<HubSpotException>()
            .WithMessage("*Deal does not exist*");
    }

    [Fact]
    public async Task ListDeals()
    {
        var createdDeals = new List<DealHubSpotModel>();

        var uniqueName1 = Guid.NewGuid().ToString("N");
        var uniqueName2 = Guid.NewGuid().ToString("N");

        var firstCreatedDeal = await CreateTestDeal($"Test Deal {uniqueName1}");
        createdDeals.Add(firstCreatedDeal);

        var secondCreatedDeal = await CreateTestDeal($"Test Deal {uniqueName2}");
        createdDeals.Add(secondCreatedDeal);

        var requestOptions = new DealListRequestOptions
        {
            PropertiesToInclude = new List<string> { "dealname" },
            Limit = 1
        };

        await Task.Delay(5000);

        var allDeals = await DealApi.ListAsync<DealHubSpotModel>(requestOptions);

        using (new AssertionScope())
        {
            allDeals.Should().NotBeNull();
            allDeals.Deals.Count.Should().Be(1);
            allDeals.Paging.Next.After.Should().NotBeEmpty();
            allDeals.Paging.Next.Link.Should().Contain("https://api.hubapi.com/crm/v3/objects/deals?");
        }

        foreach (var deal in createdDeals)
        {
            await DealApi.DeleteAsync(deal.Id.Value);
        }
    }

    [Fact]
    public async Task UpdateDeal()
    {
        var createdDeal = await CreateTestDeal();
        createdDeal.Name += "New Name";
        createdDeal.Amount += 1;

        var updatedDeal = await DealApi.UpdateAsync(createdDeal);
        ValidateDeal(updatedDeal);

        var retrievedDeal = await DealApi.GetByIdAsync<DealHubSpotModel>(createdDeal.Id.Value);
        ValidateDeal(retrievedDeal);

        await DealApi.DeleteAsync(createdDeal.Id.Value);

        void ValidateDeal(DealHubSpotModel dealToValidate)
        {
            using (new AssertionScope())
            {
                dealToValidate.Should().NotBeNull();
                dealToValidate.Id.Should().Be(createdDeal.Id);
                dealToValidate.Name.Should().Be(createdDeal.Name);
                dealToValidate.Amount.Should().Be(createdDeal.Amount);
            }
        }
    }

    [Fact]
    public async Task SearchDeals()
    {
        const double amount = 42;
        var createdDeal = await CreateTestDeal(amount: amount);

        await Task.Delay(10000);

        var filterGroup = new SearchRequestFilterGroup { Filters = new List<SearchRequestFilter>() };
        filterGroup.Filters.Add(new SearchRequestFilter
        {
            PropertyName = "dealname",
            Operator = SearchRequestFilterOperatorType.EqualTo,
            Value = createdDeal.Name
        });

        var searchOptions = new SearchRequestOptions
        {
            FilterGroups = new List<SearchRequestFilterGroup>(),
            PropertiesToInclude = new List<string>
            {
                "dealname", "DateCreated", "dealstage", "amount",
                "closedate", "owner", "associatedcompanyid", "associatedcontactids", "dealtype"
            }
        };

        searchOptions.FilterGroups.Add(filterGroup);

        var searchResults = await DealApi.SearchAsync<DealHubSpotModel>(searchOptions);

        using (new AssertionScope())
        {
            var foundDeal = searchResults.Results.FirstOrDefault(c => c.Id == createdDeal.Id);

            foundDeal.Should().NotBeNull("Expected to find the created deal in search results.");
            foundDeal.DateCreated.Should().NotBeNull("Expected CreatedAt to have a value.");
            foundDeal.Stage.Should().BeNull();
            foundDeal.OwnerId.Should().BeNull();
            foundDeal.DealType.Should().BeNull();
            foundDeal.Amount.Should().Be(amount);
            foundDeal.Name.Should().Be(createdDeal.Name);
        }

        await DealApi.DeleteAsync(createdDeal.Id.Value);
    }

    private async Task<(DealHubSpotModel NewDeal, DealHubSpotModel CreatedDeal)> PrepareDeal()
    {
        var newDeal = new DealHubSpotModel
        {
            Name = Guid.NewGuid().ToString()
        };
        var createdDeal = await DealApi.CreateAsync(newDeal);
        await Task.Delay(5000);
        return (newDeal, createdDeal);
    }

    private Task<DealHubSpotModel> CreateTestDeal(string? dealName = null, double? amount = null)
    {
        dealName ??= "Unique Deal Name " + Guid.NewGuid().ToString("N");
        var newDeal = new DealHubSpotModel { Name = dealName, Amount = amount };
        return DealApi.CreateAsync(newDeal);
    }
}