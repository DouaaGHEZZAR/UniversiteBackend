using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetEtudiantByIdUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Etudiant?> ExecuteAsync(long idEtudiant)
    {
        await CheckBusinessRules();
        Etudiant? etudiant = await repositoryFactory.EtudiantRepository().FindAsync(idEtudiant);

        if (etudiant == null)
        {
            throw new EtudiantNotFoundException($"Aucun étudiant trouvé avec l'ID {idEtudiant}");
        }

        return etudiant;
    }

    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        IEtudiantRepository etudiantRepository = repositoryFactory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
    }

    public bool IsAuthorized(string role, IUniversiteUser user, long idEtudiant)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        // Si c'est un étudiant, il ne peut voir que ses propres informations
        return user.Etudiant != null && role.Equals(Roles.Etudiant) && user.Etudiant.Id == idEtudiant;
    }
}