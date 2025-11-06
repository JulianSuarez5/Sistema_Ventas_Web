-- Creación de la base de datos
CREATE DATABASE DB_CARRITO;
GO

-- Uso de la base de datos
USE DB_CARRITO;
GO

----------------------------------------------------------------------
-- DDL - CREACIÓN DE TABLAS
----------------------------------------------------------------------

CREATE TABLE CATEGORIA (
    IdCategoria    INT PRIMARY KEY IDENTITY,
    Descripcion    VARCHAR(100),
    Activo         BIT DEFAULT 1,
    FechaRegistro  DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE MARCA (
    IdMarca        INT PRIMARY KEY IDENTITY,
    Descripcion    VARCHAR(100),
    Activo         BIT DEFAULT 1,
    FechaRegistro  DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE PRODUCTO (
    IdProducto     INT PRIMARY KEY IDENTITY,
    Nombre         VARCHAR(500),
    Descripcion    VARCHAR(500),
    IdMarca        INT REFERENCES MARCA(IdMarca),
    IdCategoria    INT REFERENCES CATEGORIA(IdCategoria),
    Precio         DECIMAL(10, 2) DEFAULT 0,
    Stock          INT,
    RutaImagen     VARCHAR(100),
    NombreImagen   VARCHAR(100),
    Activo         BIT DEFAULT 1,
    FechaRegistro  DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE CLIENTE (
    IdCliente      INT PRIMARY KEY IDENTITY,
    Nombres        VARCHAR(100),
    Apellidos      VARCHAR(100),
    Correo         VARCHAR(100),
    Clave          VARCHAR(150),
    Restablecer    BIT DEFAULT 0,
    FechaRegistro  DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE CARRITO_COMPRAS (
    IdCarrito      INT PRIMARY KEY IDENTITY,
    IdCliente      INT REFERENCES CLIENTE(IdCliente),
    IdProducto     INT REFERENCES PRODUCTO(IdProducto),
    Cantidad       INT
);
GO

CREATE TABLE VENTA (
    IdVenta        INT PRIMARY KEY IDENTITY,
    IdCliente      INT REFERENCES CLIENTE(IdCliente),
    TotalProducto  INT,
    MontoTotal     DECIMAL(10, 2),
    Contacto       VARCHAR(50),
    IdLocalidad    VARCHAR(50),
    Telefono       VARCHAR(50),
    Direccion      VARCHAR(500),
    IdTransaccion  VARCHAR(50),
    FechaVenta     DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE DETALLE_VENTA (
    IdDetalleVenta INT PRIMARY KEY IDENTITY,
    IdVenta        INT REFERENCES VENTA(IdVenta),
    IdProducto     INT REFERENCES PRODUCTO(IdProducto),
    Cantidad       INT,
    Total          DECIMAL(10, 2)
);
GO

CREATE TABLE USUARIO (
    IdUsuario      INT PRIMARY KEY IDENTITY,
    Nombres        VARCHAR(100),
    Apellidos      VARCHAR(100),
    Correo         VARCHAR(150),
    Clave          VARCHAR(150),
    Restablecer    BIT DEFAULT 1,
    Activo         BIT DEFAULT 1,
    FechaRegistro  DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE DEPARTAMENTO (
    IdDepartamento  VARCHAR(100) NOT NULL,
    Descripcion     VARCHAR(100) NOT NULL
);
GO

CREATE TABLE MUNICIPIO (
    IdMunicipio     VARCHAR(100) NOT NULL,
    Descripcion     VARCHAR(100) NOT NULL,
    IdDepartamento  VARCHAR(100) NOT NULL
);
GO

CREATE TABLE LOCALIDAD (
    IdLocalidad     VARCHAR(100) NOT NULL,
    Descripcion     VARCHAR(100) NOT NULL,
    IdMunicipio     VARCHAR(100) NOT NULL,
    IdDepartamento  VARCHAR(100) NOT NULL
);
GO

----------------------------------------------------------------------
-- DDL - TIPOS DE TABLA DEFINIDOS 
----------------------------------------------------------------------

CREATE TYPE EDetalle_Venta AS TABLE (
    IdProducto  INT,
    Cantidad    INT,
    Total       DECIMAL(18, 2)
);
GO

----------------------------------------------------------------------
-- DML - PROCEDIMIENTOS ALMACENADOS
----------------------------------------------------------------------

-- PROCEDIMIENTO ALMACENADO PARA REGISTRAR UN USUARIO
CREATE PROCEDURE USP_RegistrarUsuario (
    @Nombres    VARCHAR(100),
    @Apellidos  VARCHAR(100),
    @Correo     VARCHAR(100),
    @Clave      VARCHAR(100),
    @Activo     BIT,
    @Mensaje    VARCHAR(500) OUTPUT,
    @Resultado  INT OUTPUT
)
AS
BEGIN
    SET @Resultado = 0;
    IF NOT EXISTS (SELECT * FROM USUARIO WHERE Correo = @Correo)
    BEGIN
        INSERT INTO USUARIO (Nombres, Apellidos, Correo, Clave, Activo)
        VALUES (@Nombres, @Apellidos, @Correo, @Clave, @Activo);

        SET @Resultado = SCOPE_IDENTITY();
    END;
    ELSE
    BEGIN
        SET @Mensaje = 'El correo del usuario ya existe';
    END;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA ACTUALIZAR USUARIO
CREATE PROCEDURE USP_ActualizarUsuario (
    @IdUsuario  INT,
    @Nombres    VARCHAR(100),
    @Apellidos  VARCHAR(100),
    @Correo     VARCHAR(100),
    @Activo     BIT,
    @Mensaje    VARCHAR(500) OUTPUT,
    @Resultado  INT OUTPUT
)
AS
BEGIN
    SET @Resultado = 0;
    IF NOT EXISTS (SELECT * FROM USUARIO WHERE Correo = @Correo AND IdUsuario != @IdUsuario)
    BEGIN
        UPDATE TOP (1) USUARIO
        SET
            Nombres   = @Nombres,
            Apellidos = @Apellidos,
            Correo    = @Correo,
            Activo    = @Activo
        WHERE
            IdUsuario = @IdUsuario;

        SET @Resultado = 1;
    END;
    ELSE
    BEGIN
        SET @Mensaje = 'El correo del usuario ya existe';
    END;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA REGISTRAR CATEGORÍA
CREATE PROCEDURE USP_RegistrarCategoria (
    @Descripcion  VARCHAR(100),
    @Activo       BIT,
    @Mensaje      VARCHAR(500) OUTPUT,
    @Resultado    INT OUTPUT
)
AS
BEGIN
    SET @Resultado = 0;
    IF NOT EXISTS (SELECT * FROM CATEGORIA WHERE Descripcion = @Descripcion)
    BEGIN
        INSERT INTO CATEGORIA (Descripcion, Activo)
        VALUES (@Descripcion, @Activo);

        SET @Resultado = SCOPE_IDENTITY();
    END;
    ELSE
    BEGIN
        SET @Mensaje = 'La categoria ya existe';
    END;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA ACTUALIZAR UNA CATEGORIA
CREATE PROCEDURE USP_ActualizarCategoria (
    @IdCategoria  INT,
    @Descripcion  VARCHAR(100),
    @Activo       BIT,
    @Mensaje      VARCHAR(500) OUTPUT,
    @Resultado    BIT OUTPUT
)
AS
BEGIN
    SET @Resultado = 0;
    IF NOT EXISTS (SELECT * FROM CATEGORIA WHERE Descripcion = @Descripcion AND IdCategoria != @IdCategoria)
    BEGIN
        UPDATE TOP (1) CATEGORIA
        SET
            Descripcion = @Descripcion,
            Activo      = @Activo
        WHERE
            IdCategoria = @IdCategoria;

        SET @Resultado = 1;
    END;
    ELSE
    BEGIN
        SET @Mensaje = 'La categoria ya existe';
    END;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA ELIMINAR UNA CATEGORIA
CREATE PROCEDURE USP_EliminarCategoria (
    @IdCategoria  INT,
    @Mensaje      VARCHAR(500) OUTPUT,
    @Resultado    BIT OUTPUT
)
AS
BEGIN
    SET @Resultado = 0;
    IF NOT EXISTS (
        SELECT
            *
        FROM
            PRODUCTO p
            INNER JOIN CATEGORIA c ON c.IdCategoria = p.IdCategoria
        WHERE
            P.IdCategoria = @IdCategoria
    )
    BEGIN
        DELETE TOP (1)
        FROM
            CATEGORIA
        WHERE
            IdCategoria = @IdCategoria;
        SET @Resultado = 1;
    END;
    ELSE
    BEGIN
        SET @Mensaje = 'La categoria se encuentra relacionada a un producto';
    END;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA REGISTRAR MARCA
CREATE PROCEDURE USP_RegistrarMarca (
    @Descripcion  VARCHAR(100),
    @Activo       BIT,
    @Mensaje      VARCHAR(500) OUTPUT,
    @Resultado    INT OUTPUT
)
AS
BEGIN
    SET @Resultado = 0;
    IF NOT EXISTS (SELECT * FROM MARCA WHERE Descripcion = @Descripcion)
    BEGIN
        INSERT INTO MARCA (Descripcion, Activo)
        VALUES (@Descripcion, @Activo);

        SET @Resultado = SCOPE_IDENTITY();
    END;
    ELSE
    BEGIN
        SET @Mensaje = 'La marca ya existe';
    END;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA ACTUALIZAR UNA MARCA
CREATE PROCEDURE USP_ActualizarMarca (
    @IdMarca      INT,
    @Descripcion  VARCHAR(100),
    @Activo       BIT,
    @Mensaje      VARCHAR(500) OUTPUT,
    @Resultado    BIT OUTPUT
)
AS
BEGIN
    SET @Resultado = 0;
    IF NOT EXISTS (SELECT * FROM MARCA WHERE Descripcion = @Descripcion AND IdMarca != @IdMarca)
    BEGIN
        UPDATE TOP (1) MARCA
        SET
            Descripcion = @Descripcion,
            Activo      = @Activo
        WHERE
            IdMarca = @IdMarca;

        SET @Resultado = 1;
    END;
    ELSE
    BEGIN
        SET @Mensaje = 'La marca ya existe';
    END;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA ELIMINAR UNA MARCA
CREATE PROCEDURE USP_EliminarMarca (
    @IdMarca    INT,
    @Mensaje    VARCHAR(500) OUTPUT,
    @Resultado  BIT OUTPUT
)
AS
BEGIN
    SET @Resultado = 0;
    IF NOT EXISTS (
        SELECT
            *
        FROM
            PRODUCTO p
            INNER JOIN MARCA m ON m.IdMarca = p.IdMarca
        WHERE
            p.IdMarca = @IdMarca
    )
    BEGIN
        DELETE TOP (1)
        FROM
            MARCA
        WHERE
            IdMarca = @IdMarca;
        SET @Resultado = 1;
    END;
    ELSE
    BEGIN
        SET @Mensaje = 'La marca se encuentra relacionada a un producto';
    END;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA REGISTRAR PRODUCTOS
CREATE PROCEDURE USP_RegistrarProducto (
    @Nombre      VARCHAR(500),
    @Descripcion VARCHAR(500),
    @IdMarca     VARCHAR(100),
    @IdCategoria VARCHAR(100),
    @Precio      DECIMAL(10, 2),
    @Stock       INT,
    @Activo      BIT,
    @Mensaje     VARCHAR(500) OUTPUT,
    @Resultado   INT OUTPUT
)
AS
BEGIN
    SET @Resultado = 0;
    IF NOT EXISTS (SELECT * FROM PRODUCTO WHERE Nombre = @Nombre)
    BEGIN
        INSERT INTO PRODUCTO (Nombre, Descripcion, IdMarca, IdCategoria, Precio, Stock, Activo)
        VALUES (@Nombre, @Descripcion, @IdMarca, @IdCategoria, @Precio, @Stock, @Activo);

        SET @Resultado = SCOPE_IDENTITY();
    END;
    ELSE
    BEGIN
        SET @Mensaje = 'El producto ya existe';
    END;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA ACTUALIZAR UN PRODUCTO
CREATE PROCEDURE USP_ActualizarProducto (
    @IdProducto   INT,
    @Nombre       VARCHAR(500),
    @Descripcion  VARCHAR(500),
    @IdMarca      VARCHAR(100),
    @IdCategoria  VARCHAR(100),
    @Precio       DECIMAL(10, 2),
    @Stock        INT,
    @Activo       BIT,
    @Mensaje      VARCHAR(500) OUTPUT,
    @Resultado    INT OUTPUT
)
AS
BEGIN
    SET @Resultado = 0;
    IF NOT EXISTS (SELECT * FROM PRODUCTO WHERE Nombre = @Nombre AND IdProducto != @IdProducto)
    BEGIN
        UPDATE PRODUCTO
        SET
            Nombre      = @Nombre,
            Descripcion = @Descripcion,
            IdMarca     = @IdMarca,
            IdCategoria = IdCategoria,
            Precio      = @Precio,
            Stock       = @Stock,
            Activo      = @Activo
        WHERE
            IdProducto = @IdProducto;

        SET @Resultado = 1;
    END;
    ELSE
    BEGIN
        SET @Mensaje = 'Ell producto ya existe';
    END;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA ELIMINAR UN PRODUCTO
CREATE PROCEDURE USP_EliminarProducto (
    @IdProducto  INT,
    @Mensaje     VARCHAR(500) OUTPUT,
    @Resultado   BIT OUTPUT
)
AS
BEGIN
    SET @Resultado = 0;
    IF NOT EXISTS (
        SELECT
            *
        FROM
            DETALLE_VENTA dv
            INNER JOIN PRODUCTO p ON p.IdProducto = dv.IdProducto
        WHERE
            p.IdProducto = @IdProducto
    )
    BEGIN
        DELETE TOP (1)
        FROM
            PRODUCTO
        WHERE
            IdProducto = @IdProducto;
        SET @Resultado = 1;
    END;
    ELSE
    BEGIN
        SET @Mensaje = 'El producto se encuentra relacionado a una venta';
    END;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA REPORTE DE DASHBOARD
CREATE PROCEDURE USP_ReporteDashboard
AS
BEGIN
    SELECT
        (SELECT COUNT(*) FROM CLIENTE)         AS TotalCliente,
        (SELECT ISNULL(SUM(Cantidad), 0) FROM DETALLE_VENTA) AS TotalVenta,
        (SELECT COUNT(*) FROM PRODUCTO)        AS TotalProducto;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA REPORTE DE VENTAS
CREATE PROCEDURE USP_ReporteVentas (
    @FechaInicio  VARCHAR(10),
    @FechaFin     VARCHAR(10),
    @IdTransaccion VARCHAR(50)
)
AS
BEGIN
    SET DATEFORMAT DMY;

    SELECT
        CONVERT(CHAR(10), v.FechaVenta, 103) AS FechaVenta,
        CONCAT(c.Nombres, ' ', c.Apellidos)  AS Cliente,
        p.Nombre                             AS Producto,
        p.Precio,
        dv.Cantidad,
        dv.Total,
        v.IdTransaccion
    FROM
        DETALLE_VENTA dv
        INNER JOIN PRODUCTO p ON p.IdProducto = dv.IdProducto
        INNER JOIN VENTA v ON v.IdVenta = dv.IdVenta
        INNER JOIN CLIENTE c ON c.IdCliente = v.IdCliente
    WHERE
        CONVERT(DATE, v.FechaVenta) BETWEEN @FechaInicio AND @FechaFin
        AND v.IdTransaccion = IIF(@IdTransaccion = '', v.IdTransaccion, @IdTransaccion);
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA REGISTRAR CLIENTES
CREATE PROCEDURE USP_RegistrarClientes (
    @Nombres    VARCHAR(100),
    @Apellidos  VARCHAR(100),
    @Correo     VARCHAR(100),
    @Clave      VARCHAR(100),
    @Mensaje    VARCHAR(100) OUTPUT,
    @Resultado  INT OUTPUT
)
AS
BEGIN
    SET @Resultado = 0;
    IF NOT EXISTS (SELECT * FROM CLIENTE WHERE Correo = @Correo)
    BEGIN
        INSERT INTO CLIENTE (Nombres, Apellidos, Correo, Clave, Restablecer)
        VALUES (@Nombres, @Apellidos, @Correo, @Clave, 0);

        SET @Resultado = SCOPE_IDENTITY();
    END;
    ELSE
    BEGIN
        SET @Mensaje = 'El correo del cliente ya existe';
    END;
END;
GO

-- Consulta de ejemplo (no es un procedimiento almacenado)
DECLARE @IdCategoria INT = 0;

SELECT DISTINCT
    m.IdMarca,
    m.Descripcion
FROM
    PRODUCTO p
    INNER JOIN CATEGORIA c ON c.IdCategoria = p.IdCategoria
    INNER JOIN MARCA m ON m.IdMarca = p.IdMarca AND m.Activo = 1
WHERE
    c.IdCategoria = IIF(@IdCategoria = 0, c.IdCategoria, @IdCategoria);

SELECT
    *
FROM
    PRODUCTO;
GO

-- PROCEDIMIENTO ALMACENADO PARA COMPROBAR EXISTENCIA EN CARRITO
CREATE PROCEDURE USP_EXISTSCART (
    @IdCliente   INT,
    @IdProducto  INT,
    @Resultado   BIT OUTPUT
)
AS
BEGIN
    IF EXISTS (SELECT * FROM CARRITO_COMPRAS WHERE IdCliente = @IdCliente AND IdProducto = @IdProducto)
        SET @Resultado = 1;
    ELSE
        SET @Resultado = 0;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA OPERACIONES EN CARRITO
ALTER PROCEDURE USP_OperacionDelCarrito (
    @IdCliente   INT,
    @IdProducto  INT,
    @Sumar       BIT,
    @Mensaje     VARCHAR(500) OUTPUT,
    @Resultado   BIT OUTPUT
)
AS
BEGIN
    SET @Resultado = 1;
    SET @Mensaje = '';

    DECLARE @existecarrito BIT = IIF(EXISTS(SELECT * FROM CARRITO_COMPRAS WHERE IdCliente = @IdCliente AND IdProducto = @IdProducto), 1, 0);
    DECLARE @productostock INT = (SELECT Stock FROM PRODUCTO WHERE IdProducto = @IdProducto);

    BEGIN TRY
        BEGIN TRANSACTION OPERACION;

        IF (@Sumar = 1)
        BEGIN
            IF (@productostock > 0)
            BEGIN
                IF (@existecarrito = 1)
                    UPDATE CARRITO_COMPRAS SET Cantidad = Cantidad + 1 WHERE IdCliente = @IdCliente AND IdProducto = @IdProducto;
                ELSE
                    INSERT INTO CARRITO_COMPRAS (IdCliente, IdProducto, Cantidad) VALUES (@IdCliente, @IdProducto, 1);

                UPDATE PRODUCTO SET Stock = Stock - 1 WHERE IdProducto = @IdProducto;
            END;
            ELSE
            BEGIN
                SET @Resultado = 0;
                SET @Mensaje = 'El producto se encuentra agotado o no cuenta con stock disponible';
            END;
        END;
        ELSE
        BEGIN
            UPDATE CARRITO_COMPRAS SET Cantidad = Cantidad - 1 WHERE IdCliente = @IdCliente AND IdProducto = @IdProducto;
            UPDATE PRODUCTO SET Stock = Stock + 1 WHERE IdProducto = @IdProducto;
        END;

        COMMIT TRANSACTION OPERACION;
    END TRY
    BEGIN CATCH
        SET @Resultado = 0;
        SET @Mensaje = ERROR_MESSAGE();
        ROLLBACK TRANSACTION OPERACION;
    END CATCH;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA REGISTRAR VENTA
ALTER PROCEDURE USP_RegistrarVenta (
    @IdCliente      INT,
    @TotalProducto  INT,
    @MontoTotal     DECIMAL(18, 2),
    @Contacto       VARCHAR(200),
    @IdLocalidad    VARCHAR(50),
    @Telefono       VARCHAR(20),
    @Direccion      VARCHAR(200),
    @IdTransaccion  VARCHAR(100),
    @DetalleVenta   EDetalle_Venta READONLY,
    @Resultado      BIT OUTPUT,
    @Mensaje        VARCHAR(500) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        DECLARE @idventa INT = 0;
        SET @Resultado = 1;
        SET @Mensaje = '';

        BEGIN TRANSACTION REGISTRO;

        INSERT INTO VENTA (IdCliente, TotalProducto, MontoTotal, Contacto, IdLocalidad, Telefono, Direccion, IdTransaccion)
        VALUES (@IdCliente, @TotalProducto, @MontoTotal, @Contacto, @IdLocalidad, @Telefono, @Direccion, @IdTransaccion);

        SET @idventa = SCOPE_IDENTITY();

        INSERT INTO DETALLE_VENTA (IdVenta, IdProducto, Cantidad, Total)
        SELECT
            @idventa,
            IdProducto,
            Cantidad,
            Total
        FROM
            @DetalleVenta;

        DELETE FROM CARRITO_COMPRAS WHERE IdCliente = @IdCliente;

        COMMIT TRANSACTION REGISTRO;
    END TRY
    BEGIN CATCH
        SET @Resultado = 0;
        SET @Mensaje = ERROR_MESSAGE();
        ROLLBACK TRANSACTION REGISTRO;
    END CATCH;
END;
GO

-- PROCEDIMIENTO ALMACENADO PARA ELIMINAR CARRITO
CREATE PROCEDURE USP_EliminarCarrito (
    @IdCliente   INT,
    @IdProducto  INT,
    @Resultado   BIT OUTPUT
)
AS
BEGIN
    SET @Resultado = 1;
    DECLARE @cantidadproducto INT = (SELECT Cantidad FROM CARRITO_COMPRAS WHERE IdCliente = @IdCliente AND IdProducto = @IdProducto);

    BEGIN TRY
        BEGIN TRANSACTION OPERACION;

        UPDATE PRODUCTO SET Stock = Stock + @cantidadproducto WHERE IdProducto = @IdProducto;
        DELETE TOP (1) FROM CARRITO_COMPRAS WHERE IdCliente = @IdCliente AND IdProducto = @IdProducto;

        COMMIT TRANSACTION OPERACION;
    END TRY
    BEGIN CATCH
        SET @Resultado = 0;
        ROLLBACK TRANSACTION OPERACION;
    END CATCH;
END;
GO

----------------------------------------------------------------------
-- DML - FUNCIONES
----------------------------------------------------------------------

-- FUNCIÓN PARA OBTENER EL CARRITO DE UN CLIENTE
CREATE FUNCTION FN_ObtenerCarritoCliente (
    @IdCliente INT
)
RETURNS TABLE
AS
RETURN (
    SELECT
        p.IdProducto,
        m.Descripcion AS DescripcionMarca,
        p.Nombre,
        p.Precio,
        c.Cantidad,
        p.RutaImagen,
        p.NombreImagen
    FROM
        CARRITO_COMPRAS c
        INNER JOIN PRODUCTO p ON p.IdProducto = c.IdProducto
        INNER JOIN MARCA m ON m.IdMarca = p.IdMarca
    WHERE
        c.IdCliente = @IdCliente
);
GO

-- FUNCIÓN PARA LISTAR COMPRAS DE UN CLIENTE
CREATE FUNCTION FN_ListaCompras (
    @IdCliente INT
)
RETURNS TABLE
AS
RETURN (
    SELECT
        p.RutaImagen,
        p.NombreImagen,
        p.Nombre,
        p.Precio,
        dv.Cantidad,
        dv.Total,
        v.IdTransaccion
    FROM
        DETALLE_VENTA dv
        INNER JOIN PRODUCTO p ON p.IdProducto = dv.IdProducto
        INNER JOIN VENTA v ON v.IdVenta = dv.IdVenta
    WHERE
        v.IdCliente = @IdCliente
);
GO

SELECT * FROM CLIENTE