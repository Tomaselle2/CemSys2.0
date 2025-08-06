--Actualizaciones del 30/07/2025
ALTER TABLE EmpresaFunebre
ADD visibilidad BIT NOT NULL DEFAULT 1;
go

ALTER TABLE TipoTramite
ADD visibilidad BIT NOT NULL DEFAULT 1;
go

ALTER TABLE Parcela
add nombrePanteon nvarchar(100)
go

ALTER TABLE Cementerios
ADD visibilidad BIT NOT NULL DEFAULT 1;

go

alter table parcela
add infoAdicional nvarchar(max);

go
--del 05/08/2025
ALTER TABLE ConceptosFactura
ADD tipoConceptoFacturaId INT NULL,
CONSTRAINT FK_ConceptosFactura_TiposConceptoTarifaria 
FOREIGN KEY (tipoConceptoFacturaId) REFERENCES TiposConceptoTarifaria(id);