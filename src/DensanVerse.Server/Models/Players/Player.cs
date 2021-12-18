namespace DensanVerse.Server.Models.Players;

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; }
    public PlayerTitle Title { get; set; }
    public Rank Rank { get; set; }

    //public Alliance Alliance{get;set;}
}