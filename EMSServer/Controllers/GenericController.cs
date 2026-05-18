using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EMSServer.Repositories.Interfaces;

namespace EMSServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T>(IGenericRepositoryInterface<T> genericRepository) : Controller where T : class
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await genericRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("single/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Request Sent");

            var result = await genericRepository.GetById(id);
            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(T model)
        {
            if (model is null)
                return BadRequest("Bad Request Made!");

            var result = await genericRepository.Insert(model);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(T model)
        {
            if (model is null)
                return BadRequest("Bad Request Made");

            var result = await genericRepository.Update(model);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if(id <= 0)
                return BadRequest("Invalid Request Sent!");

            var result = await genericRepository.DeleteById(id);
            return Ok(result);
        }
    }
}
