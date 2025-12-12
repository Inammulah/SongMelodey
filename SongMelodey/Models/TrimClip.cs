using System;
using System.Collections.Generic;

namespace SongMelodey.Models;

public partial class TrimClip
{
    public int TrimClipId { get; set; }

    public int SongId { get; set; }

    public int StartMs { get; set; }

    public int EndMs { get; set; }

    public int ClipLengthMs { get; set; }

    public string FilePath { get; set; } = null!;

    public virtual ICollection<MedleyClip> MedleyClips { get; set; } = new List<MedleyClip>();

    public virtual Song Song { get; set; } = null!;
}
