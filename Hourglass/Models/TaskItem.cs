using System;
using System.Collections.Generic;

namespace Hourglass.Models;

public class TaskItem {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string HyperLink { get; set; } = string.Empty;
    public TimeSpan EstimatedHours { get; set; }
    public int StoryId { get; set; }
    public Story Story { get; set; } = null!;
    public List<WorkSession> WorkSessions { get; set; } = new();
}
