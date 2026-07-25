

using Application.Features.SampleEntities.Commands.CreateSampleEntity;
using Application.Features.SampleEntities.Queries.GetSampleEntitiesByNumber;

namespace Web.Controllers.V1;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SampleEntityController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<GetSampleEntitiesByNumberResponse>), StatusCodes.Status200OK)]
    public async Task<IResult> GetSampleEntitiesByNumber([FromQuery] GetSampleEntitiesByNumberQuery query, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateSampleEntityResponse), StatusCodes.Status200OK)]
    public async Task<IResult> CreateSampleEntity([FromBody] CreateSampleEntityCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return result.Match(Results.Created, CustomResults.Problem);
    }
}