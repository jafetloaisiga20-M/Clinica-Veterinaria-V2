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
    public class RolesRepository : IRolesRepository
    {

        private DBconnecctionFactory _dBconnecttionFactory;

        public RolesRepository(DBconnecctionFactory bconnecttionFactory)
        {
            _dBconnecttionFactory = bconnecttionFactory;
        }
        public async Task<IEnumerable<Roles>> ListarRolesAsync()
        {
            var olist = new List<Roles>();
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_LISTAR_ROLES", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Roles
                        {
                            ID_ROL = Convert.ToInt32(dr["ID_ROL"]),
                            NOMBRE_ROL = dr["NOMBRE_ROL"].ToString()
                        });
                    }
                }
                return olist;
            }
            throw new NotImplementedException();
        }

        public async Task NuevoRolAsync(Roles oRoles)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_NUEVO_ROL", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@NUEVO_ROL", oRoles.NOMBRE_ROL));


                await cmd.ExecuteNonQueryAsync();

            }
        }
    }
}
