using System;
using System.Collections.Generic;
using GesInv.Models;
using Microsoft.EntityFrameworkCore;

namespace GesInv.Data;

public  class GestionInvContext : DbContext
{


    public GestionInvContext(DbContextOptions<GestionInvContext> options)
    : base(options)
    {
    }


    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<MovimientoInventario> MovimientoInventarios { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__F353C1E5979A939A");

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<MovimientoInventario>(entity =>
        {
            entity.HasKey(e => e.MovimientoId).HasName("PK__Movimien__BF923C2C4D1F0734");

            entity.ToTable("MovimientoInventario");

            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notas).HasMaxLength(255);
            entity.Property(e => e.TipoMovimiento).HasMaxLength(10);

            entity.HasOne(d => d.Producto).WithMany(p => p.MovimientoInventarios)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK__Movimient__Produ__1920BF5C");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__A430AEA3289804DB");

            entity.ToTable("Producto");

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .HasConstraintName("FK__Producto__Catego__15502E78");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Productos)
                .HasForeignKey(d => d.ProveedorId)
                .HasConstraintName("FK__Producto__Provee__164452B1");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.ProveedorId).HasName("PK__Proveedo__61266A592B6E3FC7");

            entity.ToTable("Proveedor");

            entity.Property(e => e.CorreoElectronico).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.NombreContacto).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(15);
        });

    }

}
