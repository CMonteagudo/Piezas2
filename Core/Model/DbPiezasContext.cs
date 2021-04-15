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

    public virtual DbSet<Categorium> Categorias { get; set; }
    public virtual DbSet<Coche> Coches { get; set; }
    public virtual DbSet<Fabricante> Fabricantes { get; set; }
    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<ItemCoche> ItemCoches { get; set; }
    public virtual DbSet<Marca> Marcas { get; set; }
    public virtual DbSet<Modelo> Modelos { get; set; }
    public virtual DbSet<Motor> Motors { get; set; }
    public virtual DbSet<MotorCoche> MotorCoches { get; set; }
    public virtual DbSet<SubCategorium> SubCategorias { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Ventum> Ventas { get; set; }

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
         entity.ToTable( "Categoria" );

         entity.HasIndex( e => e.Nombre, "IX_Categoria" )
                  .IsUnique();

         entity.Property( e => e.Id ).ValueGeneratedNever();

         entity.Property( e => e.Logo ).HasMaxLength( 50 );

         entity.Property( e => e.Nombre )
                  .IsRequired()
                  .HasMaxLength( 50 );
       } );

      modelBuilder.Entity<Coche>( entity =>
       {
         entity.ToTable( "Coche" );

         entity.Property( e => e.Caja ).HasMaxLength( 50 );

         entity.Property( e => e.Carroceria ).HasMaxLength( 50 );

         entity.Property( e => e.Foto ).HasMaxLength( 80 );

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
                  .HasComment( "Código del Item" );

         entity.Property( e => e.Descripcion ).HasComment( "Descripción del Item" );

         entity.Property( e => e.Fabricante ).HasComment( "Codigo del fabricante" );

         entity.Property( e => e.Foto )
                  .HasMaxLength( 50 )
                  .HasComment( "Nombre de la imagen con la foto" );

         entity.Property( e => e.Nombre )
                  .IsRequired()
                  .HasMaxLength( 80 )
                  .HasComment( "Nombre del Item" );

         entity.Property( e => e.Precio )
                  .HasColumnType( "decimal(18, 2)" )
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
                  .HasConstraintName( "FK_Coche" );

         entity.HasOne( d => d.IdItemNavigation )
                  .WithMany( p => p.ItemCoches )
                  .HasForeignKey( d => d.IdItem )
                  .HasConstraintName( "FK_IteM" );
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

      modelBuilder.Entity<SubCategorium>( entity =>
       {
         entity.ToTable( "SubCategoria" );

         entity.Property( e => e.Id ).ValueGeneratedNever();

         entity.Property( e => e.Nombre )
                  .IsRequired()
                  .HasMaxLength( 50 );
       } );

      modelBuilder.Entity<Usuario>( entity =>
       {
         entity.ToTable( "Usuario" );

         entity.HasIndex( e => e.Correo, "IX_Usuario_Correo" )
                  .IsUnique();

         entity.Property( e => e.Id ).HasComment( "Identificador del usuario" );

         entity.Property( e => e.Confirmado ).HasComment( "Indica que el correo fue confirmado" );

         entity.Property( e => e.Correo )
                  .IsRequired()
                  .HasMaxLength( 30 )
                  .HasComment( "Correo electronico (Unico)" );

         entity.Property( e => e.NLogin )
                  .HasColumnName( "nLogin" )
                  .HasComment( "Número de veces que el usuario de ha logueado" );

         entity.Property( e => e.Admin )
                  .HasColumnName( "Admin" )
                  .HasComment( "Usuario puede acceder a las página de administración" );

         entity.Property( e => e.Nombre )
                  .IsRequired()
                  .HasMaxLength( 50 )
                  .HasComment( "Nombre del Usuario" );

         entity.Property( e => e.PassWord )
                  .IsRequired()
                  .HasMaxLength( 20 )
                  .HasDefaultValueSql( "('')" )
                  .HasComment( "Contraseña del usuario" );

         entity.Property( e => e.Telefonos )
                  .IsRequired()
                  .HasMaxLength( 80 )
                  .HasDefaultValueSql( "('')" )
                  .HasComment( "Lista de telefono separados por ," );
       } );

      modelBuilder.Entity<Ventum>( entity =>
       {
         entity.ToTable( "Venta" );

         entity.Property( e => e.Id ).HasComment( "Identificador de la venta" );

         entity.Property( e => e.Cantidad )
                  .HasDefaultValueSql( "((1))" )
                  .HasComment( "Catidad de items del producto" );

         entity.Property( e => e.Estado ).HasComment( "1- Pagado, 0- Sin pagar (carrito)" );

         entity.Property( e => e.Fecha )
                  .HasColumnType( "date" )
                  .HasDefaultValueSql( "(getdate())" )
                  .HasComment( "Fecha que se genero la venta" );

         entity.Property( e => e.FechaPago )
                  .HasColumnType( "date" )
                  .HasComment( "Fecha cuando se efectua el pago" );

         entity.Property( e => e.ItemId ).HasComment( "Identificador de producto a comprar" );

         entity.Property( e => e.Precio )
                  .HasColumnType( "decimal(18, 2)" )
                  .HasComment( "Precio por items" );

         entity.Property( e => e.UsuarioId ).HasComment( "Identificador del usuario que realiza el pago" );

         entity.HasOne( d => d.Item )
                  .WithMany( p => p.ItemVentas )
                  .HasForeignKey( d => d.ItemId )
                  .OnDelete( DeleteBehavior.ClientSetNull )
                  .HasConstraintName( "FK_Venta_Item" );

         entity.HasOne( d => d.Usuario )
                  .WithMany( p => p.Venta )
                  .HasForeignKey( d => d.UsuarioId )
                  .OnDelete( DeleteBehavior.ClientSetNull )
                  .HasConstraintName( "FK_Venta_Usuario" );
       } );

      OnModelCreatingPartial( modelBuilder );
      }

    partial void OnModelCreatingPartial( ModelBuilder modelBuilder );
    }
  }
