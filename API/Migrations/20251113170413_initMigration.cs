using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class initMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sae_clothes2u");

            migrationBuilder.CreateTable(
                name: "t_e_adresse_adr",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    adr_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    adr_rue = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    adr_ville = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    adr_code_postal = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_adresse_adr", x => x.adr_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_categorie_cat",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    cat_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cat_libelle_categorie = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_categorie_cat", x => x.cat_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_couleur_cou",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    cou_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cou_nom = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_couleur_cou", x => x.cou_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_etatarticle_etaart",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    etaart_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    etaart_nometat = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_etatarticle_etaart", x => x.etaart_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_marque_mar",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    mar_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mar_nommarque = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_marque_mar", x => x.mar_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_type_nottyp",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    nottyp_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nottyp_libelle_type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notification_type_nottyp", x => x.nottyp_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_signalement_type_sigtyp",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    sigtype_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sigtyp_libelle_type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_signalement_type_sigtyp", x => x.sigtype_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_statut_annonce_staann",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    staann_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    staan_libelle_statut = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_statut_annonce_staann", x => x.staann_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_statut_conversation_sta",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    sta_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sta_libelle = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_statut_conversation_sta", x => x.sta_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_statut_stauti",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    stauti_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stauti_libelle_statut = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_statut_stauti", x => x.stauti_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_tag_tag",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    tag_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tag_libelle = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_tag_tag", x => x.tag_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_type_suspension_tsu",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    tsu_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tsu_nomtypesuspension = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_type_suspension_tsu", x => x.tsu_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_souscategorie_sscat",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    sscat_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sscat_libelle_sous_categorie = table.Column<string>(type: "text", nullable: false),
                    sscat_categorie_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_souscategorie_sscat", x => x.sscat_id);
                    table.ForeignKey(
                        name: "FK_t_e_souscategorie_sscat_t_e_categorie_cat_sscat_categorie_id",
                        column: x => x.sscat_categorie_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_categorie_cat",
                        principalColumn: "cat_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_taille_tai",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    tai_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tai_libelletaille = table.Column<string>(type: "text", nullable: false),
                    tai_categorie_taille_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_taille_tai", x => x.tai_id);
                    table.ForeignKey(
                        name: "FK_t_e_taille_tai_t_e_categorie_cat_tai_categorie_taille_id",
                        column: x => x.tai_categorie_taille_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_categorie_cat",
                        principalColumn: "cat_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_utilisateur_uti",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    uti_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uti_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    uti_login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    uti_password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    uti_dateinscription = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    uti_description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    uti_adresse_id = table.Column<int>(type: "integer", nullable: true),
                    uti_statut_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_utilisateur_uti", x => x.uti_id);
                    table.ForeignKey(
                        name: "FK_Adresse_Utilisateur",
                        column: x => x.uti_adresse_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_adresse_adr",
                        principalColumn: "adr_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_e_utilisateur_uti_t_e_statut_stauti_uti_statut_id",
                        column: x => x.uti_statut_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_statut_stauti",
                        principalColumn: "stauti_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_annonce_ann",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    ann_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ann_titre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ann_dateannonce = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ann_negociable = table.Column<bool>(type: "boolean", nullable: false),
                    ann_prix = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ann_utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    ann_etat_id = table.Column<int>(type: "integer", nullable: false),
                    ann_marque_id = table.Column<int>(type: "integer", nullable: false),
                    ann_taille_id = table.Column<int>(type: "integer", nullable: false),
                    ann_sous_categorie_id = table.Column<int>(type: "integer", nullable: false),
                    ann_categorie_id = table.Column<int>(type: "integer", nullable: false),
                    ann_statut_annonce_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_annonce_ann", x => x.ann_id);
                    table.ForeignKey(
                        name: "FK_t_e_annonce_ann_t_e_categorie_cat_ann_categorie_id",
                        column: x => x.ann_categorie_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_categorie_cat",
                        principalColumn: "cat_id");
                    table.ForeignKey(
                        name: "FK_t_e_annonce_ann_t_e_etatarticle_etaart_ann_etat_id",
                        column: x => x.ann_etat_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_etatarticle_etaart",
                        principalColumn: "etaart_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_e_annonce_ann_t_e_marque_mar_ann_marque_id",
                        column: x => x.ann_marque_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_marque_mar",
                        principalColumn: "mar_id");
                    table.ForeignKey(
                        name: "FK_t_e_annonce_ann_t_e_souscategorie_sscat_ann_sous_categorie_~",
                        column: x => x.ann_sous_categorie_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_souscategorie_sscat",
                        principalColumn: "sscat_id");
                    table.ForeignKey(
                        name: "FK_t_e_annonce_ann_t_e_statut_annonce_staann_ann_statut_annonc~",
                        column: x => x.ann_statut_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_statut_annonce_staann",
                        principalColumn: "staann_id");
                    table.ForeignKey(
                        name: "FK_t_e_annonce_ann_t_e_taille_tai_ann_taille_id",
                        column: x => x.ann_taille_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_taille_tai",
                        principalColumn: "tai_id");
                    table.ForeignKey(
                        name: "FK_t_e_annonce_ann_t_e_utilisateur_uti_ann_utilisateur_id",
                        column: x => x.ann_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_bloque_blo",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    blo_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    blo_utilisateur_bloqueur_id = table.Column<int>(type: "integer", nullable: false),
                    blo_utilisateur_bloque_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_bloque_blo", x => x.blo_id);
                    table.ForeignKey(
                        name: "FK_t_e_bloque_blo_t_e_utilisateur_uti_blo_utilisateur_bloque_id",
                        column: x => x.blo_utilisateur_bloque_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_bloque_blo_t_e_utilisateur_uti_blo_utilisateur_bloqueur~",
                        column: x => x.blo_utilisateur_bloqueur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_note_utilisateur_notuti",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    notuti_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notuti_note = table.Column<int>(type: "integer", nullable: false),
                    notuti_commentaire = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    notuti_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notuti_noteur_id = table.Column<int>(type: "integer", nullable: false),
                    notuti_note_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_note_utilisateur_notuti", x => x.notuti_id);
                    table.ForeignKey(
                        name: "FK_t_e_note_utilisateur_notuti_t_e_utilisateur_uti_notuti_note~",
                        column: x => x.notuti_note_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_note_utilisateur_notuti_t_e_utilisateur_uti_notuti_not~1",
                        column: x => x.notuti_noteur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_j_abonnement_abo",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    abo_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    abo_utilisateur_suiveur = table.Column<int>(type: "integer", nullable: false),
                    abo_utilisateur_suivis = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_abonnement_abo", x => x.abo_id);
                    table.ForeignKey(
                        name: "FK_t_j_abonnement_abo_t_e_utilisateur_uti_abo_utilisateur_suiv~",
                        column: x => x.abo_utilisateur_suiveur,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_j_abonnement_abo_t_e_utilisateur_uti_abo_utilisateur_sui~1",
                        column: x => x.abo_utilisateur_suivis,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_conversation_con",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    con_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    con_datecreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    con_annonce_id = table.Column<int>(type: "integer", nullable: false),
                    con_statut_conversation_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_conversation_con", x => x.con_id);
                    table.ForeignKey(
                        name: "FK_t_e_conversation_con_t_e_annonce_ann_con_annonce_id",
                        column: x => x.con_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_conversation_con_t_e_statut_conversation_sta_con_statut~",
                        column: x => x.con_statut_conversation_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_statut_conversation_sta",
                        principalColumn: "sta_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_decision_suspension_sus",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    sus_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sus_date_debut_suspension = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sus_date_fin_suspension = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sus_motif_suspension = table.Column<string>(type: "text", nullable: false),
                    sus_utilisateur_id = table.Column<int>(type: "integer", nullable: true),
                    sus_utilisateur_admin_id = table.Column<int>(type: "integer", nullable: true),
                    sus_annonce_id = table.Column<int>(type: "integer", nullable: true),
                    sus_type_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_decision_suspension_sus", x => x.sus_id);
                    table.ForeignKey(
                        name: "FK_t_e_decision_suspension_sus_t_e_annonce_ann_sus_annonce_id",
                        column: x => x.sus_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id");
                    table.ForeignKey(
                        name: "FK_t_e_decision_suspension_sus_t_e_type_suspension_tsu_sus_typ~",
                        column: x => x.sus_type_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_type_suspension_tsu",
                        principalColumn: "tsu_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_decision_suspension_sus_t_e_utilisateur_uti_sus_utilisa~",
                        column: x => x.sus_utilisateur_admin_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id");
                    table.ForeignKey(
                        name: "FK_t_e_decision_suspension_sus_t_e_utilisateur_uti_sus_utilis~1",
                        column: x => x.sus_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id");
                });

            migrationBuilder.CreateTable(
                name: "t_j_est_de_couleur_edc",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    edc_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    edc_couleur_id = table.Column<int>(type: "integer", nullable: false),
                    edc_annonce_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_est_de_couleur_edc", x => x.edc_id);
                    table.ForeignKey(
                        name: "FK_t_j_est_de_couleur_edc_t_e_annonce_ann_edc_annonce_id",
                        column: x => x.edc_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_j_est_de_couleur_edc_t_e_couleur_cou_edc_couleur_id",
                        column: x => x.edc_couleur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_couleur_cou",
                        principalColumn: "cou_id");
                });

            migrationBuilder.CreateTable(
                name: "t_j_favoris_fav",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    fav_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fav_annonce_id = table.Column<int>(type: "integer", nullable: false),
                    fav_utilisateur_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_favoris_fav", x => x.fav_id);
                    table.ForeignKey(
                        name: "FK_t_j_favoris_fav_t_e_annonce_ann_fav_annonce_id",
                        column: x => x.fav_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_j_favoris_fav_t_e_utilisateur_uti_fav_utilisateur_id",
                        column: x => x.fav_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_j_recense_rec",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    rec_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rec_annonce_id = table.Column<int>(type: "integer", nullable: false),
                    rec_tag_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_recense_rec", x => x.rec_id);
                    table.ForeignKey(
                        name: "FK_t_j_recense_rec_t_e_annonce_ann_rec_annonce_id",
                        column: x => x.rec_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id");
                    table.ForeignKey(
                        name: "FK_t_j_recense_rec_t_e_tag_tag_rec_tag_id",
                        column: x => x.rec_tag_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_tag_tag",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_signalement_sig",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    sig_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sig_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sig_motif = table.Column<string>(type: "text", nullable: false),
                    sig_type_id = table.Column<int>(type: "integer", nullable: false),
                    sig_utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    sig_annonce_signalee_id = table.Column<int>(type: "integer", nullable: true),
                    sig_avis_id = table.Column<int>(type: "integer", nullable: true),
                    sig_utilisateur_signale_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_signalement_sig", x => x.sig_id);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_sig_t_e_annonce_ann_sig_annonce_signalee_id",
                        column: x => x.sig_annonce_signalee_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_sig_t_e_note_utilisateur_notuti_sig_avis_id",
                        column: x => x.sig_avis_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_note_utilisateur_notuti",
                        principalColumn: "notuti_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_sig_t_e_signalement_type_sigtyp_sig_type_id",
                        column: x => x.sig_type_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_signalement_type_sigtyp",
                        principalColumn: "sigtype_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_sig_t_e_utilisateur_uti_sig_utilisateur_id",
                        column: x => x.sig_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_sig_t_e_utilisateur_uti_sig_utilisateur_sig~",
                        column: x => x.sig_utilisateur_signale_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_message_mes",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    mes_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mes_date_envoie = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    mes_lu = table.Column<bool>(type: "boolean", nullable: false),
                    mes_utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    mes_conversation_id = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    mextxt_contenue_message = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_message_mes", x => x.mes_id);
                    table.ForeignKey(
                        name: "FK_t_e_message_mes_t_e_conversation_con_mes_conversation_id",
                        column: x => x.mes_conversation_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_conversation_con",
                        principalColumn: "con_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_message_mes_t_e_utilisateur_uti_mes_utilisateur_id",
                        column: x => x.mes_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_transaction_tra",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    tra_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tra_montant = table.Column<int>(type: "integer", nullable: false),
                    tra_transaction_etat = table.Column<int>(type: "integer", nullable: false),
                    tra_conversation_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_transaction_tra", x => x.tra_id);
                    table.ForeignKey(
                        name: "FK_t_e_transaction_tra_t_e_conversation_con_tra_conversation_id",
                        column: x => x.tra_conversation_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_conversation_con",
                        principalColumn: "con_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_j_achete_ach",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    ach_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ach_utilisateur_acheteur_id = table.Column<int>(type: "integer", nullable: false),
                    ach_conversation_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_achete_ach", x => x.ach_id);
                    table.ForeignKey(
                        name: "FK_t_j_achete_ach_t_e_conversation_con_ach_conversation_id",
                        column: x => x.ach_conversation_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_conversation_con",
                        principalColumn: "con_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_j_achete_ach_t_e_utilisateur_uti_ach_utilisateur_acheteur~",
                        column: x => x.ach_utilisateur_acheteur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_j_vend_ven",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    ven_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ven_utilisateur_vendeur_id = table.Column<int>(type: "integer", nullable: false),
                    ven_conversation_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_vend_ven", x => x.ven_id);
                    table.ForeignKey(
                        name: "FK_t_j_vend_ven_t_e_conversation_con_ven_conversation_id",
                        column: x => x.ven_conversation_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_conversation_con",
                        principalColumn: "con_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_j_vend_ven_t_e_utilisateur_uti_ven_utilisateur_vendeur_id",
                        column: x => x.ven_utilisateur_vendeur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_not",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    not_id = table.Column<int>(type: "integer", nullable: false),
                    not_type_id = table.Column<int>(type: "integer", nullable: false),
                    not_utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    not_message_id = table.Column<int>(type: "integer", nullable: true),
                    not_annonce_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notification_not", x => x.not_id);
                    table.ForeignKey(
                        name: "FK_t_e_notification_not_t_e_annonce_ann_not_annonce_id",
                        column: x => x.not_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_notification_not_t_e_message_mes_not_message_id",
                        column: x => x.not_message_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_mes",
                        principalColumn: "mes_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_notification_not_t_e_notification_type_nottyp_not_id",
                        column: x => x.not_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_notification_type_nottyp",
                        principalColumn: "nottyp_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_notification_not_t_e_utilisateur_uti_not_utilisateur_id",
                        column: x => x.not_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_photo_pho",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    pho_photo_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pho_uri_photo = table.Column<string>(type: "text", nullable: false),
                    MessageId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_photo_pho", x => x.pho_photo_id);
                    table.ForeignKey(
                        name: "FK_t_e_photo_pho_t_e_message_mes_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_mes",
                        principalColumn: "mes_id");
                });

            migrationBuilder.CreateTable(
                name: "t_j_illustre_annonce_illann",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    illann_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    illann_annonce_id = table.Column<int>(type: "integer", nullable: false),
                    illann_photo_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_illustre_annonce_illann", x => x.illann_id);
                    table.ForeignKey(
                        name: "FK_t_j_illustre_annonce_illann_t_e_annonce_ann_illann_annonce_~",
                        column: x => x.illann_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id");
                    table.ForeignKey(
                        name: "FK_t_j_illustre_annonce_illann_t_e_photo_pho_illann_photo_id",
                        column: x => x.illann_photo_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_photo_pho",
                        principalColumn: "pho_photo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_e_annonce_ann_ann_categorie_id",
                schema: "sae_clothes2u",
                table: "t_e_annonce_ann",
                column: "ann_categorie_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_annonce_ann_ann_dateannonce",
                schema: "sae_clothes2u",
                table: "t_e_annonce_ann",
                column: "ann_dateannonce");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_annonce_ann_ann_etat_id",
                schema: "sae_clothes2u",
                table: "t_e_annonce_ann",
                column: "ann_etat_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_annonce_ann_ann_marque_id",
                schema: "sae_clothes2u",
                table: "t_e_annonce_ann",
                column: "ann_marque_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_annonce_ann_ann_sous_categorie_id",
                schema: "sae_clothes2u",
                table: "t_e_annonce_ann",
                column: "ann_sous_categorie_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_annonce_ann_ann_statut_annonce_id",
                schema: "sae_clothes2u",
                table: "t_e_annonce_ann",
                column: "ann_statut_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_annonce_ann_ann_taille_id",
                schema: "sae_clothes2u",
                table: "t_e_annonce_ann",
                column: "ann_taille_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_annonce_ann_ann_utilisateur_id",
                schema: "sae_clothes2u",
                table: "t_e_annonce_ann",
                column: "ann_utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_bloque_blo_blo_utilisateur_bloque_id",
                schema: "sae_clothes2u",
                table: "t_e_bloque_blo",
                column: "blo_utilisateur_bloque_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_bloque_blo_blo_utilisateur_bloqueur_id",
                schema: "sae_clothes2u",
                table: "t_e_bloque_blo",
                column: "blo_utilisateur_bloqueur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_conversation_con_con_annonce_id",
                schema: "sae_clothes2u",
                table: "t_e_conversation_con",
                column: "con_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_conversation_con_con_statut_conversation_id",
                schema: "sae_clothes2u",
                table: "t_e_conversation_con",
                column: "con_statut_conversation_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_decision_suspension_sus_sus_annonce_id",
                schema: "sae_clothes2u",
                table: "t_e_decision_suspension_sus",
                column: "sus_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_decision_suspension_sus_sus_type_id",
                schema: "sae_clothes2u",
                table: "t_e_decision_suspension_sus",
                column: "sus_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_decision_suspension_sus_sus_utilisateur_admin_id",
                schema: "sae_clothes2u",
                table: "t_e_decision_suspension_sus",
                column: "sus_utilisateur_admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_decision_suspension_sus_sus_utilisateur_id",
                schema: "sae_clothes2u",
                table: "t_e_decision_suspension_sus",
                column: "sus_utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_message_mes_mes_conversation_id",
                schema: "sae_clothes2u",
                table: "t_e_message_mes",
                column: "mes_conversation_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_message_mes_mes_utilisateur_id",
                schema: "sae_clothes2u",
                table: "t_e_message_mes",
                column: "mes_utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_note_utilisateur_notuti_notuti_note_id",
                schema: "sae_clothes2u",
                table: "t_e_note_utilisateur_notuti",
                column: "notuti_note_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_note_utilisateur_notuti_notuti_noteur_id",
                schema: "sae_clothes2u",
                table: "t_e_note_utilisateur_notuti",
                column: "notuti_noteur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_not_not_annonce_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_not",
                column: "not_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_not_not_message_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_not",
                column: "not_message_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_not_not_utilisateur_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_not",
                column: "not_utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_photo_pho_MessageId",
                schema: "sae_clothes2u",
                table: "t_e_photo_pho",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_signalement_sig_sig_annonce_signalee_id",
                schema: "sae_clothes2u",
                table: "t_e_signalement_sig",
                column: "sig_annonce_signalee_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_signalement_sig_sig_avis_id",
                schema: "sae_clothes2u",
                table: "t_e_signalement_sig",
                column: "sig_avis_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_signalement_sig_sig_type_id",
                schema: "sae_clothes2u",
                table: "t_e_signalement_sig",
                column: "sig_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_signalement_sig_sig_utilisateur_id",
                schema: "sae_clothes2u",
                table: "t_e_signalement_sig",
                column: "sig_utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_signalement_sig_sig_utilisateur_signale_id",
                schema: "sae_clothes2u",
                table: "t_e_signalement_sig",
                column: "sig_utilisateur_signale_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_souscategorie_sscat_sscat_categorie_id",
                schema: "sae_clothes2u",
                table: "t_e_souscategorie_sscat",
                column: "sscat_categorie_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_taille_tai_tai_categorie_taille_id",
                schema: "sae_clothes2u",
                table: "t_e_taille_tai",
                column: "tai_categorie_taille_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_transaction_tra_tra_conversation_id",
                schema: "sae_clothes2u",
                table: "t_e_transaction_tra",
                column: "tra_conversation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_utilisateur_uti_uti_adresse_id",
                schema: "sae_clothes2u",
                table: "t_e_utilisateur_uti",
                column: "uti_adresse_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_utilisateur_uti_uti_dateinscription",
                schema: "sae_clothes2u",
                table: "t_e_utilisateur_uti",
                column: "uti_dateinscription");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_utilisateur_uti_uti_email",
                schema: "sae_clothes2u",
                table: "t_e_utilisateur_uti",
                column: "uti_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_utilisateur_uti_uti_login",
                schema: "sae_clothes2u",
                table: "t_e_utilisateur_uti",
                column: "uti_login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_utilisateur_uti_uti_statut_id",
                schema: "sae_clothes2u",
                table: "t_e_utilisateur_uti",
                column: "uti_statut_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_abonnement_abo_abo_utilisateur_suiveur",
                schema: "sae_clothes2u",
                table: "t_j_abonnement_abo",
                column: "abo_utilisateur_suiveur");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_abonnement_abo_abo_utilisateur_suiveur_abo_utilisateur_~",
                schema: "sae_clothes2u",
                table: "t_j_abonnement_abo",
                columns: new[] { "abo_utilisateur_suiveur", "abo_utilisateur_suivis" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_j_abonnement_abo_abo_utilisateur_suivis",
                schema: "sae_clothes2u",
                table: "t_j_abonnement_abo",
                column: "abo_utilisateur_suivis");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_achete_ach_ach_conversation_id",
                schema: "sae_clothes2u",
                table: "t_j_achete_ach",
                column: "ach_conversation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_j_achete_ach_ach_utilisateur_acheteur_id",
                schema: "sae_clothes2u",
                table: "t_j_achete_ach",
                column: "ach_utilisateur_acheteur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_est_de_couleur_edc_edc_annonce_id",
                schema: "sae_clothes2u",
                table: "t_j_est_de_couleur_edc",
                column: "edc_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_est_de_couleur_edc_edc_couleur_id",
                schema: "sae_clothes2u",
                table: "t_j_est_de_couleur_edc",
                column: "edc_couleur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_favoris_fav_fav_annonce_id",
                schema: "sae_clothes2u",
                table: "t_j_favoris_fav",
                column: "fav_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_favoris_fav_fav_utilisateur_id",
                schema: "sae_clothes2u",
                table: "t_j_favoris_fav",
                column: "fav_utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_illustre_annonce_illann_illann_annonce_id",
                schema: "sae_clothes2u",
                table: "t_j_illustre_annonce_illann",
                column: "illann_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_illustre_annonce_illann_illann_photo_id",
                schema: "sae_clothes2u",
                table: "t_j_illustre_annonce_illann",
                column: "illann_photo_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_recense_rec_rec_annonce_id",
                schema: "sae_clothes2u",
                table: "t_j_recense_rec",
                column: "rec_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_recense_rec_rec_tag_id",
                schema: "sae_clothes2u",
                table: "t_j_recense_rec",
                column: "rec_tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_vend_ven_ven_conversation_id",
                schema: "sae_clothes2u",
                table: "t_j_vend_ven",
                column: "ven_conversation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_j_vend_ven_ven_utilisateur_vendeur_id",
                schema: "sae_clothes2u",
                table: "t_j_vend_ven",
                column: "ven_utilisateur_vendeur_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_e_bloque_blo",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_decision_suspension_sus",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_notification_not",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_signalement_sig",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_transaction_tra",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_j_abonnement_abo",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_j_achete_ach",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_j_est_de_couleur_edc",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_j_favoris_fav",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_j_illustre_annonce_illann",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_j_recense_rec",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_j_vend_ven",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_type_suspension_tsu",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_notification_type_nottyp",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_note_utilisateur_notuti",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_signalement_type_sigtyp",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_couleur_cou",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_photo_pho",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_tag_tag",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_message_mes",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_conversation_con",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_annonce_ann",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_statut_conversation_sta",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_etatarticle_etaart",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_marque_mar",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_souscategorie_sscat",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_statut_annonce_staann",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_taille_tai",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_utilisateur_uti",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_categorie_cat",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_adresse_adr",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_statut_stauti",
                schema: "sae_clothes2u");
        }
    }
}
