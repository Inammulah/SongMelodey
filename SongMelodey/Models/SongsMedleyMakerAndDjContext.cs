using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace SongMelodey.Models;

public partial class SongsMedleyMakerAndDjContext : DbContext
{
    public SongsMedleyMakerAndDjContext()
    {
    }

    public SongsMedleyMakerAndDjContext(DbContextOptions<SongsMedleyMakerAndDjContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Medley> Medleys { get; set; }

    public virtual DbSet<MedleyClip> MedleyClips { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    public virtual DbSet<Theme> Themes { get; set; }

    public virtual DbSet<TransitionClip> TransitionClips { get; set; }

    public virtual DbSet<TrimClip> TrimClips { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {

        }
    }
  
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=BALGHARIPC; Database=Songs_Medley_Maker_AndDj; Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Medley>(entity =>
        {
            entity.HasKey(e => e.MedleyId).HasName("PK__Medley__718500B6CAA32767");

            entity.ToTable("Medley");

            entity.HasIndex(e => e.MedleyName, "IX_Medley_Name");

            entity.Property(e => e.MedleyName).HasMaxLength(300);
            entity.Property(e => e.OutputFilePath).HasMaxLength(1000);

            entity.HasOne(d => d.Theme).WithMany(p => p.Medleys)
                .HasForeignKey(d => d.ThemeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Medley_Theme");
        });

        modelBuilder.Entity<MedleyClip>(entity =>
        {
            entity.HasKey(e => e.MedleyClipId).HasName("PK__MedleyCl__603ED6BBD6CFA95A");

            entity.ToTable("MedleyClip");

            entity.HasIndex(e => e.TrimClipId, "IX_MedleyClip_TrimClip");

            entity.HasIndex(e => new { e.MedleyId, e.SequenceNumber }, "UX_Medley_Sequence").IsUnique();

            entity.HasOne(d => d.Medley).WithMany(p => p.MedleyClips)
                .HasForeignKey(d => d.MedleyId)
                .HasConstraintName("FK_MedleyClip_Medley");

            entity.HasOne(d => d.TrimClip).WithMany(p => p.MedleyClips)
                .HasForeignKey(d => d.TrimClipId)
                .HasConstraintName("FK_MedleyClip_TrimClip");
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.SongId).HasName("PK__Song__12E3D697C4B6CE2F");

            entity.ToTable("Song");

            entity.HasIndex(e => e.SongTitle, "IX_Song_Title");

            entity.Property(e => e.ArtistName).HasMaxLength(200);
            entity.Property(e => e.FilePath).HasMaxLength(1000);
            entity.Property(e => e.SongTitle).HasMaxLength(300);

            entity.HasOne(d => d.Theme).WithMany(p => p.Songs)
                .HasForeignKey(d => d.ThemeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Song_Theme");
        });

        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.ThemeId).HasName("PK__Theme__FBB3E4D9049BD725");

            entity.ToTable("Theme");

            entity.HasIndex(e => e.ThemeName, "UQ__Theme__4E60E6D06AD3B7A7").IsUnique();

            entity.Property(e => e.ThemeName).HasMaxLength(100);
        });

        modelBuilder.Entity<TransitionClip>(entity =>
        {
            entity.HasKey(e => e.TransitionClipId).HasName("PK__Transiti__3D6471625FC7FC86");

            entity.ToTable("TransitionClip");

            entity.HasIndex(e => e.MedleyClipId, "IX_Transition_MedleyClip");

            entity.HasIndex(e => e.MedleyClipId, "UQ__Transiti__603ED6BAA12A2CC2").IsUnique();

            entity.Property(e => e.FilePath).HasMaxLength(1000);
            entity.Property(e => e.TransitionName).HasMaxLength(200);

            entity.HasOne(d => d.MedleyClip).WithOne(p => p.TransitionClip)
                .HasForeignKey<TransitionClip>(d => d.MedleyClipId)
                .HasConstraintName("FK_Transition_MedleyClip");
        });

        modelBuilder.Entity<TrimClip>(entity =>
        {
            entity.HasKey(e => e.TrimClipId).HasName("PK__TrimClip__4BD2EE9A0235594E");

            entity.ToTable("TrimClip");

            entity.HasIndex(e => e.SongId, "IX_TrimClip_Song");

            entity.Property(e => e.FilePath).HasMaxLength(1000);

            entity.HasOne(d => d.Song).WithMany(p => p.TrimClips)
                .HasForeignKey(d => d.SongId)
                .HasConstraintName("FK_TrimClip_Song");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
