namespace Notes.Api.Services;

public interface INotesService
{
    Task<NoteResponse?> GetAsync(int id, string userId, CancellationToken cancellationToken);
    Task<IEnumerable<NoteResponse>> GetAllAsync(string userId, CancellationToken cancellationToken);
    Task<NoteResponse> AddAsync(AddNoteRequest request, string userId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, string userId, CancellationToken cancellationToken);
}
