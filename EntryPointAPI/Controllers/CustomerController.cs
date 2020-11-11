using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnboardingMessages;
using Rebus.Bus;

namespace EntryPointAPI.Controllers
{
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IBus _bus;

        public CustomerController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost]
        [Route("newcustomer")]
        public async Task<IActionResult> NewCustomer(string name, string email)
        {
            await _bus.Send(new OnboardNewCustomer { Name = name, Email = email });
            return Ok();
        }
    }
}