using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface IDetalleFacturaRepository
    {
        Task<IEnumerable<DetalleFactura>> ListarDetalleFacturaAsync();
        Task NuevaDetalleFacturaAsync(DetalleFactura oDetalleF);
        Task EditarDetalleFactura(DetalleFactura oDetalleF);
        Task EliminarDetalleFactura(int id);
    }
}
