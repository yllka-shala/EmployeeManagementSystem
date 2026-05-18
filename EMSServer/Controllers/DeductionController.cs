using EMSBaseLibrary.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EMSServer.Repositories.Interfaces;

namespace EMSServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeductionController(IGenericRepositoryInterface<Deduction> genericRepository) : GenericController<Deduction>(genericRepository)
    {
    }
}
