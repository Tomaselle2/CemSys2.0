-- INSERT para TipoNichos
INSERT INTO TipoNichos (tipo) VALUES 
('F�retro'),
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
('Pante�n');

-- INSERT para TipoTramite
INSERT INTO TipoTramite (tipo) VALUES 
('Introducci�n'),
('Autorizaci�n para cremaci�n'),
('Autorizaci�n para reducci�n'),
('Contrato de concesi�n'),
('Autorizaci�n para traslado'),
('Cambio de titularidad');

-- INSERT para TiposConceptoTarifaria
INSERT INTO TiposConceptoTarifaria (nombre) VALUES 
('General'),
('Contribucion'),
('Concesi�n nicho'),
('Concesi�n fosa'),
('Registro Civil'),
('Derecho de Oficina');

-- INSERT para conceptos
INSERT INTO ConceptosTarifarias (tipoConceptoId, nombre, visibilidad) VALUES
(1, 'Apertura de nicho con placa', 1),
(1, 'Apertura de nicho sin placa', 1),
(1, 'Apertura de fosa', 1),
(1, 'Cierre de nicho', 1),
(1, 'Cierre de fosa', 1),
(1, 'Inscripci�n fuera de hora', 1),
(1, 'Permiso para colocar placa', 1),
(1, 'Permiso de refacciones', 1),
(1, 'Reducci�n', 1),
(1, 'Eventuales e imprevistos', 1),
(2, 'Inhumaci�n', 1),
(5, 'Defunci�n', 1),
(5, 'Transcripci�n', 1),
(6, 'Introducci�n de f�retro', 1),
(6, 'Introducci�n de urna', 1),
(3, 'Concesi�n Nicho', 1),
(4, 'Concesi�n Fosa', 1);


-- INSERT para AniosConcesion (a�os t�picos de concesi�n)
INSERT INTO AniosConcesion (anios) VALUES 
(1),
(5),
(10),
(15),
(25);

-- INSERT para CantidadCuotas (seg�n el ejemplo proporcionado)
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

