select * from PreciosTarifarias pt inner join ConceptosTarifarias ct on pt.conceptoTarifariaId = ct.id 
where ct.tipoConceptoId = 4 and pt.tarifarioId = 11;

select * from Tarifarias

SELECT pt.*, ct.*
FROM PreciosTarifarias pt 
INNER JOIN ConceptosTarifarias ct ON pt.conceptoTarifariaId = ct.id 
WHERE ct.tipoConceptoId = 4 
  AND pt.tarifarioId = 11
  AND ct.Visibilidad = 1  -- Suponiendo que Visibilidad es bit/boolean
ORDER BY ct.Nombre;

select * from AniosConcesion

select * from ConceptosTarifarias
select * from tarifarias

select * from TipoNichos
select * from PreciosTarifarias where conceptoTarifariaId = 11 and tarifarioId = 1

select * from Facturas where tramiteId = 30
select *, tct.nombre from ConceptosFactura cf 
join ConceptosTarifarias tct on cf.conceptoTarifariaId = tct.id

select * from ConceptosFactura where facturaId = 30
select * from TiposConceptoTarifaria

select * from Tramite
select * from EstadoTramite
select * from HistorialEstadoTramite

select * from Introducciones

select * from ArchivosDocumentacion
select * from RecibosFactura
select * from Introducciones where idTramite = 32
