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