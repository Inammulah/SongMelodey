using System;
using System.Collections.Generic;

namespace SongMelodey.Models;

public partial class Medley
{
    public int MedleyId { get; set; }

    public string MedleyName { get; set; } = null!;

    public int? ThemeId { get; set; }

    public int? ClipCount { get; set; }

    public int? TotalDurationMs { get; set; }

    public bool IsFinal { get; set; }

    public string? OutputFilePath { get; set; }

    public virtual ICollection<MedleyClip> MedleyClips { get; set; } = new List<MedleyClip>();

    public virtual Theme? Theme { get; set; }
}
