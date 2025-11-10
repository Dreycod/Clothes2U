using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class Clothes2UDbContext : DbContext
{
    public DbSet<Abonnement> Abonnements { get; set; } 
    public DbSet<Adresse> Adresses { get; set; }
    public DbSet<Annonce> Annonces { get; set; } 
    
    
    public Clothes2UDbContext() { }

    public Clothes2UDbContext(DbContextOptions<Clothes2UDbContext> options) : base(options) { }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("api");
        
        
        modelBuilder.Entity<Abonnement>(entity =>
        {
            // Clé primaire
            entity.HasKey(e => e.AbonnementId);

            // Relation : Un utilisateur (suiveur) peut avoir plusieurs abonnements
            entity.HasOne(e => e.UtilisateurSuiveur)
                .WithMany(u => u.Abonnements)
                .HasForeignKey(e => e.UtilisateurSuiveurId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation : Un utilisateur (suivi) peut avoir plusieurs abonnés
            entity.HasOne(e => e.UtilisateurSuivis)
                .WithMany(u => u.Abonnes)
                .HasForeignKey(e => e.UtilisateurSuivisId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index pour améliorer les performances des requêtes
            entity.HasIndex(e => e.UtilisateurSuiveurId);
            entity.HasIndex(e => e.UtilisateurSuivisId);
                
            // Index composite pour éviter les doublons (un utilisateur ne peut suivre qu'une fois un autre)
            entity.HasIndex(e => new { e.UtilisateurSuiveurId, e.UtilisateurSuivisId })
                .IsUnique();
        });
        
        
        modelBuilder.Entity<Annonce>(entity =>
            {
                // Clé primaire
                entity.HasKey(e => e.AnnonceId);

                // Configuration des propriétés
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.DateAnnonce)
                    .IsRequired();

                entity.Property(e => e.Negociable)
                    .IsRequired();

                entity.Property(e => e.Prix)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                // Configuration des relations One-to-Many
                entity.HasOne(e => e.Utilisateur)
                    .WithMany(u => u.Annonces)
                    .HasForeignKey(e => e.UtilisateurId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Etat)
                    .WithMany(ea => ea.Annonces)
                    .HasForeignKey(e => e.EtatId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Couleur)
                    .WithMany(c => c.Annonces)
                    .HasForeignKey(e => e.CouleurId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Marque)
                    .WithMany(m => m.Annonces)
                    .HasForeignKey(e => e.MarqueId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Taille)
                    .WithMany(t => t.Annonces)
                    .HasForeignKey(e => e.TailleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.SousCategorie)
                    .WithMany(sc => sc.Annonces)
                    .HasForeignKey(e => e.SousCategorieId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Categorie)
                    .WithMany(c => c.Annonces)
                    .HasForeignKey(e => e.CategorieId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Statut)
                    .WithMany(s => s.Annonces)
                    .HasForeignKey(e => e.StatutAnnonceId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Index pour améliorer les performances
                entity.HasIndex(e => e.UtilisateurId);
                entity.HasIndex(e => e.DateAnnonce);
                entity.HasIndex(e => e.StatutAnnonceId);
                entity.HasIndex(e => e.CategorieId);
                entity.HasIndex(e => e.SousCategorieId);
            });
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}