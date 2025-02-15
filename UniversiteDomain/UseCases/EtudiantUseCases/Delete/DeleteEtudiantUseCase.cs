using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(long idEtudiant)
    {
        // Vérification que l'ID est valide
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idEtudiant);

        // Vérification que l'étudiant existe
        var etudiant = await repositoryFactory.EtudiantRepository().FindAsync(idEtudiant);
        if (etudiant == null)
        {
            throw new EtudiantNotFoundException($"L'étudiant avec l'ID {idEtudiant} n'existe pas.");
        }

        // Suppression de l'étudiant
        await repositoryFactory.EtudiantRepository().DeleteAsync(idEtudiant);
        await repositoryFactory.SaveChangesAsync();
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}