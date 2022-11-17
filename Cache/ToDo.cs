namespace Cache;

public class ToDo
{
    public ToDo()
    {
        // EF 
        Description = null!;
    }
    public int Id { get; set; }
    public string Description { get; set; }
    public bool IsDone { get; set; } = false;

}