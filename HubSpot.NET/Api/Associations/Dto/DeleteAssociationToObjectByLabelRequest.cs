using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpot.NET.Api.Associations.Dto
{
    internal class DeleteAssociationToObjectByLabelRequest
    {
        [JsonProperty(PropertyName = "inputs")]
        internal List<DeleteRequestInput> Inputs { get; set; }

        internal DeleteAssociationToObjectByLabelRequest(string objectId, string toObjectId, (string associationCategory, int associationTypeId)[] associations)
        {
            Inputs =
            [
                new DeleteRequestInput
                {
                    From = new DeleteRequestObjectReference() { Id = long.Parse(objectId) },
                    To = new DeleteRequestObjectReference() { Id = long.Parse(toObjectId) },
                    Types = []
                }
            ];

            foreach (var association in associations)
            {
                Inputs[0].Types.Add(
                    new DeleteRequestType()
                    {
                        AssociationCategory = association.associationCategory,
                        AssociationTypeId = association.associationTypeId
                    });
            }
        }
    }

    internal class DeleteRequestInput
    {
        [JsonProperty(PropertyName = "types")]
        internal List<DeleteRequestType> Types { get; set; }

        [JsonProperty(PropertyName = "from")]
        internal DeleteRequestObjectReference From { get; set; }

        [JsonProperty(PropertyName = "to")]
        internal DeleteRequestObjectReference To { get; set; }
    }

    internal class DeleteRequestType
    {
        [JsonProperty(PropertyName = "associationCategory")]
        internal string AssociationCategory { get; set; }

        [JsonProperty(PropertyName = "associationTypeId")]
        internal int AssociationTypeId { get; set; }
    }

    internal class DeleteRequestObjectReference
    {
        [JsonProperty(PropertyName = "id")]
        internal long Id { get; set; }
    }
}
