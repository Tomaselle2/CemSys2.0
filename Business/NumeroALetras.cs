namespace CemSys2.Business
{
    public static class NumeroALetras
    {
        private static readonly string[] unidades = { "", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve" };
        private static readonly string[] decenas = { "", "", "veinte", "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa" };
        private static readonly string[] especiales = { "diez", "once", "doce", "trece", "catorce", "quince", "dieciséis", "diecisiete", "dieciocho", "diecinueve" };
        private static readonly string[] centenas = { "", "ciento", "doscientos", "trescientos", "cuatrocientos", "quinientos", "seiscientos", "setecientos", "ochocientos", "novecientos" };

        public static string ConvertirALetras(decimal numero)
        {
            if (numero == 0)
                return "cero";

            bool esNegativo = numero < 0;
            numero = Math.Abs(numero);

            int parteEntera = (int)numero;
            int decimales = (int)((numero - parteEntera) * 100);

            string resultado = ConvertirEnteroALetras(parteEntera);

            if (decimales > 0)
            {
                resultado += " con " + decimales.ToString("00") + "/100";
            }

            if (esNegativo)
                resultado = "menos " + resultado;

            return resultado.Trim();
        }

        private static string ConvertirEnteroALetras(int numero)
        {
            if (numero == 0) return "";
            if (numero == 100) return "cien";
            if (numero < 10) return unidades[numero];
            if (numero >= 10 && numero < 20) return especiales[numero - 10];
            if (numero >= 20 && numero < 100) return ConvertirDecenas(numero);
            if (numero >= 100 && numero < 1000) return ConvertirCentenas(numero);
            if (numero >= 1000 && numero < 1000000) return ConvertirMiles(numero);
            if (numero >= 1000000) return ConvertirMillones(numero);

            return numero.ToString();
        }

        private static string ConvertirDecenas(int numero)
        {
            int dec = numero / 10;
            int uni = numero % 10;

            if (dec == 2 && uni > 0)
                return "veinti" + unidades[uni];

            return decenas[dec] + (uni > 0 ? " y " + unidades[uni] : "");
        }

        private static string ConvertirCentenas(int numero)
        {
            int cent = numero / 100;
            int resto = numero % 100;

            string resultado = centenas[cent];

            if (resto > 0)
            {
                if (cent == 1) resultado = "ciento";
                resultado += " " + ConvertirEnteroALetras(resto);
            }

            return resultado;
        }

        private static string ConvertirMiles(int numero)
        {
            int miles = numero / 1000;
            int resto = numero % 1000;

            string resultado = "";

            if (miles == 1)
                resultado = "mil";
            else
                resultado = ConvertirEnteroALetras(miles) + " mil";

            if (resto > 0)
                resultado += " " + ConvertirEnteroALetras(resto);

            return resultado;
        }

        private static string ConvertirMillones(int numero)
        {
            int millones = numero / 1000000;
            int resto = numero % 1000000;

            string resultado = "";

            if (millones == 1)
                resultado = "un millón";
            else
                resultado = ConvertirEnteroALetras(millones) + " millones";

            if (resto > 0)
                resultado += " " + ConvertirEnteroALetras(resto);

            return resultado;
        }
    }
}
