import { useEffect, useRef, useState } from 'react';
import './LoginView.css';

const DEMO_ACCOUNTS = [
  {
    id: 'admin',
    role: 'Administrador',
    user: 'admin',
    pass: 'Admin123!',
    hint: 'Control total del sistema',
  },
  {
    id: 'recepcion',
    role: 'Recepción',
    user: 'recepcion',
    pass: 'Recep123!',
    hint: 'Citas, clientes y leads',
  },
  {
    id: 'veterinario',
    role: 'Veterinario',
    user: 'veterinario',
    pass: 'Vet123!',
    hint: 'Pacientes y expedientes',
  },
];

const SAVED_USER_KEY = 'saludanimal_saved_user';

export default function LoginView({ onLogin, onBack, loggingIn, loginError, onClearError }) {
  const [username, setUsername] = useState(() => localStorage.getItem(SAVED_USER_KEY) || '');
  const [password, setPassword] = useState('');
  const [rememberUser, setRememberUser] = useState(() => Boolean(localStorage.getItem(SAVED_USER_KEY)));
  const [showPassword, setShowPassword] = useState(false);
  const [capsLockOn, setCapsLockOn] = useState(false);
  const [touched, setTouched] = useState({ username: false, password: false });
  const [selectedDemo, setSelectedDemo] = useState('');

  const usernameRef = useRef(null);
  const submitRef = useRef(null);

  useEffect(() => {
    usernameRef.current?.focus({ preventScroll: true });
  }, []);

  const usernameTrimmed = username.trim();
  const usernameInvalid = touched.username && !usernameTrimmed;
  const passwordInvalid = touched.password && !password;

  function handleKeyEvent(e) {
    if (typeof e.getModifierState === 'function') {
      setCapsLockOn(e.getModifierState('CapsLock'));
    }
  }

  function handleUsernameChange(e) {
    setUsername(e.target.value);
    setSelectedDemo('');
    if (loginError) onClearError?.();
  }

  function handlePasswordChange(e) {
    setPassword(e.target.value);
    setSelectedDemo('');
    if (loginError) onClearError?.();
  }

  function handleDemoSelect(account) {
    setUsername(account.user);
    setPassword(account.pass);
    setSelectedDemo(account.id);
    setTouched({ username: false, password: false });
    onClearError?.();
    submitRef.current?.focus();
  }

  function handleSubmit(e) {
    e.preventDefault();
    setTouched({ username: true, password: true });

    if (!usernameTrimmed || !password) {
      return;
    }

    if (rememberUser) {
      localStorage.setItem(SAVED_USER_KEY, usernameTrimmed);
    } else {
      localStorage.removeItem(SAVED_USER_KEY);
    }

    onLogin({ username: usernameTrimmed, password });
  }

  return (
    <section className="login-shell" aria-label="Inicio de sesión del personal">
      <div className="login-card">
        {/* Panel izquierdo: Identidad clínica y contexto */}
        <aside className="login-aside">
          <div className="login-aside-top">
            <div className="login-badge">
              <span className="login-badge-dot" aria-hidden="true" />
              Portal del Personal
            </div>

            <div className="login-brand">
              <div className="login-brand-icon" aria-hidden="true">
                <svg viewBox="0 0 32 32" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <path
                    d="M16 28C22.6274 28 28 22.6274 28 16C28 9.37258 22.6274 4 16 4C9.37258 4 4 9.37258 4 16C4 22.6274 9.37258 28 16 28Z"
                    fill="rgba(255,255,255,0.14)"
                  />
                  <ellipse cx="11.2" cy="12" rx="1.8" ry="2.4" fill="currentColor" />
                  <ellipse cx="20.8" cy="12" rx="1.8" ry="2.4" fill="currentColor" />
                  <ellipse cx="8.6" cy="16.5" rx="1.6" ry="2.1" transform="rotate(-18 8.6 16.5)" fill="currentColor" />
                  <ellipse cx="23.4" cy="16.5" rx="1.6" ry="2.1" transform="rotate(18 23.4 16.5)" fill="currentColor" />
                  <path
                    d="M16 15.2C13.1 15.2 11 17.5 11 19.9C11 21.5 12.2 22.6 13.7 22.6C14.6 22.6 15.3 22.2 16 22.2C16.7 22.2 17.4 22.6 18.3 22.6C19.8 22.6 21 21.5 21 19.9C21 17.5 18.9 15.2 16 15.2Z"
                    fill="currentColor"
                  />
                </svg>
              </div>
              <div>
                <h2>Salud Animal</h2>
                <p>Sistema de Gestión Veterinaria</p>
              </div>
            </div>

            <p className="login-aside-desc">
              Accede al entorno clínico interno para gestionar citas, propietarios, pacientes y mensajes de atención.
            </p>
          </div>

          <div className="login-roles-list" aria-label="Perfiles habilitados">
            <div className="login-role-item">
              <span className="login-role-icon" aria-hidden="true">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
                </svg>
              </span>
              <div>
                <strong>Administrador</strong>
                <span>Sucursales, usuarios y control general</span>
              </div>
            </div>

            <div className="login-role-item">
              <span className="login-role-icon" aria-hidden="true">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M4.8 2.3A.3.3 0 1 0 5 2H4a2 2 0 0 0-2 2v5a6 6 0 0 0 6 6v0a6 6 0 0 0 6-6V4a2 2 0 0 0-2-2h-1a.2.2 0 1 0 .3.3" />
                  <path d="M8 15v1a6 6 0 0 0 6 6v0a6 6 0 0 0 6-6v-4" />
                  <circle cx="20" cy="10" r="2" />
                </svg>
              </span>
              <div>
                <strong>Veterinario</strong>
                <span>Consultas, historial clínico y guardias</span>
              </div>
            </div>

            <div className="login-role-item">
              <span className="login-role-icon" aria-hidden="true">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <rect x="3" y="4" width="18" height="18" rx="2" ry="2" />
                  <line x1="16" y1="2" x2="16" y2="6" />
                  <line x1="8" y1="2" x2="8" y2="6" />
                  <line x1="3" y1="10" x2="21" y2="10" />
                </svg>
              </span>
              <div>
                <strong>Recepción</strong>
                <span>Agenda de citas, clientes y WhatsApp</span>
              </div>
            </div>
          </div>

          <div className="login-aside-footer">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" aria-hidden="true">
              <rect x="3" y="11" width="18" height="11" rx="2" ry="2" />
              <path d="M7 11V7a5 5 0 0 1 10 0v4" />
            </svg>
            <span>Sesión protegida mediante token JWT · Uso interno</span>
          </div>
        </aside>

        {/* Panel derecho: Formulario de inicio de sesión */}
        <div className="login-form-panel">
          <div className="login-panel-nav">
            <button type="button" className="login-back-btn" onClick={onBack}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" aria-hidden="true">
                <line x1="19" y1="12" x2="5" y2="12" />
                <polyline points="12 19 5 12 12 5" />
              </svg>
              Volver al sitio público
            </button>
          </div>

          <div className="login-form-header">
            <h1>Iniciar sesión</h1>
            <p>Ingresa tu usuario y contraseña institucional para continuar.</p>
          </div>

          {loginError && (
            <div className="login-error-banner" role="alert">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" aria-hidden="true">
                <circle cx="12" cy="12" r="10" />
                <line x1="12" y1="8" x2="12" y2="12" />
                <line x1="12" y1="16" x2="12.01" y2="16" />
              </svg>
              <span>{loginError}</span>
              <button
                type="button"
                className="login-error-close"
                onClick={onClearError}
                aria-label="Cerrar mensaje de error"
              >
                ×
              </button>
            </div>
          )}

          <form onSubmit={handleSubmit} className="login-form" noValidate>
            <div className={`login-field ${usernameInvalid ? 'has-error' : ''}`}>
              <label htmlFor="login-username">Usuario</label>
              <div className="login-input-wrap">
                <span className="login-input-icon" aria-hidden="true">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                    <circle cx="12" cy="7" r="4" />
                  </svg>
                </span>
                <input
                  ref={usernameRef}
                  id="login-username"
                  type="text"
                  autoComplete="username"
                  placeholder="Ej. admin, recepcion, veterinario"
                  value={username}
                  onChange={handleUsernameChange}
                  onBlur={() => setTouched((prev) => ({ ...prev, username: true }))}
                  aria-invalid={usernameInvalid}
                  aria-describedby={usernameInvalid ? 'login-username-err' : undefined}
                />
              </div>
              {usernameInvalid && (
                <span id="login-username-err" className="login-field-msg">
                  Ingresa tu nombre de usuario.
                </span>
              )}
            </div>

            <div className={`login-field ${passwordInvalid ? 'has-error' : ''}`}>
              <div className="login-label-row">
                <label htmlFor="login-password">Contraseña</label>
                {capsLockOn && (
                  <span className="login-capslock" role="status">
                    Bloq Mayús activado
                  </span>
                )}
              </div>
              <div className="login-input-wrap">
                <span className="login-input-icon" aria-hidden="true">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <rect x="3" y="11" width="18" height="11" rx="2" ry="2" />
                    <path d="M7 11V7a5 5 0 0 1 10 0v4" />
                  </svg>
                </span>
                <input
                  id="login-password"
                  type={showPassword ? 'text' : 'password'}
                  autoComplete="current-password"
                  placeholder="Ingresa tu contraseña"
                  value={password}
                  onChange={handlePasswordChange}
                  onKeyUp={handleKeyEvent}
                  onKeyDown={handleKeyEvent}
                  onBlur={() => setTouched((prev) => ({ ...prev, password: true }))}
                  aria-invalid={passwordInvalid}
                  aria-describedby={passwordInvalid ? 'login-password-err' : undefined}
                />
                <button
                  type="button"
                  className="login-toggle-pass"
                  onClick={() => setShowPassword((prev) => !prev)}
                  aria-label={showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'}
                  title={showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'}
                >
                  {showPassword ? (
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" aria-hidden="true">
                      <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94" />
                      <path d="M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19" />
                      <line x1="1" y1="1" x2="23" y2="23" />
                    </svg>
                  ) : (
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" aria-hidden="true">
                      <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
                      <circle cx="12" cy="12" r="3" />
                    </svg>
                  )}
                </button>
              </div>
              {passwordInvalid && (
                <span id="login-password-err" className="login-field-msg">
                  Ingresa tu contraseña.
                </span>
              )}
            </div>

            <div className="login-options">
              <label className="login-remember">
                <input
                  type="checkbox"
                  checked={rememberUser}
                  onChange={(e) => setRememberUser(e.target.checked)}
                />
                <span>Recordar mi usuario</span>
              </label>
              <span className="login-help-tip" title="Solicita el restablecimiento al administrador de la clínica">
                ¿Olvidaste tu clave? Contacta a Administración
              </span>
            </div>

            <button ref={submitRef} className="login-submit-btn" type="submit" disabled={loggingIn}>
              {loggingIn ? (
                <>
                  <span className="login-spinner" aria-hidden="true" />
                  Verificando credenciales...
                </>
              ) : (
                <>
                  Ingresar al panel
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" aria-hidden="true">
                    <line x1="5" y1="12" x2="19" y2="12" />
                    <polyline points="12 5 19 12 12 19" />
                  </svg>
                </>
              )}
            </button>
          </form>

          {/* Accesos rápidos para entorno de pruebas / QA */}
          <div className="login-demo-box">
            <div className="login-demo-header">
              <span>Acceso rápido (cuentas de prueba)</span>
              <small>Haz clic para rellenar</small>
            </div>
            <div className="login-demo-grid">
              {DEMO_ACCOUNTS.map((acc) => {
                const isSelected = selectedDemo === acc.id;
                return (
                  <button
                    key={acc.id}
                    type="button"
                    className={`login-demo-chip ${isSelected ? 'selected' : ''}`}
                    onClick={() => handleDemoSelect(acc)}
                    title={`Usuario: ${acc.user} / Clave: ${acc.pass}`}
                  >
                    <strong>{acc.role}</strong>
                    <span>{acc.user}</span>
                  </button>
                );
              })}
            </div>
            {selectedDemo && (
              <p className="login-demo-status">
                Credenciales de <strong>{DEMO_ACCOUNTS.find((a) => a.id === selectedDemo)?.role}</strong> cargadas. Presiona &ldquo;Ingresar al panel&rdquo;.
              </p>
            )}
          </div>
        </div>
      </div>
    </section>
  );
}
