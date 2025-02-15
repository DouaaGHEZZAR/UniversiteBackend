using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursException;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(long Id, string NomParcours, int AnneeFormaion)
    {
        var parcours = new Parcours{Id = Id, NomParcours = NomParcours, AnneeFormation = AnneeFormaion};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours pr = await repositoryFactory.ParcoursRepository().CreateAsync(parcours);
        repositoryFactory.ParcoursRepository().SaveChangesAsync().Wait();
        return pr;
    }
    
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.Id);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcours.AnneeFormation);
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        
        // On recherche un parcours avec le même Id parcours
        List<Parcours> existe = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p=>p.Id.Equals(parcours.Id));

        // Si un parcours avec le même Id existe déjà, on lève une exception personnalisée
        if (existe is {Count:>0}) throw new DuplicateIdException(parcours.Id+ " - cet Id est déjà affecté à un parcours");
        
        
        // On vérifie si l'email est déjà utilisé
        existe = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p=>p.NomParcours.Equals(parcours.NomParcours));
        // Une autre façon de tester la vacuité de la liste
        if (existe is {Count:>0}) throw new DuplicateNomParcoursException(parcours.NomParcours +" est déjà affecté à un parcours");
        
        
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}