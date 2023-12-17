using Maasgroep.Database;
using Maasgroep.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Maasgroep.Controllers.Api;

public abstract class WritableRepositoryController<TRepository, TRecord, TViewModel, TDataModel> : ReadOnlyRepositoryController<TRepository, TRecord, TViewModel>
where TRepository : IWritableRepository<TRecord, TViewModel, TDataModel>
where TRecord: GenericRecordActive
{
	public WritableRepositoryController(TRepository repository) : base(repository) {}

    public virtual long CurrentMemberId { get => 1; } ///// TODO: ergens de ingelogde member vandaan halen
    
    [HttpPost]
    public IActionResult RepositoryCreate([FromBody] TDataModel data)
    {
        var record = Repository.Create(data, CurrentMemberId) ?? throw new Exceptions.MaasgroepBadRequest($"{ItemName} kon niet worden aangemaakt");
        return Created($"/api/v1/{RouteData.Values["controller"]}/{record.Id}", Repository.GetModel(record));
    }
}