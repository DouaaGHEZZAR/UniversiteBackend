using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursException;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IParcoursRepository parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(long Id, string NomParcours, int AnneeFormaion)
    {
        var parcours = new Parcours{Id = Id, NomParcours = NomParcours, AnneeFormation = AnneeFormaion};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours pr = await parcoursRepository.CreateAsync(parcours);
        parcoursRepository.SaveChangesAsync().Wait();
        return pr;
    }
    
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.Id);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcours.AnneeFormation);
        ArgumentNullException.ThrowIfNull(parcoursRepository);
        
        // On recherche un parcours avec le même Id parcours
        List<Parcours> existe = await parcoursRepository.FindByConditionAsync(p=>p.Id.Equals(parcours.Id));

        // Si un parcours avec le même Id existe déjà, on lève une exception personnalisée
        if (existe .Any()) throw new DuplicateIdException(parcours.Id+ " - cet Id est déjà affecté à un parcours");
        
        
        // On vérifie si l'email est déjà utilisé
        existe = await parcoursRepository.FindByConditionAsync(p=>p.NomParcours.Equals(parcours.NomParcours));
        // Une autre façon de tester la vacuité de la liste
        if (existe is {Count:>0}) throw new DuplicateNomParcoursException(parcours.NomParcours +" est déjà affecté à un parcours");
        
        
    }
}