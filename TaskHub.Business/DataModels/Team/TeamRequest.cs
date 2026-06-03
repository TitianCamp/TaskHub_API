namespace TaskHub.Business.DataModels.Team;

public class TeamRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
}
