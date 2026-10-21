
using System.ComponentModel.DataAnnotations;

namespace library2.Models;

public class Note
{
    public long NoteId { get; set; }

    [Required(ErrorMessage = "Please enter a title.")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = "";

    public string? Body { get; set; }

    [Required]
    public DateOnly CreatedOn { get; set; } =
        DateOnly.FromDateTime(DateTime.Today);
}
