using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Update;

public class UpdateEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules(etudiant);
        
        Etudiant? existingEtudiant = await repositoryFactory.EtudiantRepository().FindAsync(etudiant.Id);
        if (existingEtudiant == null) throw new EtudiantNotFoundException($"Étudiant avec ID {etudiant.Id} introuvable.");

        // Mise à jour des informations
        existingEtudiant.NumEtud = etudiant.NumEtud;
        existingEtudiant.Nom = etudiant.Nom;
        existingEtudiant.Prenom = etudiant.Prenom;
        existingEtudiant.Email = etudiant.Email;

        await repositoryFactory.EtudiantRepository().UpdateAsync(existingEtudiant);
        await repositoryFactory.SaveChangesAsync();
        
        return existingEtudiant;
    }
    
    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.NumEtud);
        ArgumentNullException.ThrowIfNull(etudiant.Email);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        var etudiantRepository = repositoryFactory.EtudiantRepository();

        // Vérifier l'existence d'un autre étudiant avec le même email
        List<Etudiant> emailConflict = await etudiantRepository.FindByConditionAsync(e => e.Email == etudiant.Email && e.Id != etudiant.Id);
        if (emailConflict.Any()) throw new DuplicateEmailException($"L'email {etudiant.Email} est déjà utilisé par un autre étudiant.");

        // Vérifier l'existence d'un autre étudiant avec le même NumEtud
        List<Etudiant> numEtudConflict = await etudiantRepository.FindByConditionAsync(e => e.NumEtud == etudiant.NumEtud && e.Id != etudiant.Id);
        if (numEtudConflict.Any()) throw new DuplicateNumEtudException($"Le numéro étudiant {etudiant.NumEtud} est déjà utilisé par un autre étudiant.");
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
