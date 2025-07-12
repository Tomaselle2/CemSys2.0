--PROCEDIMIENTO ALMACENADOS
--Tarifaria
CREATE PROCEDURE sp_EmitirListadoTarifaria
AS
BEGIN
    SELECT id, nombre, visibilidad, FechaCreacionTarifaria
    FROM Tarifarias
    ORDER BY FechaCreacionTarifaria DESC;
END;
GO
---------------------------- Procedimiento almacenado para insertar una tarifaria con todos los precios-------------------------
CREATE PROCEDURE CrearTarifariaCompleta
    @NombreTarifaria NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Crear la nueva tarifaria
        INSERT INTO Tarifarias (nombre, visibilidad)
        VALUES (@NombreTarifaria, 1);

        DECLARE @TarifariaId INT = SCOPE_IDENTITY();

        -- 2. Insertar todos los conceptos generales (tipoConceptoId = 1)
        INSERT INTO PreciosTarifarias (tarifarioId, conceptoTarifariaId, precio)
        SELECT 
            @TarifariaId,
            ct.id,
            0
        FROM ConceptosTarifarias ct
        WHERE ct.tipoConceptoId = 1;

        -- 3. Insertar todos los conceptos de introducción (tipoConceptoId = 2)
        INSERT INTO PreciosTarifarias (tarifarioId, conceptoTarifariaId, precio)
        SELECT 
            @TarifariaId,
            ct.id,
            0
        FROM ConceptosTarifarias ct
        WHERE ct.tipoConceptoId = 2;

        -- 4. Insertar precios para todos los conceptos tipo 'Concesión - Nicho' (tipoConceptoId = 3)
        INSERT INTO PreciosTarifarias (
            tarifarioId, conceptoTarifariaId, precio,
            seccionId, nroFila, aniosConcesion
        )
        SELECT 
            @TarifariaId,
            ct.id,
            0,
            s.id,
            f.NumFila,
            ca.id
        FROM ConceptosTarifarias ct
        INNER JOIN Secciones s ON s.tipoParcela = 1
        CROSS APPLY (
            SELECT TOP(s.filas) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS NumFila
            FROM sys.all_objects
        ) f
        CROSS JOIN AniosConcesion ca
        WHERE ct.tipoConceptoId = 3;

        -- 5. Insertar precios para todos los conceptos tipo 'Concesión - Fosa' (tipoConceptoId = 4)
        INSERT INTO PreciosTarifarias (
            tarifarioId, conceptoTarifariaId, precio,
            seccionId, aniosConcesion
        )
        SELECT 
            @TarifariaId,
            ct.id,
            0,
            s.id,
            ca.id
        FROM ConceptosTarifarias ct
        INNER JOIN Secciones s ON s.tipoParcela = 2
        CROSS JOIN AniosConcesion ca
        WHERE ct.tipoConceptoId = 4;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        DECLARE @ErrorMsg NVARCHAR(MAX) = ERROR_MESSAGE();
        THROW 50000, @ErrorMsg, 1;
    END CATCH
END;
GO
---------------------------- Procedimiento almacenado para obtener todos los precios de una tarifaria específica-------------------------
CREATE PROCEDURE sp_ObtenerPreciosTarifaria
    @TarifarioId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        pt.id AS Id,
        pt.tarifarioId AS TarifarioId,
        pt.conceptoTarifariaId AS ConceptoTarifariaId,
        pt.precio AS Precio,
        pt.seccionId AS SeccionId,
        pt.nroFila AS NroFila,
        pt.aniosConcesion AS AniosConcesion,
        -- Datos de la navegación AniosConcesion
        ac.anios AS AniosConcesion_Anios,
        -- Datos de la navegación ConceptosTarifaria
        ct.nombre AS ConceptoTarifaria_Nombre,
        ct.visibilidad AS ConceptoTarifaria_Visibilidad,
        ct.tipoConceptoId AS ConceptoTarifaria_TipoConceptoId,
        -- Datos de la navegación Seccion (puede ser NULL)
        s.nombre AS Seccion_Nombre,
        s.visibilidad AS Seccion_Visibilidad,
        s.filas AS Seccion_Filas,
        s.nroParcelas AS Seccion_NroParcelas,
        s.tipoNumeracionParcela AS Seccion_TipoNumeracionParcela,
        s.tipoParcela AS Seccion_TipoParcela,
        -- Datos de la navegación Tarifario
        t.nombre AS Tarifario_Nombre,
        t.visibilidad AS Tarifario_Visibilidad,
        t.FechaCreacionTarifaria AS Tarifario_FechaCreacion
    FROM PreciosTarifarias pt
    INNER JOIN ConceptosTarifarias ct ON pt.conceptoTarifariaId = ct.id
    INNER JOIN Tarifarias t ON pt.tarifarioId = t.id
    LEFT JOIN AniosConcesion ac ON pt.aniosConcesion = ac.id
    LEFT JOIN Secciones s ON pt.seccionId = s.id
    WHERE pt.tarifarioId = @TarifarioId
    ORDER BY 
        ct.nombre,
        s.nombre,
        pt.nroFila,
        ac.anios DESC;
END
----------------------------FIN Procedimiento almacenado para obtener todos los precios de una tarifaria específica-------------------------


