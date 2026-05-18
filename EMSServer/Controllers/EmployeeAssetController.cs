using EMSBaseLibrary.Entities;
using EMSServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMSServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAssetController(IGenericRepositoryInterface<EmployeeAsset> genericRepository) : GenericController<EmployeeAsset>(genericRepository)
    {
    }
}
