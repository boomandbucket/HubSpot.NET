﻿namespace HubSpot.NET.Core.Interfaces
{
    using Api.Contact.Dto;
    using System.Collections.Generic;

    public interface IHubSpotContactPropertyApi
    {
        ContactPropertyModel CreateProperty(ContactPropertyModel entity);
        List<ContactPropertyModel> GetProperties();
        ContactPropertyModel GetProperty(string propertyName);
        ContactPropertyModel UpdateProperty(ContactPropertyModel model);
    }
}
