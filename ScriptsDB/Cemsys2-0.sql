create database cemsys2;
go
use cemsys2;
go
-- Script para crear todas las tablas del sistema de cementerio
-- Orden basado en dependencias de claves foráneas

-- ==========================================
-- CONFIGURACIÓN PREVIA FILESTREAM
-- ==========================================

-- NOTA: Antes de ejecutar este script
-- 1. Habilitar FILESTREAM en la instancia de SQL Server
-- 2. Cambiar 'NombreTuBaseDeDatos' por el nombre real de tu base de datos
-- 3. Cambiar 'C:\FileStreamData' por la ruta donde quieres almacenar los archivos

-- Agregar filegroup FILESTREAM a la base de datos
ALTER DATABASE [cemsys2] 
ADD FILEGROUP [CementerioFileStreamGroupArchive] CONTAINS FILESTREAM;

-- Agregar archivo físico para FILESTREAM
ALTER DATABASE [cemsys2] 
ADD FILE (
    NAME = 'CementerioFileStreamFile',
    FILENAME = 'C:\CemsysArchive' -- RUTA
) TO FILEGROUP [CementerioFileStreamGroupArchive];

PRINT 'Configuración FILESTREAM completada.';

-- ==========================================
-- TABLAS MAESTRAS (sin dependencias)
-- ==========================================

-- Tipos de nichos
CREATE TABLE TipoNichos (
    id INT PRIMARY KEY IDENTITY(1,1),
    tipo NVARCHAR(20) NOT NULL
);

-- Tipos de panteón
CREATE TABLE TipoPanteon (
    id INT PRIMARY KEY IDENTITY(1,1),
    tipo NVARCHAR(20) NOT NULL
);

-- Tipos de numeración de parcelas
CREATE TABLE TipoNumeracionParcelas (
    id INT PRIMARY KEY IDENTITY(1,1),
    tipoNumeracion NVARCHAR(30) NOT NULL
);

-- Categorías de personas
CREATE TABLE CategoriaPersonas (
    id INT PRIMARY KEY IDENTITY(1,1),
    categoria NVARCHAR(30) NOT NULL
);

-- Roles de usuarios
CREATE TABLE RolesUsuarios (
    id INT PRIMARY KEY IDENTITY(1,1),
    rol NVARCHAR(30) NOT NULL
);

-- Estado del difunto
CREATE TABLE EstadoDifunto (
    id INT PRIMARY KEY IDENTITY(1,1),
    estado NVARCHAR(30) NOT NULL
);

-- Tipo de parcela
CREATE TABLE TipoParcela (
    id INT PRIMARY KEY IDENTITY(1,1),
    tipoParcela NVARCHAR(30) NOT NULL
);

-- Empresa funebre
CREATE TABLE EmpresaFunebre (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre NVARCHAR(30) NOT NULL,
	visibilidad BIT NOT NULL DEFAULT 1

);


-- Cementerios
CREATE TABLE Cementerios (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre NVARCHAR(50) NOT NULL,
	visibilidad BIT NOT NULL DEFAULT 1
);


-- Acta defunción
CREATE TABLE ActaDefuncion (
    id INT PRIMARY KEY IDENTITY(1,1),
    acta INT NULL,
    tomo INT NULL,
    folio INT NULL,
    serie NVARCHAR(10) NULL,
    age INT NULL
);

-- Años de concesión
CREATE TABLE AniosConcesion (
    id INT PRIMARY KEY IDENTITY(1,1),
    anios INT NOT NULL
);

-- Cantidad de cuotas
CREATE TABLE CantidadCuotas (
    id INT PRIMARY KEY IDENTITY(1,1),
    cuota INT NOT NULL -- ejemplo(1,2,3,4,5,6,12,18)
);

-- Tipo de trámite
CREATE TABLE TipoTramite (
    id INT PRIMARY KEY IDENTITY(1,1),
    tipo NVARCHAR(30) NOT NULL,
	visibilidad BIT NOT NULL DEFAULT 1
);


-- Tarifarias
CREATE TABLE Tarifarias (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre NVARCHAR(20) NOT NULL,
    visibilidad BIT NOT NULL DEFAULT 0,
	FechaCreacionTarifaria DATETIME NOT NULL DEFAULT GETDATE()
);

-- Tipos de concepto tarifaria
CREATE TABLE TiposConceptoTarifaria (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre NVARCHAR(50) NOT NULL
);

-- ==========================================
-- TABLAS CON DEPENDENCIAS DE PRIMER NIVEL
-- ==========================================

-- Usuarios
CREATE TABLE Usuarios (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre NVARCHAR(60) NOT NULL,
    correo NVARCHAR(60) NOT NULL,
    usuario NVARCHAR(30) NOT NULL,
    clave NVARCHAR(300) NOT NULL,
    visibilidad BIT NOT NULL,
    rol INT NOT NULL,
    FOREIGN KEY (rol) REFERENCES RolesUsuarios(id)
);

-- Secciones
CREATE TABLE Secciones (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre NVARCHAR(50) NOT NULL,
    visibilidad BIT NOT NULL,
    filas INT NOT NULL,
    nroParcelas INT NOT NULL,
    tipoNumeracionParcela INT NOT NULL,
    tipoParcela INT NOT NULL,
    FOREIGN KEY (tipoNumeracionParcela) REFERENCES TipoNumeracionParcelas(id),
    FOREIGN KEY (tipoParcela) REFERENCES TipoParcela(id)
);

-- Personas
CREATE TABLE Personas (
    idPersona INT PRIMARY KEY IDENTITY(1,1),
    nombre NVARCHAR(60) NOT NULL,
    apellido NVARCHAR(60) NOT NULL,
    dni NVARCHAR(15) NOT NULL,
    visibilidad BIT NOT NULL,
    fechaNacimiento DATE NULL,
    fechaDefuncion DATE NULL,
    estadoDifunto INT NULL,
    actaDefuncion INT NULL,
    informacionAdicional NVARCHAR(MAX),
    categoriaPersona INT NOT NULL,
    sexo NVARCHAR(15) NOT NULL,
    correo NVARCHAR(60) NULL,
    celular NVARCHAR(25) NULL,
    domicilio NVARCHAR(100) NULL,
    domicilioEnTirolesa BIT NULL,
    fallecioEnTirolesa BIT NULL,
    FOREIGN KEY (estadoDifunto) REFERENCES EstadoDifunto(id),
    FOREIGN KEY (actaDefuncion) REFERENCES ActaDefuncion(id),
    FOREIGN KEY (categoriaPersona) REFERENCES CategoriaPersonas(id)
);

-- Conceptos tarifarias
CREATE TABLE ConceptosTarifarias (
    id INT PRIMARY KEY IDENTITY(1,1),
    tipoConceptoId INT NOT NULL,
    nombre NVARCHAR(100) NOT NULL, -- 'Apertura de nicho con placa', 'Inhumacion', etc.
    visibilidad BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (tipoConceptoId) REFERENCES TiposConceptoTarifaria(id)
);

-- Estado trámite
CREATE TABLE EstadoTramite (
    id INT PRIMARY KEY IDENTITY(1,1),
    tipoTramiteId INT NOT NULL,
    estado NVARCHAR(50) NOT NULL,
    FOREIGN KEY (tipoTramiteId) REFERENCES TipoTramite(id)
);

-- ==========================================
-- TABLAS CON DEPENDENCIAS DE SEGUNDO NIVEL
-- ==========================================

-- Parcela
CREATE TABLE Parcela (
    id INT PRIMARY KEY IDENTITY(1,1),
    visibilidad BIT NOT NULL,
    NroParcela INT NOT NULL,
    NroFila INT NOT NULL,
    cantidadDifuntos INT NOT NULL,
    seccion INT NOT NULL,
    TipoNicho INT NULL,
	TipoPanteonId INT NULL,
	nombrePanteon nvarchar(100),
	infoAdicional nvarchar(max),
    FOREIGN KEY (seccion) REFERENCES Secciones(id),
    FOREIGN KEY (TipoNicho) REFERENCES TipoNichos(id),
	FOREIGN KEY (TipoPanteonId) REFERENCES TipoPanteon(id)
);
go


-- Precios tarifarias
CREATE TABLE PreciosTarifarias (
    id INT PRIMARY KEY IDENTITY(1,1),
    tarifarioId INT NOT NULL,
    conceptoTarifariaId INT NOT NULL,
    precio DECIMAL(10,2) NOT NULL,
    -- Para concesiones específicas por sección/fila/años
    seccionId INT NULL, -- FK a Secciones (solo para concesiones)
    nroFila INT NULL, -- Solo para nichos que varían por fila
    aniosConcesion INT NULL, -- 25, 15, 10, 5, 1 (solo para concesiones)
    FOREIGN KEY (tarifarioId) REFERENCES Tarifarias(id),
    FOREIGN KEY (conceptoTarifariaId) REFERENCES ConceptosTarifarias(id),
    FOREIGN KEY (seccionId) REFERENCES Secciones(id),
	foreign key (aniosConcesion) references AniosConcesion(id)
);

-- Trámite
CREATE TABLE Tramite (
    id INT NOT NULL PRIMARY KEY, -- NO auto incremental
    tipoTramiteID INT NOT NULL,
    fechaCreacion DATETIME NOT NULL,
    usuario INT NOT NULL,
    visibilidad BIT NOT NULL,
    estadoActualID INT NULL,
    FOREIGN KEY (tipoTramiteID) REFERENCES TipoTramite(id),
    FOREIGN KEY (usuario) REFERENCES Usuarios(id),
    FOREIGN KEY (estadoActualID) REFERENCES EstadoTramite(id)
);

-- ==========================================
-- TABLAS CON DEPENDENCIAS DE TERCER NIVEL
-- ==========================================

-- Historial estado trámite
CREATE TABLE HistorialEstadoTramite (
    id INT PRIMARY KEY IDENTITY(1,1),
    tramiteID INT NOT NULL,
    estadoTramiteID INT NOT NULL,
    fecha DATETIME NOT NULL,
    FOREIGN KEY (tramiteID) REFERENCES Tramite(id),
    FOREIGN KEY (estadoTramiteID) REFERENCES EstadoTramite(id)
);

-- Introducciones
CREATE TABLE Introducciones (
    idTramite INT NOT NULL PRIMARY KEY, -- PK y FK de Tramite
    visibilidad BIT NOT NULL,
    fechaIngreso DATETIME NULL,
    empleado INT NULL,
    empresaFunebre INT NULL,
    parcelaID INT NOT NULL,
    difuntoID INT NOT NULL,
    estadoDifunto NVARCHAR(30) NULL,
    introduccionNueva BIT NOT NULL,
    fechaRetiro DATETIME NULL,
	informacionAdicional NVARCHAR(MAX),
    FOREIGN KEY (idTramite) REFERENCES Tramite(id),
    FOREIGN KEY (empleado) REFERENCES Usuarios(id),
    FOREIGN KEY (empresaFunebre) REFERENCES EmpresaFunebre(id),
    FOREIGN KEY (parcelaID) REFERENCES Parcela(id),
    FOREIGN KEY (difuntoID) REFERENCES Personas(idPersona)
);


-- Contrato concesión
CREATE TABLE ContratoConcesion (
    idTramite INT NOT NULL PRIMARY KEY, -- PK y FK de Tramite
    difuntoId INT NOT NULL,
    parcelaId INT NOT NULL,
    cantidadAnios INT NOT NULL,
    vencimiento DATETIME NOT NULL,
    concesion NVARCHAR(5) NOT NULL,
    precioTarifariaID INT NOT NULL,
    cuotaId INT NULL,
    pagoDescripcion NVARCHAR(150) NULL, -- por si elige poner manual la forma de pago
    visibilidad BIT NOT NULL,
    fechaGeneracion DATETIME NOT NULL,
    empleado INT NOT NULL,
    tipoParcela INT NOT NULL,
    FOREIGN KEY (idTramite) REFERENCES Tramite(id),
    FOREIGN KEY (difuntoId) REFERENCES Personas(idPersona),
    FOREIGN KEY (parcelaId) REFERENCES Parcela(id),
    FOREIGN KEY (cantidadAnios) REFERENCES AniosConcesion(id),
    FOREIGN KEY (precioTarifariaID) REFERENCES PreciosTarifarias(id),
    FOREIGN KEY (cuotaId) REFERENCES CantidadCuotas(id),
    FOREIGN KEY (empleado) REFERENCES Usuarios(id),
    FOREIGN KEY (tipoParcela) REFERENCES TipoParcela(id)
);

-- Titulares contrato concesión
CREATE TABLE TitularesContratoConcesion (
    id INT PRIMARY KEY IDENTITY(1,1),
    contratoId INT NOT NULL,
    personaId INT NOT NULL,
    FOREIGN KEY (contratoId) REFERENCES ContratoConcesion(idTramite),
    FOREIGN KEY (personaId) REFERENCES Personas(idPersona)
);

-- Trámite personas
CREATE TABLE TramitePersonas (
    tramiteId INT NOT NULL,
    personaId INT NOT NULL,
	FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (tramiteId, personaId),
    FOREIGN KEY (tramiteId) REFERENCES Tramite(id),
    FOREIGN KEY (personaId) REFERENCES Personas(idPersona)
);

-- Parcela difuntos
CREATE TABLE ParcelaDifuntos (
    id INT PRIMARY KEY IDENTITY(1,1),
    parcelaId INT NOT NULL,
    difuntoId INT NOT NULL,
    fechaIngreso DATETIME NOT NULL,
    fechaRetiro DATETIME NULL, -- NULL = difunto actual, con fecha = difunto histórico
    estadoActual BIT NOT NULL DEFAULT 1, -- 1 = actual, 0 = histórico
    tramiteIngresoId INT NULL,
    tramiteRetiroId INT NULL,
    FOREIGN KEY (parcelaId) REFERENCES Parcela(id),
    FOREIGN KEY (difuntoId) REFERENCES Personas(idPersona),
    FOREIGN KEY (tramiteIngresoId) REFERENCES Tramite(id),
    FOREIGN KEY (tramiteRetiroId) REFERENCES Tramite(id)
);

-- Facturas
CREATE TABLE Facturas (
    id INT PRIMARY KEY IDENTITY(1,1),
    tramiteId INT NOT NULL,
    fechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    total DECIMAL(10,2) NOT NULL, -- Monto total de la factura
    pendiente DECIMAL(10,2) NOT NULL, -- Cuánto queda por pagar
    visibilidad BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (tramiteId) REFERENCES Tramite(id)
);

-- Conceptos factura
CREATE TABLE ConceptosFactura (
    id INT PRIMARY KEY IDENTITY(1,1),
    facturaId INT NOT NULL,
    conceptoTarifariaId INT NOT NULL,
    precioUnitario DECIMAL(10,2) NOT NULL,
    cantidad INT NOT NULL DEFAULT 1,
	tipoConceptoFacturaId INT NULL,
    subtotal AS (precioUnitario * cantidad) PERSISTED,
    FOREIGN KEY (facturaId) REFERENCES Facturas(id),
    FOREIGN KEY (conceptoTarifariaId) REFERENCES ConceptosTarifarias(id),
	FOREIGN KEY (tipoConceptoFacturaId) REFERENCES TiposConceptoTarifaria(id)
);

-- Recibos factura
CREATE TABLE RecibosFactura (
    id INT PRIMARY KEY IDENTITY(1,1),
    facturaId INT NOT NULL,
    fechaPago DATETIME NOT NULL,
    concepto NVARCHAR(100) NOT NULL,
    monto DECIMAL(10,2) NOT NULL,
    archivoID UNIQUEIDENTIFIER NULL,
	decreto bit not null default 0,
    FOREIGN KEY (facturaId) REFERENCES Facturas(id)
);



-- Archivos documentación con FILESTREAM
CREATE TABLE ArchivosDocumentacion (
    ArchivoID UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL DEFAULT NEWID() PRIMARY KEY,
    CategoriaArchivo NVARCHAR(50) NOT NULL,
    TramiteID INT NULL,
    ReciboID INT NULL,
    ActaDefuncionID INT NULL,
    PersonaID INT NULL,
    NombreArchivo NVARCHAR(255) NOT NULL,
    TipoArchivo NVARCHAR(50) NOT NULL, -- 'application/pdf', 'image/jpeg', etc.
    TamanoBytes BIGINT NOT NULL,
    Contenido VARBINARY(MAX) FILESTREAM NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    FechaCreacion DATETIME2 DEFAULT SYSDATETIME(),
    visibilidad BIT DEFAULT 1,
    FOREIGN KEY (TramiteID) REFERENCES Tramite(id),
    FOREIGN KEY (ReciboID) REFERENCES RecibosFactura(id),
    FOREIGN KEY (ActaDefuncionID) REFERENCES ActaDefuncion(id),
    FOREIGN KEY (PersonaID) REFERENCES Personas(idPersona),
    CHECK (
        (CASE WHEN TramiteID IS NOT NULL THEN 1 ELSE 0 END +
         CASE WHEN ReciboID IS NOT NULL THEN 1 ELSE 0 END +
         CASE WHEN ActaDefuncionID IS NOT NULL THEN 1 ELSE 0 END +
         CASE WHEN PersonaID IS NOT NULL THEN 1 ELSE 0 END
        ) = 1
    )
);

CREATE TABLE TramiteParcela (
    tramiteId INT NOT NULL,
    parcelaId INT NOT NULL,
    fechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (tramiteId, parcelaId),
    FOREIGN KEY (tramiteId) REFERENCES Tramite(id),
    FOREIGN KEY (parcelaId) REFERENCES Parcela(id)
);


-- Agregar la FK de RecibosFactura a ArchivosDocumentacion
ALTER TABLE RecibosFactura 
ADD CONSTRAINT FK_RecibosFactura_ArchivosDocumentacion 
FOREIGN KEY (archivoID) REFERENCES ArchivosDocumentacion(ArchivoID);

PRINT 'Todas las tablas han sido creadas exitosamente.';