using System;

namespace RecursividadAvanzada
{
    /// <summary>Utilidades de lectura de datos por consola con validación.</summary>
    public static class Entrada
    {
        /// <summary>Lee un entero dentro de [minimo, maximo]; repite hasta que sea válido.</summary>
        public static int LeerEntero(string mensaje, int minimo, int maximo)
        {
            while (true)
            {
                Console.Write(mensaje);
                int valor;
                if (int.TryParse(Console.ReadLine(), out valor) && valor >= minimo && valor <= maximo)
                    return valor;

                Console.WriteLine($"Entrada inválida. Ingrese un entero entre {minimo} y {maximo}.");
            }
        }

        /// <summary>Pregunta sí/no (s/n).</summary>
        public static bool LeerSiNo(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje + " (s/n): ");
                string r = (Console.ReadLine() ?? "").Trim().ToLower();
                if (r == "s" || r == "si" || r == "sí") return true;
                if (r == "n" || r == "no") return false;
            }
        }
    }
}
