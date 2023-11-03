namespace TheWall.Models;

public class TheWallViewModel
{
    public List<Message>? newListMsg { get; set; }
    public Message newMsg { get; set; } = new Message();
    public List<Comment>? newListComm { get; set; }
    public Comment? newComment { get; set; } = new Comment();


}