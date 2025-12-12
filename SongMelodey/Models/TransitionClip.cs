using System;
using System.Collections.Generic;

namespace SongMelodey.Models;

public partial class TransitionClip
{
    public int TransitionClipId { get; set; }

    public int MedleyClipId { get; set; }

    public string TransitionName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int DurationMs { get; set; }

    public virtual MedleyClip MedleyClip { get; set; } = null!;
}
