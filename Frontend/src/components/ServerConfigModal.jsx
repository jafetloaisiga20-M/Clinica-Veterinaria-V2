import { useEffect, useRef, useState } from 'react';
import {
  API_BASE_STORAGE_KEY,
  DEFAULT_API_BASE,
  getApiBase,
  getHealthUrl,
  normalizeApiBase,
  resetApiBase,
  saveApiBase,
} from '../apiConfig';
import './ServerConfigModal.css';

export default function ServerConfigModal({ onClose, onSaved }) {
  const [value, setValue] = useState(getApiBase);
  const [testing, setTesting] = useState(false);
  const [testResult, setTestResult] = useState(null);
  const [error, setError] = useState('');
  const inputRef = useRef(null);

  useEffect(() => {
    const timer = setTimeout(() => inputRef.current?.focus(), 50);
    return () => clearTimeout(timer);
  }, []);

  useEffect(() => {
    const onKey = (event) => { if (event.key === 'Escape') onClose?.(); };
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [onClose]);

  let savedValue = '';
  try { savedValue = localStorage.getItem(API_BASE_STORAGE_KEY) || ''; } catch { /* ignore */ }

  async function testConnection() {
    setError('');
    setTestResult(null);
    const base = normalizeApiBase(value);
    if (!base) {
      setError('Escribe la direccion del servidor.');
      return;
    }

    setTesting(true);
    const controller = new AbortController();
    const timer = setTimeout(() => controller.abort(), 7000);
    try {
      const response = await fetch(getHealthUrl(base), { signal: controller.signal });
      let data = null;
      try { data = await response.json(); } catch { /* ignore */ }

      if (data?.database && data.database !== 'ok') {
        setTestResult({ ok: false, message: 'La API responde, pero la base de datos no esta OK.' });
        return;
      }
      if (!response.ok) throw new Error(`El servidor respondio ${response.status}.`);
      setTestResult({ ok: true, message: 'Conexion correcta. La API y la base de datos responden.' });
    } catch (err) {
      const message = err?.name === 'AbortError'
        ? 'La conexion tardo demasiado. Revisa la IP, el Wi-Fi y el firewall.'
        : (err instanceof TypeError
          ? 'No se pudo conectar. Verifica que la API este encendida y que el telefono este en la misma Wi-Fi.'
          : err.message || 'No se pudo conectar.');
      setTestResult({ ok: false, message });
    } finally {
      clearTimeout(timer);
      setTesting(false);
    }
  }

  function handleSave(event) {
    event.preventDefault();
    setError('');
    try {
      const normalized = saveApiBase(value);
      setValue(normalized);
      onSaved?.(normalized);
    } catch (err) {
      setError(err.message || 'No se pudo guardar la direccion.');
    }
  }

  function handleReset() {
    resetApiBase();
    const resetValue = getApiBase();
    setValue(resetValue);
    onSaved?.(resetValue);
  }

  return (
    <div
      className="server-modal-overlay"
      role="presentation"
      onMouseDown={(event) => { if (event.target === event.currentTarget) onClose?.(); }}
    >
      <div className="server-modal" role="dialog" aria-modal="true" aria-labelledby="server-modal-title">
        <div className="server-modal-header">
          <div>
            <h2 id="server-modal-title">Servidor de la clinica</h2>
            <p>Direccion de la API que usa esta app.</p>
          </div>
          <button type="button" className="server-modal-close" onClick={onClose} aria-label="Cerrar">×</button>
        </div>

        <form onSubmit={handleSave}>
          <label className="server-modal-label" htmlFor="server-url">Direccion de la API</label>
          <input
            id="server-url"
            ref={inputRef}
            className="server-modal-input"
            value={value}
            onChange={(event) => {
              setValue(event.target.value);
              setError('');
              setTestResult(null);
            }}
            placeholder="http://192.168.1.50:5000/api"
            autoCapitalize="none"
            autoCorrect="off"
            spellCheck={false}
            inputMode="url"
          />
          <p className="server-modal-hint">
            En el telefono usa la IP de la computadora, no <code>localhost</code>. Ejemplo:
            <code> http://192.168.1.50:5000/api</code>
          </p>
          <p className="server-modal-current">
            Actual: <code>{savedValue || DEFAULT_API_BASE}</code>
          </p>

          {testResult && (
            <div className={`server-modal-result ${testResult.ok ? 'ok' : 'error'}`}>
              {testResult.ok ? '✓ ' : '⚠ '}{testResult.message}
            </div>
          )}
          {error && <div className="server-modal-result error">⚠ {error}</div>}

          <div className="server-modal-actions">
            <button type="button" className="btn btn-ghost" onClick={handleReset}>Restablecer</button>
            <button type="button" className="btn btn-outline" onClick={testConnection} disabled={testing}>
              {testing ? 'Probando…' : 'Probar conexion'}
            </button>
            <button type="submit" className="btn">Guardar y recargar</button>
          </div>
        </form>
      </div>
    </div>
  );
}
