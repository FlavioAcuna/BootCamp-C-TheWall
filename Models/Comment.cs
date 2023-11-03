#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TheWall.Models;
public class Comment
{
    [Key]
    public int CommentId { get; set; }
    [Required]
    public string CommentText { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int MessageId { get; set; }
    public int UserId { get; set; }

    public User? CommentUser { get; set; }

    public Message? CommentMessage { get; set; }
}