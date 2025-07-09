# Sistema de Ventas Web (E-commerce)

Este proyecto es un sistema de ventas web completo desarrollado en ASP.NET MVC, diseñado para la gestión y operación de una tienda en línea. Incluye un panel administrativo robusto para la gestión de datos maestros y reportes, así como una interfaz pública y completa para los clientes de la tienda.

## Características Principales

### Panel Administrativo
* **Gestión de Usuarios:** Creación, lectura, actualización y eliminación (CRUD) de usuarios administradores. Funcionalidades para autenticación, cambio y restablecimiento de contraseñas de administradores.
* **Gestión de Datos Maestros:**
    * **Categorías:** CRUD de categorías de productos.
    * **Marcas:** CRUD de marcas de productos.
    * **Productos:** CRUD completo de productos, incluyendo la carga y visualización de imágenes, asociación con categorías y marcas, gestión de precios y stock.
* **Dashboard:** Panel de control con métricas clave (cantidad de clientes, total de ventas, total de productos).
* **Reportes de Ventas:** Generación de reportes detallados con filtros por fecha e ID de transacción. Exportación de reportes a Excel.

### Tienda en Línea para Clientes
* **Autenticación de Clientes:** Registro, inicio de sesión, cambio y restablecimiento de contraseña para clientes.
* **Catálogo de Productos:** Visualización de productos con imágenes, descripciones, precios, y detalles de marca. Incluye una descripción colapsable para mayor legibilidad.
* **Búsqueda y Filtrado:** Funcionalidad de búsqueda por nombre/descripción y filtros por categoría, marca y rango de precios.
* **Carrito de Compras:**
    * Agregar productos al carrito.
    * Actualizar cantidades de productos en el carrito.
    * Eliminar productos del carrito.
    * Visualización del total a pagar en el carrito.
* **Proceso de Pago con PayPal:** **Integración completa con la API REST de PayPal para procesar pagos de manera segura.** Los clientes son redirigidos a la plataforma de PayPal para completar la transacción y luego vuelven a la tienda.
* **Historial de Compras:** Los clientes pueden acceder a un historial detallado de sus compras, incluyendo productos, cantidades, totales y el ID de la transacción de PayPal.

## Tecnologías Utilizadas

* **Backend:** ASP.NET MVC (.NET Framework 4.7.2), C#.
* **Base de Datos:** SQL Server (configurado con `Integrated Security=True` y catálogo inicial `DB_CARRITO`).
* **Frontend:** HTML5, CSS3, JavaScript, jQuery, Bootstrap, DataTables (para tablas interactivas), SweetAlert (para notificaciones al usuario), LoadingOverlay (para efectos de carga).
* **Integraciones:** **PayPal REST API (para la gestión de pagos).**
* **Librerías Adicionales:** ClosedXML (para exportación de datos a Excel), Newtonsoft.Json (para serialización/deserialización JSON).

## Estructura de Capas (Modelo de Arquitectura)

El proyecto sigue una arquitectura en capas para una mejor organización, mantenibilidad y escalabilidad:

* **CapaEntidad:** Define las entidades o modelos de datos que representan la estructura de la información (Ej: `clsUsuario`, `clsProducto`, `clsCliente`).
* **CapaNegocio:** Contiene la lógica de negocio de la aplicación. Actúa como intermediario entre la Capa de Presentación y la Capa de Datos, aplicando reglas de negocio y validaciones.
* **CapaDatos:** Se encarga de la interacción directa con la base de datos (Ej: operaciones CRUD).
* **CapaPresentacionAdmin:** Proyecto ASP.NET MVC para el panel administrativo, con controladores (`AccessController`, `HomeController`, `MantenedorController`) y vistas (`.cshtml`) dedicadas a la gestión interna.
* **CapaPresentacionTienda:** Proyecto ASP.NET MVC para la tienda en línea orientada al cliente, con sus propios controladores (`AccessController`, `StoreController`) y vistas (`.cshtml`) para la experiencia de compra.

## Configuración del Proyecto

### Requisitos Previos

* Visual Studio (preferiblemente 2019 o superior)
* SQL Server
* .NET Framework 4.7.2
* IIS (Internet Information Services) para el despliegue local.

### Pasos para la Configuración

1.  **Clonar el Repositorio:**
    ```bash
    git clone <URL_DEL_REPOSITORIO>
    ```

2.  **Configuración de la Base de Datos:**
    * Crear una base de datos en SQL Server llamada `DB_CARRITO`.
    * **Importante:** Ejecutar los scripts SQL necesarios para la creación de tablas, relaciones y la inserción de datos iniciales (usuarios, categorías, etc.). Estos scripts no están incluidos en los archivos proporcionados, se asume que los tienes o deberás crearlos.
    * Actualizar la cadena de conexión en los archivos `Web.config` de `CapaPresentacionAdmin` y `CapaPresentacionTienda` para que apunten a tu instancia de SQL Server.
        ```xml
        <connectionStrings>
            <add name="CadenaConexion" connectionString="Data Source=TuServidorSQL; Initial Catalog=DB_CARRITO;Integrated Security=True" providerName="System.Data.ProviderName" />
        </connectionStrings>
        ```
        **Nota:** Reemplaza `TuServidorSQL` con el nombre de tu servidor SQL Server (ej: `.\SQLEXPRESS` o `localhost\SQLEXPRESS`).

3.  **Configuración de Rutas de Imágenes (CapaPresentacionAdmin):**
    * En el `Web.config` del proyecto `CapaPresentacionAdmin`, actualiza la clave `ServidorDeFotos` con la ruta local absoluta donde se guardarán las imágenes de los productos.
        ```xml
        <add key="ServidorDeFotos" value="C:\Ruta\Absoluta\Donde\Guardar\FotosCarrito" />
        ```
        **Nota:** Asegúrate de que la aplicación tenga permisos de escritura en esta carpeta.

4.  **Configuración de PayPal (CapaPresentacionTienda):**
    * En el `Web.config` del proyecto `CapaPresentacionTienda`, actualiza las claves `UrlPaypal`, `ClientID` y `Secret` con tus credenciales de PayPal Developer. Se recomienda encarecidamente usar el entorno Sandbox para todas las pruebas.
        ```xml
        <add key="UrlPaypal" value="[https://api-m.sandbox.paypal.com](https://api-m.sandbox.paypal.com)" />
        <add key="ClientID" value="TU_CLIENT_ID_DE_PAYPAL_SANDBOX" />
        <add key="Secret" value="TU_SECRET_DE_PAYPAL_SANDBOX" />
        ```
        Puedes obtener credenciales de prueba en [PayPal Developer](https://developer.paypal.com/).

5.  **Abrir en Visual Studio:**
    * Abrir el archivo de solución (`.sln`) del proyecto en Visual Studio.

6.  **Restaurar Paquetes NuGet:**
    * Asegúrate de que todos los paquetes NuGet se restauren correctamente. Visual Studio debería hacerlo automáticamente al abrir el proyecto, o puedes forzarlo haciendo clic derecho en la solución y seleccionando "Restore NuGet Packages".

7.  **Compilar la Solución:**
    * Compila la solución completa (Build Solution) para asegurar que no haya errores de compilación.

8.  **Ejecutar la Aplicación:**
    * Configura el proyecto `CapaPresentacionAdmin` como proyecto de inicio para acceder al panel administrativo, o `CapaPresentacionTienda` para acceder a la tienda en línea.
    * Ejecuta el proyecto desde Visual Studio (presionando `F5` o haciendo clic en el botón "IIS Express" o tu servidor web configurado).

## Contribuciones

Las contribuciones son bienvenidas. Si deseas mejorar este proyecto, por favor, sigue estos pasos:

1.  Haz un "fork" de este repositorio.
2.  Crea una nueva rama (`git checkout -b feature/AmazingFeature`).
3.  Realiza tus cambios y commitea (`git commit -m 'Add some AmazingFeature'`).
4.  Sube tus cambios (`git push origin feature/AmazingFeature`).
5.  Abre un Pull Request.

## Licencia

Este proyecto está bajo la Licencia MIT. Consulta el archivo `LICENSE` en el repositorio para más detalles.

---
**Desarrollado por:** Juan José, Julián Suárez
**Año:** 2025
