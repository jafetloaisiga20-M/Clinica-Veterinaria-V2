using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

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

        public async Task<IEnumerable<DetalleFactura>> ListarDetalleFacturaAsync()
        {
            var olist = new List<DetalleFactura>();
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_LISTAR_DETALLE_POR_FACTURA", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new DetalleFactura
                        {
                            ID_FACTURA = Convert.ToInt32(dr["ID_FACTURA"]),
                            FECHA_EMISION = Convert.ToDateTime(dr["FECHA_EMISION"]),
                            NOMBRE_COMPLETO = dr["NOMBRE_COMPLETO"].ToString(),
                            ID_SERVICIO = Convert.ToInt32(dr["ID_SERVICIO"]),
                            CANTIDAD = Convert.ToInt32(dr["CANTIDAD"]),
                            PRECIO_UNITARIO = Convert.ToDecimal(dr["PRECIO_UNITARIO"]),
                            SUBTOTAL_LINEA = Convert.ToDecimal(dr["SUBTOTAL_LINEA"]),
                            ESTADO_FACTURA = dr["ESTADO_FACTURA"].ToString()
                        });
                    }
                }
                return olist;
            }
        }

        public async Task NuevaDetalleFacturaAsync(DetalleFactura oDetalleF)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_NUEVA_DETALLE_FACTURA", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID_FACTURA", oDetalleF.ID_FACTURA));
                cmd.Parameters.Add(new SqlParameter("@ID_SERVICIO", oDetalleF.ID_SERVICIO));
                cmd.Parameters.Add(new SqlParameter("@CANTIDAD", oDetalleF.CANTIDAD));
                
                await cmd.ExecuteNonQueryAsync();
            }
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
