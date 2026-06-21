using Mapster;

using Microsoft.EntityFrameworkCore;

namespace Notes.Api.Services;

public class NotesService(ApplicationDbContext context) : INotesService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<NoteResponse> AddAsync(AddNoteRequest request, string userId, CancellationToken cancellationToken)
    {
        var note = request.Adapt<Note>();
        note.UserId = userId;

        await _context.AddAsync(note, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return note.Adapt<NoteResponse>();
    }

    public async Task<bool> DeleteAsync(int id, string userId, CancellationToken cancellationToken)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken: cancellationToken);
        if (note is null)
        {
            return false;
        }

        _context.Remove(note);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<NoteResponse>> GetAllAsync(string userId, CancellationToken cancellationToken)
    {
        var notes = await _context.Notes.Where(x => x.UserId == userId).ToListAsync(cancellationToken: cancellationToken);

        return notes.Adapt<IEnumerable<NoteResponse>>();
    }

    public async Task<NoteResponse?> GetAsync(int id, string userId, CancellationToken cancellationToken)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken: cancellationToken);
        return note.Adapt<NoteResponse>();
    }
}
