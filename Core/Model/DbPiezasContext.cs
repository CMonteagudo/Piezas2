using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Piezas2.Core.Model
  {
  public partial class DbPiezasContext : DbContext
    {
    public DbPiezasContext()
      {
      }

    public DbPiezasContext( DbContextOptions<DbPiezasContext> options )
        : base( options )
      {
      }

    public virtual DbSet<Categorium> Categoria { get; set; }
    public virtual DbSet<SubCategoria> SubCategoria { get; set; }
    public virtual DbSet<Coche> Coches { get; set; }
    public virtual DbSet<Fabricante> Fabricantes { get; set; }
    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<ItemCoche> ItemCoches { get; set; }
    public virtual DbSet<Marca> Marcas { get; set; }
    public virtual DbSet<Modelo> Modelos { get; set; }
    public virtual DbSet<Motor> Motors { get; set; }
    public virtual DbSet<MotorCoche> MotorCoches { get; set; }

    protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
      {
      if( !optionsBuilder.IsConfigured )
        optionsBuilder.UseSqlServer( Startup.conneStr );
      }

    protected override void OnModelCreating( ModelBuilder modelBuilder )
      {
      modelBuilder.HasAnnotation( "Relational:Collation", "SQL_Latin1_General_CP1_CI_AS" );

      modelBuilder.Entity<Categorium>( entity =>
       {
         entity.HasIndex( e => e.Nombre, "IX_Categoria" )
                  .IsUnique();

         entity.Property( e => e.Imagen ).HasMaxLength( 50 );

         entity.Property( e => e.Nombre )
                  .IsRequired()
                  .HasMaxLength( 50 );
       } );

      modelBuilder.Entity<Coche>( entity =>
       {
         entity.ToTable( "Coche" );

         entity.Property( e => e.Caja ).HasMaxLength( 50 );

         entity.Property( e => e.Carroceria ).HasMaxLength( 50 );

         entity.Property( e => e.Foto ).HasMaxLength( 50 );

         entity.HasOne( d => d.MarcaNavigation )
                  .WithMany( p => p.Coches )
                  .HasForeignKey( d => d.Marca )
                  .OnDelete( DeleteBehavior.ClientSetNull )
                  .HasConstraintName( "FK_Marca" );

         entity.HasOne( d => d.ModeloNavigation )
                  .WithMany( p => p.Coches )
                  .HasForeignKey( d => d.Modelo )
                  .OnDelete( DeleteBehavior.ClientSetNull )
                  .HasConstraintName( "FK_Modelo" );

         entity.HasOne( d => d.MotorNavigation )
                  .WithMany( p => p.Coches )
                  .HasForeignKey( d => d.Motor )
                  .OnDelete( DeleteBehavior.ClientSetNull )
                  .HasConstraintName( "FK_Motor" );
       } );

      modelBuilder.Entity<Fabricante>( entity =>
       {
         entity.ToTable( "Fabricante" );

         entity.HasIndex( e => e.Nombre, "IX_Fabricante" )
                  .IsUnique();

         entity.Property( e => e.Logo ).HasMaxLength( 50 );

         entity.Property( e => e.Nombre )
                  .IsRequired()
                  .HasMaxLength( 50 );
       } );

      modelBuilder.Entity<Item>( entity =>
       {
         entity.ToTable( "Item" );

         entity.Property( e => e.Id ).HasComment( "Identificador del Item" );

         entity.Property( e => e.Categoria ).HasComment( "Código del conjunto que pertence el Item" );

         entity.Property( e => e.Codigo )
                  .IsRequired()
                  .HasMaxLength( 50 )
                  .IsFixedLength( true )
                  .HasComment( "Código del Item" );

         entity.Property( e => e.Descripcion )
                  .HasColumnType( "ntext" )
                  .HasComment( "Descripción del Item" );

         entity.Property( e => e.Fabricante ).HasComment( "Codigo del fabricante" );

         entity.Property( e => e.Foto )
                  .HasMaxLength( 50 )
                  .IsFixedLength( true )
                  .HasComment( "Nombre de la imagen con la foto" );

         entity.Property( e => e.Nombre )
                  .IsRequired()
                  .HasMaxLength( 80 )
                  .HasComment( "Nombre del Item" );

         entity.Property( e => e.Precio )
                  .HasColumnType( "decimal(18, 0)" )
                  .HasComment( "Precio del Item en euros" );

         entity.HasOne( d => d.CategoriaNavigation )
                  .WithMany( p => p.Items )
                  .HasForeignKey( d => d.Categoria )
                  .OnDelete( DeleteBehavior.ClientSetNull )
                  .HasConstraintName( "FK_Categoria" );

         entity.HasOne( d => d.FabricanteNavigation )
                  .WithMany( p => p.Items )
                  .HasForeignKey( d => d.Fabricante )
                  .OnDelete( DeleteBehavior.ClientSetNull )
                  .HasConstraintName( "FK_Fabricante" );
       } );

      modelBuilder.Entity<ItemCoche>( entity =>
       {
         entity.HasKey( e => new { e.IdItem, e.IdCoche } );

         entity.ToTable( "ItemCoche" );

         entity.Property( e => e.IdItem ).HasComment( "Identificador del Item" );

         entity.Property( e => e.IdCoche ).HasComment( "Identificador del coche donde se usa" );

         entity.HasOne( d => d.IdCocheNavigation )
                  .WithMany( p => p.ItemCoches )
                  .HasForeignKey( d => d.IdCoche )
                  .OnDelete( DeleteBehavior.ClientSetNull )
                  .HasConstraintName( "FK_Coche" );

         entity.HasOne( d => d.IdItemNavigation )
                  .WithMany( p => p.ItemCoches )
                  .HasForeignKey( d => d.IdItem )
                  .OnDelete( DeleteBehavior.ClientSetNull )
                  .HasConstraintName( "FK_Item" );
       } );

      modelBuilder.Entity<Marca>( entity =>
       {
         entity.ToTable( "Marca" );

         entity.HasIndex( e => e.Nombre, "IX_Marca" )
                  .IsUnique();

         entity.Property( e => e.Id ).HasComment( "Codigo que identfica la marca" );

         entity.Property( e => e.Descripcion )
                  .HasColumnType( "text" )
                  .HasComment( "Descripción sobre la marca" );

         entity.Property( e => e.Logo )
                  .HasMaxLength( 50 )
                  .HasComment( "Nombre de la imagen con el logotipo de la marca" );

         entity.Property( e => e.Nombre )
                  .IsRequired()
                  .HasMaxLength( 50 )
                  .HasComment( "Nombre de la marca" );
       } );

      modelBuilder.Entity<Modelo>( entity =>
       {
         entity.ToTable( "Modelo" );

         entity.HasIndex( e => e.Nombre, "IX_Modelo" )
                  .IsUnique();

         entity.Property( e => e.Nombre )
                  .IsRequired()
                  .HasMaxLength( 50 );
       } );

      modelBuilder.Entity<Motor>( entity =>
       {
         entity.ToTable( "Motor" );

         entity.HasIndex( e => e.Nombre, "IX_Motor" )
                  .IsUnique();

         entity.Property( e => e.Capacidad ).HasComment( "Capacidad en Litros" );

         entity.Property( e => e.Combustible ).HasComment( "Tipo de combustible 0- Gasolina 1-Diesel" );

         entity.Property( e => e.Nombre )
                  .IsRequired()
                  .HasMaxLength( 50 );

         entity.Property( e => e.Potencia ).HasComment( "Potencia en CV" );
       } );


      modelBuilder.Entity<MotorCoche>( entity =>
      {
        entity.HasKey( e => new { e.Id, e.Modelo, e.Nombre, e.Marca } );

        entity.ToTable( "MotorCoche" );

        entity.Property( e => e.Nombre ).HasMaxLength( 50 );

        entity.HasOne( d => d.MarcaNavigation )
            .WithMany( p => p.MotorCoches )
            .HasForeignKey( d => d.Marca )
            .OnDelete( DeleteBehavior.ClientSetNull )
            .HasConstraintName( "FK_MC_Marca" );

        entity.HasOne( d => d.ModeloNavigation )
            .WithMany( p => p.MotorCoches )
            .HasForeignKey( d => d.Modelo )
            .OnDelete( DeleteBehavior.ClientSetNull )
            .HasConstraintName( "FK_MC_Modelo" );
      } );


      OnModelCreatingPartial( modelBuilder );
      }

    partial void OnModelCreatingPartial( ModelBuilder modelBuilder );
    }
  }
