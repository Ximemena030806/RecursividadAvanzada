using System;
using System.Collections.Generic;
using System.Threading;

namespace RecursividadAvanzada
{
    public enum ModoVisual
    {
        Silencioso,   // solo la lista de movimientos
        Animado,      // dibuja las torres con una pausa entre movimientos
        PasoAPaso     // dibuja las torres y espera Enter en cada movimiento
    }

    /// <summary>
    /// Simulación visual de las Torres de Hanoi con recursividad.
    /// Cada torre es una pila (Stack) de discos: se verifica que NUNCA se coloque un disco
    /// grande sobre uno pequeño, lo que demuestra que el algoritmo recursivo es correcto.
    /// </summary>
    public class SimuladorHanoi
    {
        private readonly Stack<int>[] torres = { new Stack<int>(), new Stack<int>(), new Stack<int>() };
        private readonly char[] nombres = { 'A', 'B', 'C' };
        private readonly int numDiscos;
        private readonly ModoVisual modo;
        private readonly int retardoMs;

        // Estadísticas
        public long Movimientos { get; private set; }
        public long LlamadasRecursivas { get; private set; }
        public int ProfundidadMaxima { get; private set; }

        public SimuladorHanoi(int numDiscos, ModoVisual modo, int retardoMs)
        {
            this.numDiscos = numDiscos;
            this.modo = modo;
            this.retardoMs = retardoMs;

            // Todos los discos empiezan en la torre A (el más grande abajo).
            for (int d = numDiscos; d >= 1; d--)
                torres[0].Push(d);
        }

        public void Ejecutar()
        {
            Console.WriteLine("\nEstado inicial:");
            Dibujar();

            Resolver(numDiscos, 0, 2, 1, 1); // Mover de A (0) a C (2) usando B (1).

            long esperados = (1L << numDiscos) - 1;
            Console.WriteLine("\n=== RESULTADO ===");
            Console.WriteLine($"Movimientos realizados : {Movimientos} (esperados 2^{numDiscos} - 1 = {esperados})");
            Console.WriteLine($"Llamadas recursivas    : {LlamadasRecursivas}");
            Console.WriteLine($"Profundidad máxima     : {ProfundidadMaxima}");
            Console.WriteLine($"Solución correcta      : {Movimientos == esperados && TorreDestinoCorrecta()}");
        }

        /// <summary>
        /// Algoritmo recursivo de Hanoi.
        ///   Caso Base: n == 1 -> mover el disco directamente.
        ///   Caso Recursivo: mover n-1 discos a la torre auxiliar,
        ///                   mover el disco n al destino,
        ///                   mover n-1 discos desde el auxiliar al destino.
        /// </summary>
        private void Resolver(int n, int origen, int destino, int auxiliar, int profundidad)
        {
            LlamadasRecursivas++;
            if (profundidad > ProfundidadMaxima) ProfundidadMaxima = profundidad;

            if (n == 1)
            {
                Mover(origen, destino);
                return;
            }

            Resolver(n - 1, origen, auxiliar, destino, profundidad + 1);
            Mover(origen, destino);
            Resolver(n - 1, auxiliar, destino, origen, profundidad + 1);
        }

        private void Mover(int origen, int destino)
        {
            int disco = torres[origen].Pop();

            // Regla del juego: no se puede poner un disco grande sobre uno pequeño.
            if (torres[destino].Count > 0 && torres[destino].Peek() < disco)
                throw new InvalidOperationException("Movimiento ilegal detectado.");

            torres[destino].Push(disco);
            Movimientos++;

            Console.WriteLine($"Movimiento {Movimientos}: disco {disco} de {nombres[origen]} a {nombres[destino]}");

            if (modo != ModoVisual.Silencioso)
            {
                Dibujar();
                if (modo == ModoVisual.Animado) Thread.Sleep(retardoMs);
                else { Console.Write("(Enter para continuar)"); Console.ReadLine(); }
            }
        }

        private bool TorreDestinoCorrecta()
        {
            if (torres[2].Count != numDiscos) return false;
            int esperado = 1;
            foreach (int disco in torres[2]) // Stack se recorre de arriba hacia abajo: 1, 2, 3...
            {
                if (disco != esperado) return false;
                esperado++;
            }
            return true;
        }

        // Dibuja las tres torres lado a lado con barras de ancho proporcional al disco.
        private void Dibujar()
        {
            int ancho = 2 * numDiscos + 1;

            for (int nivel = numDiscos - 1; nivel >= 0; nivel--)
            {
                string linea = "";
                for (int t = 0; t < 3; t++)
                {
                    int[] discos = torres[t].ToArray(); // arriba -> abajo
                    Array.Reverse(discos);              // abajo -> arriba

                    string celda = nivel < discos.Length
                        ? new string('=', 2 * discos[nivel] + 1)
                        : "|";

                    linea += Centrar(celda, ancho) + "   ";
                }
                Console.WriteLine(linea);
            }

            string base_ = "";
            for (int t = 0; t < 3; t++)
                base_ += Centrar(nombres[t].ToString(), ancho) + "   ";
            Console.WriteLine(new string('-', ancho * 3 + 6));
            Console.WriteLine(base_);
            Console.WriteLine();
        }

        private static string Centrar(string texto, int ancho)
        {
            int izquierda = (ancho - texto.Length) / 2;
            return new string(' ', izquierda) + texto + new string(' ', ancho - texto.Length - izquierda);
        }
    }
}
