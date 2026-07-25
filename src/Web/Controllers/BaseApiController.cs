

namespace Web.Controllers;
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class BaseApiController : ControllerBase
{
    protected IMediator Mediator =>
        _mediator ??=
            HttpContext.RequestServices.GetService<IMediator>()
            ?? throw new InvalidOperationException("IMediator cannot be retrieved from request services.");

    private IMediator? _mediator;

    #region [ApiController] attributes kullanıldığı için kapatılmıştır. Eğer ApiController attribute'u kaldırılırsa, aşağıdaki kod bloğu açılmalıdır.
    /*
    protected List<string> CheckModelState()
    {
        var errors = ModelState
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage)
            .Where(y => y.Length != 0)
            .ToList();
        return errors;
    }
    */
    #endregion
}