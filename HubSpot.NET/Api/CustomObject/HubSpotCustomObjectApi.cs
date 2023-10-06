using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Extensions;
using HubSpot.NET.Core.Interfaces;
using Newtonsoft.Json;
using RestSharp;

namespace HubSpot.NET.Api.CustomObject;

public class CustomObjectListHubSpotModel<T> : IHubSpotModel where T: CustomObjectHubSpotModel, new()
{
    [DataMember(Name = "results")]
    public IList<T> Results { get; set; } = new List<T>();
    public bool IsNameValue => false;        
    
    public string RouteBasePath => "crm/v3/objects";
    public virtual void ToHubSpotDataEntity(ref dynamic converted)
    {
    }

    public virtual void FromHubSpotDataEntity(dynamic hubspotData)
    {
    }
}

[DataContract]
public class CreateCustomObjectHubSpotModel : IHubSpotModel
{
    [IgnoreDataMember]
    public string SchemaId { get; set; }


    [JsonProperty(PropertyName = "properties")]
    public Dictionary<string, object> Properties { get; set; } = new();

    public bool IsNameValue => false;
    public void ToHubSpotDataEntity(ref dynamic dataEntity)
    {
    }

    public void FromHubSpotDataEntity(dynamic hubspotData)
    {
    }

    public string RouteBasePath => "crm/v3/objects";
    [JsonProperty(PropertyName = "associations")]
    public List<Association> Associations { get; set; } = new();

    public class Association
    {
        public To To { get; set; }
        public List<TypeElement> Types { get; set; }
    }

    public class To
    {
        public string Id { get; set; }
    }

    public class TypeElement
    {
        // either HUBSPOT_DEFINED, USER_DEFINED, INTEGRATOR_DEFINED
        public string AssociationCategory { get; set; }
        public long? AssociationTypeId { get; set; }
    }
}

[DataContract]
public class UpdateCustomObjectHubSpotModel : IHubSpotModel
{
    [DataMember(Name = "id")]
    public string Id { get; set; }
    
    [IgnoreDataMember]
    public string SchemaId { get; set; }


    [JsonProperty(PropertyName = "properties")]
    public Dictionary<string, object> Properties { get; set; } = new();

    public bool IsNameValue => false;
    public void ToHubSpotDataEntity(ref dynamic dataEntity)
    {
    }

    public void FromHubSpotDataEntity(dynamic hubspotData)
    {
    }

    public string RouteBasePath => "crm/v3/objects";
}

[DataContract]
public class CustomObjectHubSpotModel : IHubSpotModel
{

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "createdAt")]
    public DateTime? CreatedAt { get; set; }

    [DataMember(Name = "updatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [IgnoreDataMember]
    [JsonProperty(PropertyName = "properties")]
    public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    public bool IsNameValue => false;
    public void ToHubSpotDataEntity(ref dynamic dataEntity)
    {
    }

    public void FromHubSpotDataEntity(dynamic hubspotData)
    {
    }

    public string RouteBasePath => "crm/v3/objects";
}

public class CustomObjectListAssociationsModel<T> : IHubSpotModel where T : CustomObjectAssociationModel, new()
{
    [DataMember(Name = "results")]
    public IList<T> Results { get; set; } = new List<T>();
    public bool IsNameValue => false;        
        
    public string RouteBasePath => "crm/v3/objects";
    public virtual void ToHubSpotDataEntity(ref dynamic converted)
    {
    }

    public virtual void FromHubSpotDataEntity(dynamic hubspotData)
    {
    }
}


public class CustomObjectAssociationModel
{
    public string Id { get; set; }
    public string Type { get; set; }
}


public class HubSpotCustomObjectApi : IHubSpotCustomObjectApi
{
    private readonly IHubSpotClient _client;

    private readonly string RouteBasePath = "crm/v3/objects";
    private readonly IHubSpotAssociationsApi _hubSpotAssociationsApi;

    public HubSpotCustomObjectApi(IHubSpotClient client, IHubSpotAssociationsApi hubSpotAssociationsApi)
    {
        _client = client;
        _hubSpotAssociationsApi = hubSpotAssociationsApi;
    }
    
    /// <summary>
    /// List all objects of a custom object type in your system
    /// </summary>
    /// <param name="idForCustomObject">Should be prefaced with "2-"</param>
    /// <param name="opts"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public CustomObjectListHubSpotModel<T> List<T>(string idForCustomObject, ListRequestOptions opts = null) where T : CustomObjectHubSpotModel, new()
    {
        opts ??= new ListRequestOptions();

        var path = $"{RouteBasePath}/{idForCustomObject}"
            .SetQueryParam("count", opts.Limit);

        if (opts.PropertiesToInclude.Any())
            path = path.SetQueryParam("property", opts.PropertiesToInclude);

        if (opts.Offset.HasValue)
            path = path.SetQueryParam("vidOffset", opts.Offset);

        var response = _client.ExecuteList<CustomObjectListHubSpotModel<T>>(path, convertToPropertiesSchema: false);
        return response;
    }

    /// <summary>
    /// Get the list of associations between two objects (BOTH CUSTOM and NOT) 
    /// </summary>
    /// <param name="objectTypeId"></param>
    /// <param name="customObjectId"></param>
    /// <param name="idForDesiredAssociation"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public CustomObjectListAssociationsModel<T> GetAssociationsToCustomObject<T>(string objectTypeId,
        string customObjectId,
        string idForDesiredAssociation, CancellationToken cancellationToken) where T : CustomObjectAssociationModel, new()
    {
        var path = $"{RouteBasePath}/{objectTypeId}/{customObjectId}/associations/{idForDesiredAssociation}";

        var response = _client.ExecuteList<CustomObjectListAssociationsModel<T>>(path, convertToPropertiesSchema:  false);
        return response;
    }


    /// <summary>
    /// Adds the ability to create a custom object inside hubspot
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="associateObjectType"></param>
    /// <param name="associateToObjectId"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public string CreateWithDefaultAssociationToObject<T>(T entity, string associateObjectType, string associateToObjectId) where T : CreateCustomObjectHubSpotModel, new()
    {
        var path = $"{RouteBasePath}/{entity.SchemaId}";

        var response =
            _client.Execute<CreateCustomObjectHubSpotModel>(path, entity, Method.POST, convertToPropertiesSchema: false);

        
        if (response.Properties.TryGetValue("hs_object_id", out var parsedId))
        {
            _hubSpotAssociationsApi.AssociationToObject(entity.SchemaId, parsedId.ToString(), associateObjectType, associateToObjectId);
            return parsedId.ToString();
        }
        return string.Empty;
    }
    
    /// <summary>
    /// Update a custom object inside hubspot
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public string UpdateObject<T>(T entity) where T : UpdateCustomObjectHubSpotModel, new()
    {
        var path = $"{RouteBasePath}/{entity.SchemaId}/{entity.Id}";

        _client.Execute<UpdateCustomObjectHubSpotModel>(path, entity, Method.PATCH, convertToPropertiesSchema: false);
        
        return string.Empty;
    }

    const string allEquipmentObjectProperties = "additional_information,backoffice_id,bb_eng_legacy_equipmentcategory_slug,category,city,commission_percent,date_of_first_qualified_lead,equipment_title,fleet_identifier,hs_all_accessible_team_ids,hs_all_assigned_business_unit_ids,hs_all_owner_ids,hs_all_team_ids,hs_created_by_user_id,hs_createdate,hs_lastmodifieddate,hs_merged_object_ids,hs_object_id,hs_object_source,hs_object_source_id,hs_object_source_user_id,hs_pinned_engagement_id,hs_read_only,hs_unique_creation_key,hs_updated_by_user_id,hs_user_ids_of_all_notification_followers,hs_user_ids_of_all_notification_unfollowers,hs_user_ids_of_all_owners,hs_was_imported,hubspot_owner_assigneddate,hubspot_owner_id,hubspot_team_id,item_number,karintest,line_item_id,listing_to_lead_time,location,location_combined,machine_posted_datestamp,make,make__other,new_associated_deal,photo,poc_email,product_id,qualified_to_buy_datestamp,seller_company_id,seller_company_name,seller_contact_id,seller_deal_id,state,submission_idempotent_id,time_between_posted_and_first_buyer,warranty_eligible,year,year1,zip_code,name,model,vin,hoursmileage";
    public T GetEquipmentDataById<T>(string schemaId, string entityId, string properties=allEquipmentObjectProperties) where T : GetHubspotEquipmentObjectModel, new()
    {
        var path = $"{RouteBasePath}/{schemaId}/{entityId}";

        path = path.SetQueryParam("properties", properties); //properties is comma seperated value of properties to include

        var res = _client.Execute<T>(path, Method.GET, convertToPropertiesSchema: true);

        return res;
    }
}