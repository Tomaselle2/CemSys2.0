--PROCEDIMIENTO ALMACENADOS
--Tarifaria
CREATE PROCEDURE sp_EmitirListadoTarifaria
AS
BEGIN
    SELECT id, nombre, visibilidad, FechaCreacionTarifaria
    FROM Tarifarias
    ORDER BY FechaCreacionTarifaria DESC;
END;