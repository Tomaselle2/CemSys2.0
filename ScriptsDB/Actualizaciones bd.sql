alter table introducciones
add precio DECIMAL(10,2) NOT NULL DEFAULT 0
go

alter table introducciones
add pendiente DECIMAL(10,2) NULL

go

create table MetodoPago (id INT PRIMARY KEY IDENTITY(1,1), 
descripcion nvarchar(20) not null, 
visibilidad bit not null
);

go

ALTER TABLE Facturas DROP COLUMN pendiente;
go

ALTER TABLE Facturas
ADD 
    tipoTramiteId INT NULL,
    UsuarioEmiteId INT NULL,
    EstadoId INT NULL,
    ContribuyenteId INT NULL,
    MetodoPagoId INT NULL,
    UsuarioCajeroId INT NULL;

go

ALTER TABLE Facturas
add descripcion nvarchar(100) null;

go

ALTER TABLE Facturas
ADD CONSTRAINT FK_Facturas_TipoTramite FOREIGN KEY (tipoTramiteId) REFERENCES TipoTramite(id),
    CONSTRAINT FK_Facturas_UsuarioEmite FOREIGN KEY (UsuarioEmiteId) REFERENCES Usuarios(id),
    CONSTRAINT FK_Facturas_Contribuyente FOREIGN KEY (ContribuyenteId) REFERENCES Personas(idPersona),
    CONSTRAINT FK_Facturas_MetodoPago FOREIGN KEY (MetodoPagoId) REFERENCES MetodoPago(id),
    CONSTRAINT FK_Facturas_UsuarioCajero FOREIGN KEY (UsuarioCajeroId) REFERENCES Usuarios(id);

go

alter table ContratoConcesion
add Pendiente DECIMAL(10,2) NOT NULL DEFAULT 0
go

INSERT INTO TiposConceptoTarifaria (nombre) VALUES 
('Fondo');
go

INSERT INTO ConceptosTarifarias (tipoConceptoId, nombre, visibilidad) VALUES
(7, 'Fondo de ayuda centro de salud (%)', 1),
(7, 'Monto mínimo de fondo', 1);

go

-- Facturas Internas Precios
CREATE TABLE FacturasInternasPrecios (
    id INT PRIMARY KEY IDENTITY(1,1),
    tramiteId INT NOT NULL,
    fechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    total DECIMAL(10,2) NOT NULL, -- Monto total de la factura
    visibilidad BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (tramiteId) REFERENCES Tramite(id)
);

go

-- Conceptos factura
CREATE TABLE ConceptosFacturaInternasPrecios (
    id INT PRIMARY KEY IDENTITY(1,1),
    facturaId INT NOT NULL,
    conceptoTarifariaId INT NOT NULL,
    precioUnitario DECIMAL(10,2) NOT NULL,
    cantidad INT NOT NULL DEFAULT 1,
	tipoConceptoFacturaId INT NULL,
    subtotal AS (precioUnitario * cantidad) PERSISTED,
    FOREIGN KEY (facturaId) REFERENCES FacturasInternasPrecios(id),
    FOREIGN KEY (conceptoTarifariaId) REFERENCES ConceptosTarifarias(id),
	FOREIGN KEY (tipoConceptoFacturaId) REFERENCES TiposConceptoTarifaria(id)
);


--del 30/09

-- 2) Crear la tabla de estados de factura (si no la tienes ya)
CREATE TABLE EstadoFactura (
    id INT PRIMARY KEY IDENTITY(1,1),
    estado NVARCHAR(30) NOT NULL
);
GO

-- 3) Insertar los estados iniciales
INSERT INTO EstadoFactura (estado) VALUES
('Creado'),
('Emitido'),
('Pendiente de cobro'),
('Cobrado'),
('Anulado');
GO

-- 4) Crear la nueva FK en Facturas -> EstadoFactura
ALTER TABLE Facturas
ADD CONSTRAINT FK_Facturas_EstadoFactura FOREIGN KEY (EstadoId) REFERENCES EstadoFactura(id);
GO

CREATE TABLE HistorialEstadosFactura (
    Id INT PRIMARY KEY IDENTITY,
    FacturaId INT NOT NULL,
    EstadoId INT NOT NULL,
    FechaCambio DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (FacturaId) REFERENCES Facturas(id),
    FOREIGN KEY (EstadoId) REFERENCES EstadoFactura(id)
);
go

ALTER TABLE Facturas
ADD Vuelto decimal(10,2) NULL
go

ALTER TABLE Facturas
ADD 
    FechaVencimiento DATE NULL,           -- fecha límite de pago
    InteresAplicado DECIMAL(10,2) NULL;   -- interés final cobrado si hubo mora
