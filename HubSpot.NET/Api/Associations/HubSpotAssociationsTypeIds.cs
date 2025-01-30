namespace HubSpot.NET.Api.Associations;


public static class HubSpotAssociationsTypeIds
{
    public static class CompanyAssociationsTypeIds
    {
        public static int CompanyToPrimaryContact = 2;
        public static int CompanyToContact = 280;
    }
}

public static class HubSpotAssociationsTypeIdsExtensions
{
    public static bool IsPrimaryContactForCompany(this int associationsTypeId)
    {
        return associationsTypeId == HubSpotAssociationsTypeIds.CompanyAssociationsTypeIds.CompanyToPrimaryContact;
    }

    public static bool IsContactForCompany(this int associationsTypeId)
    {
        return associationsTypeId == HubSpotAssociationsTypeIds.CompanyAssociationsTypeIds.CompanyToContact;
    }
}