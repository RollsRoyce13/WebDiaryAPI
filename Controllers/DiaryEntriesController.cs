using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDiaryAPI.Data;
using WebDiaryAPI.Models;

namespace WebDiaryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaryEntriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DiaryEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiaryEntry>>> GetDiaryEntries()
        {
            return await _context.DiaryEntries.ToListAsync();
        }

        [HttpGet("{id}")] // Route for getting a specific diary entry by ID
        public async Task<ActionResult<DiaryEntry>> GetDiaryEntry(int id)
        {
            // Database query to find a diary entry by its ID
            var diaryEntry = await _context.DiaryEntries.FindAsync(id);

            if (diaryEntry == null)
            {
                return NotFound();
            }

            return diaryEntry;
        }

        [HttpPost]
        public async Task<ActionResult<DiaryEntry>> PostDiaryEntry(DiaryEntry diaryEntry)
        {
            diaryEntry.Id = 0; // Ensure the ID is set to 0 for new entries

            _context.DiaryEntries.Add(diaryEntry);

            await _context.SaveChangesAsync();

            var resourceURL = Url.Action(nameof(GetDiaryEntry), new { id = diaryEntry.Id });

            return Created(resourceURL, diaryEntry);
        }

        [HttpPut("{id}")] // Route for updating a specific diary entry by ID
        // Use [FromBody] to bind the request body to the diaryEntry parameter
        public async Task<IActionResult> PutDiaryEntry(int id, [FromBody] DiaryEntry diaryEntry)
        {
            if (id != diaryEntry.Id)
            {
                // Return a 400 BadRequest if the ID in the URL does not match the ID in the body
                return BadRequest();
            }

            _context.Entry(diaryEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiaryEntryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Return 204 No Content on successful update
        }

        [HttpDelete("{id}")] // Route for deleting a specific diary entry by ID
        public async Task<IActionResult> DeleteDiaryEntry(int id)
        {
            var diaryEntry = await _context.DiaryEntries.FindAsync(id);

            if (diaryEntry == null)
            {
                return NotFound(); // Return 404 Not Found if the entry does not exist
            }

            _context.DiaryEntries.Remove(diaryEntry);
            await _context.SaveChangesAsync();

            return NoContent(); // Return 204 No Content on successful deletion
        }

        private bool DiaryEntryExists(int id)
        {
            // Check if a diary entry with the specified ID exists in the database
            return _context.DiaryEntries.Any(e => e.Id == id);
        }
    }
}
