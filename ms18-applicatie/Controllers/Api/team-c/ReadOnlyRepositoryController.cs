using Maasgroep.Database;
using Maasgroep.Exceptions;
using Maasgroep.SharedKernel.Interfaces;
using Maasgroep.SharedKernel.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Maasgroep.Controllers.Api;

[Route("api/v1/[controller]")]
[ApiController]
public abstract class ReadOnlyRepositoryController<TRepository, TRecord, TViewModel> : ControllerBase
where TRepository : IReadOnlyRepository<TRecord, TViewModel>
where TRecord: GenericRecordActive
{
	protected readonly TRepository Repository;
    public virtual MemberModel? CurrentMember { get => HttpContext.Items["CurrentUser"] as MemberModel; }
        
    protected virtual bool AllowList()
        => true; // By default, everyone is allowed to list all items
    
    protected virtual bool AllowView(TRecord record)
        => true; // By default, everyone can view every single item

    public virtual string ItemName { get => "Item"; }

	public ReadOnlyRepositoryController(TRepository repository) 
	    => Repository = repository;

    [HttpGet]
    public IActionResult RepositoryGet([FromQuery] int offset = default, [FromQuery] int limit = default)
    {
        if (!AllowList())
            NoAccess();
        return Ok(Repository.ListAll(offset, limit));
    }
    
    [HttpGet("{id}")]
    public IActionResult RepositoryGetById(long id) {
        var record = Repository.GetById(id) ?? throw new MaasgroepNotFound($"{ItemName} niet gevonden");
        if (!AllowView(record))
            NoAccess();
        return Ok(Repository.GetModel(record));
    }

    protected void NoAccess()
    {
        // Throw an Unauthorized or Forbidden error, depending on login state
        if (CurrentMember == null)
            throw new MaasgroepUnauthorized();
        else
            throw new MaasgroepForbidden();
    }
}