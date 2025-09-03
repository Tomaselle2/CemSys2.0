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


select *, tct.nombre from ConceptosFactura cf 
join ConceptosTarifarias tct on cf.conceptoTarifariaId = tct.id

select * from ConceptosFactura where facturaId = 30
select * from TiposConceptoTarifaria


select * from EstadoTramite
select * from HistorialEstadoTramite where tramiteID = 34

select * from Introducciones

select * from ArchivosDocumentacion
select * from Introducciones where idTramite = 34

select * from Personas


-----
select * from TipoTramite
select * from Tramite
select * from RecibosFactura
select * from Facturas where tramiteId = 30

select tra.id as tramite, rf.fechaPago, rf.concepto, rf.monto, rf.contribuyente, rf.archivoID
from RecibosFactura rf 
inner join  Personas per on per.idPersona = rf.contribuyente
inner join Facturas fac on fac.id = rf.facturaId
inner join Tramite tra on tra.id = fac.tramiteId
where rf.contribuyente = 1031 
order by rf.fechaPago Desc
select * from Parcela
select * from TramitePersonas
select * from ConceptosTarifarias
select * from ParcelaDifuntos where parcelaId = 33 and fechaRetiro is null


SELECT pre.id,
       pre.conceptoTarifariaId,
       pre.precio,
       pre.seccionId,
       pre.nroFila,
       anio.anios
FROM PreciosTarifarias pre
INNER JOIN AniosConcesion anio
    ON anio.id = pre.aniosConcesion
WHERE pre.conceptoTarifariaId = 16
  AND pre.tarifarioId = 3
  AND pre.seccionId = 1
  AND pre.nroFila = 5
ORDER BY anio.anios;

select p.id AS parcelaId,
        sec.tipoParcela,
		sec.id AS seccionId,
        sec.nombre AS NombreSeccion,
        p.NroParcela,
        p.NroFila from Parcela p 
		JOIN Secciones sec 
        ON sec.id = p.seccion
		where p.id = 2

select * from PreciosTarifarias where id = 138
