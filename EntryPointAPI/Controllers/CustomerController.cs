using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EntryPointAPI.Controllers
{
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("newcustomer")]
        public IActionResult NewCustomer()
        {
            return Ok();
        }
    }
}