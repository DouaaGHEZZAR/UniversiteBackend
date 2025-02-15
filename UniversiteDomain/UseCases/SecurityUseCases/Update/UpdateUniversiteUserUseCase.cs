using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.SecurityExceptions;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.SecurityUseCases.Update;

public class UpdateUniversiteUserUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<IUniversiteUser> ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules(etudiant);

        // 🔥 Correction : Recherche de l'utilisateur par `EtudiantId`
        IUniversiteUser? user = await repositoryFactory.UniversiteUserRepository().FindByEmailAsync(etudiant.Email);
        if (user == null) throw new UniversiteUserNotFoundException($"Aucun utilisateur trouvé pour l'étudiant ID : {etudiant.Id}");

        // ✅ Mise à jour des informations du user
        user.Etudiant = etudiant; 
        
        // 🔥 Correction : UpdateAsync prend 1 seul argument
        await repositoryFactory.UniversiteUserRepository().UpdateAsync(user);
        await repositoryFactory.SaveChangesAsync();

        return user;
    }

    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.Email);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        IUniversiteUserRepository userRepository = repositoryFactory.UniversiteUserRepository();

        // Vérification si un autre utilisateur a déjà cet email
        IUniversiteUser? existingUser = await userRepository.FindByEmailAsync(etudiant.Email);
        if (existingUser != null && existingUser.Etudiant?.Id != etudiant.Id)
        {
            throw new DuplicateUserEmailException($"L'email {etudiant.Email} est déjà utilisé par un autre utilisateur.");
        }
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}