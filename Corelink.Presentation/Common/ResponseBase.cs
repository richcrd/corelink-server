using Corelink.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace corelink_server.Common;

public abstract class ResponseBase : ControllerBase
{
    protected ActionResult HandleResponse<T>(Answer<T> answer)
        => StatusCode(answer.StatusCode, answer);

    protected ActionResult HandleResponse(AnswerBase answer)
        => StatusCode(answer.StatusCode, answer);
}