# Backlog trazable - Salud Animal

Mapeo de los requerimientos de la especificacion (Fase 1) contra el estado actual
del repositorio. Sirve para priorizar sin perder de vista el objetivo final y para
no "quemar" datos ni funcionalidad ya validada.

## Leyenda de estado
- **Hecho**: implementado y probado.
- **Parcial**: existe la base (tabla/SP/repo) pero falta controller, validacion o regla.
- **Pendiente**: no existe.
- **Infra**: cubierto por infraestructura (deploy, backup, CI/CD).

## Requerimientos funcionales

| RF | Descripcion | Estado | Responsable sugerido | Nota |
| --- | --- | --- | --- | --- |
| RF-01 | Sincronizacion multi-sucursal | Parcial | Jean | Existe `SUCURSAL`; falta logica de sincronizacion/replicacion. |
| RF-02 | Registrar >= 8 veterinarios con especialidad y CV | Parcial | Jean | Tabla `VETERINARIO`; seed solo 2; falta endpoint y carga de CV. |
| RF-03 | Expediente clinico (EHR) inmutable + enmiendas | Parcial | Jean | Tablas `EXPEDIENTE_MEDICO` y `ENMIENDA`; falta controller y regla de inmutabilidad. |
| RF-04 | Agendar cita con canal (Llamada/WhatsApp) y choque de horarios | Parcial | Jean | Tabla `CITA` y SP; falta validacion de colision del medico. |
| RF-05 | Turnos de guardia diurno/nocturno | Parcial | Jean | Tabla `TURNO_GUARDIA`; falta controller. |
| RF-06 | Factura detallada + impresion | Parcial | Jean | Tablas `FACTURA` y `DETALLE_FACTURA`; falta controller e impresion. |

## Historias de usuario

| US | Descripcion | Estado | Responsable sugerido |
| --- | --- | --- | --- |
| US-01 | Recepcionista agenda cita con canal | Parcial | Jean + Alfredo |
| US-02 | Veterinario registra diagnostico inalterable | Parcial | Jean + Alfredo |
| US-03 | Administrador genera reporte de ingresos | Pendiente | Jean + Alfredo |

## Requerimientos no funcionales

| RNF | Descripcion | Estado | Responsable sugerido | Nota |
| --- | --- | --- | --- | --- |
| RNF-01 | Respuesta <= 2s | Infra/Parcial | Yeskel + Jean | Health y smoke test; faltan pruebas de carga. |
| RNF-02 | Disponibilidad 99.9% | Infra | Yeskel | App Service + Azure SQL; monitoreo a definir. |
| RNF-03 | bcrypt + HTTPS/TLS 1.2+ | Pendiente | Jean + Yeskel | Passwords del seed son `PENDIENTE_DE_HASH`; falta HTTPS Only. |
| RNF-04 | RBAC y aislamiento por clinica | Pendiente | Jean | Sin autenticacion ni roles en la API. |
| RNF-05 | SOLID + Clean Architecture | Hecho/Parcial | Equipo | Estructura correcta; faltan pruebas unitarias. |

## Modulos expuestos por la API hoy

- `/health`
- `/api/Cliente` (GET, POST NuevoCliente)
- `/api/Paciente` (GET, POST "Nueva cita")
- `/api/Citas` (GET, POST "Nueva cita")

## Modulos en BD pero sin endpoint HTTP

- Factura y DetalleFactura
- Servicio
- Veterinario
- Sucursal
- TurnoGuardia
- Usuario y Rol
- EnmiendaMedica y ExpedienteMedico

## Calidad y seguridad pendientes (antes de pruebas formales)

1. Autenticacion y RBAC (login, roles, aislamiento por sucursal).
2. Hashing bcrypt para usuarios; quitar `PENDIENTE_DE_HASH`.
3. HTTPS Only en App Service y HSTS en la API.
4. Validaciones con atributos (`[Required]`, `[EmailAddress]`, etc.).
5. Mensajes de error neutros y sin `ex.Message`.
6. Corregir casing JSON (`PropertyNamingPolicy`) y rutas con espacio.
7. Inmutabilidad de notas clinicas y enmiendas anexas.
8. Validacion de choque de horarios en citas.
9. Pruebas unitarias (Dominio/Application) y de integracion (ADO.NET).
10. Pruebas de carga con 3 sucursales concurrentes.
11. Datos de prueba aislados y limpieza automatica.

## Infraestructura (Yeskel) - estado

- Base de datos completa (13 tablas, 36 SPs, seed).
- Docker Compose para QA.
- CI/CD (build, deploy manual, backup diario).
- Scripts de init/reset/backup/restore/smoke-test.
- Documentacion y mensaje al equipo.

## Orden sugerido de trabajo

1. Estabilizacion y seguridad (auth, bcrypt, HTTPS, validaciones).
2. Modulos clinicos (EHR, citas con colision, turnos, veterinarios).
3. Facturacion e informes.
4. Frontend multi-modulo y PWA.
5. Pruebas automatizadas y de carga.

## Regla de no quemar datos

- Toda prueba manual debe usar datos con prefijo `QA` o `Demo`.
- Limpiar al terminar o usar `scripts/reset-db.ps1` en local.
- Antes de cada ronda de QA, ejecutar el workflow "Backup base de datos".
- No modificar datos de produccion sin respaldo verificado.