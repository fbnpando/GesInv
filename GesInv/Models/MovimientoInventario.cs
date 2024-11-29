using System;
using System.Collections.Generic;

namespace GesInv.Models;

public partial class MovimientoInventario
{
    public int MovimientoId { get; set; }

    public int? ProductoId { get; set; }

    public int Cantidad { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public string? TipoMovimiento { get; set; }

    public string? Notas { get; set; }

    public virtual Producto? Producto { get; set; }
}
