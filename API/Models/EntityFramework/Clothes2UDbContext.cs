using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class Clothes2UDbContext : DbContext
{
    public DbSet<Abonnement> Abonnements { get; set; } 
    public DbSet<Adresse> Adresses { get; set; }
    public DbSet<Annonce> Annonces { get; set; } 
    public DbSet<Categorie>  Categories { get; set; }
    public DbSet<Couleur>  Couleurs { get; set; }
    public DbSet<Illustre_Annonce> Illustre_Annonces { get; set; }
    public DbSet<Marque> Marques { get; set; }
    public DbSet<Photo> Photos { get; set; }
    
    public DbSet<Utilisateur> Utilisateurs { get; set; }
    
    
    public Clothes2UDbContext() { }

    public Clothes2UDbContext(DbContextOptions<Clothes2UDbContext> options) : base(options) { }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("sae_clothes2u");
        
        
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

        modelBuilder.Entity<Adresse>(entity =>
        {
            entity.HasKey(e => e.AdresseId);
            
            entity.HasMany(e => e.Utilisateurs)
                .WithOne(u => u.Adresse)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Adresse_Utilisateur");
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

        modelBuilder.Entity<Categorie>(entity =>
        {
            entity.HasKey(e => e.CategorieId);
            
            entity.HasMany(e => e.Annonces)
                .WithOne(a => a.Categorie)
                .HasForeignKey(e => e.CategorieId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            entity.HasMany(e => e.SousCategories)
                .WithOne(s => s.Categorie)
                .HasForeignKey(e => e.CategorieId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            entity.HasMany(e => e.Tailles)
                .WithOne(t => t.Categorie)
                .HasForeignKey(e => e.CategorieTailleId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SousCategorie>(entity =>
        {
            entity.HasKey(e =>  e.SousCategorieId);
            
            entity.HasOne(e => e.Categorie)
                .WithMany(a => a.SousCategories)
                .HasForeignKey(e => e.SousCategorieId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            entity.HasMany(e => e.Annonces)
                .WithOne(a => a.SousCategorie)
                .HasForeignKey(e => e.SousCategorieId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        
        modelBuilder.Entity<Couleur>(entity =>
        {
            entity.HasKey(e => e.CouleurId);
            
            entity.HasMany(e => e.Annonces)
                .WithOne(c => c.Couleur)
                .HasForeignKey(e => e.CouleurId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Illustre_Annonce>(entity =>
        {
            entity.HasKey(e => e.IllustId);
            
            entity.HasOne(e => e.Annonce)
                .WithMany(a => a.Photos)
                .HasForeignKey(e => e.AnnonceId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            entity.HasOne(e => e.Photo)
                .WithMany(p => p.Annonces)
                .HasForeignKey(e => e.PhotoId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Marque>(entity =>
        {
            entity.HasKey(e => e.MarqueId);
            
            entity.HasMany(e => e.Annonces)
                .WithOne(a => a.Marque)
                .HasForeignKey(e => e.MarqueId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        
        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.PhotoId);
            
            entity.HasMany(e => e.Annonces)
                .WithOne(a => a.Photo)
                .HasForeignKey(e => e.PhotoId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Utilisateur>(entity =>
        {
            // Clé primaire
            entity.HasKey(e => e.UtilisateurId);

            // Configuration des propriétés
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Login)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Dateinscription)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            // Relation avec Adresse
            entity.HasOne(e => e.Adresse)
                .WithMany(a => a.Utilisateurs)
                .HasForeignKey(e => e.AdresseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation avec StatutUtilisateur
            entity.HasOne(e => e.Statut)
                .WithMany(s => s.Utilisateurs)
                .HasForeignKey(e => e.StatutId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index pour améliorer les performances
            entity.HasIndex(e => e.Email)
                .IsUnique();
                
            entity.HasIndex(e => e.Login)
                .IsUnique();
                
            entity.HasIndex(e => e.StatutId);
            entity.HasIndex(e => e.AdresseId);
            entity.HasIndex(e => e.Dateinscription);
        });
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}