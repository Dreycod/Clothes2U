using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class Clothes2UDbContext : DbContext
{
    public DbSet<Abonnement> Abonnements { get; set; } 
    public DbSet<Achete> Achetes { get; set; }
    public DbSet<Adresse> Adresses { get; set; }
    public DbSet<Annonce> Annonces { get; set; } 
    public DbSet<Bloque> Bloques { get; set; }
    public DbSet<Categorie>  Categories { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Couleur>  Couleurs { get; set; }
    public DbSet<Decision_suspension> DecisionSuspensions { get; set; }
    public DbSet<DemandeRestauration> DemandesRestauration { get; set; }
    public DbSet<Est_De_Couleur> Est_De_Couleurs { get; set; }
    public DbSet<EtatArticle> EtatArticles { get; set; }
    public DbSet<Favoris> Favorises { get; set; }
    public DbSet<Illustre_Annonce> Illustre_Annonces { get; set; }
    public DbSet<Marque> Marques { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MessageContientImage> MessageContientImages { get; set; }
    public DbSet<MessageDemande> MessageDemandes { get; set; }
    public DbSet<MessageTexte> MessageTextes { get; set; }
    public DbSet<MessageValidation> MessageValidations { get; set; }
    public DbSet<NoteUtilisateur>  NoteUtilisateurs { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationAdmin> NotificationAdmins { get; set; }
    public DbSet<NotificationAvertissement> NotificationAvertissements { get; set; }
    public DbSet<NotificationMessage> NotificationMessages { get; set; }
    public DbSet<NotificationModificationAnnonce> NotificationModificationAnnonces { get; set; }
    public DbSet<NotificationNouvelleAnnonce> NotificationNouvelleAnnonces { get; set; }
    public DbSet<NotificationType> NotificationTypes { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Recense> Recenses { get; set; }
    public DbSet<RoleUtilisateur> RolesUtilisateurs { get; set; }
    public DbSet<Signalement> Signalements { get; set; }
    public DbSet<SignalementAnnonce> SignalementAnnonces { get; set; }
    public DbSet<SignalementAvis>  SignalementAvises { get; set; }
    public DbSet<SignalementUtilisateur> SignalementUtilisateurs { get; set; }
    public DbSet<SousCategorie>  SousCategories { get; set; }
    public DbSet<StatutAnnonce> StatutAnnonces { get; set; }
    public DbSet<StatutConversation> StatutConversations { get; set; }
    public DbSet<StatutUtilisateur> StatutUtilisateurs { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Taille> Tailles { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TypeSignalement> TypeSignalements { get; set; }
    public DbSet<TypeSuspension> TypeSuspensions { get; set; }
    public DbSet<Utilisateur> Utilisateurs { get; set; }
    public DbSet<Vend> Vends { get; set; }
    public DbSet<Visualisation> Visualisations { get; set; }



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

        modelBuilder.Entity<Achete>(entity =>
        {
            entity.ToTable("t_j_achete_ach");
    
            entity.HasKey(e => e.AcheteId);
    
            entity.Property(e => e.AcheteId)
                .HasColumnName("ach_id")
                .ValueGeneratedOnAdd();
    
            entity.Property(e => e.UtilisateurAcheteurId)
                .HasColumnName("ach_utilisateur_acheteur_id")
                .IsRequired();
    
            entity.Property(e => e.ConversationId)
                .HasColumnName("ach_conversation_id")
                .IsRequired();
    
            // Relation avec Utilisateur (acheteur)
            entity.HasOne(a => a.UtilisateurAcheteur)
                .WithMany(u => u.Achats)
                .HasForeignKey(a => a.UtilisateurAcheteurId)
                .OnDelete(DeleteBehavior.Restrict);
    
            // Relation avec Conversation
            entity.HasOne(a => a.Conversation)
                .WithOne(c => c.Acheteur)
                .HasForeignKey<Achete>(a => a.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
    
            // Index unique sur ConversationId (relation one-to-one)
            entity.HasIndex(e => e.ConversationId)
                .IsUnique()
                .HasDatabaseName("idx_achete_conversation_unique");
    
            // Index pour rechercher les achats d'un utilisateur
            entity.HasIndex(e => e.UtilisateurAcheteurId)
                .HasDatabaseName("idx_achete_utilisateur");
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

            entity.HasMany(e => e.Couleurs)
                .WithOne(edc => edc.Annonce)
                .HasForeignKey(edc => edc.AnnonceId)
                .OnDelete(DeleteBehavior.Cascade);

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
            
            entity.HasMany(e => e.Tags)
                .WithOne(t => t.Annonce)
                .HasForeignKey(t => t.AnnonceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index pour améliorer les performances
            entity.HasIndex(e => e.UtilisateurId);
            entity.HasIndex(e => e.DateAnnonce);
            entity.HasIndex(e => e.StatutAnnonceId);
            entity.HasIndex(e => e.CategorieId);
            entity.HasIndex(e => e.SousCategorieId);
        });

        modelBuilder.Entity<Bloque>(entity =>
        {
            entity.HasKey(e => e.BloqueId);
            
            entity.HasOne(e => e.UtilisateurBloque)
                .WithMany(u => u.BloqueParUtilisateurs)
                .HasForeignKey(e => e.UtilisateurBloqueId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.UtilisateurBloqueur)
                .WithMany(u => u.UtilisateursBloques)
                .HasForeignKey(e => e.UtilisateurBloqueurId)
                .OnDelete(DeleteBehavior.Restrict);
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

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId);
            
            entity.HasOne(e => e.LAnnonce)
                .WithMany(a => a.LesConversations)
                .HasForeignKey(e => e.AnnonceId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        
        modelBuilder.Entity<Couleur>(entity =>
        {
            entity.HasKey(e => e.CouleurId);
            
            entity.HasMany(e => e.Annonces)
                .WithOne(edc => edc.Couleur)
                .HasForeignKey(edc => edc.CouleurId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Decision_suspension>(entity =>
        {
            entity.ToTable("t_e_decision_suspension_sus");
            
            entity.HasKey(e => e.Decision_suspensionId);
            
            entity.Property(e => e.Decision_suspensionId)
                .HasColumnName("sus_id")
                .ValueGeneratedOnAdd();
            
            entity.Property(e => e.DateDebutSuspension)
                .HasColumnName("sus_date_debut_suspension")
                .IsRequired();
            
            entity.Property(e => e.DateFinSuspension)
                .HasColumnName("sus_date_fin_suspension")
                .IsRequired();
            
            entity.Property(e => e.MotifSuspension)
                .HasColumnName("sus_motif_suspension")
                .IsRequired();
            
            entity.Property(e => e.UtilisateurId)
                .HasColumnName("sus_utilisateur_id")
                .IsRequired(false);
            
            entity.Property(e => e.UtilisateurAdminId)
                .HasColumnName("sus_utilisateur_admin_id")
                .IsRequired(false);
            
            entity.Property(e => e.AnnonceId)
                .HasColumnName("sus_annonce_id")
                .IsRequired(false);
            
            entity.Property(e => e.TypeSuspensionId)
                .HasColumnName("sus_type_id")
                .IsRequired();
            
            // Relation avec Utilisateur suspendu
            entity.HasOne(d => d.UtilisateurSuspendu)
                .WithMany(u => u.LesSuspensions)
                .HasForeignKey(d => d.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
            
            // Relation avec Utilisateur admin (décisionnaire)
            entity.HasOne(d => d.Decisionnaire)
                .WithMany(u => u.LesDecisions)
                .HasForeignKey(d => d.UtilisateurAdminId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
            
            // Relation avec Annonce suspendue
            entity.HasOne(d => d.AnnonceSuspendu)
                .WithMany(a => a.Decisions)
                .HasForeignKey(d => d.AnnonceId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
            
            // Relation avec TypeSuspension
            entity.HasOne(d => d.TypeSuspension)
                .WithMany(t => t.Decision_suspensions)
                .HasForeignKey(d => d.TypeSuspensionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            
            // Contrainte : au moins un des deux doit être renseigné (utilisateur OU annonce)
            // Note: Cette contrainte logique doit être gérée au niveau applicatif ou via un check constraint SQL
            
            // Index pour optimiser les recherches
            entity.HasIndex(e => e.UtilisateurId)
                .HasDatabaseName("idx_decision_suspension_utilisateur");
            
            entity.HasIndex(e => e.AnnonceId)
                .HasDatabaseName("idx_decision_suspension_annonce");
            
            entity.HasIndex(e => e.TypeSuspensionId)
                .HasDatabaseName("idx_decision_suspension_type");
            
            entity.HasIndex(e => new { e.DateDebutSuspension, e.DateFinSuspension })
                .HasDatabaseName("idx_decision_suspension_dates");
        });

        modelBuilder.Entity<DemandeRestauration>(entity =>
        {
            entity.HasKey(e => e.DemandeRestaurationId);

            entity.HasOne(e => e.Plaignant)
                .WithMany(u => u.DemandesRestauration)
                .HasForeignKey(e => e.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Suspension)
                .WithMany(s => s.DemandesRes)
                .HasForeignKey(e => e.SuspensionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UtilisateurId);
            entity.HasIndex(e => e.SuspensionId);
        });

        modelBuilder.Entity<Est_De_Couleur>(entity =>
        {
            entity.HasKey(e => e.EstDeCouleurId);
    
            entity.HasOne(e => e.Annonce)
                .WithMany(a => a.Couleurs)
                .HasForeignKey(e => e.AnnonceId)
                .OnDelete(DeleteBehavior.Cascade);
    
            entity.HasOne(e => e.Couleur)
                .WithMany(c => c.Annonces)
                .HasForeignKey(e => e.CouleurId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EtatArticle>(entity =>
        {
            entity.HasKey(e => e.EtatArticleId);
        });

        modelBuilder.Entity<Favoris>(entity =>
        {
            entity.HasKey(e => e.FavorisId);
            
            entity.HasOne(e => e.Annonce)
                .WithMany(a => a.UtilisateursFavoris)
                .HasForeignKey(e => e.AnnonceId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            entity.HasOne(e => e.Utilisateur)
                .WithMany(u => u.AnnoncesFavorites)
                .HasForeignKey(e => e.UtilisateurId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
       
        
        
        modelBuilder.Entity<Illustre_Annonce>(entity =>
        {
            entity.HasKey(e => e.IllustId);
    
            entity.HasOne(e => e.Annonce)
                .WithMany(a => a.Photos)
                .HasForeignKey(e => e.AnnonceId)
                .OnDelete(DeleteBehavior.Cascade);
    
            entity.HasOne(e => e.Photo)
                .WithMany(p => p.Annonces)
                .HasForeignKey(e => e.PhotoId)
                .OnDelete(DeleteBehavior.Restrict);
    
            entity.HasIndex(e => new { e.AnnonceId, e.PhotoId })
                .IsUnique();
        });

        modelBuilder.Entity<Marque>(entity =>
        {
            entity.HasKey(e => e.MarqueId);
            
            entity.HasMany(e => e.Annonces)
                .WithOne(a => a.Marque)
                .HasForeignKey(e => e.MarqueId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId);
            
            entity.HasOne(e => e.Utilisateur)
                .WithMany(u => u.Messages)
                .HasForeignKey(e => e.UtilisateurId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            entity.HasOne(e => e.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(e => e.ConversationId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.MessageTexte)
                .WithOne(m => m.Message)
                .HasForeignKey<MessageTexte>(m => m.MessageId);

            entity.HasOne(e => e.MessageDemande)
                .WithOne(m => m.Message)
                .HasForeignKey<MessageDemande>(m => m.MessageId);

            entity.HasOne(e => e.MessageValidation)
                .WithOne(m => m.Message)
                .HasForeignKey<MessageValidation>(m => m.MessageId);
        });
        
        modelBuilder.Entity<MessageContientImage>(entity =>
        {
            entity.HasKey(e => e.MessageContientImageId);
    
            entity.HasOne(e => e.Message)
                .WithMany(m => m.Photos)
                .HasForeignKey(e => e.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
    
            entity.HasOne(e => e.Photo)
                .WithMany(p => p.Messages)
                .HasForeignKey(e => e.PhotoId) 
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => new { e.MessageId, e.PhotoId })
                .IsUnique();
        });
        
        
        
        modelBuilder.Entity<MessageTexte>(entity =>
        {
            entity.HasKey(e => e.MessageTexteId);
    
            entity.HasOne(e => e.Message)
                .WithOne(m => m.MessageTexte)
                .HasForeignKey<MessageTexte>(e => e.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
    
        });
        
        modelBuilder.Entity<MessageDemande>(entity =>
        {
            entity.ToTable("t_e_message_demande_mesdem");

            entity.HasKey(e => e.MessageDemandeId);

            entity.HasOne(e => e.Message)
                .WithOne(m => m.MessageDemande)
                .HasForeignKey<MessageDemande>(e => e.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Offre)
                .WithMany(e => e.ContreOffres)
                .HasForeignKey(e => e.DemandeId)
                .OnDelete(DeleteBehavior.NoAction); 
        });

        modelBuilder.Entity<MessageValidation>(entity =>
        {
            entity.ToTable("t_e_message_validation_mesval");

            entity.HasKey(e => e.MessageValidationId);

            entity.HasOne(e => e.Message)
                .WithOne(m => m.MessageValidation)
                .HasForeignKey<MessageValidation>(e => e.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<NoteUtilisateur>(entity =>
        {
            entity.HasKey(e => e.NoteUtilisateurId);
            
            entity.HasOne(e => e.Auteur)
                .WithMany(u => u.NotesAuteur)
                .HasForeignKey(e => e.NoteUtilisateurId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.Cible)
                .WithMany(u => u.NotesCible)
                .HasForeignKey(e => e.NoteId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            entity.HasMany(e => e.Signalements)
                .WithOne(s => s.Avis)
                .HasForeignKey(s => s.AvisId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Notification>(entity =>
    {
        entity.ToTable("t_e_notification_not");
        
        entity.HasKey(e => e.NotificationId);
        
        entity.Property(e => e.NotificationId)
            .HasColumnName("not_id")
            .ValueGeneratedOnAdd();
        
        entity.Property(e => e.NotificationTypeId)
            .HasColumnName("not_type_id")
            .IsRequired();
        
        entity.Property(e => e.UtilisateurId)
            .HasColumnName("not_utilisateur_id")
            .IsRequired();
        
        // Relation avec NotificationType
        entity.HasOne(n => n.NotificationType)
            .WithMany(nt => nt.Notifications)
            .HasForeignKey(n => n.NotificationTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Relation avec Utilisateur
        entity.HasOne(n => n.Utilisateur)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UtilisateurId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Relations optionnelles (one-to-one)
        entity.HasOne(n => n.NotificationAdmins)
            .WithOne(na => na.LaNotification)
            .HasForeignKey<NotificationAdmin>(na => na.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasOne(n => n.NotificationAvertissements)
            .WithOne(na => na.LaNotification)
            .HasForeignKey<NotificationAvertissement>(na => na.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasOne(n => n.NotificationMessages)
            .WithOne(nm => nm.LaNotification)
            .HasForeignKey<NotificationMessage>(nm => nm.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasOne(n => n.NotificationModifications)
            .WithOne(nm => nm.LaNotification)
            .HasForeignKey<NotificationModificationAnnonce>(nm => nm.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasOne(n => n.NotificationNouvellesAnnonces)
            .WithOne(nn => nn.LaNotification)
            .HasForeignKey<NotificationNouvelleAnnonce>(nn => nn.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    });
    
    // Configuration de NotificationType
    modelBuilder.Entity<NotificationType>(entity =>
    {
        entity.ToTable("t_e_notification_type_nottyp");
        
        entity.HasKey(e => e.NotificationTypeId);
        
        entity.Property(e => e.NotificationTypeId)
            .HasColumnName("nottyp_id")
            .ValueGeneratedOnAdd();
        
        entity.Property(e => e.LibelleType)
            .HasColumnName("nottyp_libelle_type")
            .IsRequired()
            .HasMaxLength(100);
    });
    
    // Configuration de NotificationAdmin
    modelBuilder.Entity<NotificationAdmin>(entity =>
    {
        entity.ToTable("t_e_notification_admin_notadm");
        
        entity.HasKey(e => e.NotificationAdminId);
        
        entity.Property(e => e.NotificationAdminId)
            .HasColumnName("notadm_id")
            .ValueGeneratedOnAdd();
        
        entity.Property(e => e.AdminText)
            .HasColumnName("notadm_admin_text")
            .IsRequired();
        
        entity.Property(e => e.NotificationId)
            .HasColumnName("notadm_notification_id")
            .IsRequired();
        
        // Index unique pour garantir qu'une notification n'a qu'une seule NotificationAdmin
        entity.HasIndex(e => e.NotificationId)
            .IsUnique();
    });
    
    // Configuration de NotificationAvertissement
    modelBuilder.Entity<NotificationAvertissement>(entity =>
    {
        entity.ToTable("t_e_notification_avertissement_notave");
        
        entity.HasKey(e => e.NotificationAvertissementId);
        
        entity.Property(e => e.NotificationAvertissementId)
            .HasColumnName("notave_id")
            .ValueGeneratedOnAdd();
        
        entity.Property(e => e.MessageAvertissement)
            .HasColumnName("notave_avertissement_message")
            .IsRequired();
        
        entity.Property(e => e.NotificationId)
            .HasColumnName("notave_notification_id")
            .IsRequired();
        
        // Index unique pour garantir qu'une notification n'a qu'un seul avertissement
        entity.HasIndex(e => e.NotificationId)
            .IsUnique();
    });
    
    // Configuration de NotificationMessage
    modelBuilder.Entity<NotificationMessage>(entity =>
    {
        entity.ToTable("t_e_notification_message_notmes");
        
        entity.HasKey(e => e.NotificationMessageId);
        
        entity.Property(e => e.NotificationMessageId)
            .HasColumnName("notmes_id")
            .ValueGeneratedOnAdd();
        
        entity.Property(e => e.MessageId)
            .HasColumnName("notmes_message_id")
            .IsRequired();
        
        entity.Property(e => e.NotificationId)
            .HasColumnName("notmes_notification_id")
            .IsRequired();
        
        // Index unique pour garantir qu'une notification n'a qu'un seul message
        entity.HasIndex(e => e.NotificationId)
            .IsUnique();
        
        // Relation avec Message
        entity.HasOne(nm => nm.Message)
            .WithMany(m => m.NotificationsMessage)
            .HasForeignKey(nm => nm.MessageId)
            .OnDelete(DeleteBehavior.Restrict);
    });
    
    // Configuration de NotificationModificationAnnonce
    modelBuilder.Entity<NotificationModificationAnnonce>(entity =>
    {
        entity.ToTable("t_e_notification_modifiaction_notmod");
        
        entity.HasKey(e => e.NotificationModificationId);
        
        entity.Property(e => e.NotificationModificationId)
            .HasColumnName("notmod_id")
            .ValueGeneratedOnAdd();
        
        entity.Property(e => e.AnnonceId)
            .HasColumnName("notmod_annonce_id")
            .IsRequired();
        
        entity.Property(e => e.NotificationId)
            .HasColumnName("notmod_notification_id")
            .IsRequired();
        
        // Index unique pour garantir qu'une notification n'a qu'une seule modification
        entity.HasIndex(e => e.NotificationId)
            .IsUnique();
        
        // Relation avec Annonce
        entity.HasOne(nm => nm.Annonce)
            .WithMany(a => a.NotificationsModificationAnnonces)
            .HasForeignKey(nm => nm.AnnonceId)
            .OnDelete(DeleteBehavior.Restrict);
    });
    
    // Configuration de NotificationNouvelleAnnonce
    modelBuilder.Entity<NotificationNouvelleAnnonce>(entity =>
    {
        entity.ToTable("t_e_notification_nouvelle_annonce_notnou");
        
        entity.HasKey(e => e.NotificationNouvelleAnnonceId);
        
        entity.Property(e => e.NotificationNouvelleAnnonceId)
            .HasColumnName("notnou_id")
            .ValueGeneratedOnAdd();
        
        entity.Property(e => e.AnnonceId)
            .HasColumnName("notnou_annonce_id")
            .IsRequired();
        
        entity.Property(e => e.NotificationId)
            .HasColumnName("notnou_notification_id")
            .IsRequired();
        
        // Index unique pour garantir qu'une notification n'a qu'une seule nouvelle annonce
        entity.HasIndex(e => e.NotificationId)
            .IsUnique();
        
        // Relation avec Annonce
        entity.HasOne(nn => nn.Annonce)
            .WithMany(a => a.NotificationsNouvelleAnnonces)
            .HasForeignKey(nn => nn.AnnonceId)
            .OnDelete(DeleteBehavior.Restrict);
    });
        
        
    modelBuilder.Entity<Photo>(entity =>
    {
        entity.HasKey(e => e.PhotoId);
    
    });

    modelBuilder.Entity<Recense>(entity =>
    {
        entity.HasKey(e => e.RecenseId);
        
        entity.HasOne(e => e.Annonce)
            .WithMany(a => a.Tags)
            .HasForeignKey(e => e.AnnonceId)
            .OnDelete(DeleteBehavior.ClientSetNull);
        
        entity.HasOne(e => e.Tag)
            .WithMany(t => t.Annonces)
            .HasForeignKey(e => e.TagId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    });

    modelBuilder.Entity<RoleUtilisateur>(entity =>
    {
        entity.HasKey(e => e.RoleUtilisateurId);
        
        entity.HasMany(e => e.Utilisateurs)
            .WithOne(a => a.Role)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    });
    
    
    
    modelBuilder.Entity<Signalement>(entity =>
    {
        entity.ToTable("t_e_signalement_sig");
        
        entity.HasKey(e => e.SignalementId);
        
        entity.Property(e => e.SignalementId)
            .HasColumnName("sig_id")
            .ValueGeneratedOnAdd();
        
        entity.Property(e => e.SignalementDate)
            .HasColumnName("sig_date")
            .IsRequired();
        
        entity.Property(e => e.SignalementMotif)
            .HasColumnName("sig_motif")
            .IsRequired();
        
        entity.Property(e => e.SignalementTypeId)
            .HasColumnName("sig_type_id")
            .IsRequired();
        
        entity.Property(e => e.UtilisateurId)
            .HasColumnName("sig_utilisateur_id")
            .IsRequired();
        
        // Relation avec TypeSignalement
        entity.HasOne(s => s.TypeSignalement)
            .WithMany(t => t.Signalements)
            .HasForeignKey(s => s.SignalementTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Relation avec Utilisateur (celui qui fait le signalement)
        entity.HasOne(s => s.Utilisateur)
            .WithMany(u => u.Signalements)
            .HasForeignKey(s => s.UtilisateurId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Relations one-to-one optionnelles avec les types spécifiques de signalement
        entity.HasOne(s => s.SignalementsAnnonce)
            .WithOne(sa => sa.Signalement)
            .HasForeignKey<SignalementAnnonce>(sa => sa.SignalementId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasOne(s => s.SignalementsAvis)
            .WithOne(sa => sa.Signalement)
            .HasForeignKey<SignalementAvis>(sa => sa.SignalementId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasOne(s => s.SignalementsUtilisateur)
            .WithOne(su => su.Signalement)
            .HasForeignKey<SignalementUtilisateur>(su => su.SignalementId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Index pour optimiser les recherches
        entity.HasIndex(e => e.UtilisateurId)
            .HasDatabaseName("idx_signalement_utilisateur");
        
        entity.HasIndex(e => e.SignalementTypeId)
            .HasDatabaseName("idx_signalement_type");
        
        entity.HasIndex(e => e.SignalementDate)
            .HasDatabaseName("idx_signalement_date");
});

        // Configuration de SignalementAnnonce
        modelBuilder.Entity<SignalementAnnonce>(entity =>
        {
            entity.ToTable("t_e_signalement_annonce_sigan");
            
            entity.HasKey(e => e.SignalementAnnonceId);
            
            entity.Property(e => e.SignalementAnnonceId)
                .HasColumnName("sigan_id")
                .ValueGeneratedOnAdd();
            
            entity.Property(e => e.AnnonceSignaleeId)
                .HasColumnName("sigan_annonce_signalee_id")
                .IsRequired();
            
            entity.Property(e => e.SignalementId)
                .HasColumnName("sigan_signalement_id")
                .IsRequired();
            
            // Index unique pour garantir qu'un signalement ne peut concerner qu'une seule annonce
            entity.HasIndex(e => e.SignalementId)
                .IsUnique()
                .HasDatabaseName("idx_signalement_annonce_signalement_unique");
            
            // Relation avec Annonce
            entity.HasOne(sa => sa.Annonce)
                .WithMany(a => a.Signalements)
                .HasForeignKey(sa => sa.AnnonceSignaleeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Index pour rechercher les signalements d'une annonce
            entity.HasIndex(e => e.AnnonceSignaleeId)
                .HasDatabaseName("idx_signalement_annonce_annonce");
        });

        // Configuration de SignalementAvis
        modelBuilder.Entity<SignalementAvis>(entity =>
        {
            entity.ToTable("t_e_signalement_avis_sigavs");
            
            entity.HasKey(e => e.SignalementAvisId);
            
            entity.Property(e => e.SignalementAvisId)
                .HasColumnName("sigavs_id")
                .ValueGeneratedOnAdd();
            
            entity.Property(e => e.AvisId)
                .HasColumnName("sigavs_avis_id")
                .IsRequired();
            
            entity.Property(e => e.SignalementId)
                .HasColumnName("sigavs_signalement_id")
                .IsRequired();
            
            // Index unique pour garantir qu'un signalement ne peut concerner qu'un seul avis
            entity.HasIndex(e => e.SignalementId)
                .IsUnique()
                .HasDatabaseName("idx_signalement_avis_signalement_unique");
            
            // Relation avec NoteUtilisateur (Avis)
            entity.HasOne(sa => sa.Avis)
                .WithMany(n => n.Signalements)
                .HasForeignKey(sa => sa.AvisId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Index pour rechercher les signalements d'un avis
            entity.HasIndex(e => e.AvisId)
                .HasDatabaseName("idx_signalement_avis_avis");
        });

        // Configuration de SignalementUtilisateur
        modelBuilder.Entity<SignalementUtilisateur>(entity =>
        {
            entity.ToTable("t_e_signalement_utilisateur_siguti");
            
            entity.HasKey(e => e.SignalementUtilisateurId);
            
            entity.Property(e => e.SignalementUtilisateurId)
                .HasColumnName("siguti_id")
                .ValueGeneratedOnAdd();
            
            entity.Property(e => e.UtilisateurSignaleId)
                .HasColumnName("siguti_utilisateur_signale_id")
                .IsRequired();
            
            entity.Property(e => e.SignalementId)
                .HasColumnName("siguti_signalement_id")
                .IsRequired();
            
            // Index unique pour garantir qu'un signalement ne peut concerner qu'un seul utilisateur
            entity.HasIndex(e => e.SignalementId)
                .IsUnique()
                .HasDatabaseName("idx_signalement_utilisateur_signalement_unique");
            
            // Relation avec Utilisateur signalé
            entity.HasOne(su => su.UtilisateurSignale)
                .WithMany(u => u.SignalementsUtilisateurs)
                .HasForeignKey(su => su.UtilisateurSignaleId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Index pour rechercher les signalements d'un utilisateur
            entity.HasIndex(e => e.UtilisateurSignaleId)
                .HasDatabaseName("idx_signalement_utilisateur_utilisateur");
        });

        
        modelBuilder.Entity<SousCategorie>(entity =>
        {
            entity.HasKey(e =>  e.SousCategorieId);
            
            entity.HasOne(e => e.Categorie)
                .WithMany(a => a.SousCategories)
                .HasForeignKey(e => e.CategorieId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            entity.HasMany(e => e.Annonces)
                .WithOne(a => a.SousCategorie)
                .HasForeignKey(e => e.SousCategorieId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StatutAnnonce>(entity =>
        {
            entity.HasKey(e => e.StatutAnnonceId);
            
            entity.HasMany(e => e.Annonces)
                .WithOne(a => a.Statut)
                .HasForeignKey(e => e.StatutAnnonceId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        
        modelBuilder.Entity<StatutConversation>(entity =>
        {
            entity.ToTable("t_e_statut_conversation_sta");
    
            entity.HasKey(e => e.StatutConversationId);
    
            entity.Property(e => e.StatutConversationId)
                .HasColumnName("sta_id")
                .ValueGeneratedOnAdd();
    
            entity.Property(e => e.StatutConversationLibelle)
                .HasColumnName("sta_libelle")
                .IsRequired()
                .HasMaxLength(100);
    
            // Index unique sur le libellé pour éviter les doublons
            entity.HasIndex(e => e.StatutConversationLibelle)
                .IsUnique()
                .HasDatabaseName("idx_statut_conversation_libelle_unique");
        });

        modelBuilder.Entity<StatutUtilisateur>(entity =>
        {
            entity.HasKey(e => e.StatutUtilisateurId);
            
            entity.HasMany(e => e.Utilisateurs)
                .WithOne(u => u.Statut)
                .HasForeignKey(u => u.StatutId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId);
            
            entity.HasMany(e => e.Annonces)
                .WithOne(a => a.Tag)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Taille>(entity =>
        {
            entity.HasKey(e => e.TailleId);
            
            entity.HasMany(e => e.Annonces)
                .WithOne(a => a.Taille)
                .HasForeignKey(e => e.TailleId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            
            entity.HasOne(e => e.Categorie)
                .WithMany(a => a.Tailles)
                .HasForeignKey(e => e.CategorieTailleId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("t_e_transaction_tra");
    
            entity.HasKey(e => e.TransactionId);
    
            entity.Property(e => e.TransactionId)
                .HasColumnName("tra_id")
                .ValueGeneratedOnAdd();
    
            entity.Property(e => e.TransactionMontant)
                .HasColumnName("tra_montant")
                .IsRequired();
    
            entity.Property(e => e.TransactionEtat)
                .HasColumnName("tra_transaction_etat")
                .IsRequired();
    
            entity.Property(e => e.ConversationId)
                .HasColumnName("tra_conversation_id")
                .IsRequired();
    
            // Relation avec Conversation (one-to-one)
            entity.HasOne(t => t.Conversation)
                .WithOne(c => c.Transaction)
                .HasForeignKey<Transaction>(t => t.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
    
            // Index unique sur ConversationId (relation one-to-one)
            entity.HasIndex(e => e.ConversationId)
                .IsUnique()
                .HasDatabaseName("idx_transaction_conversation_unique");
    
            // Index pour rechercher par état
            entity.HasIndex(e => e.TransactionEtat)
                .HasDatabaseName("idx_transaction_etat");
        });
        
        modelBuilder.Entity<TypeSignalement>(entity =>
        {
            entity.ToTable("t_e_signalement_type_sigtyp");
    
            entity.HasKey(e => e.SignalementTypeId);
    
            entity.Property(e => e.SignalementTypeId)
                .HasColumnName("sigtype_id")
                .ValueGeneratedOnAdd();
    
            entity.Property(e => e.SignalementTypeLibelle)
                .HasColumnName("sigtyp_libelle_type")
                .IsRequired()
                .HasMaxLength(100);
    
            // Index unique sur le libellé pour éviter les doublons
            entity.HasIndex(e => e.SignalementTypeLibelle)
                .IsUnique()
                .HasDatabaseName("idx_type_signalement_libelle_unique");
        });

// Configuration de TypeSuspension
        modelBuilder.Entity<TypeSuspension>(entity =>
        {
            entity.ToTable("t_e_type_suspension_tsu");
    
            entity.HasKey(e => e.TypeSuspensionId);
    
            entity.Property(e => e.TypeSuspensionId)
                .HasColumnName("tsu_id")
                .ValueGeneratedOnAdd();
    
            entity.Property(e => e.NomTypeSuspension)
                .HasColumnName("tsu_nomtypesuspension")
                .IsRequired()
                .HasMaxLength(100);
    
            // Index unique sur le nom pour éviter les doublons
            entity.HasIndex(e => e.NomTypeSuspension)
                .IsUnique()
                .HasDatabaseName("idx_type_suspension_nom_unique");
        });

        modelBuilder.Entity<StatutUtilisateur>(entity =>
        {
            entity.HasKey(e => e.StatutUtilisateurId);

            entity.HasMany(e => e.Utilisateurs)
                .WithOne(u => u.Statut)
                .HasForeignKey(u => u.StatutId);
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
            
           

            //relation avec la table role
            entity.HasOne(e => e.Role)
                .WithMany(r => r.Utilisateurs)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            
            
            // Index pour améliorer les performances
            entity.HasIndex(e => e.Email)
                .IsUnique();
                
            entity.HasIndex(e => e.Login)
                .IsUnique();
                
            entity.HasIndex(e => e.StatutId);
            entity.HasIndex(e => e.AdresseId);
            entity.HasIndex(e => e.Dateinscription);
            
            
        });
        
        modelBuilder.Entity<Vend>(entity =>
        {
            entity.ToTable("t_j_vend_ven");
    
            entity.HasKey(e => e.VendId);
    
            entity.Property(e => e.VendId)
                .HasColumnName("ven_id")
                .ValueGeneratedOnAdd();
    
            entity.Property(e => e.UtilisateurVendeurId)
                .HasColumnName("ven_utilisateur_vendeur_id")
                .IsRequired();
    
            entity.Property(e => e.ConversationId)
                .HasColumnName("ven_conversation_id")
                .IsRequired();
    
            // Relation avec Utilisateur (vendeur)
            entity.HasOne(v => v.UtilisateurVendeur)
                .WithMany(u => u.Ventes)
                .HasForeignKey(v => v.UtilisateurVendeurId)
                .OnDelete(DeleteBehavior.Restrict);
    
            // Relation avec Conversation
            entity.HasOne(v => v.LaConversation)
                .WithOne(c => c.Vendeur)
                .HasForeignKey<Vend>(v => v.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
    
            // Index unique sur ConversationId (relation one-to-one)
            entity.HasIndex(e => e.ConversationId)
                .IsUnique()
                .HasDatabaseName("idx_vend_conversation_unique");
    
            // Index pour rechercher les ventes d'un utilisateur
            entity.HasIndex(e => e.UtilisateurVendeurId)
                .HasDatabaseName("idx_vend_utilisateur");
        });

        modelBuilder.Entity<Visualisation>(entity =>
        {
            entity.ToTable("t_j_visualisation_vis");

            entity.HasKey(e => e.VisualisationId);

            entity.Property(e => e.VisualisationId)
                .HasColumnName("vis_id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UtilisateurId)
                .HasColumnName("vis_utilisateur_id")
                .IsRequired();

            entity.Property(e => e.AnnonceId)
                .HasColumnName("vis_annonce_id")
                .IsRequired();

            entity.Property(e => e.DateVisualisation)
                .HasColumnName("vis_date")
                .IsRequired();

            entity.HasOne(v => v.UtilisateurVisu)
                .WithMany(u => u.Visualisations)
                .HasForeignKey(v => v.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(v => v.Annonce)
                .WithMany(a => a.LesVisualisations)
                .HasForeignKey(v => v.AnnonceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UtilisateurId)
                .HasDatabaseName("idx_visualisation_utilisateur_utilisateurid");

            entity.HasIndex(e => e.AnnonceId)
                .HasDatabaseName("idx_visualisation_annonce_annonceid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}