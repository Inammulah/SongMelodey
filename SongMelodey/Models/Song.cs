using System;
using System.Collections.Generic;

namespace SongMelodey.Models;

public partial class Song
{
    public int SongId { get; set; }

    public int? ThemeId { get; set; }

    public string SongTitle { get; set; } = null!;

    public string? ArtistName { get; set; }

    public int DurationSec { get; set; }

    public string FilePath { get; set; } = null!;

    public virtual Theme? Theme { get; set; }

    public virtual ICollection<TrimClip> TrimClips { get; set; } = new List<TrimClip>();
}
