using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class UsuarioServices
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioServices(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        //Listar ---------------------------------------------------------------------
        public async Task<IEnumerable<UsuarioDTOs>> ListarUsuario()
        {
            var lista = await _repository.ListarUsuarioAsync();
            return lista.Select(p => new UsuarioDTOs
            {
                ID_USUARIO = p.ID_USUARIO,
                ID_ROL = p.ID_ROL,
                ID_SUCURSAL = p.ID_SUCURSAL,
                NOMBRE_USUARIO = p.NOMBRE_USUARIO,
                PASSWORD_HASH = p.PASSWORD_HASH,
                ESTADO = p.ESTADO
            });
        }

        //insertar ---------------------------------------------------------------------
        public async Task NuevoUsuario(UsuarioDTOs Usua)
        {
            var oUsua = new Usuario
            {
                ID_ROL = Usua.ID_ROL,
                ID_SUCURSAL = Usua.ID_SUCURSAL,
                NOMBRE_USUARIO = Usua.NOMBRE_USUARIO,
                PASSWORD_HASH = Usua.PASSWORD_HASH,
                ESTADO = Usua.ESTADO
            };
            await _repository.NuevoUsuario(oUsua);
        }

        //Editar ---------------------------------------------------------------------
        public async Task EditarUsuario(UsuarioDTOs Usua)
        {
            var oUsua = new Usuario
            {
                ID_USUARIO = Usua.ID_USUARIO,
                ID_ROL = Usua.ID_ROL,
                ID_SUCURSAL = Usua.ID_SUCURSAL,
                NOMBRE_USUARIO = Usua.NOMBRE_USUARIO,
                PASSWORD_HASH = Usua.PASSWORD_HASH,
                ESTADO = Usua.ESTADO
            };
            await _repository.EditarUsuario(oUsua);
        }

        //Eliminar ---------------------------------------------------------------------
        public async Task ELiminarUsuario(int id)
        {
            await _repository.EliminarUsuario(id);
        }
    }
}
