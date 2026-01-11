using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UpdBDDAjoutTableChgmtModele : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sae_clothes2u");

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
                name: "t_e_genre_gen",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    gen_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gen_nomgenre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_genre_gen", x => x.gen_id);
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
                name: "t_e_mot_interdit_motint",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    motint_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    motint_libelle_mot = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_mot_interdit_motint", x => x.motint_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_type_nottyp",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    nottyp_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nottyp_libelle_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notification_type_nottyp", x => x.nottyp_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_photo_pho",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    pho_photo_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pho_image = table.Column<byte[]>(type: "bytea", nullable: false),
                    pho_validation = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_photo_pho", x => x.pho_photo_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_role_utilisateur_roluti",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    roluti_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roluti_libelle = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_role_utilisateur_roluti", x => x.roluti_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_signalement_type_sigtyp",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    sigtype_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sigtyp_libelle_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
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
                    sta_libelle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
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
                name: "t_e_taille_tai",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    tai_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tai_libelletaille = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_taille_tai", x => x.tai_id);
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
                name: "t_e_utilisateur_uti",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    uti_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uti_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    uti_telephone = table.Column<string>(type: "text", nullable: true),
                    uti_login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    uti_password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    uti_dateinscription = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    uti_description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    uti_valid_email = table.Column<bool>(type: "boolean", nullable: false),
                    uti_valid_telephone = table.Column<bool>(type: "boolean", nullable: false),
                    uti_preference_notif_mail = table.Column<bool>(type: "boolean", nullable: false),
                    uti_preference_theme = table.Column<bool>(type: "boolean", nullable: false),
                    uti_preference_cookies = table.Column<bool>(type: "boolean", nullable: false),
                    uti_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    uti_statut_id = table.Column<int>(type: "integer", nullable: false),
                    uti_id_photo = table.Column<int>(type: "integer", nullable: true),
                    uti_role_id = table.Column<int>(type: "integer", nullable: false),
                    uti_deleted_by_admin_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_utilisateur_uti", x => x.uti_id);
                    table.ForeignKey(
                        name: "FK_t_e_utilisateur_uti_t_e_photo_pho_uti_id_photo",
                        column: x => x.uti_id_photo,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_photo_pho",
                        principalColumn: "pho_photo_id");
                    table.ForeignKey(
                        name: "FK_t_e_utilisateur_uti_t_e_role_utilisateur_roluti_uti_role_id",
                        column: x => x.uti_role_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_role_utilisateur_roluti",
                        principalColumn: "roluti_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_utilisateur_uti_t_e_statut_stauti_uti_statut_id",
                        column: x => x.uti_statut_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_statut_stauti",
                        principalColumn: "stauti_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_e_utilisateur_uti_t_e_utilisateur_uti_uti_deleted_by_admi~",
                        column: x => x.uti_deleted_by_admin_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_j_mesure_mes",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    mes_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mes_taille_id = table.Column<int>(type: "integer", nullable: false),
                    mes_sous_categorie_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_mesure_mes", x => x.mes_id);
                    table.ForeignKey(
                        name: "FK_t_j_mesure_mes_t_e_souscategorie_sscat_mes_sous_categorie_id",
                        column: x => x.mes_sous_categorie_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_souscategorie_sscat",
                        principalColumn: "sscat_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_j_mesure_mes_t_e_taille_tai_mes_taille_id",
                        column: x => x.mes_taille_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_taille_tai",
                        principalColumn: "tai_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_adresse_adr",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    adr_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    adr_rue = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    adr_ville = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    adr_code_postal = table.Column<string>(type: "text", nullable: false),
                    adr_pays = table.Column<string>(type: "text", nullable: false),
                    adr_is_default = table.Column<bool>(type: "boolean", nullable: false),
                    adr_utilisateur_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_adresse_adr", x => x.adr_id);
                    table.ForeignKey(
                        name: "FK_Adresse_Utilisateur",
                        column: x => x.adr_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
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
                    ann_description = table.Column<string>(type: "text", nullable: false),
                    ann_dateannonce = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ann_negociable = table.Column<bool>(type: "boolean", nullable: false),
                    ann_prix = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ann_utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    ann_etat_id = table.Column<int>(type: "integer", nullable: false),
                    ann_marque_id = table.Column<int>(type: "integer", nullable: false),
                    ann_taille_id = table.Column<int>(type: "integer", nullable: false),
                    ann_sous_categorie_id = table.Column<int>(type: "integer", nullable: false),
                    ann_categorie_id = table.Column<int>(type: "integer", nullable: false),
                    ann_statut_annonce_id = table.Column<int>(type: "integer", nullable: false),
                    ann_genre_id = table.Column<int>(type: "integer", nullable: false)
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
                        name: "FK_t_e_annonce_ann_t_e_genre_gen_ann_genre_id",
                        column: x => x.ann_genre_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_genre_gen",
                        principalColumn: "gen_id",
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_annonce_preference_utilisateur_annprefuti",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    annprefuti_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    annprefuti_marque = table.Column<string>(type: "text", nullable: false),
                    annprefuti_categorie = table.Column<string>(type: "text", nullable: false),
                    annprefuti_souscategorie = table.Column<string>(type: "text", nullable: false),
                    annprefuti_prix = table.Column<double>(type: "double precision", nullable: false),
                    annprefuti_taille = table.Column<string>(type: "text", nullable: false),
                    annprefuti_etat = table.Column<string>(type: "text", nullable: false),
                    annprefuti_ponderarion = table.Column<int>(type: "integer", nullable: false),
                    annprefuti_couleur_dominante = table.Column<string>(type: "text", nullable: false),
                    annprefuti_utilisateur_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_annonce_preference_utilisateur_annprefuti", x => x.annprefuti_id);
                    table.ForeignKey(
                        name: "FK_t_e_annonce_preference_utilisateur_annprefuti_t_e_utilisate~",
                        column: x => x.annprefuti_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_e_bloque_blo_t_e_utilisateur_uti_blo_utilisateur_bloqueur~",
                        column: x => x.blo_utilisateur_bloqueur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_decision_dec",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    dec_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dec_moderateur_id = table.Column<int>(type: "integer", nullable: false),
                    dec_utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    dec_date_decision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_decision_dec", x => x.dec_id);
                    table.ForeignKey(
                        name: "FK_t_e_decision_dec_t_e_utilisateur_uti_dec_moderateur_id",
                        column: x => x.dec_moderateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_decision_dec_t_e_utilisateur_uti_dec_utilisateur_id",
                        column: x => x.dec_utilisateur_id,
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
                    notuti_statut = table.Column<bool>(type: "boolean", nullable: false),
                    notuti_noteur_id = table.Column<int>(type: "integer", nullable: false),
                    notuti_cible_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_note_utilisateur_notuti", x => x.notuti_id);
                    table.ForeignKey(
                        name: "FK_t_e_note_utilisateur_notuti_t_e_utilisateur_uti_notuti_cibl~",
                        column: x => x.notuti_cible_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id");
                    table.ForeignKey(
                        name: "FK_t_e_note_utilisateur_notuti_t_e_utilisateur_uti_notuti_note~",
                        column: x => x.notuti_noteur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_not",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    not_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    not_date_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    not_est_lu = table.Column<bool>(type: "boolean", nullable: false),
                    not_type_id = table.Column<int>(type: "integer", nullable: false),
                    not_utilisateur_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notification_not", x => x.not_id);
                    table.ForeignKey(
                        name: "FK_t_e_notification_not_t_e_notification_type_nottyp_not_type_~",
                        column: x => x.not_type_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_notification_type_nottyp",
                        principalColumn: "nottyp_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_e_notification_not_t_e_utilisateur_uti_not_utilisateur_id",
                        column: x => x.not_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_password_reset",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    reset_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reset_token = table.Column<string>(type: "text", nullable: false),
                    reset_expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reset_used = table.Column<bool>(type: "boolean", nullable: false),
                    reset_utilisateur_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_password_reset", x => x.reset_id);
                    table.ForeignKey(
                        name: "FK_t_e_password_reset_t_e_utilisateur_uti_reset_utilisateur_id",
                        column: x => x.reset_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id");
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
                    sig_utilisateur_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_signalement_sig", x => x.sig_id);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_sig_t_e_signalement_type_sigtyp_sig_type_id",
                        column: x => x.sig_type_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_signalement_type_sigtyp",
                        principalColumn: "sigtype_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_sig_t_e_utilisateur_uti_sig_utilisateur_id",
                        column: x => x.sig_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_verification_code_ver",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    ver_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ver_utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    ver_code = table.Column<string>(type: "text", nullable: false),
                    ver_type = table.Column<int>(type: "integer", nullable: false),
                    ver_date_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ver_date_expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ver_est_utilise = table.Column<bool>(type: "boolean", nullable: false),
                    ver_tentatives = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_verification_code_ver", x => x.ver_id);
                    table.ForeignKey(
                        name: "FK_t_e_verification_code_ver_t_e_utilisateur_uti_ver_utilisate~",
                        column: x => x.ver_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_h_utilisateur_transactions_histuti",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    histuti_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    histuti_utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    histuti_type_transaction = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    histuti_annonce_id = table.Column<int>(type: "integer", nullable: true),
                    histuti_montant = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    histuti_date_transaction = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    histuti_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    histuti_deleted_by_admin_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_h_utilisateur_transactions_histuti", x => x.histuti_id);
                    table.ForeignKey(
                        name: "FK_t_h_utilisateur_transactions_histuti_t_e_utilisateur_uti_hi~",
                        column: x => x.histuti_deleted_by_admin_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_h_utilisateur_transactions_histuti_t_e_utilisateur_uti_h~1",
                        column: x => x.histuti_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "t_e_commande_cmd",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    cmd_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cmd_date_commande = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cmd_montant_total = table.Column<decimal>(type: "numeric", nullable: false),
                    cmd_frais_service = table.Column<decimal>(type: "numeric", nullable: false),
                    cmd_frais_livraison = table.Column<decimal>(type: "numeric", nullable: false),
                    cmd_statut = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cmd_stripe_payment_intent_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    cmd_numero_suivi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cmd_date_expedition = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cmd_date_livraison = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cmd_annonce_id = table.Column<int>(type: "integer", nullable: false),
                    cmd_acheteur_id = table.Column<int>(type: "integer", nullable: false),
                    cmd_vendeur_id = table.Column<int>(type: "integer", nullable: false),
                    cmd_adresse_livraison_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_commande_cmd", x => x.cmd_id);
                    table.ForeignKey(
                        name: "FK_Commande_Acheteur",
                        column: x => x.cmd_acheteur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commande_AdresseLivraison",
                        column: x => x.cmd_adresse_livraison_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_adresse_adr",
                        principalColumn: "adr_id");
                    table.ForeignKey(
                        name: "FK_Commande_Annonce",
                        column: x => x.cmd_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id");
                    table.ForeignKey(
                        name: "FK_Commande_Vendeur",
                        column: x => x.cmd_vendeur_id,
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
                    con_statut_conversation_id = table.Column<int>(type: "integer", nullable: false),
                    con_prix = table.Column<decimal>(type: "numeric", nullable: false)
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
                        principalColumn: "cou_id",
                        onDelete: ReferentialAction.Restrict);
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
                        principalColumn: "ann_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_j_illustre_annonce_illann_t_e_photo_pho_illann_photo_id",
                        column: x => x.illann_photo_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_photo_pho",
                        principalColumn: "pho_photo_id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "t_j_visualisation_vis",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    vis_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vis_utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    vis_annonce_id = table.Column<int>(type: "integer", nullable: false),
                    vis_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_visualisation_vis", x => x.vis_id);
                    table.ForeignKey(
                        name: "FK_t_j_visualisation_vis_t_e_annonce_ann_vis_annonce_id",
                        column: x => x.vis_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_j_visualisation_vis_t_e_utilisateur_uti_vis_utilisateur_id",
                        column: x => x.vis_utilisateur_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_decision_avertissement_decave",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    decave_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    decave_decision_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_decision_avertissement_decave", x => x.decave_id);
                    table.ForeignKey(
                        name: "FK_t_e_decision_avertissement_decave_t_e_decision_dec_decave_d~",
                        column: x => x.decave_decision_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_decision_dec",
                        principalColumn: "dec_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_demande_restauration_demres",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    demres_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    demres_message = table.Column<string>(type: "text", nullable: false),
                    demres_statut = table.Column<string>(type: "text", nullable: false),
                    demres_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    demres_decision_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_demande_restauration_demres", x => x.demres_id);
                    table.ForeignKey(
                        name: "FK_t_e_demande_restauration_demres_t_e_decision_dec_demres_dec~",
                        column: x => x.demres_decision_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_decision_dec",
                        principalColumn: "dec_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_element_decision_eledec",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    eledec_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    eledec_sanction_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_element_decision_eledec", x => x.eledec_id);
                    table.ForeignKey(
                        name: "FK_t_e_element_decision_eledec_t_e_decision_dec_eledec_sanctio~",
                        column: x => x.eledec_sanction_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_decision_dec",
                        principalColumn: "dec_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_sanction_san",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    san_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    san_en_cours = table.Column<bool>(type: "boolean", nullable: false),
                    san_decision_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_sanction_san", x => x.san_id);
                    table.ForeignKey(
                        name: "FK_t_e_sanction_san_t_e_decision_dec_san_decision_id",
                        column: x => x.san_decision_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_decision_dec",
                        principalColumn: "dec_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_achat_notach",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    notach_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notach_annonce_id = table.Column<int>(type: "integer", nullable: false),
                    notach_notification_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notification_achat_notach", x => x.notach_id);
                    table.ForeignKey(
                        name: "FK_t_e_notification_achat_notach_t_e_annonce_ann_notach_annonc~",
                        column: x => x.notach_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_notification_achat_notach_t_e_notification_not_notach_n~",
                        column: x => x.notach_notification_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_notification_not",
                        principalColumn: "not_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_admin_notadm",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    notadm_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notadm_admin_text = table.Column<string>(type: "text", nullable: false),
                    notadm_notification_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notification_admin_notadm", x => x.notadm_id);
                    table.ForeignKey(
                        name: "FK_t_e_notification_admin_notadm_t_e_notification_not_notadm_n~",
                        column: x => x.notadm_notification_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_notification_not",
                        principalColumn: "not_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_avertissement_notave",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    notave_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notave_avertissement_message = table.Column<string>(type: "text", nullable: false),
                    notave_notification_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notification_avertissement_notave", x => x.notave_id);
                    table.ForeignKey(
                        name: "FK_t_e_notification_avertissement_notave_t_e_notification_not_~",
                        column: x => x.notave_notification_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_notification_not",
                        principalColumn: "not_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_modifiaction_notmod",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    notmod_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notmod_annonce_id = table.Column<int>(type: "integer", nullable: false),
                    notmod_notification_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notification_modifiaction_notmod", x => x.notmod_id);
                    table.ForeignKey(
                        name: "FK_t_e_notification_modifiaction_notmod_t_e_annonce_ann_notmod~",
                        column: x => x.notmod_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_e_notification_modifiaction_notmod_t_e_notification_not_n~",
                        column: x => x.notmod_notification_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_notification_not",
                        principalColumn: "not_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_nouvelle_annonce_notnou",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    notnou_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notnou_annonce_id = table.Column<int>(type: "integer", nullable: false),
                    notnou_notification_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notification_nouvelle_annonce_notnou", x => x.notnou_id);
                    table.ForeignKey(
                        name: "FK_t_e_notification_nouvelle_annonce_notnou_t_e_annonce_ann_no~",
                        column: x => x.notnou_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_e_notification_nouvelle_annonce_notnou_t_e_notification_n~",
                        column: x => x.notnou_notification_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_notification_not",
                        principalColumn: "not_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_signalement_annonce_sigan",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    sigan_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sigan_annonce_signalee_id = table.Column<int>(type: "integer", nullable: false),
                    sigan_signalement_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_signalement_annonce_sigan", x => x.sigan_id);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_annonce_sigan_t_e_annonce_ann_sigan_annonce~",
                        column: x => x.sigan_annonce_signalee_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_annonce_sigan_t_e_signalement_sig_sigan_sig~",
                        column: x => x.sigan_signalement_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_signalement_sig",
                        principalColumn: "sig_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_signalement_avis_sigavs",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    sigavs_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sigavs_avis_id = table.Column<int>(type: "integer", nullable: false),
                    sigavs_signalement_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_signalement_avis_sigavs", x => x.sigavs_id);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_avis_sigavs_t_e_note_utilisateur_notuti_sig~",
                        column: x => x.sigavs_avis_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_note_utilisateur_notuti",
                        principalColumn: "notuti_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_avis_sigavs_t_e_signalement_sig_sigavs_sign~",
                        column: x => x.sigavs_signalement_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_signalement_sig",
                        principalColumn: "sig_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_signalement_utilisateur_siguti",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    siguti_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    siguti_utilisateur_signale_id = table.Column<int>(type: "integer", nullable: false),
                    siguti_signalement_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_signalement_utilisateur_siguti", x => x.siguti_id);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_utilisateur_siguti_t_e_signalement_sig_sigu~",
                        column: x => x.siguti_signalement_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_signalement_sig",
                        principalColumn: "sig_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_utilisateur_siguti_t_e_utilisateur_uti_sigu~",
                        column: x => x.siguti_utilisateur_signale_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_utilisateur_uti",
                        principalColumn: "uti_id",
                        onDelete: ReferentialAction.Restrict);
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
                    mes_statut = table.Column<bool>(type: "boolean", nullable: false),
                    mes_utilisateur_id = table.Column<int>(type: "integer", nullable: false),
                    mes_conversation_id = table.Column<int>(type: "integer", nullable: false)
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
                name: "t_e_element_decision_annonce_eda",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    eda_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    eda_annonce_id = table.Column<int>(type: "integer", nullable: false),
                    eda_element_decision_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_element_decision_annonce_eda", x => x.eda_id);
                    table.ForeignKey(
                        name: "FK_t_e_element_decision_annonce_eda_t_e_annonce_ann_eda_annonc~",
                        column: x => x.eda_annonce_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_annonce_ann",
                        principalColumn: "ann_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_element_decision_annonce_eda_t_e_element_decision_elede~",
                        column: x => x.eda_element_decision_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_element_decision_eledec",
                        principalColumn: "eledec_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_element_decision_avis_edav",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    edav_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    edav_avis_id = table.Column<int>(type: "integer", nullable: false),
                    edav_element_decision_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_element_decision_avis_edav", x => x.edav_id);
                    table.ForeignKey(
                        name: "FK_t_e_element_decision_avis_edav_t_e_element_decision_eledec_~",
                        column: x => x.edav_element_decision_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_element_decision_eledec",
                        principalColumn: "eledec_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_element_decision_avis_edav_t_e_note_utilisateur_notuti_~",
                        column: x => x.edav_avis_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_note_utilisateur_notuti",
                        principalColumn: "notuti_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_element_decision_utilisateur_edu",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    edu_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    edu_element_decision_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_element_decision_utilisateur_edu", x => x.edu_id);
                    table.ForeignKey(
                        name: "FK_t_e_element_decision_utilisateur_edu_t_e_element_decision_e~",
                        column: x => x.edu_element_decision_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_element_decision_eledec",
                        principalColumn: "eledec_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_sanction_bannissement_sanban",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    sanban_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sanban_decision_sanction_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_sanction_bannissement_sanban", x => x.sanban_id);
                    table.ForeignKey(
                        name: "FK_t_e_sanction_bannissement_sanban_t_e_sanction_san_sanban_de~",
                        column: x => x.sanban_decision_sanction_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_sanction_san",
                        principalColumn: "san_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_sanction_suspension_sansus",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    sansus_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sansus_date_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sansus_decision_sanction_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_sanction_suspension_sansus", x => x.sansus_id);
                    table.ForeignKey(
                        name: "FK_t_e_sanction_suspension_sansus_t_e_sanction_san_sansus_deci~",
                        column: x => x.sansus_decision_sanction_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_sanction_san",
                        principalColumn: "san_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_element_decision_message_edm",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    edm_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    edm_message_id = table.Column<int>(type: "integer", nullable: false),
                    edm_element_decision_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_element_decision_message_edm", x => x.edm_id);
                    table.ForeignKey(
                        name: "FK_t_e_element_decision_message_edm_t_e_element_decision_elede~",
                        column: x => x.edm_element_decision_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_element_decision_eledec",
                        principalColumn: "eledec_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_element_decision_message_edm_t_e_message_mes_edm_messag~",
                        column: x => x.edm_message_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_mes",
                        principalColumn: "mes_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_message_demande_mesdem",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    mesdem_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mesdem_prix_propose = table.Column<decimal>(type: "numeric", nullable: false),
                    mesdem_est_acceptee = table.Column<bool>(type: "boolean", nullable: false),
                    mesdem_est_en_attente = table.Column<bool>(type: "boolean", nullable: false),
                    mesdem_message_id = table.Column<int>(type: "integer", nullable: false),
                    mesdem_demande_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_message_demande_mesdem", x => x.mesdem_id);
                    table.ForeignKey(
                        name: "FK_t_e_message_demande_mesdem_t_e_message_demande_mesdem_mesde~",
                        column: x => x.mesdem_demande_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_demande_mesdem",
                        principalColumn: "mesdem_id");
                    table.ForeignKey(
                        name: "FK_t_e_message_demande_mesdem_t_e_message_mes_mesdem_message_id",
                        column: x => x.mesdem_message_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_mes",
                        principalColumn: "mes_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_message_payee_mespay",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    mespay_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mespay_message_id = table.Column<int>(type: "integer", nullable: false),
                    mespay_est_acceptee = table.Column<bool>(type: "boolean", nullable: false),
                    mespay_est_annulee = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_message_payee_mespay", x => x.mespay_id);
                    table.ForeignKey(
                        name: "FK_t_e_message_payee_mespay_t_e_message_mes_mespay_message_id",
                        column: x => x.mespay_message_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_mes",
                        principalColumn: "mes_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_message_texte_mestex",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    mestex_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mestex_contenu_message = table.Column<string>(type: "text", nullable: false),
                    mestex_message_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_message_texte_mestex", x => x.mestex_id);
                    table.ForeignKey(
                        name: "FK_t_e_message_texte_mestex_t_e_message_mes_mestex_message_id",
                        column: x => x.mestex_message_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_mes",
                        principalColumn: "mes_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_message_notmes",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    notmes_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notmes_message_id = table.Column<int>(type: "integer", nullable: false),
                    notmes_message_preview = table.Column<string>(type: "text", nullable: false),
                    notmes_notification_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notification_message_notmes", x => x.notmes_id);
                    table.ForeignKey(
                        name: "FK_t_e_notification_message_notmes_t_e_message_mes_notmes_mess~",
                        column: x => x.notmes_message_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_mes",
                        principalColumn: "mes_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_notification_message_notmes_t_e_notification_not_notmes~",
                        column: x => x.notmes_notification_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_notification_not",
                        principalColumn: "not_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_signalement_message_sigmes",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    sigmes_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sigmes_signalement_id = table.Column<int>(type: "integer", nullable: false),
                    sigmes_message_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_signalement_message_sigmes", x => x.sigmes_id);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_message_sigmes_t_e_message_mes_sigmes_messa~",
                        column: x => x.sigmes_message_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_mes",
                        principalColumn: "mes_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_e_signalement_message_sigmes_t_e_signalement_sig_sigmes_s~",
                        column: x => x.sigmes_signalement_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_signalement_sig",
                        principalColumn: "sig_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_proposition_notpro",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    notpro_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notpro_proposition_id = table.Column<int>(type: "integer", nullable: false),
                    notpro_notification_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notification_proposition_notpro", x => x.notpro_id);
                    table.ForeignKey(
                        name: "FK_t_e_notification_proposition_notpro_t_e_message_demande_mes~",
                        column: x => x.notpro_proposition_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_demande_mesdem",
                        principalColumn: "mesdem_id");
                    table.ForeignKey(
                        name: "FK_t_e_notification_proposition_notpro_t_e_notification_not_no~",
                        column: x => x.notpro_notification_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_notification_not",
                        principalColumn: "not_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_message_envoie_colis_mesenvcol",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    mesenvcol_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mesenvcol_message_id = table.Column<int>(type: "integer", nullable: false),
                    mesenvcol_photo_id = table.Column<int>(type: "integer", nullable: false),
                    mesenvcol_est_payee_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_message_envoie_colis_mesenvcol", x => x.mesenvcol_id);
                    table.ForeignKey(
                        name: "FK_t_e_message_envoie_colis_mesenvcol_t_e_message_mes_mesenvco~",
                        column: x => x.mesenvcol_message_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_mes",
                        principalColumn: "mes_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_message_envoie_colis_mesenvcol_t_e_message_payee_mespay~",
                        column: x => x.mesenvcol_est_payee_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_payee_mespay",
                        principalColumn: "mespay_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_message_envoie_colis_mesenvcol_t_e_photo_pho_mesenvcol_~",
                        column: x => x.mesenvcol_photo_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_photo_pho",
                        principalColumn: "pho_photo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_j_message_contient_image_messconima",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    messconima_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    messconima_message_texte_id = table.Column<int>(type: "integer", nullable: false),
                    messconima_image_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_message_contient_image_messconima", x => x.messconima_id);
                    table.ForeignKey(
                        name: "FK_t_j_message_contient_image_messconima_t_e_message_texte_mes~",
                        column: x => x.messconima_message_texte_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_texte_mestex",
                        principalColumn: "mestex_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_j_message_contient_image_messconima_t_e_photo_pho_messcon~",
                        column: x => x.messconima_image_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_photo_pho",
                        principalColumn: "pho_photo_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_message_recu_mesrecu",
                schema: "sae_clothes2u",
                columns: table => new
                {
                    mesrecu_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mesrecu_est_conforme = table.Column<bool>(type: "boolean", nullable: false),
                    mesrecu_photo_id = table.Column<int>(type: "integer", nullable: true),
                    mesrecu_description = table.Column<string>(type: "text", nullable: true),
                    mesrecu_message_id = table.Column<int>(type: "integer", nullable: false),
                    mesrecu_est_envoie_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_message_recu_mesrecu", x => x.mesrecu_id);
                    table.ForeignKey(
                        name: "FK_t_e_message_recu_mesrecu_t_e_message_envoie_colis_mesenvcol~",
                        column: x => x.mesrecu_est_envoie_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_envoie_colis_mesenvcol",
                        principalColumn: "mesenvcol_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_message_recu_mesrecu_t_e_message_mes_mesrecu_message_id",
                        column: x => x.mesrecu_message_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_message_mes",
                        principalColumn: "mes_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_e_message_recu_mesrecu_t_e_photo_pho_mesrecu_photo_id",
                        column: x => x.mesrecu_photo_id,
                        principalSchema: "sae_clothes2u",
                        principalTable: "t_e_photo_pho",
                        principalColumn: "pho_photo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_e_adresse_adr_adr_utilisateur_id",
                schema: "sae_clothes2u",
                table: "t_e_adresse_adr",
                column: "adr_utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "idx_annonce_genre",
                schema: "sae_clothes2u",
                table: "t_e_annonce_ann",
                column: "ann_genre_id");

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
                name: "IX_t_e_annonce_preference_utilisateur_annprefuti_annprefuti_ut~",
                schema: "sae_clothes2u",
                table: "t_e_annonce_preference_utilisateur_annprefuti",
                column: "annprefuti_utilisateur_id");

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
                name: "IX_t_e_commande_cmd_cmd_acheteur_id",
                schema: "sae_clothes2u",
                table: "t_e_commande_cmd",
                column: "cmd_acheteur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_commande_cmd_cmd_adresse_livraison_id",
                schema: "sae_clothes2u",
                table: "t_e_commande_cmd",
                column: "cmd_adresse_livraison_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_commande_cmd_cmd_annonce_id",
                schema: "sae_clothes2u",
                table: "t_e_commande_cmd",
                column: "cmd_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_commande_cmd_cmd_vendeur_id",
                schema: "sae_clothes2u",
                table: "t_e_commande_cmd",
                column: "cmd_vendeur_id");

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
                name: "IX_t_e_decision_avertissement_decave_decave_decision_id",
                schema: "sae_clothes2u",
                table: "t_e_decision_avertissement_decave",
                column: "decave_decision_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_decision_dec_dec_moderateur_id",
                schema: "sae_clothes2u",
                table: "t_e_decision_dec",
                column: "dec_moderateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_decision_dec_dec_utilisateur_id",
                schema: "sae_clothes2u",
                table: "t_e_decision_dec",
                column: "dec_utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_demande_restauration_demres_demres_decision_id",
                schema: "sae_clothes2u",
                table: "t_e_demande_restauration_demres",
                column: "demres_decision_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_element_decision_annonce_eda_eda_annonce_id",
                schema: "sae_clothes2u",
                table: "t_e_element_decision_annonce_eda",
                column: "eda_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_element_decision_annonce_eda_eda_element_decision_id",
                schema: "sae_clothes2u",
                table: "t_e_element_decision_annonce_eda",
                column: "eda_element_decision_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_element_decision_avis_edav_edav_avis_id",
                schema: "sae_clothes2u",
                table: "t_e_element_decision_avis_edav",
                column: "edav_avis_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_element_decision_avis_edav_edav_element_decision_id",
                schema: "sae_clothes2u",
                table: "t_e_element_decision_avis_edav",
                column: "edav_element_decision_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_element_decision_eledec_eledec_sanction_id",
                schema: "sae_clothes2u",
                table: "t_e_element_decision_eledec",
                column: "eledec_sanction_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_element_decision_message_edm_edm_element_decision_id",
                schema: "sae_clothes2u",
                table: "t_e_element_decision_message_edm",
                column: "edm_element_decision_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_element_decision_message_edm_edm_message_id",
                schema: "sae_clothes2u",
                table: "t_e_element_decision_message_edm",
                column: "edm_message_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_element_decision_utilisateur_edu_edu_element_decision_id",
                schema: "sae_clothes2u",
                table: "t_e_element_decision_utilisateur_edu",
                column: "edu_element_decision_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_genre_gen_gen_id",
                schema: "sae_clothes2u",
                table: "t_e_genre_gen",
                column: "gen_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_genre_gen_gen_nomgenre",
                schema: "sae_clothes2u",
                table: "t_e_genre_gen",
                column: "gen_nomgenre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_message_demande_mesdem_mesdem_demande_id",
                schema: "sae_clothes2u",
                table: "t_e_message_demande_mesdem",
                column: "mesdem_demande_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_message_demande_mesdem_mesdem_message_id",
                schema: "sae_clothes2u",
                table: "t_e_message_demande_mesdem",
                column: "mesdem_message_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_message_envoie_colis_mesenvcol_mesenvcol_est_payee_id",
                schema: "sae_clothes2u",
                table: "t_e_message_envoie_colis_mesenvcol",
                column: "mesenvcol_est_payee_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_message_envoie_colis_mesenvcol_mesenvcol_message_id",
                schema: "sae_clothes2u",
                table: "t_e_message_envoie_colis_mesenvcol",
                column: "mesenvcol_message_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_message_envoie_colis_mesenvcol_mesenvcol_photo_id",
                schema: "sae_clothes2u",
                table: "t_e_message_envoie_colis_mesenvcol",
                column: "mesenvcol_photo_id",
                unique: true);

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
                name: "IX_t_e_message_payee_mespay_mespay_message_id",
                schema: "sae_clothes2u",
                table: "t_e_message_payee_mespay",
                column: "mespay_message_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_message_recu_mesrecu_mesrecu_est_envoie_id",
                schema: "sae_clothes2u",
                table: "t_e_message_recu_mesrecu",
                column: "mesrecu_est_envoie_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_message_recu_mesrecu_mesrecu_message_id",
                schema: "sae_clothes2u",
                table: "t_e_message_recu_mesrecu",
                column: "mesrecu_message_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_message_recu_mesrecu_mesrecu_photo_id",
                schema: "sae_clothes2u",
                table: "t_e_message_recu_mesrecu",
                column: "mesrecu_photo_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_message_texte_mestex_mestex_message_id",
                schema: "sae_clothes2u",
                table: "t_e_message_texte_mestex",
                column: "mestex_message_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_mot_interdit_motint_motint_libelle_mot",
                schema: "sae_clothes2u",
                table: "t_e_mot_interdit_motint",
                column: "motint_libelle_mot",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_note_utilisateur_notuti_notuti_cible_id",
                schema: "sae_clothes2u",
                table: "t_e_note_utilisateur_notuti",
                column: "notuti_cible_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_note_utilisateur_notuti_notuti_noteur_id",
                schema: "sae_clothes2u",
                table: "t_e_note_utilisateur_notuti",
                column: "notuti_noteur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_achat_notach_notach_annonce_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_achat_notach",
                column: "notach_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_achat_notach_notach_notification_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_achat_notach",
                column: "notach_notification_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_admin_notadm_notadm_notification_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_admin_notadm",
                column: "notadm_notification_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_avertissement_notave_notave_notification_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_avertissement_notave",
                column: "notave_notification_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_message_notmes_notmes_message_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_message_notmes",
                column: "notmes_message_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_message_notmes_notmes_notification_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_message_notmes",
                column: "notmes_notification_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_modifiaction_notmod_notmod_annonce_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_modifiaction_notmod",
                column: "notmod_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_modifiaction_notmod_notmod_notification_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_modifiaction_notmod",
                column: "notmod_notification_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_not_not_type_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_not",
                column: "not_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_not_not_utilisateur_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_not",
                column: "not_utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_nouvelle_annonce_notnou_notnou_annonce_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_nouvelle_annonce_notnou",
                column: "notnou_annonce_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_nouvelle_annonce_notnou_notnou_notificatio~",
                schema: "sae_clothes2u",
                table: "t_e_notification_nouvelle_annonce_notnou",
                column: "notnou_notification_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_proposition_notpro_notpro_notification_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_proposition_notpro",
                column: "notpro_notification_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_proposition_notpro_notpro_proposition_id",
                schema: "sae_clothes2u",
                table: "t_e_notification_proposition_notpro",
                column: "notpro_proposition_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_password_reset_utilisateur",
                schema: "sae_clothes2u",
                table: "t_e_password_reset",
                column: "reset_utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_sanction_bannissement_sanban_sanban_decision_sanction_id",
                schema: "sae_clothes2u",
                table: "t_e_sanction_bannissement_sanban",
                column: "sanban_decision_sanction_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_sanction_san_san_decision_id",
                schema: "sae_clothes2u",
                table: "t_e_sanction_san",
                column: "san_decision_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_sanction_suspension_sansus_sansus_decision_sanction_id",
                schema: "sae_clothes2u",
                table: "t_e_sanction_suspension_sansus",
                column: "sansus_decision_sanction_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_signalement_annonce_annonce",
                schema: "sae_clothes2u",
                table: "t_e_signalement_annonce_sigan",
                column: "sigan_annonce_signalee_id");

            migrationBuilder.CreateIndex(
                name: "idx_signalement_annonce_signalement_unique",
                schema: "sae_clothes2u",
                table: "t_e_signalement_annonce_sigan",
                column: "sigan_signalement_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_signalement_avis_avis",
                schema: "sae_clothes2u",
                table: "t_e_signalement_avis_sigavs",
                column: "sigavs_avis_id");

            migrationBuilder.CreateIndex(
                name: "idx_signalement_avis_signalement_unique",
                schema: "sae_clothes2u",
                table: "t_e_signalement_avis_sigavs",
                column: "sigavs_signalement_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_signalement_message_sigmes_sigmes_message_id",
                schema: "sae_clothes2u",
                table: "t_e_signalement_message_sigmes",
                column: "sigmes_message_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_signalement_message_sigmes_sigmes_signalement_id",
                schema: "sae_clothes2u",
                table: "t_e_signalement_message_sigmes",
                column: "sigmes_signalement_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_signalement_date",
                schema: "sae_clothes2u",
                table: "t_e_signalement_sig",
                column: "sig_date");

            migrationBuilder.CreateIndex(
                name: "idx_signalement_type",
                schema: "sae_clothes2u",
                table: "t_e_signalement_sig",
                column: "sig_type_id");

            migrationBuilder.CreateIndex(
                name: "idx_signalement_utilisateur",
                schema: "sae_clothes2u",
                table: "t_e_signalement_sig",
                column: "sig_utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "idx_type_signalement_libelle_unique",
                schema: "sae_clothes2u",
                table: "t_e_signalement_type_sigtyp",
                column: "sigtyp_libelle_type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_signalement_utilisateur_signalement_unique",
                schema: "sae_clothes2u",
                table: "t_e_signalement_utilisateur_siguti",
                column: "siguti_signalement_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_signalement_utilisateur_utilisateur",
                schema: "sae_clothes2u",
                table: "t_e_signalement_utilisateur_siguti",
                column: "siguti_utilisateur_signale_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_souscategorie_sscat_sscat_categorie_id",
                schema: "sae_clothes2u",
                table: "t_e_souscategorie_sscat",
                column: "sscat_categorie_id");

            migrationBuilder.CreateIndex(
                name: "idx_statut_conversation_libelle_unique",
                schema: "sae_clothes2u",
                table: "t_e_statut_conversation_sta",
                column: "sta_libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_transaction_conversation_unique",
                schema: "sae_clothes2u",
                table: "t_e_transaction_tra",
                column: "tra_conversation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_transaction_etat",
                schema: "sae_clothes2u",
                table: "t_e_transaction_tra",
                column: "tra_transaction_etat");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_utilisateur_uti_uti_dateinscription",
                schema: "sae_clothes2u",
                table: "t_e_utilisateur_uti",
                column: "uti_dateinscription");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_utilisateur_uti_uti_deleted_by_admin_id",
                schema: "sae_clothes2u",
                table: "t_e_utilisateur_uti",
                column: "uti_deleted_by_admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_utilisateur_uti_uti_email",
                schema: "sae_clothes2u",
                table: "t_e_utilisateur_uti",
                column: "uti_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_utilisateur_uti_uti_id_photo",
                schema: "sae_clothes2u",
                table: "t_e_utilisateur_uti",
                column: "uti_id_photo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_utilisateur_uti_uti_login",
                schema: "sae_clothes2u",
                table: "t_e_utilisateur_uti",
                column: "uti_login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_utilisateur_uti_uti_role_id",
                schema: "sae_clothes2u",
                table: "t_e_utilisateur_uti",
                column: "uti_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_utilisateur_uti_uti_statut_id",
                schema: "sae_clothes2u",
                table: "t_e_utilisateur_uti",
                column: "uti_statut_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_verification_code_ver_ver_utilisateur_id",
                schema: "sae_clothes2u",
                table: "t_e_verification_code_ver",
                column: "ver_utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_h_utilisateur_transactions_histuti_histuti_deleted_by_adm~",
                schema: "sae_clothes2u",
                table: "t_h_utilisateur_transactions_histuti",
                column: "histuti_deleted_by_admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_h_utilisateur_transactions_histuti_histuti_utilisateur_id",
                schema: "sae_clothes2u",
                table: "t_h_utilisateur_transactions_histuti",
                column: "histuti_utilisateur_id");

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
                name: "idx_achete_conversation_unique",
                schema: "sae_clothes2u",
                table: "t_j_achete_ach",
                column: "ach_conversation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_achete_utilisateur",
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
                name: "IX_t_j_illustre_annonce_illann_illann_annonce_id_illann_photo_~",
                schema: "sae_clothes2u",
                table: "t_j_illustre_annonce_illann",
                columns: new[] { "illann_annonce_id", "illann_photo_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_j_illustre_annonce_illann_illann_photo_id",
                schema: "sae_clothes2u",
                table: "t_j_illustre_annonce_illann",
                column: "illann_photo_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_message_contient_image_messconima_messconima_image_id",
                schema: "sae_clothes2u",
                table: "t_j_message_contient_image_messconima",
                column: "messconima_image_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_message_contient_image_messconima_messconima_message_te~",
                schema: "sae_clothes2u",
                table: "t_j_message_contient_image_messconima",
                columns: new[] { "messconima_message_texte_id", "messconima_image_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_j_mesure_mes_mes_sous_categorie_id",
                schema: "sae_clothes2u",
                table: "t_j_mesure_mes",
                column: "mes_sous_categorie_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_mesure_mes_mes_taille_id_mes_sous_categorie_id",
                schema: "sae_clothes2u",
                table: "t_j_mesure_mes",
                columns: new[] { "mes_taille_id", "mes_sous_categorie_id" },
                unique: true);

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
                name: "idx_vend_conversation_unique",
                schema: "sae_clothes2u",
                table: "t_j_vend_ven",
                column: "ven_conversation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_vend_utilisateur",
                schema: "sae_clothes2u",
                table: "t_j_vend_ven",
                column: "ven_utilisateur_vendeur_id");

            migrationBuilder.CreateIndex(
                name: "idx_visualisation_annonce_annonceid",
                schema: "sae_clothes2u",
                table: "t_j_visualisation_vis",
                column: "vis_annonce_id");

            migrationBuilder.CreateIndex(
                name: "idx_visualisation_utilisateur_utilisateurid",
                schema: "sae_clothes2u",
                table: "t_j_visualisation_vis",
                column: "vis_utilisateur_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_e_annonce_preference_utilisateur_annprefuti",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_bloque_blo",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_commande_cmd",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_decision_avertissement_decave",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_demande_restauration_demres",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_element_decision_annonce_eda",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_element_decision_avis_edav",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_element_decision_message_edm",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_element_decision_utilisateur_edu",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_message_recu_mesrecu",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_mot_interdit_motint",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_notification_achat_notach",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_notification_admin_notadm",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_notification_avertissement_notave",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_notification_message_notmes",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_notification_modifiaction_notmod",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_notification_nouvelle_annonce_notnou",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_notification_proposition_notpro",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_password_reset",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_sanction_bannissement_sanban",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_sanction_suspension_sansus",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_signalement_annonce_sigan",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_signalement_avis_sigavs",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_signalement_message_sigmes",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_signalement_utilisateur_siguti",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_transaction_tra",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_verification_code_ver",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_h_utilisateur_transactions_histuti",
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
                name: "t_j_message_contient_image_messconima",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_j_mesure_mes",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_j_recense_rec",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_j_vend_ven",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_j_visualisation_vis",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_adresse_adr",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_element_decision_eledec",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_message_envoie_colis_mesenvcol",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_message_demande_mesdem",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_notification_not",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_sanction_san",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_note_utilisateur_notuti",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_signalement_sig",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_couleur_cou",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_message_texte_mestex",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_tag_tag",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_message_payee_mespay",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_notification_type_nottyp",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_decision_dec",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_signalement_type_sigtyp",
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
                name: "t_e_genre_gen",
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
                name: "t_e_photo_pho",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_role_utilisateur_roluti",
                schema: "sae_clothes2u");

            migrationBuilder.DropTable(
                name: "t_e_statut_stauti",
                schema: "sae_clothes2u");
        }
    }
}
