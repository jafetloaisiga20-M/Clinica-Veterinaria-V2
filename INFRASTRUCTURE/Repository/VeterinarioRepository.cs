using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class VeterinarioRepository : IVeterinarioRepository
    {
        private readonly DBconnecctionFactory _conncetionFactory;

        public VeterinarioRepository(DBconnecctionFactory conncetionFactory)
        {
            _conncetionFactory = conncetionFactory;
        }

        //listar
        public Task<IEnumerable<Veterinario>> ListarVeterinariosAsync()
        {
            throw new NotImplementedException();
        }

        //Insertar
        public Task NuevoVeterinario(Veterinario oVeterinario)
        {
            throw new NotImplementedException();
        }

        //editar
        public Task EditarVeterinario(Veterinario oVeterinario)
        {
            throw new NotImplementedException();
        }

        //ELiminar
        public Task EliminarVeterinario(int id)
        {
            throw new NotImplementedException();
        }

      
    }
}

