using System;
using System.Collections.Generic;

namespace GesInv.Models;

public partial class Producto
{
    public int ProductoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public int? CategoriaId { get; set; }

    public int? ProveedorId { get; set; }

    public virtual Categorium? Categoria { get; set; }

    public virtual ICollection<MovimientoInventario> MovimientoInventarios { get; set; } = new List<MovimientoInventario>();

    public virtual Proveedor? Proveedor { get; set; }
}
