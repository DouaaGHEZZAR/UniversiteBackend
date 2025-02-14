using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using Microsoft.EntityFrameworkCore;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    public async Task<Note> CreateAsync(Note note)
    {
        if (note == null)
            throw new ArgumentNullException(nameof(note), "La note ne peut pas être null.");

        // Vérification que l'étudiant et l'UE existent avant d'ajouter la note
        var etudiant = await Context.Etudiants!.FindAsync(note.EtudiantId);
        var ue = await Context.Ues!.FindAsync(note.UeId);

        if (etudiant == null || ue == null)
            throw new ArgumentException("L'étudiant ou l'UE associée à cette note est introuvable.");

        // Vérification qu'une note n'existe pas déjà pour cet étudiant et cette UE
        var existingNote = await Context.Notes!
            .FirstOrDefaultAsync(n => n.EtudiantId == note.EtudiantId && n.UeId == note.UeId);

        if (existingNote != null)
            throw new InvalidOperationException($"Une note existe déjà pour l'étudiant {note.EtudiantId} dans l'UE {note.UeId}.");

        // Ajout de la note
        var res = Context.Notes!.Add(note);
        await Context.SaveChangesAsync();
        return res.Entity;
    }

    public async Task<Note?> GetNoteAsync(long etudiantId, long ueId)
    {
        return await Context.Notes!.FirstOrDefaultAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);
    }

    public async Task<List<Note>> GetNotesByEtudiantAsync(long etudiantId)
    {
        return await Context.Notes!.Where(n => n.EtudiantId == etudiantId).ToListAsync();
    }

    public async Task<List<Note>> GetNotesByUeAsync(long ueId)
    {
        return await Context.Notes!.Where(n => n.UeId == ueId).ToListAsync();
    }

    public async Task UpdateNoteAsync(Note note)
    {
        if (note == null)
            throw new ArgumentNullException(nameof(note), "La note ne peut pas être null.");

        var existingNote = await GetNoteAsync(note.EtudiantId, note.UeId);
        if (existingNote == null)
            throw new KeyNotFoundException($"La note de l'étudiant {note.EtudiantId} pour l'UE {note.UeId} est introuvable.");

        Context.Notes!.Update(note);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteNoteAsync(long etudiantId, long ueId)
    {
        var note = await GetNoteAsync(etudiantId, ueId);
        if (note == null)
            throw new KeyNotFoundException($"Impossible de supprimer : la note de l'étudiant {etudiantId} pour l'UE {ueId} est introuvable.");

        Context.Notes!.Remove(note);
        await Context.SaveChangesAsync();
    }
}
