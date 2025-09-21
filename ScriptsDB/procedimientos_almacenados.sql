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
Create PROCEDURE CrearTarifariaCompleta
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

		-- 7. Insertar todos los conceptos de fondo (tipoConceptoId = 7)
        INSERT INTO PreciosTarifarias (tarifarioId, conceptoTarifariaId, precio)
        SELECT 
            @TarifariaId,
            ct.id,
            0
        FROM ConceptosTarifarias ct
        WHERE ct.tipoConceptoId = 7 and ct.visibilidad = 1;

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
        pd.fechaIngreso DESC;
END
go
--------------------------------------------------------------------------------------------------------------------------
create PROCEDURE PersonasHistorialTramites
    @idPersona INT
AS
BEGIN
    SELECT 
        t.id AS TramiteId,
        per.idPersona AS PersonaId,
        t.fechaCreacion AS FechaInicio,
        tipo.id AS TipoTramite,
		t.estadoActualID
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
        t.fechaCreacion DESC,
		t.id DESC;
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
        AND pd.parcelaId = @parcelaId
        ORDER BY 
    pd.fechaIngreso DESC,
    p.idPersona DESC;
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
        AND pd.parcelaId = @parcelaId
	ORDER BY 
    pd.fechaIngreso DESC,
    p.idPersona DESC;
END
go
--------------------------------------------------------------------------------------
create PROCEDURE ObtenerTramitesPorParcela
    @parcelaId INT
AS
BEGIN
    SELECT 
        t.id AS TramiteId, 
        t.fechaCreacion AS FechaCreacion, 
        tipo.id AS TipoTramite, 
        tp.parcelaId AS ParcelaId,
		t.estadoActualID
    FROM 
        TramiteParcela tp
    INNER JOIN 
        Tramite t ON t.id = tp.tramiteId
    INNER JOIN 
        TipoTramite tipo ON tipo.id = t.tipoTramiteID
    WHERE 
        tp.parcelaId = @parcelaId
	  ORDER BY 
        t.fechaCreacion DESC,
        t.id DESC;
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
go
----------------------obtiene los recibos en personas--------------------------------------
CREATE PROCEDURE ObtenerRecibosPorContribuyente
    @ContribuyenteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        tra.id AS Tramite,
        rf.fechaPago,
        rf.concepto,
        rf.monto,
        rf.contribuyente,
        rf.archivoID
    FROM RecibosFactura rf
    INNER JOIN Personas per ON per.idPersona = rf.contribuyente
    INNER JOIN Facturas fac ON fac.id = rf.facturaId
    INNER JOIN Tramite tra ON tra.id = fac.tramiteId
    WHERE rf.contribuyente = @ContribuyenteId
    ORDER BY rf.fechaPago DESC;
END;
GO
----------parcelas sin contrato de concesion---------------------------------------
create PROCEDURE ParcelasSinContrato
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
    p.id AS parcelaId,
    sec.tipoParcela,
    sec.nombre AS NombreSeccion,
    p.NroParcela,
    p.NroFila,
    d.Difuntos,
    MAX(tra.estadoActualID) AS estadoTramite
FROM Parcela p
LEFT JOIN ContratoConcesion cc
    ON p.id = cc.parcelaId
JOIN Secciones sec 
    ON sec.id = p.seccion
CROSS APPLY (
    SELECT STRING_AGG(per.apellido + ' ' + per.nombre, ', ')
    FROM ParcelaDifuntos pd 
    JOIN Personas per ON per.idPersona = pd.difuntoId
    WHERE pd.parcelaId = p.id
) d(Difuntos)
JOIN Introducciones intro 
    ON intro.parcelaID = p.id
JOIN Tramite tra 
    ON tra.id = intro.idTramite
WHERE cc.parcelaId IS NULL 
  AND p.cantidadDifuntos > 0
  AND sec.tipoParcela <> 3
GROUP BY 
    p.id, sec.tipoParcela, sec.nombre, p.NroParcela, p.NroFila, d.Difuntos;

END
GO
--------------------Obtene los difuntos actuales en parcela para hacer un contrato-------------------------------------------
CREATE PROCEDURE sp_GetDifuntosActualesPorParcela
    @parcelaId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        pd.difuntoId AS DifuntoId,
        p.dni AS DNI,
        p.nombre AS Nombre,
        p.apellido AS Apellido,
        pd.fechaIngreso AS FechaIngreso,
        ed.estado AS EstadoDifunto
    FROM ParcelaDifuntos pd
    INNER JOIN Personas p ON pd.difuntoId = p.idPersona
    LEFT JOIN EstadoDifunto ed ON p.estadoDifunto = ed.id
    WHERE pd.parcelaId = @parcelaId
      AND pd.fechaRetiro IS NULL;
END;
GO
--------------obtiene los datos de la parcela que esta haciendo un contrato-------------------------------------
CREATE PROCEDURE DatosParcelaConcesion
    @parcelaId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.id AS parcelaId,
        sec.tipoParcela,
        sec.id AS seccionId,
        sec.nombre AS NombreSeccion,
        p.NroParcela,
        p.NroFila 
    FROM Parcela p 
    JOIN Secciones sec ON sec.id = p.seccion
    WHERE p.id = @parcelaId;
END
GO
------------------obtiene los precios de una parcela para hacer contrato-----------------------------------
create PROCEDURE obtenerPreciosParcelaContrato
    @conceptoTarifariaId INT,
    @tarifarioId INT,
    @seccionId INT,
    @nroFila INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        pre.id,
        pre.conceptoTarifariaId,
        pre.precio,
        pre.seccionId,
        pre.nroFila,
        anio.anios,
		anio.id AS cantidadAniosId
    FROM PreciosTarifarias pre
    INNER JOIN AniosConcesion anio
        ON anio.id = pre.aniosConcesion
    WHERE pre.conceptoTarifariaId = @conceptoTarifariaId
      AND pre.tarifarioId = @tarifarioId
      AND pre.seccionId = @seccionId
      AND pre.nroFila = @nroFila
    ORDER BY anio.anios;
END
GO
---Metodo para la tabla general de concesiones en el index--------------------
create PROCEDURE sp_ListadoContratosConcesiones
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH ContratosCTE AS (
        SELECT 
            cc.idTramite,
            cc.concesion,
            sec.nombre AS Seccion,
            sec.tipoParcela,         -- número del tipo de parcela
            p.NroParcela,
            p.NroFila,
            cc.vencimiento,
            tra.estadoActualID,      -- solo ID
            -- Difuntos actuales en la parcela
            d.Difuntos,
            
            -- Titulares del contrato
            t.Titulares,
			cc.parcelaId
        FROM ContratoConcesion cc
        INNER JOIN Tramite tra ON tra.id = cc.idTramite
        INNER JOIN Parcela p ON p.id = cc.parcelaId
        INNER JOIN Secciones sec ON sec.id = p.seccion
        
        -- Difuntos (string concatenado)
        OUTER APPLY (
            SELECT STRING_AGG(perDif.apellido + ' ' + perDif.nombre, ', ')
            FROM ParcelaDifuntos pd
            INNER JOIN Personas perDif ON perDif.idPersona = pd.difuntoId
            WHERE pd.parcelaId = p.id
              AND (pd.fechaRetiro IS NULL OR pd.estadoActual = 1)
        ) d(Difuntos)
        
        -- Titulares (string concatenado)
        OUTER APPLY (
            SELECT STRING_AGG(perTit.apellido + ' ' + perTit.nombre, ', ')
            FROM TitularesContratoConcesion tcc
            INNER JOIN Personas perTit ON perTit.idPersona = tcc.personaId
            WHERE tcc.contratoId = cc.idTramite
        ) t(Titulares)
    )
    -- 1) Listado paginado
    SELECT 
        idTramite,
        concesion,
        Difuntos,
        Seccion,
        tipoParcela,      -- solo el número
        NroParcela,       -- sin formato
        NroFila,          -- sin formato
        Titulares,
        vencimiento,
        estadoActualID,    -- solo el ID
		parcelaId
    FROM ContratosCTE
    ORDER BY idTramite DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    -- 2) Total de registros (para paginación)
    SELECT COUNT(*) AS TotalRegistros
    FROM ContratoConcesion;
END;
GO