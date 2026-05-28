namespace Notes.Api.Services;

public interface INotesService
{
    Task<NoteResponse?> GetAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<NoteResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<NoteResponse> AddAsync(AddNoteRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
