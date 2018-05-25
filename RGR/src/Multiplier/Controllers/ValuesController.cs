using Microsoft.AspNetCore.Mvc;
using Multiplier.Dto;

namespace Multiplier.Controllers
{
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		[HttpPost]
		public double Post([FromBody] DataDto value)
		{
			return value.operand1 * value.operand2;
		}
	}
}