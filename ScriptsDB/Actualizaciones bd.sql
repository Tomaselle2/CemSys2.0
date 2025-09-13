INSERT INTO EstadoTramite (tipoTramiteId, estado)
VALUES 
(4, 'Iniciado'),
(4, 'Pendiente de documentación'),
(4, 'Activa'),
(4, 'Vencida'),
(4, 'Inactiva'),
(4, 'Renovación');


--del 12/09
ALTER TABLE ContratoConcesion 
ALTER COLUMN vencimiento DATE NOT NULL;

go

ALTER TABLE TitularesContratoConcesion 
ADD fecha DATETIME NOT NULL DEFAULT GETDATE();
