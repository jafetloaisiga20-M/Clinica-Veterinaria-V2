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

        public async Task<IEnumerable<Factura>> ListarFacturaAsync()
        {
            var olist = new List<Factura>();
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_LISTAR_FACTURA", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Factura
                        {
                            ID_FACTURA = Convert.ToInt32(dr["ID_CLIENTE"]),
                            ID_CITA = Convert.ToInt32(dr["ID_CLIENTE"]),
                            ID_CLIENTE = Convert.ToInt32(dr["ID_CLIENTE"]),
                            NUMERO_FACTURA = dr["WHATSAPP"].ToString(),
                            TICKET_IMPRESO = Convert.ToBoolean(dr["ID_CLIENTE"]),
                            FECHA_EMISION = Convert.ToDateTime(dr["ID_CLIENTE"]),
                            SUBTOTAL = Convert.ToDecimal(dr["ID_CLIENTE"]),
                            IMPUESTOS = Convert.ToDecimal(dr["ID_CLIENTE"]),
                            TOTAL_PAGAR = Convert.ToDecimal(dr["ID_CLIENTE"]),
                            METODO_PAGO = dr["ESTADO"].ToString(),
                            ESTADO_FACTURA = dr["ESTADO"].ToString()

                        });
                    }
                }
                return olist;
            }
        }

        public async Task NuevaFactura(Factura oFactura)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_NUEVA_FACTURA", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID_CITA", oFactura.ID_CITA));
                cmd.Parameters.Add(new SqlParameter("@ID_CLIENTE", oFactura.ID_CLIENTE));
                cmd.Parameters.Add(new SqlParameter("@SUB_TOTAL", oFactura.SUBTOTAL));
                cmd.Parameters.Add(new SqlParameter("@IMPUESTOS", oFactura.IMPUESTOS));
                cmd.Parameters.Add(new SqlParameter("@TOTAL_PAGAR", oFactura.TOTAL_PAGAR));
                cmd.Parameters.Add(new SqlParameter("@METODO_DE_PAGO", oFactura.METODO_PAGO));

                await cmd.ExecuteNonQueryAsync();
            }
        }


        public Task EditarFactura(Factura oFactura)
        {
            throw new NotImplementedException();
        }

        public async Task EliminarFactura(int id)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_ANULAR_FACTURA", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", id));

                await cmd.ExecuteNonQueryAsync();
            }
        }
        

    }
}
