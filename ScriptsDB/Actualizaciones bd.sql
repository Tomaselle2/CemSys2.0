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

INSERT INTO TipoTramite (tipo) VALUES ('Facturación');

go
insert into EstadoTramite (tipoTramiteId, estado) values (7, 'Emitido'), (7, 'Pendiente de cobro'), (7, 'Cobrado'), (7,'Anulada');

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
ADD CONSTRAINT FK_Facturas_TipoTramite FOREIGN KEY (tipoTramiteId) REFERENCES TipoTramite(id),
    CONSTRAINT FK_Facturas_UsuarioEmite FOREIGN KEY (UsuarioEmiteId) REFERENCES Usuarios(id),
    CONSTRAINT FK_Facturas_Estado FOREIGN KEY (EstadoId) REFERENCES EstadoTramite(id),
    CONSTRAINT FK_Facturas_Contribuyente FOREIGN KEY (ContribuyenteId) REFERENCES Personas(idPersona),
    CONSTRAINT FK_Facturas_MetodoPago FOREIGN KEY (MetodoPagoId) REFERENCES MetodoPago(id),
    CONSTRAINT FK_Facturas_UsuarioCajero FOREIGN KEY (UsuarioCajeroId) REFERENCES Usuarios(id);

go

alter table ContratoConcesion
add Pendiente DECIMAL(10,2) NOT NULL DEFAULT 0
go

