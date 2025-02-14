using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using Microsoft.EntityFrameworkCore;

namespace UniversiteEFDataProvider.Repositories;

public class UeRepository(UniversiteDbContext context) : Repository<Ue>(context), IUeRepository
{
    public async Task<Ue> CreateAsync(Ue ue)
    {
        if (ue == null)
            throw new ArgumentNullException(nameof(ue), "L'UE ne peut pas être null.");

        // Vérification si l'UE avec le même numéro existe déjà
        var existingUe = await Context.Ues!.FirstOrDefaultAsync(u => u.NumeroUe == ue.NumeroUe);
        if (existingUe != null)
            throw new InvalidOperationException($"L'UE avec le numéro {ue.NumeroUe} existe déjà.");

        var res = Context.Ues!.Add(ue);
        await Context.SaveChangesAsync();
        return res.Entity;
    }

    public async Task<Ue?> GetUeByNumeroAsync(string numeroUe)
    {
        return await Context.Ues!.FirstOrDefaultAsync(u => u.NumeroUe == numeroUe);
    }

    public async Task<List<Ue>> GetAllUesAsync()
    {
        return await Context.Ues!.ToListAsync();
    }

    public async Task<List<Ue>> GetUesByParcoursAsync(long idParcours)
    {
        return await Context.Ues!
            .Where(ue => ue.EnseigneeDans!.Any(p => p.Id == idParcours))
            .ToListAsync();
    }

    public async Task UpdateUeAsync(Ue ue)
    {
        if (ue == null)
            throw new ArgumentNullException(nameof(ue), "L'UE ne peut pas être null.");

        var existingUe = await Context.Ues!.FindAsync(ue.Id);
        if (existingUe == null)
            throw new KeyNotFoundException($"L'UE avec l'ID {ue.Id} est introuvable.");

        Context.Ues!.Update(ue);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteUeAsync(long id)
    {
        var ue = await Context.Ues!.FindAsync(id);
        if (ue == null)
            throw new KeyNotFoundException($"Impossible de supprimer : L'UE avec l'ID {id} est introuvable.");

        Context.Ues!.Remove(ue);
        await Context.SaveChangesAsync();
    }
}
