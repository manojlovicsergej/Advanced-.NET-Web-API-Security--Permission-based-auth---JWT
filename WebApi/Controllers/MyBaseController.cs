using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
public class MyBaseController<T> : ControllerBase
{
    private ISender _sender  { get; set; }

    public ISender MediatorSender => _sender ??= HttpContext.RequestServices.GetService<ISender>();
}