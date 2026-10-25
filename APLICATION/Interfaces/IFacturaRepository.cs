using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface IFacturaRepository
    {
        Task<IEnumerable<Factura>> ListarFacturaAsync();
        Task NuevaFactura(Factura oFactura);
        Task EditarFactura(Factura oFactura);
        Task EliminarFactura(int id);
    }
}
