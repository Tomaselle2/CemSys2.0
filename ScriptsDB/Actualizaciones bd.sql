--del 29/08/2025
ALTER TABLE ContratoConcesion
ADD contratoAnteriorId INT NULL;

go

ALTER TABLE ContratoConcesion
ADD CONSTRAINT FK_ContratoConcesion_ContratoAnterior
FOREIGN KEY (contratoAnteriorId) REFERENCES ContratoConcesion(idTramite);

go

ALTER TABLE ContratoConcesion
ADD precio DECIMAL(10,2) NOT NULL DEFAULT 0;

go

CREATE TABLE HistorialTitularesContrato (
    id INT PRIMARY KEY IDENTITY(1,1),
    contratoId INT NOT NULL,
    personaId INT NOT NULL,
    fechaInicio DATETIME NOT NULL DEFAULT GETDATE(),
    fechaFin DATETIME NULL,
    FOREIGN KEY (contratoId) REFERENCES ContratoConcesion(idTramite),
    FOREIGN KEY (personaId) REFERENCES Personas(idPersona)
);

go

-- 1. Eliminar la FK
ALTER TABLE ContratoConcesion
DROP CONSTRAINT FK__ContratoC__difun__02FC7413; -- reemplazar con el nombre real de la FK
--sp_help 'ContratoConcesion' para buscar la tabla y la relacion
go

-- 2. Eliminar la columna difuntoId
ALTER TABLE ContratoConcesion
DROP COLUMN difuntoId;

