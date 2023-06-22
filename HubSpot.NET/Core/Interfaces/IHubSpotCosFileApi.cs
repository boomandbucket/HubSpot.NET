using System.Threading.Tasks;
using HubSpot.NET.Api.Files.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotCosFileApi
    {
        FolderHubSpotModel CreateFolder(FolderHubSpotModel folder);
        Task<FileHubSpotResponseModel> UploadFile(FileHubSpotRequestModel entity);
    }
}