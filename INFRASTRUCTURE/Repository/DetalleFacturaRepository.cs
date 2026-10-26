using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class DetalleFacturaRepository : IDetalleFacturaRepository
    {

        /// <summary>
        /// Repositorio: Implementación concreta de las interfaces de acceso a datos definidas en la capa de Aplicación o Dominio. 
        /// Aquí reside el código directo de ADO.NET (consultas SQL, ejecución de procedimientos almacenados y mapeo de SqlDataReader a Entidades).
        /// </summary>

        private DBconnecctionFactory _dBconnecttionFactory;

        public DetalleFacturaRepository(DBconnecctionFactory dbconnecttionFactory)
        {
            _dBconnecttionFactory = dbconnecttionFactory;
        }

        public Task<IEnumerable<DetalleFactura>> ListarDetalleFacturaAsync()
        {
            throw new NotImplementedException();
        }

        public Task NuevaDetalleFacturaAsync(DetalleFactura oDetalleF)
        {
            throw new NotImplementedException();
        }
        public Task EditarDetalleFactura(DetalleFactura oDetalleF)
        {
            throw new NotImplementedException();
        }

        public Task EliminarDetalleFactura(int id)
        {
            throw new NotImplementedException();
        }

      
    }
}
