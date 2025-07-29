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
('Fallecido');

-- INSERT para RolesUsuarios
INSERT INTO RolesUsuarios (rol) VALUES 
('Empleado'),
('Encargado');

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
('Derecho de Oficina');

-- INSERT para conceptos
INSERT INTO ConceptosTarifarias (tipoConceptoId, nombre, visibilidad) VALUES
(1, 'Apertura de nicho con placa', 1),
(1, 'Apertura de nicho sin placa', 1),
(1, 'Apertura de fosa', 1),
(1, 'Cierre de nicho', 1),
(1, 'Cierre de fosa', 1),
(1, 'Inscripción fuera de hora', 1),
(1, 'Permiso para colocar placa', 1),
(1, 'Permiso de refacciones', 1),
(1, 'Reducción', 1),
(1, 'Eventuales e imprevistos', 1),
(2, 'Inhumación', 1),
(5, 'Defunción', 1),
(5, 'Transcripción', 1),
(6, 'Introducción de féretro', 1),
(6, 'Introducción de urna', 1),
(3, 'Concesión Nicho', 1),
(4, 'Concesión Fosa', 1);


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
(1, 'Registrado'),
(1, 'Impago'),
(1, 'Cobrado'),
(1, 'Finalizado');

INSERT INTO Usuarios (nombre, correo, usuario, clave, visibilidad, rol) values ('Tomas Carreras', 'tomaselle2@gmail.com', 'tomaselle2', '1234', 1, 2);

