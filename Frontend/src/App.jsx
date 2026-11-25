import { useEffect, useState } from 'react';
import LoginView from './components/LoginView';
import ServerConfigModal from './components/ServerConfigModal';
import { API_BASE_STORAGE_KEY, getApiBase } from './apiConfig';
import './App.css';

const WA_NUMBER = import.meta.env.VITE_WHATSAPP_NUMBER || '50500000000';
const YEAR = new Date().getFullYear();

const SUCURSALES = [
  { id: 1, nombre: 'Sucursal Central' },
  { id: 2, nombre: 'Sucursal Carretera Sur' },
];
const VETERINARIOS = [
  { id: 1, nombre: 'Dra. María López' },
  { id: 2, nombre: 'Dr. Carlos Ruiz' },
];
const SERVICIOS = [
  { nombre: 'Consulta general',  categoria: 'Consulta',    precio: 350,  desc: 'Evaluación clínica general de la mascota',      icon: '🩺', color: '#dbeafe' },
  { nombre: 'Vacunación',        categoria: 'Preventivo',  precio: 500,  desc: 'Aplicación de vacuna según calendario',           icon: '💉', color: '#dcfce7' },
  { nombre: 'Desparasitación',   categoria: 'Preventivo',  precio: 250,  desc: 'Tratamiento antiparasitario interno y externo',   icon: '🛡️', color: '#dcfce7' },
  { nombre: 'Baño y corte',      categoria: 'Estética',    precio: 300,  desc: 'Baño, secado y corte de pelo profesional',        icon: '✂️', color: '#fce7f3' },
  { nombre: 'Cirugía menor',     categoria: 'Cirugía',     precio: 2500, desc: 'Procedimiento quirúrgico ambulatorio',             icon: '🔬', color: '#fef3c7' },
  { nombre: 'Rayos X',           categoria: 'Diagnóstico', precio: 1200, desc: 'Estudio radiográfico simple de alta resolución',   icon: '📡', color: '#f3e8ff' },
];

function canon(obj) {
  if (!obj || typeof obj !== 'object') return obj;
  const out = {};
  for (const key of Object.keys(obj)) out[key.toLowerCase()] = obj[key];
  return out;
}
const canonList = (list) => (Array.isArray(list) ? list.map(canon) : []);

const TOKEN_KEY = 'saludanimal_token';
const ROLE_KEY  = 'saludanimal_role';
const SERVER_PROMPTED_KEY = 'saludanimal_server_prompted';

function parseJwtSession(jwtToken) {
  if (!jwtToken || typeof jwtToken !== 'string') return null;
  try {
    const parts = jwtToken.split('.');
    if (parts.length < 2) return null;
    const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=');
    const payload = JSON.parse(decodeURIComponent(
      atob(padded).split('').map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join('')
    ));
    if (payload.exp && payload.exp * 1000 < Date.now()) return null;
    return {
      username: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || payload.unique_name || payload.name || '',
      role:     payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || payload.role || '',
      sucursal: payload.sucursal || '',
      exp:      payload.exp || 0,
    };
  } catch { return null; }
}

function getInitialToken() {
  const stored = localStorage.getItem(TOKEN_KEY) || '';
  if (!stored) return '';
  if (!parseJwtSession(stored)) { localStorage.removeItem(TOKEN_KEY); localStorage.removeItem(ROLE_KEY); return ''; }
  return stored;
}

function statusBadge(estado) {
  if (!estado) return <span className="badge badge-gray">—</span>;
  const map = {
    Activo: 'badge-green', Activa: 'badge-green', Programada: 'badge-blue',
    Cancelada: 'badge-red', Inactivo: 'badge-gray', Inactiva: 'badge-gray',
    Nuevo: 'badge-blue', Atendido: 'badge-green', Anulada: 'badge-red',
  };
  return <span className={`badge ${map[estado] || 'badge-gray'}`}>{estado}</span>;
}

function channelChip(canal) {
  const map = {
    WhatsApp: 'channel-whatsapp', Facebook: 'channel-facebook',
    Instagram: 'channel-instagram', Web: 'channel-web', Telefono: 'channel-telefono',
  };
  const cls = map[canal] || 'channel-web';
  return <span className={`channel-chip ${cls}`}>{canal}</span>;
}

/* Labeled form field */
function Field({ label, children }) {
  return (
    <div className="field">
      {label && <label>{label}</label>}
      {children}
    </div>
  );
}

export default function App() {
  const [page, setPage]         = useState('home');
  const [adminTab, setAdminTab] = useState('clientes');
  const [serverConfigOpen, setServerConfigOpen] = useState(() => {
    try {
      return Boolean(window.Capacitor?.isNativePlatform?.())
        && !localStorage.getItem(API_BASE_STORAGE_KEY)
        && !localStorage.getItem(SERVER_PROMPTED_KEY);
    } catch {
      return false;
    }
  });

  // Auth
  const [token,      setToken]      = useState(getInitialToken);
  const [role,       setRole]       = useState(() => localStorage.getItem(ROLE_KEY) || '');
  const [loginError, setLoginError] = useState('');
  const [loggingIn,  setLoggingIn]  = useState(false);

  // En el APK, mostrar el selector una vez si aun no hay servidor guardado.
  useEffect(() => {
    if (!serverConfigOpen) return;
    try { localStorage.setItem(SERVER_PROMPTED_KEY, '1'); } catch { /* ignore */ }
  }, [serverConfigOpen]);

  const session        = parseJwtSession(token);
  const activeRole     = session?.role || role;
  const activeUser     = session?.username || '';
  const activeSucursal = SUCURSALES.find((s) => String(s.id) === String(session?.sucursal))?.nombre || '';

  function authHeaders() { return token ? { Authorization: 'Bearer ' + token } : {}; }

  function logout(reason = '') {
    localStorage.removeItem(TOKEN_KEY); localStorage.removeItem(ROLE_KEY);
    setToken(''); setRole('');
    setClientes([]); setPacientes([]); setCitas([]); setMensajes([]);
    if (reason) setLoginError(reason);
  }

  function handleServerChanged() {
    localStorage.removeItem(TOKEN_KEY); localStorage.removeItem(ROLE_KEY);
    setToken(''); setRole('');
    setServerConfigOpen(false);
    window.location.reload();
  }

  async function login({ username, password }) {
    setLoggingIn(true); setLoginError('');
    try {
      const r = await fetch(`${getApiBase()}/Auth/Login`, {
        method: 'POST', headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Username: username, Password: password }),
      });
      if (!r.ok) {
        let serverMsg = '';
        try { const d = await r.json(); serverMsg = d?.msj || d?.message || ''; } catch { /* ignore */ }
        throw new Error(r.status === 401
          ? 'Usuario o contraseña incorrectos. Verifica tus datos e intenta de nuevo.'
          : serverMsg || `No se pudo iniciar sesión (código ${r.status}).`);
      }
      const data = await r.json();
      if (!data?.token) throw new Error('El servidor no devolvió un token de sesión válido.');
      const parsed = parseJwtSession(data.token);
      const rname = parsed?.role || '';
      localStorage.setItem(TOKEN_KEY, data.token); localStorage.setItem(ROLE_KEY, rname);
      setRole(rname); setToken(data.token);
      showToast(`Bienvenido, ${parsed?.username || username}${rname ? ` · ${rname}` : ''}`, 'success');
    } catch (e) {
      setLoginError(e instanceof TypeError
        ? 'No se pudo conectar con el servidor. Verifica que la API esté en ejecución.'
        : e.message || 'Error al iniciar sesión.');
    } finally { setLoggingIn(false); }
  }

  // Data
  const [clientes,  setClientes]  = useState([]);
  const [pacientes, setPacientes] = useState([]);
  const [citas,     setCitas]     = useState([]);
  const [mensajes,  setMensajes]  = useState([]);
  const [loading,   setLoading]   = useState(false);
  const [error,     setError]     = useState('');
  const [toasts,    setToasts]    = useState([]);

  // Admin forms — clientes
  const [cNombre,    setCNombre]    = useState('');
  const [cTelefono,  setCTelefono]  = useState('');
  const [cEmail,     setCEmail]     = useState('');
  const [cDireccion, setCDireccion] = useState('');

  // Admin forms — pacientes
  const [pCliente, setPCliente] = useState('');
  const [pNombre,  setPNombre]  = useState('');
  const [pEspecie, setPEspecie] = useState('');
  const [pRaza,    setPRaza]    = useState('');
  const [pSexo,    setPSexo]    = useState('Macho');
  const [pFecha,   setPFecha]   = useState('');
  const [pPeso,    setPPeso]    = useState('');

  // Admin forms — citas
  const [ciPaciente,    setCiPaciente]    = useState('');
  const [ciSucursal,    setCiSucursal]    = useState('1');
  const [ciVeterinario, setCiVeterinario] = useState('1');
  const [ciFechaHora,   setCiFechaHora]   = useState('');
  const [ciOrigen,      setCiOrigen]      = useState('Telefono');

  // Admin forms — mensajes
  const [mCanal,     setMCanal]     = useState('WhatsApp');
  const [mRemitente, setMRemitente] = useState('');
  const [mTexto,     setMTexto]     = useState('');

  // Public booking
  const [bNombre,      setBNombre]      = useState('');
  const [bTelefono,    setBTelefono]    = useState('');
  const [bEmail,       setBEmail]       = useState('');
  const [bDireccion,   setBDireccion]   = useState('');
  const [bMascota,     setBMascota]     = useState('');
  const [bEspecie,     setBEspecie]     = useState('');
  const [bRaza,        setBRaza]        = useState('');
  const [bSexo,        setBSexo]        = useState('Macho');
  const [bFechaNac,    setBFechaNac]    = useState('');
  const [bPeso,        setBPeso]        = useState('');
  const [bServicio,    setBServicio]    = useState(SERVICIOS[0].nombre);
  const [bSucursal,    setBSucursal]    = useState('1');
  const [bVeterinario, setBVeterinario] = useState('1');
  const [bFechaHora,   setBFechaHora]   = useState('');
  const [bOrigen,      setBOrigen]      = useState('Web');
  const [booking,      setBooking]      = useState(false);
  const [bookingDone,  setBookingDone]  = useState(false);

  function showToast(msg, type = 'success') {
    const id = Date.now();
    setToasts((prev) => [...prev, { id, msg, type }]);
    setTimeout(() => setToasts((prev) => prev.filter((t) => t.id !== id)), 3500);
  }

  function go(p) { setPage(p); setError(''); window.scrollTo({ top: 0, behavior: 'smooth' }); }

  async function fetchClientes() {
    if (!token) { setLoading(false); return; }
    setLoading(true); setError('');
    try {
      const r = await fetch(`${getApiBase()}/Cliente`, { headers: authHeaders() });
      if (r.status === 401) { logout('Tu sesión expiró. Inicia sesión nuevamente.'); return; }
      if (!r.ok) throw new Error(`Error ${r.status}`);
      setClientes(canonList(await r.json()));
    } catch (e) { setError('No se pudo cargar clientes: ' + e.message); }
    finally { setLoading(false); }
  }
  async function fetchPacientes() {
    if (!token) { setLoading(false); return; }
    setLoading(true); setError('');
    try {
      const r = await fetch(`${getApiBase()}/Paciente`, { headers: authHeaders() });
      if (r.status === 401) { logout('Tu sesión expiró. Inicia sesión nuevamente.'); return; }
      if (!r.ok) throw new Error(`Error ${r.status}`);
      setPacientes(canonList(await r.json()));
    } catch (e) { setError('No se pudo cargar pacientes: ' + e.message); }
    finally { setLoading(false); }
  }
  async function fetchCitas() {
    if (!token) { setLoading(false); return; }
    setLoading(true); setError('');
    try {
      const r = await fetch(`${getApiBase()}/Citas`, { headers: authHeaders() });
      if (r.status === 401) { logout('Tu sesión expiró. Inicia sesión nuevamente.'); return; }
      if (!r.ok) throw new Error(`Error ${r.status}`);
      setCitas(canonList(await r.json()));
    } catch (e) { setError('No se pudo cargar citas: ' + e.message); }
    finally { setLoading(false); }
  }
  async function fetchMensajes() {
    if (!token) { setLoading(false); return; }
    setLoading(true); setError('');
    try {
      const r = await fetch(`${getApiBase()}/MensajeSocial`, { headers: authHeaders() });
      if (r.status === 401) { logout('Tu sesión expiró. Inicia sesión nuevamente.'); return; }
      if (!r.ok) throw new Error(`Error ${r.status}`);
      setMensajes(canonList(await r.json()));
    } catch (e) { setError('No se pudo cargar mensajes: ' + e.message); }
    finally { setLoading(false); }
  }

  useEffect(() => {
    if (!token) return;
    const timer = setTimeout(() => fetchClientes(), 0);
    return () => clearTimeout(timer);
  }, [token]); // eslint-disable-line react-hooks/exhaustive-deps

  async function crearCliente(e) {
    e.preventDefault();
    try {
      const r = await fetch(`${getApiBase()}/Cliente/NuevoCliente`, {
        method: 'POST', headers: { 'Content-Type': 'application/json', ...authHeaders() },
        body: JSON.stringify({ NOMBRE_COMPLETO: cNombre, TELEFONO: cTelefono, EMAIL: cEmail, DIRECCION: cDireccion }),
      });
      if (!r.ok) throw new Error(`Error ${r.status}`);
      showToast('✓ Cliente agregado correctamente');
      setCNombre(''); setCTelefono(''); setCEmail(''); setCDireccion('');
      fetchClientes();
    } catch (e) { setError('Error al guardar cliente: ' + e.message); }
  }
  async function crearPaciente(e) {
    e.preventDefault();
    try {
      const r = await fetch(`${getApiBase()}/Paciente/NuevaCita`, {
        method: 'POST', headers: { 'Content-Type': 'application/json', ...authHeaders() },
        body: JSON.stringify({ ID_CLIENTE: Number(pCliente), NOMBRE: pNombre, ESPECIE: pEspecie, RAZA: pRaza, SEXO: pSexo, FECHA_NACIMIENTO: pFecha, PESO_ACTUAL: Number(pPeso) }),
      });
      if (!r.ok) throw new Error(`Error ${r.status}`);
      showToast('✓ Mascota registrada correctamente');
      setPNombre(''); setPEspecie(''); setPRaza(''); setPFecha(''); setPPeso('');
      fetchPacientes();
    } catch (e) { setError('Error al registrar mascota: ' + e.message); }
  }
  async function crearCita(e) {
    e.preventDefault();
    try {
      const r = await fetch(`${getApiBase()}/Citas/NuevaCita`, {
        method: 'POST', headers: { 'Content-Type': 'application/json', ...authHeaders() },
        body: JSON.stringify({ ID_PACIENTE: Number(ciPaciente), ID_SUCURSAL: Number(ciSucursal), ID_VETERINARIO: Number(ciVeterinario), FECHA_HORA: ciFechaHora, ORIGEN_CONTACTO: ciOrigen, ESTADO_CITA: 'Programada' }),
      });
      if (!r.ok) throw new Error(`Error ${r.status}`);
      showToast('✓ Cita agendada correctamente');
      setCiFechaHora('');
      fetchCitas();
    } catch (e) { setError('Error al agendar cita: ' + e.message); }
  }
  async function crearMensaje(e) {
    e.preventDefault();
    try {
      const r = await fetch(`${getApiBase()}/MensajeSocial`, {
        method: 'POST', headers: { 'Content-Type': 'application/json', ...authHeaders() },
        body: JSON.stringify({ CANAL: mCanal, REMITENTE: mRemitente, TEXTO: mTexto, ESTADO: 'Nuevo' }),
      });
      if (!r.ok) throw new Error(`Error ${r.status}`);
      showToast('✓ Mensaje registrado');
      setMRemitente(''); setMTexto('');
      fetchMensajes();
    } catch (e) { setError('Error al registrar mensaje: ' + e.message); }
  }
  async function simularWhatsApp() {
    const payload = { entry: [{ changes: [{ value: { messages: [{ from: WA_NUMBER, id: 'wamid.sim.' + Date.now(), timestamp: String(Math.floor(Date.now() / 1000)), text: { body: 'Hola, quiero agendar una cita para mi mascota' } }] } }] }] };
    try {
      const r = await fetch(`${getApiBase()}/MensajeSocial/webhooks/whatsapp`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
      if (!r.ok) throw new Error(`Error ${r.status}`);
      showToast('✓ Mensaje de WhatsApp simulado');
      fetchMensajes();
    } catch (e) { setError('Error al simular mensaje: ' + e.message); }
  }
  async function reservarCita(e) {
    e.preventDefault();
    setBooking(true); setBookingDone(false); setError('');
    try {
      const rc = await fetch(`${getApiBase()}/Cliente/NuevoCliente`, {
        method: 'POST', headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ NOMBRE_COMPLETO: bNombre, TELEFONO: bTelefono, EMAIL: bEmail, DIRECCION: bDireccion }),
      });
      if (!rc.ok) throw new Error('No se pudo registrar el cliente');
      const clienteData = await rc.json();
      if (!clienteData.id) throw new Error('No se pudo obtener el cliente');

      const rp = await fetch(`${getApiBase()}/Paciente/NuevaCita`, {
        method: 'POST', headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ID_CLIENTE: clienteData.id, NOMBRE: bMascota, ESPECIE: bEspecie, RAZA: bRaza, SEXO: bSexo, FECHA_NACIMIENTO: bFechaNac, PESO_ACTUAL: Number(bPeso) || 0 }),
      });
      if (!rp.ok) throw new Error('No se pudo registrar la mascota');
      const pacienteData = await rp.json();
      if (!pacienteData.id) throw new Error('No se pudo obtener la mascota');

      const ra = await fetch(`${getApiBase()}/Citas/NuevaCita`, {
        method: 'POST', headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ID_PACIENTE: pacienteData.id, ID_SUCURSAL: Number(bSucursal), ID_VETERINARIO: Number(bVeterinario), FECHA_HORA: bFechaHora, ORIGEN_CONTACTO: bOrigen, ESTADO_CITA: 'Programada' }),
      });
      if (!ra.ok) { const txt = await ra.text(); throw new Error(txt || 'No se pudo agendar la cita'); }

      setBookingDone(true);
      showToast('✓ Cita agendada. Te contactaremos para confirmar.');
      setBNombre(''); setBTelefono(''); setBEmail(''); setBDireccion('');
      setBMascota(''); setBEspecie(''); setBRaza(''); setBFechaNac(''); setBPeso('');
      setBFechaHora('');
    } catch (e) { setError('Error al reservar: ' + e.message); }
    finally { setBooking(false); }
  }

  return (
    <div className="app">
      {/* TOAST CONTAINER */}
      <div className="toast-fixed" aria-live="polite">
        {toasts.map((t) => (
          <div key={t.id} className={`toast ${t.type === 'error' ? 'error-toast' : 'success'}`}>
            {t.msg}
          </div>
        ))}
      </div>

      {/* HEADER */}
      <header className="app-header">
        <div className="brand" onClick={() => go('home')}>
          <div className="logo">🐾</div>
          <div>
            <h1>Salud Animal</h1>
            <p>Clínica Veterinaria · Managua</p>
          </div>
        </div>
        <nav className="tabs">
          <button className={page === 'home'      ? 'active' : ''} onClick={() => go('home')}>Inicio</button>
          <button className={page === 'servicios' ? 'active' : ''} onClick={() => go('servicios')}>Servicios</button>
          <button className={page === 'agendar'   ? 'active' : ''} onClick={() => go('agendar')}>Agendar cita</button>
          <button className={page === 'contacto'  ? 'active' : ''} onClick={() => go('contacto')}>Contacto</button>
          <button
            className={`nav-admin-btn ${page === 'admin' ? 'active' : ''}`}
            onClick={() => { go('admin'); if (token) fetchClientes(); }}
          >
            {token ? `⚙ ${activeUser || 'Panel'}` : '🔐 Acceso interno'}
          </button>
        </nav>
      </header>

      <main className="app-main">
        {error && (
          <div className="alert error">
            ⚠ {error}
            <button
              onClick={() => setError('')}
              style={{ marginLeft: 'auto', background: 'none', border: 'none', cursor: 'pointer', color: 'inherit', fontSize: '1.1rem', lineHeight: 1 }}
              aria-label="Cerrar"
            >×</button>
          </div>
        )}

        {/* ===================== HOME ===================== */}
        {page === 'home' && (
          <>
            <section className="hero">
              <div className="hero-deco">🐾</div>
              <div className="hero-eyebrow">
                <span className="dot" />
                Clínica veterinaria en Managua, Nicaragua
              </div>
              <div className="hero-content">
                <h2>Cuidamos la salud de tu <em>mascota</em> con amor y profesionalismo</h2>
                <p>Consultas, vacunación, cirugía y diagnóstico por imagen. Agenda tu cita en línea en segundos y recibe atención de primer nivel.</p>
                <div className="hero-actions">
                  <button className="btn" onClick={() => go('agendar')}>📅 Agendar cita ahora</button>
                  <a className="btn btn-outline" href={`https://wa.me/${WA_NUMBER}?text=Hola,%20necesito%20informacion`} target="_blank" rel="noreferrer">💬 WhatsApp</a>
                </div>
              </div>
              <div className="hero-stats">
                <div className="hero-stat"><strong>6+</strong><span>Servicios disponibles</span></div>
                <div className="hero-stat"><strong>24/7</strong><span>Atención de emergencias</span></div>
                <div className="hero-stat"><strong>2</strong><span>Sucursales en Managua</span></div>
                <div className="hero-stat"><strong>100%</strong><span>Expediente digital</span></div>
              </div>
            </section>

            <div className="card">
              <div className="card-header">
                <div>
                  <h2>Nuestros servicios</h2>
                  <div className="card-subtitle">Atención integral para el bienestar de tu mascota</div>
                </div>
                <button className="btn btn-sm btn-ghost" onClick={() => go('servicios')}>Ver todos →</button>
              </div>
              <div className="services-grid">
                {SERVICIOS.map((s) => (
                  <div key={s.nombre} className="service-card">
                    <div className="service-icon" style={{ background: s.color }}>{s.icon}</div>
                    <div className="service-cat">{s.categoria}</div>
                    <h3>{s.nombre}</h3>
                    <p>{s.desc}</p>
                    <div className="service-price">C$ {s.precio.toFixed(2)}</div>
                  </div>
                ))}
              </div>
            </div>

            <div className="card">
              <div className="card-header">
                <div>
                  <h2>¿Por qué elegirnos?</h2>
                  <div className="card-subtitle">Comprometidos con la salud y bienestar animal</div>
                </div>
              </div>
              <div className="features">
                {[
                  { icon: '🩺', title: 'Equipo profesional',   desc: 'Veterinarios especializados y en constante capacitación para ofrecer el mejor cuidado.' },
                  { icon: '🕐', title: 'Atención 24/7',        desc: 'Turnos de guardia para emergencias en cualquier momento del día o de la noche.' },
                  { icon: '📋', title: 'Expediente digital',   desc: 'Historial clínico completo e inalterable por mascota. Siempre disponible.' },
                  { icon: '💬', title: 'Canales digitales',    desc: 'Agenda por teléfono, WhatsApp, Facebook o web. Como más te sea cómodo.' },
                ].map((f) => (
                  <div key={f.title} className="feature">
                    <div className="feature-icon-wrap">{f.icon}</div>
                    <h4>{f.title}</h4>
                    <p>{f.desc}</p>
                  </div>
                ))}
              </div>
            </div>
          </>
        )}

        {/* ===================== SERVICIOS ===================== */}
        {page === 'servicios' && (
          <div className="card">
            <div className="card-header">
              <div>
                <h2>Servicios y precios</h2>
                <div className="card-subtitle">Todos nuestros servicios en un solo lugar</div>
              </div>
              <button className="btn" onClick={() => go('agendar')}>📅 Agendar cita</button>
            </div>
            <div className="servicios-grid">
              {SERVICIOS.map((s) => (
                <div key={s.nombre} className="servicio-row">
                  <div className="servicio-row-icon" style={{ background: s.color }}>{s.icon}</div>
                  <div className="servicio-row-info">
                    <strong>{s.nombre}</strong>
                    <span>{s.desc}</span>
                  </div>
                  <div className="servicio-row-price">C$ {s.precio.toFixed(2)}</div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* ===================== AGENDAR ===================== */}
        {page === 'agendar' && (
          <>
            {bookingDone ? (
              <div className="card">
                <div className="booking-success">
                  <span className="success-icon">✅</span>
                  <h2>¡Cita agendada con éxito!</h2>
                  <p>Tu cita fue registrada correctamente. Te contactaremos pronto para confirmar los detalles y el horario.</p>
                  <button className="btn" onClick={() => { setBookingDone(false); go('home'); }}>← Volver al inicio</button>
                </div>
              </div>
            ) : (
              <div className="card">
                <div className="card-header">
                  <div>
                    <h2>Agendar cita en línea</h2>
                    <div className="card-subtitle">Completa el formulario y te confirmaremos tu cita</div>
                  </div>
                </div>

                {/* Steps indicator */}
                <div className="booking-steps">
                  <div className="booking-step done">
                    <span className="step-num">1</span> Propietario
                  </div>
                  <span className="booking-step-sep">›</span>
                  <div className="booking-step done">
                    <span className="step-num">2</span> Mascota
                  </div>
                  <span className="booking-step-sep">›</span>
                  <div className="booking-step done">
                    <span className="step-num">3</span> Cita
                  </div>
                </div>

                <form onSubmit={reservarCita}>
                  {/* Propietario */}
                  <div className="section-title">
                    <div className="section-icon">👤</div>
                    Datos del propietario
                  </div>
                  <div className="form-section" style={{ marginBottom: '1.75rem' }}>
                    <Field label="Nombre completo *">
                      <input required placeholder="Ej. Juan Pérez García" value={bNombre} onChange={(e) => setBNombre(e.target.value)} />
                    </Field>
                    <Field label="Teléfono *">
                      <input required placeholder="Ej. 8888-0000" value={bTelefono} onChange={(e) => setBTelefono(e.target.value)} />
                    </Field>
                    <Field label="Correo electrónico *">
                      <input type="email" required placeholder="correo@ejemplo.com" value={bEmail} onChange={(e) => setBEmail(e.target.value)} />
                    </Field>
                    <Field label="Dirección">
                      <input placeholder="Barrio, ciudad…" value={bDireccion} onChange={(e) => setBDireccion(e.target.value)} />
                    </Field>
                  </div>

                  {/* Mascota */}
                  <div className="section-title">
                    <div className="section-icon">🐾</div>
                    Datos de la mascota
                  </div>
                  <div className="form-section" style={{ marginBottom: '1.75rem' }}>
                    <Field label="Nombre de la mascota *">
                      <input required placeholder="Ej. Max, Luna…" value={bMascota} onChange={(e) => setBMascota(e.target.value)} />
                    </Field>
                    <Field label="Especie *">
                      <input required placeholder="Perro, Gato, Ave…" value={bEspecie} onChange={(e) => setBEspecie(e.target.value)} />
                    </Field>
                    <Field label="Raza">
                      <input placeholder="Labrador, Siamés…" value={bRaza} onChange={(e) => setBRaza(e.target.value)} />
                    </Field>
                    <Field label="Sexo">
                      <select value={bSexo} onChange={(e) => setBSexo(e.target.value)}>
                        <option>Macho</option><option>Hembra</option>
                      </select>
                    </Field>
                    <Field label="Fecha de nacimiento *">
                      <input type="date" required value={bFechaNac} onChange={(e) => setBFechaNac(e.target.value)} />
                    </Field>
                    <Field label="Peso (kg)">
                      <input type="number" step="0.1" min="0" placeholder="Ej. 5.2" value={bPeso} onChange={(e) => setBPeso(e.target.value)} />
                    </Field>
                  </div>

                  {/* Cita */}
                  <div className="section-title">
                    <div className="section-icon">📅</div>
                    Detalles de la cita
                  </div>
                  <div className="form-section" style={{ marginBottom: '1.75rem' }}>
                    <Field label="Servicio solicitado">
                      <select value={bServicio} onChange={(e) => setBServicio(e.target.value)}>
                        {SERVICIOS.map((s) => <option key={s.nombre} value={s.nombre}>{s.nombre} · C$ {s.precio}</option>)}
                      </select>
                    </Field>
                    <Field label="Sucursal">
                      <select value={bSucursal} onChange={(e) => setBSucursal(e.target.value)}>
                        {SUCURSALES.map((s) => <option key={s.id} value={s.id}>{s.nombre}</option>)}
                      </select>
                    </Field>
                    <Field label="Veterinario">
                      <select value={bVeterinario} onChange={(e) => setBVeterinario(e.target.value)}>
                        {VETERINARIOS.map((v) => <option key={v.id} value={v.id}>{v.nombre}</option>)}
                      </select>
                    </Field>
                    <Field label="Fecha y hora *">
                      <input type="datetime-local" required value={bFechaHora} onChange={(e) => setBFechaHora(e.target.value)} />
                    </Field>
                    <Field label="¿Cómo nos contactaste?">
                      <select value={bOrigen} onChange={(e) => setBOrigen(e.target.value)}>
                        <option>Web</option><option>Telefono</option><option>WhatsApp</option>
                      </select>
                    </Field>
                  </div>

                  <div className="form-actions">
                    <button className="btn btn-ghost" type="button" onClick={() => go('home')}>Cancelar</button>
                    <button className="btn" type="submit" disabled={booking}>
                      {booking ? '⏳ Reservando…' : '✓ Confirmar cita'}
                    </button>
                  </div>
                </form>
              </div>
            )}
          </>
        )}

        {/* ===================== CONTACTO ===================== */}
        {page === 'contacto' && (
          <div className="card">
            <div className="card-header">
              <div>
                <h2>Contacto</h2>
                <div className="card-subtitle">Estamos aquí para ayudarte</div>
              </div>
            </div>
            <div className="contact-grid">
              {[
                { icon: '📍', title: 'Dirección',  value: 'Sucursal Central, Managua, Nicaragua' },
                { icon: '📞', title: 'Teléfono',   value: '2222-0000' },
                { icon: '✉️', title: 'Correo',     value: 'info@saludanimal.com' },
                { icon: '🕐', title: 'Horario',    value: 'Lun–Sáb 8:00 am – 6:00 pm · Emergencias 24/7' },
              ].map((c) => (
                <div key={c.title} className="contact-item">
                  <div className="contact-item-icon">{c.icon}</div>
                  <div>
                    <strong>{c.title}</strong>
                    <p>{c.value}</p>
                  </div>
                </div>
              ))}
            </div>
            <div style={{ marginTop: '1.5rem', display: 'flex', gap: '0.75rem', flexWrap: 'wrap' }}>
              <a className="btn" href={`https://wa.me/${WA_NUMBER}?text=Hola,%20necesito%20informacion`} target="_blank" rel="noreferrer" style={{ background: '#16a34a' }}>
                💬 Escribir por WhatsApp
              </a>
            </div>
          </div>
        )}

        {/* ===================== ADMIN LOGIN ===================== */}
        {page === 'admin' && !token && (
          <LoginView
            onLogin={login}
            onBack={() => go('home')}
            loggingIn={loggingIn}
            loginError={loginError}
            onClearError={() => setLoginError('')}
          />
        )}

        {/* ===================== ADMIN PANEL ===================== */}
        {page === 'admin' && token && (
          <>
            {/* Session bar */}
            <div className="session-bar">
              <div className="session-info">
                <div className="session-avatar">{(activeUser || activeRole || 'U').slice(0, 1).toUpperCase()}</div>
                <div>
                  <div className="session-name">{activeUser || 'Personal de clínica'}</div>
                  <div className="session-meta">
                    <span className="session-role-pill">{activeRole || 'Usuario'}</span>
                    {activeSucursal && <span className="session-sucursal">· {activeSucursal}</span>}
                  </div>
                </div>
              </div>
              <button className="btn session-logout-btn" onClick={() => logout()}>Cerrar sesión</button>
            </div>

            {/* Admin nav */}
            <div className="admin-tabs">
              {[
                { key: 'clientes',  icon: '👥', label: 'Clientes',  count: clientes.length,  fetch: fetchClientes },
                { key: 'pacientes', icon: '🐾', label: 'Mascotas',  count: pacientes.length, fetch: fetchPacientes },
                { key: 'citas',     icon: '📅', label: 'Citas',     count: citas.length,     fetch: fetchCitas },
                { key: 'mensajes',  icon: '💬', label: 'Mensajes',  count: mensajes.length,  fetch: fetchMensajes },
              ].map(({ key, icon, label, count, fetch: fn }) => (
                <button
                  key={key}
                  className={`admin-tab-btn ${adminTab === key ? 'active' : ''}`}
                  onClick={() => { setAdminTab(key); fn(); }}
                >
                  <span className="admin-tab-icon">{icon}</span>
                  {label}
                  {count > 0 && <span className="admin-tab-count">{count}</span>}
                </button>
              ))}
            </div>

            {loading && <div className="loading-bar" />}

            {/* ---- CLIENTES ---- */}
            {adminTab === 'clientes' && (
              <>
                <div className="card">
                  <div className="card-header">
                    <div>
                      <h2>Registrar nuevo cliente</h2>
                      <div className="card-subtitle">Completa los datos del propietario</div>
                    </div>
                  </div>
                  <form onSubmit={crearCliente}>
                    <div className="form-section">
                      <Field label="Nombre completo *">
                        <input required placeholder="Juan Pérez" value={cNombre} onChange={(e) => setCNombre(e.target.value)} />
                      </Field>
                      <Field label="Teléfono *">
                        <input required placeholder="8888-0000" value={cTelefono} onChange={(e) => setCTelefono(e.target.value)} />
                      </Field>
                      <Field label="Correo electrónico *">
                        <input type="email" required placeholder="correo@ejemplo.com" value={cEmail} onChange={(e) => setCEmail(e.target.value)} />
                      </Field>
                      <Field label="Dirección">
                        <input placeholder="Barrio, ciudad…" value={cDireccion} onChange={(e) => setCDireccion(e.target.value)} />
                      </Field>
                    </div>
                    <div className="form-actions">
                      <button className="btn" type="submit">✓ Guardar cliente</button>
                    </div>
                  </form>
                </div>
                <div className="card">
                  <div className="card-header">
                    <h2>Clientes registrados</h2>
                    <button className="btn btn-sm btn-ghost" onClick={fetchClientes}>↻ Actualizar</button>
                  </div>
                  {clientes.length === 0 ? (
                    <div className="empty-state">
                      <span className="empty-icon">👥</span>
                      <p>No hay clientes registrados aún.</p>
                    </div>
                  ) : (
                    <div className="table-wrap">
                      <table>
                        <thead>
                          <tr><th>ID</th><th>Nombre</th><th>Teléfono</th><th>Correo</th><th>Dirección</th><th>Estado</th></tr>
                        </thead>
                        <tbody>
                          {clientes.map((c) => (
                            <tr key={c.id_cliente}>
                              <td><span className="badge badge-gray">#{c.id_cliente}</span></td>
                              <td><strong>{c.nombre_completo}</strong></td>
                              <td>{c.telefono || '—'}</td>
                              <td>{c.email || '—'}</td>
                              <td>{c.direccion || '—'}</td>
                              <td>{statusBadge(c.estado)}</td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </div>
              </>
            )}

            {/* ---- PACIENTES ---- */}
            {adminTab === 'pacientes' && (
              <>
                <div className="card">
                  <div className="card-header">
                    <div>
                      <h2>Registrar nueva mascota</h2>
                      <div className="card-subtitle">Asocia la mascota a un propietario existente</div>
                    </div>
                  </div>
                  <form onSubmit={crearPaciente}>
                    <div className="form-section">
                      <Field label="Propietario *">
                        <select required value={pCliente} onChange={(e) => setPCliente(e.target.value)}>
                          <option value="">— Seleccionar propietario —</option>
                          {clientes.map((c) => <option key={c.id_cliente} value={c.id_cliente}>{c.nombre_completo}</option>)}
                        </select>
                      </Field>
                      <Field label="Nombre de la mascota *">
                        <input required placeholder="Max, Luna…" value={pNombre} onChange={(e) => setPNombre(e.target.value)} />
                      </Field>
                      <Field label="Especie *">
                        <input required placeholder="Perro, Gato…" value={pEspecie} onChange={(e) => setPEspecie(e.target.value)} />
                      </Field>
                      <Field label="Raza">
                        <input placeholder="Labrador, Siamés…" value={pRaza} onChange={(e) => setPRaza(e.target.value)} />
                      </Field>
                      <Field label="Sexo">
                        <select value={pSexo} onChange={(e) => setPSexo(e.target.value)}>
                          <option>Macho</option><option>Hembra</option>
                        </select>
                      </Field>
                      <Field label="Fecha de nacimiento *">
                        <input type="date" required value={pFecha} onChange={(e) => setPFecha(e.target.value)} />
                      </Field>
                      <Field label="Peso (kg)">
                        <input type="number" step="0.1" min="0" placeholder="5.2" value={pPeso} onChange={(e) => setPPeso(e.target.value)} />
                      </Field>
                    </div>
                    <div className="form-actions">
                      <button className="btn" type="submit">✓ Guardar mascota</button>
                    </div>
                  </form>
                </div>
                <div className="card">
                  <div className="card-header">
                    <h2>Mascotas registradas</h2>
                    <button className="btn btn-sm btn-ghost" onClick={fetchPacientes}>↻ Actualizar</button>
                  </div>
                  {pacientes.length === 0 ? (
                    <div className="empty-state">
                      <span className="empty-icon">🐾</span>
                      <p>No hay mascotas registradas aún.</p>
                    </div>
                  ) : (
                    <div className="table-wrap">
                      <table>
                        <thead>
                          <tr><th>ID</th><th>Nombre</th><th>Especie</th><th>Raza</th><th>Sexo</th><th>Nacimiento</th><th>Peso</th><th>Propietario</th><th>Estado</th></tr>
                        </thead>
                        <tbody>
                          {pacientes.map((p) => (
                            <tr key={p.id_paciente}>
                              <td><span className="badge badge-gray">#{p.id_paciente}</span></td>
                              <td><strong>{p.nombre}</strong></td>
                              <td>{p.especie}</td>
                              <td>{p.raza || '—'}</td>
                              <td>{p.sexo}</td>
                              <td>{p.fecha_nacimiento ? new Date(p.fecha_nacimiento).toLocaleDateString('es-NI') : '—'}</td>
                              <td>{p.peso_actual ? `${p.peso_actual} kg` : '—'}</td>
                              <td>{clientes.find((c) => c.id_cliente === p.id_cliente)?.nombre_completo || `ID ${p.id_cliente}`}</td>
                              <td>{statusBadge(p.estado)}</td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </div>
              </>
            )}

            {/* ---- CITAS ---- */}
            {adminTab === 'citas' && (
              <>
                <div className="card">
                  <div className="card-header">
                    <div>
                      <h2>Agendar nueva cita</h2>
                      <div className="card-subtitle">Registra una cita para una mascota existente</div>
                    </div>
                  </div>
                  <form onSubmit={crearCita}>
                    <div className="form-section">
                      <Field label="Mascota *">
                        <select required value={ciPaciente} onChange={(e) => setCiPaciente(e.target.value)}>
                          <option value="">— Seleccionar mascota —</option>
                          {pacientes.map((p) => <option key={p.id_paciente} value={p.id_paciente}>{p.nombre} · {p.especie}</option>)}
                        </select>
                      </Field>
                      <Field label="Sucursal">
                        <select value={ciSucursal} onChange={(e) => setCiSucursal(e.target.value)}>
                          {SUCURSALES.map((s) => <option key={s.id} value={s.id}>{s.nombre}</option>)}
                        </select>
                      </Field>
                      <Field label="Veterinario">
                        <select value={ciVeterinario} onChange={(e) => setCiVeterinario(e.target.value)}>
                          {VETERINARIOS.map((v) => <option key={v.id} value={v.id}>{v.nombre}</option>)}
                        </select>
                      </Field>
                      <Field label="Fecha y hora *">
                        <input type="datetime-local" required value={ciFechaHora} onChange={(e) => setCiFechaHora(e.target.value)} />
                      </Field>
                      <Field label="Canal de origen">
                        <select value={ciOrigen} onChange={(e) => setCiOrigen(e.target.value)}>
                          <option>Telefono</option><option>WhatsApp</option><option>Facebook</option><option>Instagram</option><option>Web</option><option>Presencial</option>
                        </select>
                      </Field>
                    </div>
                    <div className="form-actions">
                      <button className="btn" type="submit">📅 Agendar cita</button>
                    </div>
                  </form>
                </div>
                <div className="card">
                  <div className="card-header">
                    <h2>Citas agendadas</h2>
                    <button className="btn btn-sm btn-ghost" onClick={fetchCitas}>↻ Actualizar</button>
                  </div>
                  {citas.length === 0 ? (
                    <div className="empty-state">
                      <span className="empty-icon">📅</span>
                      <p>No hay citas agendadas aún.</p>
                    </div>
                  ) : (
                    <div className="table-wrap">
                      <table>
                        <thead>
                          <tr><th>ID</th><th>Mascota</th><th>Sucursal</th><th>Veterinario</th><th>Fecha y hora</th><th>Origen</th><th>Estado</th></tr>
                        </thead>
                        <tbody>
                          {citas.map((c) => (
                            <tr key={c.id_citas}>
                              <td><span className="badge badge-gray">#{c.id_citas}</span></td>
                              <td><strong>{pacientes.find((p) => p.id_paciente === c.id_paciente)?.nombre || `ID ${c.id_paciente}`}</strong></td>
                              <td>{SUCURSALES.find((s) => s.id === c.id_sucursal)?.nombre || `Suc. ${c.id_sucursal}`}</td>
                              <td>{VETERINARIOS.find((v) => v.id === c.id_veterinario)?.nombre || `Vet. ${c.id_veterinario}`}</td>
                              <td>{c.fecha_hora ? new Date(c.fecha_hora).toLocaleString('es-NI', { dateStyle: 'medium', timeStyle: 'short' }) : '—'}</td>
                              <td>{channelChip(c.origen_contacto)}</td>
                              <td>{statusBadge(c.estado_cita)}</td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </div>
              </>
            )}

            {/* ---- MENSAJES ---- */}
            {adminTab === 'mensajes' && (
              <>
                <div className="card">
                  <div className="card-header">
                    <div>
                      <h2>Leads y mensajes entrantes</h2>
                      <div className="card-subtitle">Registra o simula mensajes recibidos por redes sociales</div>
                    </div>
                  </div>
                  <div style={{ display: 'flex', gap: '0.6rem', flexWrap: 'wrap', marginBottom: '1.5rem' }}>
                    <a className="btn btn-sm" style={{ background: '#16a34a' }} href={`https://wa.me/${WA_NUMBER}?text=Hola,%20quiero%20agendar%20una%20cita`} target="_blank" rel="noreferrer">💬 Abrir WhatsApp</a>
                    <a className="btn btn-sm" style={{ background: '#1877F2' }} href="https://m.me/clinicasaludanimal?ref=cita" target="_blank" rel="noreferrer">📘 Facebook</a>
                    <button className="btn btn-sm btn-ghost" onClick={simularWhatsApp}>⚡ Simular mensaje WA</button>
                  </div>
                  <form onSubmit={crearMensaje}>
                    <div className="form-section">
                      <Field label="Canal">
                        <select value={mCanal} onChange={(e) => setMCanal(e.target.value)}>
                          <option>WhatsApp</option><option>Facebook</option><option>Instagram</option><option>Web</option><option>Telefono</option>
                        </select>
                      </Field>
                      <Field label="Remitente *">
                        <input required placeholder="Teléfono o ID del remitente" value={mRemitente} onChange={(e) => setMRemitente(e.target.value)} />
                      </Field>
                      <Field label="Mensaje *">
                        <input required placeholder="Texto del mensaje recibido" value={mTexto} onChange={(e) => setMTexto(e.target.value)} />
                      </Field>
                    </div>
                    <div className="form-actions">
                      <button className="btn" type="submit">✓ Registrar mensaje</button>
                    </div>
                  </form>
                </div>
                <div className="card">
                  <div className="card-header">
                    <h2>Mensajes recibidos</h2>
                    <button className="btn btn-sm btn-ghost" onClick={fetchMensajes}>↻ Actualizar</button>
                  </div>
                  {mensajes.length === 0 ? (
                    <div className="empty-state">
                      <span className="empty-icon">💬</span>
                      <p>No hay mensajes registrados aún.</p>
                    </div>
                  ) : (
                    <div className="table-wrap">
                      <table>
                        <thead>
                          <tr><th>ID</th><th>Canal</th><th>Remitente</th><th>Mensaje</th><th>Fecha</th><th>Estado</th><th>Cita</th></tr>
                        </thead>
                        <tbody>
                          {mensajes.map((m) => (
                            <tr key={m.id_mensaje}>
                              <td><span className="badge badge-gray">#{m.id_mensaje}</span></td>
                              <td>{channelChip(m.canal)}</td>
                              <td><strong>{m.remitente}</strong></td>
                              <td style={{ maxWidth: 220, whiteSpace: 'normal', fontSize: '0.82rem' }}>{m.texto}</td>
                              <td>{m.fecha_recepcion ? new Date(m.fecha_recepcion).toLocaleString('es-NI', { dateStyle: 'short', timeStyle: 'short' }) : '—'}</td>
                              <td>{statusBadge(m.estado)}</td>
                              <td>{m.id_cita ? <span className="badge badge-blue">#{m.id_cita}</span> : <span style={{ color: 'var(--muted)' }}>—</span>}</td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </div>
              </>
            )}
          </>
        )}
      </main>

      {/* FOOTER */}
      <footer className="app-footer">
        <div className="footer-inner">
          <div className="footer-brand">
            <div className="footer-brand-row">
              <div className="footer-logo">🐾</div>
              <span className="footer-name">Salud Animal</span>
            </div>
            <p className="footer-desc">
              Clínica veterinaria en Managua, Nicaragua. Brindamos atención profesional y amorosa para el bienestar de tu mascota.
            </p>
          </div>
          <div className="footer-links">
            <div className="footer-link-group">
              <span>Contacto</span>
              <div className="footer-info-item">📍 Managua, Nicaragua</div>
              <div className="footer-info-item">📞 2222-0000</div>
              <div className="footer-info-item">✉️ info@saludanimal.com</div>
              <div className="footer-info-item">🕐 Lun–Sáb 8:00–18:00 · Emergencias 24/7</div>
            </div>
          </div>
        </div>
        <div className="footer-bottom">
          <span className="footer-copy">© {YEAR} Clínica Veterinaria Salud Animal · Managua</span>
          <div className="footer-badges">
            <span className="footer-badge">🩺 Veterinaria</span>
            <span className="footer-badge">📋 Expediente digital</span>
            <span className="footer-badge">🔒 Datos seguros</span>
          </div>
        </div>
      </footer>

      <button
        type="button"
        className="server-config-fab"
        onClick={() => setServerConfigOpen(true)}
        title="Configurar servidor"
        aria-label="Configurar servidor"
      >
        ⚙ Servidor
      </button>

      {serverConfigOpen && (
        <ServerConfigModal
          onClose={() => setServerConfigOpen(false)}
          onSaved={handleServerChanged}
        />
      )}
    </div>
  );
}
