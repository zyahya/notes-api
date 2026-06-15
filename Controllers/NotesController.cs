using Notes.Api.Services;

namespace Notes.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotesController(INotesService notesService) : ControllerBase
{
    private readonly INotesService _notesService = notesService;

    [HttpGet("{id}")]
    public async Task<ActionResult<NoteResponse>> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var note = await _notesService.GetAsync(id, cancellationToken);
        return note == null ? (ActionResult<NoteResponse>)NotFound() : (ActionResult<NoteResponse>)Ok(note);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var notes = await _notesService.GetAllAsync(cancellationToken);
        return Ok(notes);
    }

    [HttpPost]
    public async Task<ActionResult<NoteResponse>> Add([FromBody] AddNoteRequest request, CancellationToken cancellationToken)
    {
        var note = await _notesService.AddAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var deleted = await _notesService.DeleteAsync(id, cancellationToken);
        return !deleted ? NotFound() : NoContent();
    }
}
