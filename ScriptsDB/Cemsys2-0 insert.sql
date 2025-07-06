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
('Introduccion'),
('Autorizacion para cremacion'),
('Autorizacion para reduccion'),
('Contrato de concesi�n'),
('Autorizacion para traslado'),
('Cambio de titularidad');

-- INSERT para TiposConceptoTarifaria
INSERT INTO TiposConceptoTarifaria (nombre) VALUES 
('General'),
('Introducci�n'),
('Concesion nicho'),
('Concesion fosa');

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

INSERT INTO Usuarios (nombre, correo, usuario, clave, visibilidad, rol) values ('Tomas Carreras', 'tomaselle2@gmail.com', 'tomaselle2', '1234', 1, 2);