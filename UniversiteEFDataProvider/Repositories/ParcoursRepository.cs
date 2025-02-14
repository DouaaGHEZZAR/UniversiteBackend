using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using Microsoft.EntityFrameworkCore;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        return await AddEtudiantAsync(await Context.Parcours!.FindAsync(idParcours), await Context.Etudiants!.FindAsync(idEtudiant));
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        if (parcours == null || etudiant == null)
            throw new ArgumentNullException($"Parcours ou étudiant introuvable : {parcours?.Id}, {etudiant?.Id}");

        parcours.Inscrits ??= new List<Etudiant>();
        parcours.Inscrits.Add(etudiant);
        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
    {
        if (parcours == null || etudiants.Count == 0)
            throw new ArgumentNullException($"Parcours ou liste d'étudiants introuvable.");

        parcours.Inscrits ??= new List<Etudiant>();
        parcours.Inscrits.AddRange(etudiants);
        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        var parcours = await Context.Parcours!.Include(p => p.Inscrits).FirstOrDefaultAsync(p => p.Id == idParcours);
        var etudiants = await Context.Etudiants!.Where(e => idEtudiants.Contains(e.Id)).ToListAsync();

        if (parcours == null || etudiants.Count == 0)
            throw new ArgumentNullException($"Parcours ou étudiants introuvables : {idParcours}");

        parcours.Inscrits ??= new List<Etudiant>();
        parcours.Inscrits.AddRange(etudiants);
        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        return await AddUeAsync(await Context.Parcours!.FindAsync(idParcours), await Context.Ues!.FindAsync(idUe));
    }

    public async Task<Parcours> AddUeAsync(Parcours? parcours, Ue? ue)
    {
        if (parcours == null || ue == null)
            throw new ArgumentNullException($"Parcours ou UE introuvable.");

        parcours.UesEnseignees ??= new List<Ue>();
        parcours.UesEnseignees.Add(ue);
        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
    {
        var parcours = await Context.Parcours!.Include(p => p.UesEnseignees).FirstOrDefaultAsync(p => p.Id == idParcours);
        var ues = await Context.Ues!.Where(u => idUes.Contains(u.Id)).ToListAsync();

        if (parcours == null || ues.Count == 0)
            throw new ArgumentNullException($"Parcours ou UEs introuvables : {idParcours}");

        parcours.UesEnseignees ??= new List<Ue>();
        parcours.UesEnseignees.AddRange(ues);
        await Context.SaveChangesAsync();
        return parcours;
    }
}