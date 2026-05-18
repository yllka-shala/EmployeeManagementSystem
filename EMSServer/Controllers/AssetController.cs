using EMSBaseLibrary.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EMSServer.Repositories.Interfaces;

namespace EMSServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController(IGenericRepositoryInterface<Asset> genericRepository) : GenericController<Asset>(genericRepository)
    {
    }
}
