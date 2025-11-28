public class RelayConfiguration
{
    public int ListenPort { get; set; }
    public required string RelayHost { get; set; }
    public int RelayPort { get; set; }
    public string? RelayUsername { get; set; }
    public string? RelayPassword { get; set; }
    public bool RelayUseTls { get; set; }
    public bool RequireAuth { get; set; }
    public string? AuthUsername { get; set; }
    public string? AuthPassword { get; set; }
}
