using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace INFRASTRUCTURE.Repository
{
    public class SucursalRepository : ISucursalRepository
    {
        private DBconnecctionFactory _dbConncetionFactory;

        public SucursalRepository(DBconnecctionFactory dbConncetionFactory)
        {
            _dbConncetionFactory = dbConncetionFactory;
        }

        public async Task<IEnumerable<Sucursal>> ListarSucursalAsync()
        {
            var olist = new List<Sucursal>();
            using var con = _dbConncetionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_LISTAR_SUCURSAL", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Sucursal
                        {
                            ID_SUCURSAL = Convert.ToInt32(dr["ID_SUCURSAL"]),
                            NOMBRE_SUCURSAL = dr["NOMBRE_SUCURSAL"].ToString(),
                            DIRECCION = dr["DIRECCION"].ToString(),
                            CIUDAD = dr["CIUDAD"].ToString(),
                            TELEFONO = dr["TELEFONO"].ToString(),
                            ESTADO = dr["ESTADO"].ToString()
                        });
                    }
                }
                return olist;
            }
        }

        public async Task NuevaSucursal(Sucursal oSucursal)
        {
            using var con = _dbConncetionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_NUEVA_SUCURSAL", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@NOMBRE_SUCURSAL", oSucursal.NOMBRE_SUCURSAL));
                cmd.Parameters.Add(new SqlParameter("@DIRECCION", oSucursal.DIRECCION));
                cmd.Parameters.Add(new SqlParameter("@CIUDAD", oSucursal.CIUDAD));
                cmd.Parameters.Add(new SqlParameter("@TELEFONO", oSucursal.TELEFONO));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarSucursal(Sucursal oSucursal)
        {
            using var con = _dbConncetionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_EDITAR_SUCURSAL", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", oSucursal.ID_SUCURSAL));
                cmd.Parameters.Add(new SqlParameter("@NOMBRE_SUCURSAL", oSucursal.NOMBRE_SUCURSAL));
                cmd.Parameters.Add(new SqlParameter("@DIRECCION", oSucursal.DIRECCION));
                cmd.Parameters.Add(new SqlParameter("@CIUDAD", oSucursal.CIUDAD));
                cmd.Parameters.Add(new SqlParameter("@TELEFONO", oSucursal.TELEFONO));
                cmd.Parameters.Add(new SqlParameter("@ESTADO", oSucursal.ESTADO));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public Task EliminarSucursal(int id)
        {
            throw new NotImplementedException();
        }

       
    }
}
