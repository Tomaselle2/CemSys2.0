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
create PROCEDURE CrearTarifariaCompleta
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
        WHERE ct.tipoConceptoId = 1 and ct.visibilidad = 1;

        -- 3. Insertar todos los conceptos de introducción (tipoConceptoId = 2)
        INSERT INTO PreciosTarifarias (tarifarioId, conceptoTarifariaId, precio)
        SELECT 
            @TarifariaId,
            ct.id,
            0
        FROM ConceptosTarifarias ct
        WHERE ct.tipoConceptoId = 2 and ct.visibilidad = 1;

        -- 4. Insertar precios para todos los conceptos tipo 'Concesión - Nicho' (tipoConceptoId = 3) y fosa (tipoConceptoId = 4)
			INSERT INTO PreciosTarifarias (
			tarifarioId,
			conceptoTarifariaId,
			precio,
			seccionId,
			nroFila,
			aniosConcesion
		)
		SELECT
			@TarifariaId,
			ct.id,
			0,
			s.id,
			f.NumFila,
			ac.id
		FROM
			ConceptosTarifarias ct
		INNER JOIN
			Secciones s ON
				((s.tipoParcela = 1 AND ct.tipoConceptoId = 3) OR -- Asegura que este OR se evalúe primero
				 (s.tipoParcela = 2 AND ct.tipoConceptoId = 4))
				AND s.visibilidad = 1 -- Y esta condición se aplica a todo el resultado del OR
		CROSS APPLY (
			SELECT TOP(s.filas) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS NumFila
			FROM sys.all_objects
		) f
		CROSS JOIN
			AniosConcesion ac
		WHERE
			(ct.tipoConceptoId = 3 OR ct.tipoConceptoId = 4)
			AND ct.visibilidad = 1;

		-- 5. Insertar todos los conceptos de registro Civil (tipoConceptoId = 5)
        INSERT INTO PreciosTarifarias (tarifarioId, conceptoTarifariaId, precio)
        SELECT 
            @TarifariaId,
            ct.id,
            0
        FROM ConceptosTarifarias ct
        WHERE ct.tipoConceptoId = 5 and ct.visibilidad = 1;

		-- 6. Insertar todos los conceptos de derecho de oficina (tipoConceptoId = 6)
        INSERT INTO PreciosTarifarias (tarifarioId, conceptoTarifariaId, precio)
        SELECT 
            @TarifariaId,
            ct.id,
            0
        FROM ConceptosTarifarias ct
        WHERE ct.tipoConceptoId = 6 and ct.visibilidad = 1;

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
go
----------------------------FIN Procedimiento almacenado para obtener todos los precios de una tarifaria específica-------------------------
---------------------------- Procedimiento almacenado para obtener los datos de una introduccion-------------------------
create PROCEDURE ResumenIntroduccion
    @IdTramite INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        i.idTramite AS id, 
        i.fechaIngreso AS FechaIngreso, 
        e.nombre AS Empresa, 
		p.dni As dni,
        p.nombre AS nombre, 
        p.apellido AS apellido, 
        p.fechaNacimiento, 
        p.fechaDefuncion,
        i.estadoDifunto AS EstadoDifunto, 
        p.informacionAdicional, 
        ac.acta, 
        ac.tomo, 
        ac.folio, 
        ac.serie, 
        ac.age, 
        u.nombre AS Empleado, 
        par.NroParcela, 
        par.NroFila, 
        sec.nombre AS Seccion,
		sec.tipoParcela as TipoParcela,
		p.domicilioEnTirolesa,
		p.fallecioEnTirolesa,
		par.cantidadDifuntos,
		tra.estadoActualID,
		i.informacionAdicional as informacionAdicionalTramite
    FROM Introducciones i
    INNER JOIN EmpresaFunebre e ON i.empresaFunebre = e.id
    INNER JOIN Personas p ON i.difuntoID = p.idPersona
    INNER JOIN ActaDefuncion ac ON p.actaDefuncion = ac.id
    INNER JOIN Usuarios u ON i.empleado = u.id
    INNER JOIN Parcela par ON i.parcelaID = par.id
    INNER JOIN Secciones sec ON par.seccion = sec.id
	INNER JOIN Tramite tra ON i.idTramite = tra.id
    WHERE i.idTramite = @IdTramite;
END
go
----------------------------fin  Procedimiento almacenado para obtener los datos de una introduccion-------------------------
---------------------------  Procedimiento almacenado para obtener los datos de personas en Personas Index-------------------------
CREATE PROCEDURE sp_BuscarPersonas
    @DNI VARCHAR(20) = NULL,
    @Nombre VARCHAR(100) = NULL,
    @Apellido VARCHAR(100) = NULL,
    @CategoriaId INT = NULL,
    @RegistrosPorPagina INT = 10,
    @Pagina INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Consulta base con conteo total
    ;WITH PersonasFiltradas AS (
        SELECT 
            p.idPersona,
            p.nombre,
            p.apellido,
            p.dni,
            p.sexo,
            cp.categoria,
            cp.id AS categoriaPersona
        FROM Personas p
        INNER JOIN CategoriaPersonas cp ON p.categoriaPersona = cp.id
        WHERE 
            (@DNI IS NULL OR p.dni LIKE '%' + @DNI + '%') AND
            (@Nombre IS NULL OR p.nombre LIKE '%' + @Nombre + '%') AND
            (@Apellido IS NULL OR p.apellido LIKE '%' + @Apellido + '%') AND
            (@CategoriaId IS NULL OR p.categoriaPersona = @CategoriaId)
    ),
    ConteoTotal AS (
        SELECT COUNT(*) AS TotalRegistros FROM PersonasFiltradas
    )
    
    -- Consulta paginada
    SELECT 
        p.idPersona AS IdPersona,
        p.nombre AS Nombre,
        p.apellido AS Apellido,
        p.dni AS Dni,
        p.sexo AS Sexo,
        p.categoriaPersona AS CategoriaPersona,
        p.categoria AS CategoriaNombre,
        c.TotalRegistros
    FROM PersonasFiltradas p
    CROSS JOIN ConteoTotal c
    ORDER BY p.apellido, p.nombre
    OFFSET (@Pagina - 1) * @RegistrosPorPagina ROWS
    FETCH NEXT @RegistrosPorPagina ROWS ONLY;
END
go
---------------------------  Procedimiento almacenado para obtener los datos del historial de parcelas en Personas-------------------------

create PROCEDURE PersonasHistorialParcelas
    @idPersona INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
		p.id,
        pd.fechaIngreso,
        pd.fechaRetiro,
        p.NroParcela,
        p.NroFila,
        s.nombre AS Seccion,
		s.tipoParcela
    FROM 
        ParcelaDifuntos pd
    INNER JOIN 
        Parcela p ON pd.parcelaId = p.id
    INNER JOIN 
        Secciones s ON p.seccion = s.id
    INNER JOIN 
        Personas per ON pd.difuntoId = per.idPersona
    WHERE 
        per.idPersona = @idPersona
    ORDER BY
        pd.fechaIngreso;
END
go
--------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE PersonasHistorialTramites
    @idPersona INT
AS
BEGIN
    SELECT 
        t.id AS TramiteId,
        per.idPersona AS PersonaId,
        t.fechaCreacion AS FechaInicio,
        tipo.tipo AS TipoTramite
    FROM 
        TramitePersonas tp
    INNER JOIN 
        Tramite t ON t.id = tp.tramiteId
    INNER JOIN 
        TipoTramite tipo ON t.tipoTramiteID = tipo.id
    INNER JOIN 
        Personas per ON per.idPersona = tp.personaId
    WHERE 
        per.idPersona = @idPersona
    ORDER BY
        t.fechaCreacion DESC;
END
go
------------------------------------------sp para parcelas, obtiene los difuntos actuales de la parcela---------------------------------------------------
create PROCEDURE ObtenerDifuntosEnParcela
    @parcelaId INT
AS
BEGIN
    SELECT 
        pd.difuntoId AS DifuntoId, 
        pd.fechaIngreso, 
        p.nombre, 
        p.apellido,
		p.dni AS Dni,  -- Agregado este campo
        pd.parcelaId,
		p.estadoDifunto
    FROM 
        ParcelaDifuntos pd
    INNER JOIN 
        Personas p ON p.idPersona = pd.difuntoId
    WHERE 
        p.categoriaPersona = 2 
        AND pd.fechaRetiro IS NULL 
        AND pd.parcelaId = @parcelaId;
END
go
------------------------------------------Obtiene el encabezado del historial de una parcela-----------------------------------------
create PROCEDURE ObtenerEncabezadoParcela
    @parcelaId INT
AS
BEGIN
    SELECT 
        p.id AS ParcelaId, 
        p.NroParcela, 
        p.NroFila, 
        s.nombre AS NombreSeccion, 
        s.tipoParcela AS TipoParcela,
		p.TipoNicho,
		p.TipoPanteonId,
		p.nombrePanteon,
		p.infoAdicional
    FROM 
        Parcela p
    INNER JOIN 
        Secciones s ON s.id = p.seccion
    WHERE 
        p.id = @parcelaId;
END
go
--------------------------------------------------------------------------------
create PROCEDURE ObtenerDifuntosHistoricosEnParcela
    @parcelaId INT
AS
BEGIN
    SELECT 
        pd.difuntoId AS DifuntoId, 
        pd.fechaIngreso, 
        p.nombre, 
        p.apellido,
        p.dni AS Dni,
		pd.fechaRetiro
    FROM 
        ParcelaDifuntos pd
    INNER JOIN 
        Personas p ON p.idPersona = pd.difuntoId
    WHERE 
        p.categoriaPersona = 2 
        AND pd.parcelaId = @parcelaId;
END
go
--------------------------------------------------------------------------------------
CREATE PROCEDURE ObtenerTramitesPorParcela
    @parcelaId INT
AS
BEGIN
    SELECT 
        t.id AS TramiteId, 
        t.fechaCreacion AS FechaCreacion, 
        tipo.tipo AS TipoTramite, 
        tp.parcelaId AS ParcelaId
    FROM 
        TramiteParcela tp
    INNER JOIN 
        Tramite t ON t.id = tp.tramiteId
    INNER JOIN 
        TipoTramite tipo ON tipo.id = t.tipoTramiteID
    WHERE 
        tp.parcelaId = @parcelaId;
END
go
--------------------------------------------------------------------------------------
create PROCEDURE difuntosExel
    @idPersona INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        per.dni,
        per.nombre,
        per.apellido,
        per.sexo,
        est.estado,
        per.fechaDefuncion,
        per.fechaNacimiento,
        pd.fechaIngreso,
        pd.fechaRetiro,
        p.NroParcela,
        p.NroFila,
        s.nombre AS nombreSeccion,
        ad.acta,
        ad.tomo,
        ad.folio,
        ad.serie,
        ad.age,
        per.informacionAdicional,
        s.tipoParcela AS TipoParcela
    FROM 
        ParcelaDifuntos pd
    INNER JOIN 
        Parcela p ON pd.parcelaId = p.id
    INNER JOIN 
        Secciones s ON p.seccion = s.id
    INNER JOIN 
        Personas per ON pd.difuntoId = per.idPersona
    INNER JOIN 
        ActaDefuncion ad ON ad.id = per.actaDefuncion
    INNER JOIN 
        EstadoDifunto est ON est.id = per.estadoDifunto
    WHERE 
        per.idPersona = @idPersona;
END;