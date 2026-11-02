using System;
using System.Text;
using System.Threading;

namespace RecursividadAvanzada
{
    /// <summary>
    /// Resuelve un laberinto con BACKTRACKING recursivo (búsqueda en profundidad).
    ///
    /// Idea: desde la celda actual se prueba cada dirección. Si una dirección lleva a un
    /// callejón sin salida, la llamada recursiva devuelve false y se "retrocede" para
    /// probar la siguiente. Si alguna devuelve true, el camino correcto se va marcando
    /// mientras se deshacen las llamadas.
    /// </summary>
    public class ResolutorLaberinto
    {
        // Símbolos de dibujo
        private const char Pared = '█';
        private const char EnCurso = '·';   // celda del camino que se está probando
        private const char SinSalida = 'x'; // celda de la que ya se retrocedió
        private const char Camino = '*';    // camino final correcto

        // Orden de exploración: abajo, derecha, arriba, izquierda.
        private static readonly int[] DF = { 1, 0, -1, 0 };
        private static readonly int[] DC = { 0, 1, 0, -1 };

        private readonly Laberinto laberinto;
        private readonly bool[,] visitado;   // celdas ya exploradas (evita ciclos infinitos)
        private readonly char[,] vista;      // matriz que se dibuja en pantalla
        private readonly int retardoMs;
        private bool animar;
        private int filaCursor;

        // Estadísticas
        public long Llamadas { get; private set; }
        public long Retrocesos { get; private set; }
        public int ProfundidadMaxima { get; private set; }
        public int LongitudCamino { get; private set; }

        public ResolutorLaberinto(Laberinto laberinto, bool animar, int retardoMs)
        {
            this.laberinto = laberinto;
            this.animar = animar;
            this.retardoMs = retardoMs;
            visitado = new bool[laberinto.Filas, laberinto.Columnas];
            vista = new char[laberinto.Filas, laberinto.Columnas];

            for (int f = 0; f < laberinto.Filas; f++)
                for (int c = 0; c < laberinto.Columnas; c++)
                    vista[f, c] = laberinto.Celdas[f, c] == Laberinto.Pared ? Pared : ' ';
        }

        /// <summary>Lanza la búsqueda. Devuelve true si existe camino de S a E.</summary>
        public bool Resolver()
        {
            if (animar) PrepararAnimacion();

            bool exito = Explorar(laberinto.InicioF, laberinto.InicioC, 1);

            if (animar)
            {
                Dibujar();
                try { Console.SetCursorPosition(0, filaCursor + laberinto.Filas); }
                catch (Exception) { }
            }
            else
            {
                Dibujar(); // Sin animación: se imprime solo el resultado final.
            }
            return exito;
        }

        /// <summary>
        /// FUNCIÓN RECURSIVA DE BACKTRACKING.
        ///
        /// Casos base (fracaso, retorna false):
        ///   - Posición fuera de la matriz.
        ///   - La celda es una pared.
        ///   - La celda ya fue visitada.
        /// Caso base (éxito, retorna true):
        ///   - La celda actual es la salida.
        /// Caso recursivo:
        ///   - Probar las 4 direcciones; si alguna tiene éxito, esta celda forma parte del camino.
        ///   - Si ninguna tiene éxito: BACKTRACKING -> marcar como sin salida y retornar false.
        /// </summary>
        private bool Explorar(int f, int c, int profundidad)
        {
            Llamadas++;
            if (profundidad > ProfundidadMaxima) ProfundidadMaxima = profundidad;

            // --- Casos base de fracaso ---
            if (f < 0 || c < 0 || f >= laberinto.Filas || c >= laberinto.Columnas) return false;
            if (laberinto.Celdas[f, c] == Laberinto.Pared) return false;
            if (visitado[f, c]) return false;

            // --- Se prueba esta celda ---
            visitado[f, c] = true;
            Marcar(f, c, EnCurso);

            // --- Caso base de éxito ---
            if (f == laberinto.FinF && c == laberinto.FinC)
            {
                LongitudCamino = profundidad;
                Marcar(f, c, Camino);
                return true;
            }

            // --- Caso recursivo: probar cada dirección ---
            for (int d = 0; d < 4; d++)
            {
                if (Explorar(f + DF[d], c + DC[d], profundidad + 1))
                {
                    Marcar(f, c, Camino); // Esta celda pertenece al camino correcto.
                    return true;
                }
            }

            // --- BACKTRACKING: ninguna dirección funcionó ---
            Marcar(f, c, SinSalida);
            Retrocesos++;
            return false;
        }

        // ---------------------------------------------------------------- Dibujo

        private void Marcar(int f, int c, char simbolo)
        {
            vista[f, c] = simbolo;
            if (animar)
            {
                Dibujar();
                Thread.Sleep(retardoMs);
            }
        }

        private void PrepararAnimacion()
        {
            try
            {
                // Se reservan líneas en pantalla y se redibuja siempre sobre las mismas.
                for (int i = 0; i < laberinto.Filas; i++) Console.WriteLine();
                filaCursor = Math.Max(0, Console.CursorTop - laberinto.Filas);
                Console.SetCursorPosition(0, filaCursor);
            }
            catch (Exception)
            {
                animar = false; // Consola sin soporte de cursor: se omite la animación.
            }
        }

        private void Dibujar()
        {
            StringBuilder sb = new StringBuilder();
            for (int f = 0; f < laberinto.Filas; f++)
            {
                for (int c = 0; c < laberinto.Columnas; c++)
                {
                    if (f == laberinto.InicioF && c == laberinto.InicioC) sb.Append('S');
                    else if (f == laberinto.FinF && c == laberinto.FinC) sb.Append('E');
                    else sb.Append(vista[f, c]);
                }
                sb.AppendLine();
            }

            if (animar)
            {
                try { Console.SetCursorPosition(0, filaCursor); }
                catch (Exception) { animar = false; }
            }
            Console.Write(sb.ToString());
        }
    }
}
