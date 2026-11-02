using System;

namespace RecursividadAvanzada
{
    /// <summary>
    /// Representa un laberinto como una matriz de celdas ('#' = pared, ' ' = libre).
    /// Puede generarse de forma aleatoria (también con recursividad) o leerse desde texto.
    /// </summary>
    public class Laberinto
    {
        public const char Pared = '#';
        public const char Libre = ' ';

        public char[,] Celdas { get; private set; }
        public int Filas { get; private set; }
        public int Columnas { get; private set; }

        // Posiciones de entrada (S) y salida (E)
        public int InicioF { get; private set; }
        public int InicioC { get; private set; }
        public int FinF { get; private set; }
        public int FinC { get; private set; }

        // Desplazamientos en pasos de 2 celdas para la generación: abajo, derecha, arriba, izquierda
        private static readonly int[] DF = { 1, 0, -1, 0 };
        private static readonly int[] DC = { 0, 1, 0, -1 };

        private Laberinto(int filas, int columnas)
        {
            Filas = filas;
            Columnas = columnas;
            Celdas = new char[filas, columnas];
        }

        /// <summary>
        /// Crea un laberinto aleatorio.
        /// Algoritmo: "recursive backtracker" (DFS recursivo que va tallando pasillos).
        ///  - Caso Base: no hay vecino sin visitar -> la función termina y retrocede.
        ///  - Caso Recursivo: elegir un vecino al azar, abrir la pared entre ambos y tallar desde allí.
        /// Con 'probabilidadLazos' > 0 se abren paredes extra para crear varios caminos posibles.
        /// </summary>
        public static Laberinto Generar(int filas, int columnas, Random azar, double probabilidadLazos)
        {
            // Las dimensiones deben ser impares para que paredes y pasillos se alternen.
            if (filas % 2 == 0) filas++;
            if (columnas % 2 == 0) columnas++;

            Laberinto lab = new Laberinto(filas, columnas);

            for (int f = 0; f < filas; f++)
                for (int c = 0; c < columnas; c++)
                    lab.Celdas[f, c] = Pared;

            lab.Tallar(1, 1, azar);   // Recursión de generación

            // Paredes extra abiertas al azar (solo las que separan dos pasillos).
            for (int f = 1; f < filas - 1; f++)
                for (int c = 1; c < columnas - 1; c++)
                {
                    bool paredEntrePasillos = (f % 2 == 1) != (c % 2 == 1);
                    if (lab.Celdas[f, c] == Pared && paredEntrePasillos && azar.NextDouble() < probabilidadLazos)
                        lab.Celdas[f, c] = Libre;
                }

            lab.InicioF = 1;           lab.InicioC = 1;
            lab.FinF = filas - 2;      lab.FinC = columnas - 2;
            return lab;
        }

        /// <summary>Construye un laberinto desde líneas de texto con '#', ' ', 'S' (inicio) y 'E' (fin).</summary>
        public static Laberinto DesdeTexto(string[] lineas)
        {
            Laberinto lab = new Laberinto(lineas.Length, lineas[0].Length);

            for (int f = 0; f < lab.Filas; f++)
                for (int c = 0; c < lab.Columnas; c++)
                {
                    char ch = lineas[f][c];
                    if (ch == 'S') { lab.InicioF = f; lab.InicioC = c; ch = Libre; }
                    else if (ch == 'E') { lab.FinF = f; lab.FinC = c; ch = Libre; }
                    lab.Celdas[f, c] = ch == Pared ? Pared : Libre;
                }
            return lab;
        }

        // Función recursiva que talla pasillos (DFS).
        private void Tallar(int f, int c, Random azar)
        {
            Celdas[f, c] = Libre;

            int[] orden = { 0, 1, 2, 3 };
            Mezclar(orden, azar);

            foreach (int d in orden)
            {
                int nf = f + DF[d] * 2;
                int nc = c + DC[d] * 2;

                bool dentro = nf > 0 && nf < Filas - 1 && nc > 0 && nc < Columnas - 1;
                if (dentro && Celdas[nf, nc] == Pared)
                {
                    Celdas[f + DF[d], c + DC[d]] = Libre; // Abre la pared intermedia.
                    Tallar(nf, nc, azar);                 // Llamada recursiva.
                }
            }
            // Al terminar el ciclo no hay más vecinos: se retrocede (caso base implícito).
        }

        // Mezcla Fisher-Yates.
        private static void Mezclar(int[] arreglo, Random azar)
        {
            for (int i = arreglo.Length - 1; i > 0; i--)
            {
                int j = azar.Next(i + 1);
                int temporal = arreglo[i];
                arreglo[i] = arreglo[j];
                arreglo[j] = temporal;
            }
        }
    }
}
