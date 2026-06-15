using Mapster;

using Microsoft.EntityFrameworkCore;

namespace Notes.Api.Services;

public class NotesService(ApplicationDbContext context) : INotesService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<NoteResponse> AddAsync(AddNoteRequest request, CancellationToken cancellationToken)
    {
        var note = request.Adapt<Note>();

        await _context.AddAsync(note, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return note.Adapt<NoteResponse>();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var note = await _context.FindAsync<Note>(id);
        if (note is null)
        {
            return false;
        }

        _context.Remove(note);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<NoteResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var notes = await _context.Notes.ToListAsync(cancellationToken: cancellationToken);

        return notes.Adapt<IEnumerable<NoteResponse>>();
    }

    public async Task<NoteResponse?> GetAsync(int id, CancellationToken cancellationToken)
    {
        var note = await _context.FindAsync<Note>(id);
        return note?.Adapt<NoteResponse>();
    }
}
