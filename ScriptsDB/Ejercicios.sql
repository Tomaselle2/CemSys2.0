select * from PreciosTarifarias pt inner join ConceptosTarifarias ct on pt.conceptoTarifariaId = ct.id 
where ct.tipoConceptoId = 4 and pt.tarifarioId = 11;

select * from Tarifarias

select * from AniosConcesion

select * from ConceptosTarifarias
select * from tarifarias

select * from TipoNichos
select * from PreciosTarifarias where conceptoTarifariaId = 11 and tarifarioId = 1


select *, tct.nombre from ConceptosFactura cf 
join ConceptosTarifarias tct on cf.conceptoTarifariaId = tct.id

select * from ConceptosFactura where facturaId = 30
select * from TiposConceptoTarifaria

select * from Usuarios

select * from EstadoTramite
select * from HistorialEstadoTramite where tramiteID = 43

select * from Introducciones

select * from ArchivosDocumentacion
select * from Introducciones where idTramite = 34

select * from Personas
select * from TramiteParcela where tramiteId = 43
SELECT @@VERSION;
-----
select * from TipoTramite
select * from RecibosFactura
select * from Facturas where tramiteId = 30






select * from PreciosTarifarias where id = 138

select * from Tramite

select * from ContratoConcesion
select * from TramitePersonas
select * from TitularesContratoConcesion
select * from HistorialTitularesContrato

select * from ConceptosFactura
select * from PreciosTarifarias
select * from Personas
select * from TiposConceptoTarifaria
select * from ConceptosTarifarias

select * from PreciosTarifarias where id = 114
select * from Facturas where tramiteId = 62
select * from ConceptosFactura where facturaId = 1052
select * from ArchivosDocumentacion

SELECT CASE WHEN EXISTS (
    SELECT 1 
    FROM ArchivosDocumentacion 
    WHERE TramiteID = 62
      AND CategoriaArchivo = 'Contrato_Concesion'
) THEN 1 ELSE 0 END AS ArchivoSubido;

select * from ArchivosDocumentacion where TramiteID = 62 and CategoriaArchivo <> 'Recibo'
select * from ArchivosDocumentacion where TramiteID = 69
select * from RecibosFactura