using System;
using System.Collections.Generic;

namespace SongMelodey.Models;

public partial class MedleyClip
{
    public int MedleyClipId { get; set; }

    public int MedleyId { get; set; }

    public int TrimClipId { get; set; }

    public int SequenceNumber { get; set; }

    public virtual Medley Medley { get; set; } = null!;

    public virtual TransitionClip? TransitionClip { get; set; }

    public virtual TrimClip TrimClip { get; set; } = null!;
}
