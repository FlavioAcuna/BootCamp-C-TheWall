#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;


namespace TheWall.Models;
public class Message
{
    [Key]
    public int MessageId { get; set; }
    [Required]
    [DataType(DataType.Text)]
    public string MessageText { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int UserId { get; set; }
    public List<Comment> MessageComment { get; set; } = new List<Comment>();
    public User? Creador { get; set; }
}
