using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Interfaces;
using RabbitMQ.Models.Messages;

namespace RabbitMQ.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProducerController : ControllerBase
    {
        private readonly IProducerService _producerService;

        public ProducerController(IProducerService producerService)
        {
            _producerService = producerService;
        }

        [HttpPost]
        public async ValueTask<IActionResult> PostAsync(Message message)
        {
            _producerService.Send(message);
            return Ok();
        }
    }
}