using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApplication5.Models;





namespace WebApplication5.Models
{
    public class modelContext : DbContext
    {
        //ako treba moracemo da pravimo migracije kao iz web 

        public modelContext()
        {
        }

        public modelContext(DbContextOptions<modelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DSadrzi> DSadrzi { get; set; }
        public virtual DbSet<Dogadjaj> Dogadjaj { get; set; }
        public virtual DbSet<KSadrzi> KSadrzi { get; set; }
        public virtual DbSet<Katalog> Katalog { get; set; }
        public virtual DbSet<Korisnik> Korisnik { get; set; }
        public virtual DbSet<NSadrzi> NSadrzi { get; set; }
        public virtual DbSet<Narudzbina> Narudzbina { get; set; }
        public virtual DbSet<Proizvod> Proizvod { get; set; }
        public virtual DbSet<Radnik> Radnik { get; set; }
        public virtual DbSet<PocetniPodaci> PocetniPodaci { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies(true) 
                        .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=model;Trusted_Connection=True;");
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<DSadrzi>(entity =>
            {
                entity.HasKey(e => new { e.Iddogadjaja, e.Idkatalog });

                entity.ToTable("D_Sadrzi");

                entity.HasIndex(e => e.Iddogadjaja)
                    .HasName("fkIdx_107");

                entity.HasIndex(e => e.Idkatalog)
                    .HasName("fkIdx_111");

                entity.Property(e => e.Iddogadjaja).HasColumnName("IDDogadjaja");

                entity.Property(e => e.Idkatalog).HasColumnName("IDKatalog");

                entity.HasOne(d => d.IddogadjajaNavigation)
                    .WithMany(p => p.DSadrzi)
                    .HasForeignKey(d => d.Iddogadjaja)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_107");

                entity.HasOne(d => d.IdkatalogNavigation)
                    .WithMany(p => p.DSadrzi)
                    .HasForeignKey(d => d.Idkatalog)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_111");
            });

            modelBuilder.Entity<Dogadjaj>(entity =>
            {
                entity.HasKey(e => e.Iddogadjaja);

                entity.Property(e => e.Iddogadjaja).HasColumnName("IDDogadjaja");

                entity.Property(e => e.VrstaDogadjaja)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<KSadrzi>(entity =>
            {
                entity.HasKey(e => new { e.Idkatalog, e.Idproizvoda });

                entity.ToTable("K_Sadrzi");

                entity.HasIndex(e => e.Idkatalog)
                    .HasName("fkIdx_114");

                entity.HasIndex(e => e.Idproizvoda)
                    .HasName("fkIdx_117");

                entity.Property(e => e.Idkatalog).HasColumnName("IDKatalog");

                entity.Property(e => e.Idproizvoda).HasColumnName("IDProizvoda");

                entity.HasOne(d => d.IdkatalogNavigation)
                    .WithMany(p => p.KSadrzi)
                    .HasForeignKey(d => d.Idkatalog)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_114");

                entity.HasOne(d => d.IdproizvodaNavigation)
                    .WithMany(p => p.KSadrzi)
                    .HasForeignKey(d => d.Idproizvoda)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_117");
            });

            modelBuilder.Entity<Katalog>(entity =>
            {
                entity.HasKey(e => e.Idkatalog);

                entity.Property(e => e.Idkatalog).HasColumnName("IDKatalog");

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Kreirao).HasColumnName("Kreirao");

            });

            modelBuilder.Entity<Korisnik>(entity =>
            {
                entity.HasKey(e => e.Idkorisnika);

                entity.Property(e => e.Idkorisnika).HasColumnName("IDKorisnika");

                entity.Property(e => e.BrojTelefona).HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.Property(e => e.Prezime).HasMaxLength(50);

                entity.Property(e => e.Sifra)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Tip)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<NSadrzi>(entity =>
            {
                entity.HasKey(e => new { e.Idnarudzbine, e.Idkatalog });

                entity.ToTable("N_Sadrzi");

                entity.HasIndex(e => e.Idkatalog)
                    .HasName("fkIdx_103");

                entity.HasIndex(e => e.Idnarudzbine)
                    .HasName("fkIdx_96");

                entity.Property(e => e.Idnarudzbine).HasColumnName("IDNarudzbine");

                entity.Property(e => e.Idkatalog).HasColumnName("IDKatalog");

                entity.HasOne(d => d.IdkatalogNavigation)
                    .WithMany(p => p.NSadrzi)
                    .HasForeignKey(d => d.Idkatalog)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_103");

                entity.HasOne(d => d.IdnarudzbineNavigation)
                    .WithMany(p => p.NSadrzi)
                    .HasForeignKey(d => d.Idnarudzbine)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_96");
            });

            modelBuilder.Entity<Narudzbina>(entity =>
            {
                entity.HasKey(e => e.Idnarudzbine);

                entity.HasIndex(e => e.Idkorisnika)
                    .HasName("fkIdx_127");

                entity.HasIndex(e => e.Idradnika)
                    .HasName("fkIdx_87");

                entity.Property(e => e.Idnarudzbine).HasColumnName("IDNarudzbine");

                entity.Property(e => e.AdresaIsporuke)
                    .IsRequired()
                    .HasColumnName("Adresa_Isporuke")
                    .HasMaxLength(50);

                entity.Property(e => e.DatumIsporuke)
                    .HasColumnName("Datum_Isporuke")
                    .HasColumnType("date");

                entity.Property(e => e.DatumNarucivanja)
                    .HasColumnName("Datum_Narucivanja")
                    .HasColumnType("date");

                entity.Property(e => e.Idkorisnika).HasColumnName("IDKorisnika");

                entity.Property(e => e.Idradnika).HasColumnName("IDRadnika");

                entity.Property(e => e.UkupnaCena).HasColumnType("decimal(6, 0)");

                entity.Property(e => e.VremeIsporuke).HasColumnName("Vreme_Isporuke");

                entity.HasOne(d => d.IdkorisnikaNavigation)
                    .WithMany(p => p.Narudzbina)
                    .HasForeignKey(d => d.Idkorisnika)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_127");

                entity.HasOne(d => d.IdradnikaNavigation)
                    .WithMany(p => p.Narudzbina)
                    .HasForeignKey(d => d.Idradnika)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_87");


                //dodatak kolone za status narudzbine
                entity.Property(e => e.StatusNarudzbine).HasColumnName("StatusNarudzbine");




            });

            modelBuilder.Entity<Proizvod>(entity =>
            {
                entity.HasKey(e => e.Idproizvoda);

                entity.Property(e => e.Idproizvoda).HasColumnName("IDProizvoda");

                entity.Property(e => e.Boja).HasMaxLength(20);

                entity.Property(e => e.CenaDekoracije).HasColumnType("decimal(5, 0)");

                entity.Property(e => e.CenaPorcije).HasColumnType("decimal(5, 0)");

                entity.Property(e => e.Gramaza).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Opis)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PutanjaDoSlike).HasMaxLength(100);

                entity.Property(e => e.VrstaDekoracije).HasMaxLength(50);

                entity.Property(e => e.VrstaObroka).HasMaxLength(50);
            });

            modelBuilder.Entity<Radnik>(entity =>
            {
                entity.HasKey(e => e.Idradnika);

                entity.Property(e => e.Idradnika).HasColumnName("IDRadnika");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Prezime)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Telefon).HasMaxLength(15);
            });

            //moj dodatak
            modelBuilder.Entity<PocetniPodaci>(entity =>
            {
                entity.HasKey(e => e.Id_pocetne);
                entity.Property(e => e.Id_pocetne).HasColumnName("Id");
                entity.Property(e => e.ONama).HasMaxLength(3000);
                entity.Property(e=> e.KorisnickaUsluga).HasMaxLength(3000);
                entity.Property(e => e.UsloviKoriscenja).HasMaxLength(3000);
                entity.Property(e => e.PolitikaPrivatnost).HasMaxLength(3000);
                entity.Property(e => e.InformacijeODostavi).HasMaxLength(3000);
            }
            );
        }


        //public DbSet<WebApplication5.Models.KatalogSaProizvodima> KatalogSaProizvodima { get; set; }
    }
}
