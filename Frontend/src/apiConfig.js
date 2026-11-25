// Direccion de la API. En la web se toma de VITE_API_URL; en el APK se puede
// cambiar desde el boton "Servidor" y queda guardada en el telefono.
const DEFAULT_API_BASE = (import.meta.env.VITE_API_URL || 'http://localhost:5000/api')
  .replace(/\/+$/, '');

export const API_BASE_STORAGE_KEY = 'saludanimal_api_base';

export function normalizeApiBase(value) {
  let normalized = String(value || '').trim();
  if (!normalized) return '';

  if (!/^https?:\/\//i.test(normalized)) normalized = `http://${normalized}`;
  normalized = normalized.replace(/\/+$/, '');
  // Si pegaron la URL de /health, dejar solo la base.
  normalized = normalized.replace(/\/health$/i, '');
  if (!/\/api$/i.test(normalized)) normalized = `${normalized}/api`;
  return normalized;
}

export function getApiBase() {
  try {
    return normalizeApiBase(localStorage.getItem(API_BASE_STORAGE_KEY)) || DEFAULT_API_BASE;
  } catch {
    return DEFAULT_API_BASE;
  }
}

export function saveApiBase(value) {
  const normalized = normalizeApiBase(value);
  if (!normalized) throw new Error('Escribe la direccion del servidor.');
  localStorage.setItem(API_BASE_STORAGE_KEY, normalized);
  return normalized;
}

export function resetApiBase() {
  try { localStorage.removeItem(API_BASE_STORAGE_KEY); } catch { /* ignore */ }
}

export function getHealthUrl(apiBase = getApiBase()) {
  return `${apiBase.replace(/\/api$/i, '')}/health`;
}

export { DEFAULT_API_BASE };
