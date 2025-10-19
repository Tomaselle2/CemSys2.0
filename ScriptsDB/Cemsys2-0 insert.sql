-- INSERT para TipoNichos
INSERT INTO TipoNichos (tipo) VALUES 
('Féretro'),
('Urnario'),
('Especial');

-- INSERT para TipoPanteon
INSERT INTO TipoPanteon (tipo) VALUES 
('Con nichos'),
('Sin nichos');

-- INSERT para TipoNumeracionParcelas
INSERT INTO TipoNumeracionParcelas (tipoNumeracion) VALUES 
('Nueva (nichos repetidos)'),
('Antigua (sin repetir)');

-- INSERT para CategoriaPersonas
INSERT INTO CategoriaPersonas (categoria) VALUES 
('Titular'),
('Fallecido'),
('Contribuyente');

-- INSERT para RolesUsuarios
INSERT INTO RolesUsuarios (rol) VALUES 
('Empleado'),
('Encargado'),
('Cajero');

-- INSERT para EstadoDifunto
INSERT INTO EstadoDifunto (estado) VALUES 
('Cuerpo completo'),
('Reducido'),
('Cremado');

-- INSERT para TipoParcela
INSERT INTO TipoParcela (tipoParcela) VALUES 
('Nicho'),
('Fosa'),
('Panteón');

-- INSERT para TipoTramite
INSERT INTO TipoTramite (tipo) VALUES 
('Introducción'),
('Autorización para cremación'),
('Autorización para reducción'),
('Contrato de concesión'),
('Autorización para traslado'),
('Cambio de titularidad');

-- INSERT para TiposConceptoTarifaria
INSERT INTO TiposConceptoTarifaria (nombre) VALUES 
('General'),
('Contribucion'),
('Concesión nicho'),
('Concesión fosa'),
('Registro Civil'),
('Derecho de Oficina'),
('Fondo');

-- INSERT para conceptos
INSERT INTO ConceptosTarifarias (tipoConceptoId, nombre, visibilidad) VALUES
(1, 'Apertura de nicho con placa', 1),
(1, 'Apertura de nicho sin placa', 1),
(1, 'Apertura de fosa', 1),
(2, 'Cierre de nicho', 1),
(2, 'Cierre de fosa', 1),
(1, 'Inscripción fuera de hora', 1),
(1, 'Permiso para colocar placa', 1),
(1, 'Permiso de refacciones', 1),
(1, 'Reducción', 1),
(1, 'Eventuales e imprevistos', 1),
(2, 'Inhumación nicho féretro', 1),
(5, 'Defunción', 1),
(5, 'Transcripción', 1),
(6, 'Introducción de féretro', 1),
(6, 'Introducción de urna', 1),
(3, 'Concesión Nicho', 1),
(4, 'Concesión Fosa', 1),
(2, 'Inhumación nicho urna', 1),
(2, 'Inhumación fosa féretro', 1),
(2, 'Inhumación fosa urna', 1),
(2, 'Inhumación panteón féretro', 1),
(2, 'Inhumación panteón urna', 1),
(7, 'Fondo de ayuda centro de salud (%)', 1),
(7, 'Monto mínimo de fondo', 1);
go

INSERT INTO ConceptosTarifarias (tipoConceptoId, nombre, visibilidad) VALUES
(1, 'Precio manual', 1);



-- INSERT para AniosConcesion (años típicos de concesión)
INSERT INTO AniosConcesion (anios) VALUES 
(1),
(5),
(10),
(15),
(25);

-- INSERT para CantidadCuotas (según el ejemplo proporcionado)
INSERT INTO CantidadCuotas (cuota) VALUES 
(1),
(2),
(3),
(4),
(5),
(6);

INSERT INTO EstadoTramite (tipoTramiteId, estado)
VALUES 
(1, 'Registrado'), --introduccion
(1, 'Cobrado'),
(1, 'Finalizado'),
(4, 'Iniciado'), --contrato de conescion
(4, 'Pendiente de documentación'),
(4, 'Activa'),
(4, 'Vencida'),
(4, 'Inactiva'),
(4, 'Renovación');

INSERT INTO EstadoFactura (estado) VALUES
('Creado'),
('Emitido'),
('Pendiente de cobro'),
('Cobrado'),
('Anulado');
GO

INSERT INTO MetodoPago (descripcion, visibilidad) VALUES
('Efectivo',1),
('Tarjeta',1),
('QR',1);

go

insert into Personas (nombre, apellido, dni, visibilidad, categoriaPersona, sexo) values ('', 'Municipalidad Colonia Tirolesa', '00000000', 1, 3, 'otro');

INSERT INTO Usuarios (nombre, correo, usuario, clave, visibilidad, rol) values ('Tomas Carreras', 'tomaselle2@gmail.com', 'Tomaselle2', '12345', 1, 2);

