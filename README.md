<div align="center">

# Blaxtor Gym

### La gestión de tu gimnasio, más clara y organizada.

Aplicación de escritorio para Windows que sienta las bases para administrar clientes y organizar la operación diaria de un gimnasio desde un solo lugar.

<br />

![Plataforma](https://img.shields.io/badge/plataforma-Windows-2563EB?style=for-the-badge&logo=windows&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Interfaz](https://img.shields.io/badge/interfaz-WPF-0F766E?style=for-the-badge)
![Estado](https://img.shields.io/badge/estado-en%20desarrollo-F59E0B?style=for-the-badge)

</div>

---

## Una herramienta para el día a día

Blaxtor Gym es un sistema de gestión de escritorio diseñado para ayudar a mantener la información del gimnasio ordenada y accesible. El proyecto está construido sobre una arquitectura por capas y utiliza una base de datos local, por lo que puede ejecutarse en Windows sin depender de un servidor de base de datos externo.

### Funcionalidad disponible

| Área | Descripción |
| --- | --- |
| **Acceso** | Inicio de sesión con validación de credenciales, contraseñas protegidas mediante hash y bloqueo temporal después de varios intentos fallidos. |
| **Clientes** | Registro y edición de datos, consulta de clientes y activación o desactivación de registros. |
| **Tipos de cliente** | Creación, edición y cambio de estado de categorías de clientes. |
| **Mediciones físicas** | Registro de peso y datos físicos opcionales, con historial por cliente. |
| **Datos locales** | Persistencia en SQLite y aplicación automática de migraciones al iniciar. |

### Módulos en desarrollo

La estructura del proyecto también contempla membresías, pagos, notificaciones, reportes, tarifas y configuración. Estos módulos aún no están implementados como funcionalidades listas para uso.

---

## Tecnologías

- **C# y .NET 10**
- **WPF** para la aplicación de escritorio
- **Entity Framework Core 10** para acceso a datos y migraciones
- **SQLite** como base de datos local
- **BCrypt** para el hash de contraseñas
- **MahApps.Metro** y **Material Design** para componentes visuales

## Requisitos

- Windows 10 o posterior
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git, si vas a clonar el repositorio

## Inicio rápido

Clona el repositorio y entra en la carpeta del proyecto:

```powershell
git clone https://github.com/JustinRojasJarquin/BlaxtorGym.git
cd BlaxtorGym
```

Restaura dependencias y ejecuta la aplicación:

```powershell
dotnet restore .\BlaxtorGym\BlaxtorGym\BlaxtorGym.csproj
dotnet run --project .\BlaxtorGym\BlaxtorGym\BlaxtorGym.csproj
```

Para compilar sin iniciar la aplicación:

```powershell
dotnet build .\BlaxtorGym\BlaxtorGym\BlaxtorGym.csproj
```

Al iniciar, la aplicación prepara la base de datos local y aplica las migraciones pendientes. El archivo SQLite se guarda en `Data\blaxtorgym.db` junto a la aplicación.

## Estructura del proyecto

```text
BlaxtorGym/
├── BlaxtorGym.slnx
└── BlaxtorGym/
    ├── BlaxtorGym.csproj
    ├── App.xaml
    ├── MainWindow.xaml
    ├── Migrations/
    └── src/
        ├── BlaxtorGym.Domain/          # Entidades y reglas del dominio
        ├── BlaxtorGym.Application/     # Servicios y lógica de aplicación
        ├── BlaxtorGym.Infraestructure/ # SQLite, repositorios y servicios
        └── BlaxtorGym.WPF/             # Interfaz y modelos de vista
```

## Estado del proyecto

Blaxtor Gym se encuentra en desarrollo activo. La autenticación y los flujos de administración de clientes y sus categorías forman la base funcional actual; las demás áreas están en proceso de construcción. La disponibilidad de módulos puede cambiar a medida que avance el proyecto.

## Contribuciones

Las sugerencias y contribuciones son bienvenidas. Para proponer un cambio, abre un *issue* describiendo la necesidad o envía un *pull request* con una explicación clara de la mejora.

---

<div align="center">

**Blaxtor Gym** · Organización para el gimnasio. Claridad para cada jornada.

</div>
