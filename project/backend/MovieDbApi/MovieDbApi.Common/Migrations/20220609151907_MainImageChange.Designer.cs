// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieDbApi.Common.Data.Specific;

#nullable disable

namespace MovieDbApi.Common.Migrations
{
    [DbContext(typeof(MediaContext))]
    [Migration("20220609151907_MainImageChange")]
    partial class MainImageChange
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ChapterTitle")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("LONGTEXT");

                    b.Property<string>("ExternalId")
                        .HasColumnType("longtext");

                    b.Property<string>("Group")
                        .HasColumnType("longtext");

                    b.Property<int?>("GroupId")
                        .HasColumnType("int");

                    b.Property<int>("ImageId")
                        .HasColumnType("int");

                    b.Property<string>("Instructions")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsGrouping")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Path")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .HasColumnType("longtext");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ImageId");

                    b.ToTable("MediaItems");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemAttribute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("MediaItemId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("MediaItemId");

                    b.ToTable("MediaItemAttributes");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("MediaItemId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MediaItemId");

                    b.ToTable("MediaItemImages");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemLanguage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("MediaItemId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MediaItemId");

                    b.ToTable("MediaItemLanguages");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemLink", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("MediaItemId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MediaItemId");

                    b.ToTable("MediaItemLinks");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemRelation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<int>("ItemRelatedToId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("ItemRelatedToId");

                    b.ToTable("MediaItemRelations");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemTitle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("MediaItemId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("MediaItemId");

                    b.ToTable("MediaItemTitle");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.ScannedPath", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("ScannedPaths");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.Subscriber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Subscribers");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.SubscriberMediaItemType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("SubscriberId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SubscriberId");

                    b.ToTable("SubscriberMediaItemTypes");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.TranslationCache", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Source")
                        .HasColumnType("LONGTEXT");

                    b.Property<string>("SourceHash")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Target")
                        .HasColumnType("LONGTEXT");

                    b.HasKey("Id");

                    b.HasIndex("SourceHash")
                        .IsUnique();

                    b.HasIndex("Language", "SourceHash")
                        .IsUnique();

                    b.ToTable("TranslationCache");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItem", b =>
                {
                    b.HasOne("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemImage", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Image");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemAttribute", b =>
                {
                    b.HasOne("MovieDbApi.Common.Domain.Media.Models.Data.MediaItem", null)
                        .WithMany("Attributes")
                        .HasForeignKey("MediaItemId");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemImage", b =>
                {
                    b.HasOne("MovieDbApi.Common.Domain.Media.Models.Data.MediaItem", null)
                        .WithMany("Images")
                        .HasForeignKey("MediaItemId");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemLanguage", b =>
                {
                    b.HasOne("MovieDbApi.Common.Domain.Media.Models.Data.MediaItem", null)
                        .WithMany("Languages")
                        .HasForeignKey("MediaItemId");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemLink", b =>
                {
                    b.HasOne("MovieDbApi.Common.Domain.Media.Models.Data.MediaItem", null)
                        .WithMany("Links")
                        .HasForeignKey("MediaItemId");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemRelation", b =>
                {
                    b.HasOne("MovieDbApi.Common.Domain.Media.Models.Data.MediaItem", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieDbApi.Common.Domain.Media.Models.Data.MediaItem", "ItemRelatedTo")
                        .WithMany()
                        .HasForeignKey("ItemRelatedToId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("ItemRelatedTo");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItemTitle", b =>
                {
                    b.HasOne("MovieDbApi.Common.Domain.Media.Models.Data.MediaItem", null)
                        .WithMany("Titles")
                        .HasForeignKey("MediaItemId");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.SubscriberMediaItemType", b =>
                {
                    b.HasOne("MovieDbApi.Common.Domain.Media.Models.Data.Subscriber", null)
                        .WithMany("MediaItemTypes")
                        .HasForeignKey("SubscriberId");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.MediaItem", b =>
                {
                    b.Navigation("Attributes");

                    b.Navigation("Images");

                    b.Navigation("Languages");

                    b.Navigation("Links");

                    b.Navigation("Titles");
                });

            modelBuilder.Entity("MovieDbApi.Common.Domain.Media.Models.Data.Subscriber", b =>
                {
                    b.Navigation("MediaItemTypes");
                });
#pragma warning restore 612, 618
        }
    }
}
