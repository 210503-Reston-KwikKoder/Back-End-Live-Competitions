// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LiveCompetitionDL.Migrations
{
    [DbContext(typeof(CBEDbContext))]
    [Migration("20210703230934_LastTestMigration")]
    partial class LastTestMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CBEModels.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Name")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("CBEModels.Competition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("CompetitionName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TestAuthor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TestString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserCreatedId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserCreatedId");

                    b.ToTable("Competitions");
                });

            modelBuilder.Entity("CBEModels.CompetitionStat", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("CompetitionId")
                        .HasColumnType("int");

                    b.Property<double>("Accuracy")
                        .HasColumnType("float");

                    b.Property<double>("WPM")
                        .HasColumnType("float");

                    b.Property<int>("rank")
                        .HasColumnType("int");

                    b.HasKey("UserId", "CompetitionId");

                    b.HasIndex("CompetitionId");

                    b.ToTable("CompetitionStats");
                });

            modelBuilder.Entity("CBEModels.LiveCompStat", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("LiveCompetitionId")
                        .HasColumnType("int");

                    b.Property<int>("Losses")
                        .HasColumnType("int");

                    b.Property<double>("WLRatio")
                        .HasColumnType("float");

                    b.Property<int>("Wins")
                        .HasColumnType("int");

                    b.HasKey("UserId", "LiveCompetitionId");

                    b.HasIndex("LiveCompetitionId");

                    b.ToTable("LiveCompStats");
                });

            modelBuilder.Entity("CBEModels.LiveCompetition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LiveCompetitions");
                });

            modelBuilder.Entity("CBEModels.LiveCompetitionTest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<int>("LiveCompetitionId")
                        .HasColumnType("int");

                    b.Property<string>("TestAuthor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TestString")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("LiveCompetitionId");

                    b.ToTable("LiveCompetitionTests");
                });

            modelBuilder.Entity("CBEModels.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Auth0Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Revapoints")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Auth0Id")
                        .IsUnique()
                        .HasFilter("[Auth0Id] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CBEModels.UserQueue", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("LiveCompetitionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EnterTime")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId", "LiveCompetitionId");

                    b.HasIndex("LiveCompetitionId");

                    b.ToTable("UserQueues");
                });

            modelBuilder.Entity("CBEModels.Competition", b =>
                {
                    b.HasOne("CBEModels.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CBEModels.User", "User")
                        .WithMany()
                        .HasForeignKey("UserCreatedId");

                    b.Navigation("Category");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CBEModels.CompetitionStat", b =>
                {
                    b.HasOne("CBEModels.Competition", "Competition")
                        .WithMany()
                        .HasForeignKey("CompetitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CBEModels.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Competition");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CBEModels.LiveCompStat", b =>
                {
                    b.HasOne("CBEModels.LiveCompetition", "LiveCompetition")
                        .WithMany()
                        .HasForeignKey("LiveCompetitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CBEModels.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LiveCompetition");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CBEModels.LiveCompetitionTest", b =>
                {
                    b.HasOne("CBEModels.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CBEModels.LiveCompetition", "LiveCompetition")
                        .WithMany("LiveCompetitionTests")
                        .HasForeignKey("LiveCompetitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("LiveCompetition");
                });

            modelBuilder.Entity("CBEModels.UserQueue", b =>
                {
                    b.HasOne("CBEModels.LiveCompetition", "LiveCompetition")
                        .WithMany()
                        .HasForeignKey("LiveCompetitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CBEModels.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LiveCompetition");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CBEModels.LiveCompetition", b =>
                {
                    b.Navigation("LiveCompetitionTests");
                });
#pragma warning restore 612, 618
        }
    }
}
