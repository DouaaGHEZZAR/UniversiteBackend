namespace UniversiteDomain.Entities;

public class Note
{
    public float Valeur { get; set; } // Valeur de la note (entre 0 et 20)

    // ManyToOne : une note appartient à un étudiant
    public long EtudiantId { get; set; } // Clé étrangère vers l'étudiant
    public Etudiant? Etudiant { get; set; } // Navigation vers l'étudiant

    // ManyToOne : une note appartient à une UE
    public long UeId { get; set; } // Clé étrangère vers l'UE
    public Ue? Ue { get; set; } // Navigation vers l'UE

    public override string ToString()
    {
        return $"Note : Valeur = {Valeur}, Etudiant = {Etudiant?.Nom} {Etudiant?.Prenom}, UE = {Ue?.Intitule}";
    }
}