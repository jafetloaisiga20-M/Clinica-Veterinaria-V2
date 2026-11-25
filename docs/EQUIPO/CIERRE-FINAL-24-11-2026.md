# Cierre final - Salud Animal (24/11/2026)

**Para:** Alfredo, Jean, Angeles y Julie
**De:** Yeskel (infraestructura y DevOps)
**Objetivo:** Dejar el sistema listo para mostrar el miercoles, compartir por Drive
y luego subir al repo para aprobacion.

## 1. Estado general

El sistema esta en un estado **funcional y demostrable en localhost**. Incluye un
sitio publico (inicio, servicios, agendar cita, contacto) y un panel interno.
Compila sin errores, pasa el smoke test (4/4) y tiene datos de ejemplo limpios:
3 clientes, 3 pacientes, 1 cita, 0 mensajes de redes.

## 2. Lo que se entrego

### Infraestructura
- Base de datos completa: 14 tablas (incluye `MENSAJE_SOCIAL`), 36 SPs y seed.
- Docker Compose con frontend (nginx), API (.NET 10) y SQL Server 2022.
- CI/CD: build backend/frontend/Docker, deploy manual a Azure, backup diario BACPAC.
- Scripts: `init-db.ps1`, `reset-db.ps1`, `backup-db.ps1`, `restore-db.ps1`,
  `smoke-test.ps1`, `test-webhook.ps1`.
- Documentacion: `README.md`, `docs/INFRAESTRUCTURA.md`, `docs/EQUIPO/...`.

### Backend (API)
- CORS y cadena de conexion por variables de entorno.
- Fail-fast si falta la conexion.
- `/health` (readiness, valida BD) y `/health/live` (liveness sin BD).
- `UseHsts()` en produccion.
- **Autenticacion JWT** con roles (Administrador, Veterinario, Recepcion, Cliente).
- Endpoints protegidos con `[Authorize]`; login en `/api/Auth/Login`.
- Usuarios de prueba: admin/Admin123!, recepcion/Recep123!, veterinario/Vet123!
- Passwords con bcrypt (ya no `PENDIENTE_DE_HASH`).
- Controladores: Cliente, Paciente, Citas, MensajeSocial, Expediente, Enmienda, Auth.
- Expediente clinico inmutable: solo crear y agregar enmiendas (no editar/eliminar).
- Webhooks de WhatsApp y Facebook con validacion de firma HMAC.
- Clasificacion automatica de mensajes (Solicitud de cita vs Consulta general).
- Validacion con `[Required]`, `[Range]`, `[EmailAddress]`.
- Mensajes de error neutros.
- JSON en PascalCase (ID_CLIENTE, NOMBRE_COMPLETO, ...).
- Choque de horarios al agendar citas (margen 30 min, responde 409).
- Rutas sin espacio: `NuevaCita`.
- Los POST de creacion devuelven el ID creado (`{ id, msj }`) para encadenar
  flujos (cliente -> mascota -> cita).

### Frontend (React + Vite) - sitio publico + panel interno
- Sitio publico real: Inicio, Servicios, Agendar cita, Contacto.
- Pagina de agendado en linea: crea cliente + mascota + cita en un solo flujo.
- Panel interno (Acceso interno) con login y pestanas: Clientes, Pacientes, Citas, Mensajes.
- Formularios para crear cliente, mascota, cita y registrar mensaje.
- Tablas responsivas, alertas, estilos propios, hero, tarjetas de servicios.
- Codificacion de texto corregida.
- Lint: 0 warnings. Build OK.
- Deep links a WhatsApp y Facebook + boton "Simular mensaje de WhatsApp".

### Seguridad y calidad
- Backlog trazable (`BACKLOG-RF-RNF.md`).
- Regla de no quemar datos documentada.
- Backup BACPAC generado con SHA256 y restaurado en una base de prueba.
- Respaldo nativo `.bak` verificado con `RESTORE VERIFYONLY` y restaurado en
  `Veterinaria_SaludAnimal_RestoreTest` (mismos conteos).

## 3. Como correrlo (para el profesor o QA)

```powershell
# 1. Base de datos
.\scripts\init-db.ps1 -Server "." -WindowsAuth

# 2. API
$env:ConnectionStrings__default = "Server=.;Database=Veterinaria_SaludAnimal;Integrated Security=True;TrustServerCertificate=True"
dotnet run --project PRESENTATION\PRESENTATION.csproj

# 3. Frontend (otra terminal)
cd Frontend
if (-not (Test-Path .env)) { Copy-Item .env.example .env }
npm ci
npm run dev
```

- Frontend: http://localhost:5173
- API: http://localhost:5000
- Health: http://localhost:5000/health
- Login: admin/Admin123!, recepcion/Recep123!, veterinario/Vet123!
- Smoke test: `.\scripts\smoke-test.ps1 -BaseUrl "http://localhost:5000"`

## 4. Como probar redes sociales en local (sin Meta)

- Frontend -> pestana Mensajes -> "Simular mensaje de WhatsApp".
- O ejecutar: `.\scripts\test-webhook.ps1 -Texto "Quiero agendar cita"`.
- Verificar en `GET /api/MensajeSocial` o en la pestana Mensajes.

## 5. Tareas para pulir (por persona)

### Jean (backend)
1. Autenticacion y RBAC (login, roles, aislamiento por sucursal).
2. Hashing bcrypt para usuarios.
3. Controladores faltantes: Factura, Servicio, Veterinario, Sucursal,
   TurnoGuardia, Usuario/Rol, Enmienda/Expediente.
4. Inmutabilidad del expediente clinico y enmiendas anexas.
5. Implementar editar/eliminar donde hay `NotImplementedException`.
6. Conectar credenciales de Meta y templates de notificacion.
7. Pruebas unitarias en Dominio/Application.

### Alfredo (frontend)
1. Vistas para facturas, servicios, veterinarios, turnos.
2. Manejo de errores de red en la UI.
3. Integrar autenticacion cuando este lista.
4. PWA y responsive avanzado.

### Angeles y Julie (QA)
1. Ejecutar backup antes de cada ronda.
2. Casos minimos ya documentados.
3. Reportar bugs con endpoint, cuerpo, respuesta y hora.

### Yeskel (infraestructura)
1. Crear cuenta de hosting y base en la nube (o PC-servidor con Docker).
2. Configurar secrets/vars en GitHub.
3. Ejecutar backup real y probar restauracion.
4. Configurar monitoreo (UptimeRobot).
5. Activar HTTPS Only en App Service.

## 6. Pendientes conocidos

- Sin facturacion ni impresion.
- Sin sincronizacion multi-sucursal.
- Sin pruebas automatizadas ni de carga.
- Frontend sin vistas de facturas, servicios, veterinarios, turnos.
- Integracion real con Meta (WhatsApp/Facebook) requiere credenciales.

## 7. Nota sobre commit

Los cambios aun no estan en GitHub. Se compartiran por Drive en el archivo
RAR para aprobacion del equipo y luego se subiran al repo (rama `infra/yeskel`).

## 8. Archivo compartido

`Clinica-Veterinaria-V2.rar` (escritorio) contiene el proyecto completo
sin carpetas de build (`bin`, `obj`, `node_modules`, `backups`).