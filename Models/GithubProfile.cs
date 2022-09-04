namespace BadgeCounters.Models
{
    public record class GithubProfile(
        int Id, 
        string Name, 
        int Count, 
        bool Active, 
        DateTime Created);
}
