using System.Collections.Generic;

namespace Zeitkombinat.Models;

public class Story {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string HyperLink { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public List<TaskItem> Tasks { get; set; } = new();
}
