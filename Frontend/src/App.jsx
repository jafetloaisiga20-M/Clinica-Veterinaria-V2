import { useEffect, useState } from 'react';

function App() {
  const [clientes, setClientes] = useState([]);
  
  // Estados para el nuevo cliente
  const [nombre, setNombre] = useState('');
  const [telefono, setTelefono] = useState('');
  const [email, setEmail] = useState('');

  // ⚠️ CAMBIA ESTO: Pon la IP de tu compu (saca con ipconfig en el CMD) y el puerto de tu C#
  const API_URL = 'http://192.168.123.37:5000/api/Cliente';

  // GET: Listar
  const traerClientes = async () => {
    try {
      const respuesta = await fetch(API_URL);
      if (respuesta.ok) {
        const datos = await respuesta.json();
        setClientes(datos);
      }
    } catch (error) {
      console.error("Error de conexión:", error);
    }
  };

  useEffect(() => {
    traerClientes();
  }, []);

  // POST: Agregar
  const guardarCliente = async (e) => {
    e.preventDefault(); // Evita que la página se recargue
    try {
      const respuesta = await fetch(`${API_URL}/NuevoCliente`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          NOMBRE_COMPLETO: nombre,
          TELEFONO: telefono,
          EMAIL: email
        })
      });

      if (respuesta.ok) {
        alert("¡Cliente agregado manito!");
        setNombre('');
        setTelefono('');
        setEmail('');
        traerClientes(); // Recargar la tabla
      } else {
        alert("Error al guardar");
      }
    } catch (error) {
      console.error("Error:", error);
    }
  };

  return (
    <div style={{ padding: '20px', fontFamily: 'sans-serif', maxWidth: '800px', margin: 'auto' }}>
      <h1 style={{ textAlign: 'center' }}>Lista de Clientes 🐶🐱</h1>
      
      {/* Formulario de Nuevo Cliente */}
      <form onSubmit={guardarCliente} style={{ marginBottom: '20px', padding: '15px', backgroundColor: '#e9ecef', borderRadius: '8px' }}>
        <h3>Agregar Nuevo</h3>
        <input 
          type="text" placeholder="Nombre Completo" required value={nombre} onChange={(e) => setNombre(e.target.value)} 
          style={{ marginRight: '10px', padding: '5px' }} 
        />
        <input 
          type="text" placeholder="Teléfono" required value={telefono} onChange={(e) => setTelefono(e.target.value)} 
          style={{ marginRight: '10px', padding: '5px' }} 
        />
        <input 
          type="email" placeholder="Correo" required value={email} onChange={(e) => setEmail(e.target.value)} 
          style={{ marginRight: '10px', padding: '5px' }} 
        />
        <button type="submit" style={{ padding: '6px 15px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '4px' }}>
          Guardar Cliente
        </button>
      </form>

      {/* Tabla */}
      <table border="1" cellPadding="10" style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'center' }}>
        <thead style={{ backgroundColor: '#f2f2f2' }}>
          <tr>
            <th>ID</th>
            <th>Nombre Completo</th>
            <th>Teléfono</th>
            <th>Email</th>
          </tr>
        </thead>
        <tbody>
          {clientes.map((cliente, index) => (
            <tr key={index}>
              <td>{cliente.iD_CLIENTE || index + 1}</td>
              <td>{cliente.nombrE_COMPLETO || cliente.NOMBRE_COMPLETO}</td>
              <td>{cliente.telefono || cliente.TELEFONO}</td>
              <td>{cliente.email || cliente.EMAIL}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default App;