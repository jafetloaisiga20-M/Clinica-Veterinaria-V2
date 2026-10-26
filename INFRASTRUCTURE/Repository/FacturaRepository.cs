using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class FacturaRepository:IFacturaRepository
    {

        /// <summary>
        /// Repositorio: Implementación concreta de las interfaces de acceso a datos definidas en la capa de Aplicación o Dominio. 
        /// Aquí reside el código directo de ADO.NET (consultas SQL, ejecución de procedimientos almacenados y mapeo de SqlDataReader a Entidades).
        /// </summary>


        private DBconnecctionFactory _dBconnecttionFactory;

        public FacturaRepository(DBconnecctionFactory bconnecttionFactory)
        {
            _dBconnecttionFactory = bconnecttionFactory;
        }

        public Task<IEnumerable<Factura>> ListarFacturaAsync()
        {
            throw new NotImplementedException();
        }

        public Task NuevaFactura(Factura oFactura)
        {
            throw new NotImplementedException();
        }


        public Task EditarFactura(Factura oFactura)
        {
            throw new NotImplementedException();
        }

        public Task EliminarFactura(int id)
        {
            throw new NotImplementedException();
        }


    }
}
