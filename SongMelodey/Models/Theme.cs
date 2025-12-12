using System;
using System.Collections.Generic;

namespace SongMelodey.Models;

public partial class Theme
{
    public int ThemeId { get; set; }

    public string ThemeName { get; set; } = null!;

    public virtual ICollection<Medley> Medleys { get; set; } = new List<Medley>();

    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
}
