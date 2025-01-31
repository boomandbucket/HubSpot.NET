namespace HubSpot.NET.Api.Associations;


public static class HubSpotAssociationsTypeIds
{
    public static class CompanyAssociationsTypeIds
    {
        public static long CompanyToPrimaryContact = 2;
        public static long CompanyToContact = 280;
    }
}

public static class HubSpotAssociationsTypeIdsExtensions
{
    public static bool IsPrimaryContactForCompany(this long associationsTypeId)
    {
        return associationsTypeId == HubSpotAssociationsTypeIds.CompanyAssociationsTypeIds.CompanyToPrimaryContact;
    }

    public static bool IsContactForCompany(this long associationsTypeId)
    {
        return associationsTypeId == HubSpotAssociationsTypeIds.CompanyAssociationsTypeIds.CompanyToContact;
    }
}