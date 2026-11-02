using System;
using System.Text;

namespace RecursividadAvanzada
{
    /// <summary>
    /// Aplicación de consola con dos problemas complejos resueltos con recursividad:
    ///   1/2. Laberinto con backtracking (aleatorio o predefinido).
    ///   3.   Simulación visual de las Torres de Hanoi.
    /// </summary>
    public class Program
    {
        // Laberinto predefinido: la primera ruta que se prueba (hacia abajo) es un callejón sin salida,
        // por eso se aprecia bien el backtracking.
        private static readonly string[] LaberintoFijo =
        {
            "#########",
            "#S  #   #",
            "# # # # #",
            "# #   # #",
            "# ##### #",
            "#     #E#",
            "#########"
        };

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            bool salir = false;

            while (!salir)
            {
                Console.WriteLine("\n==========================================");
                Console.WriteLine("   RECURSIVIDAD: PROBLEMAS COMPLEJOS");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Laberinto aleatorio (backtracking)");
                Console.WriteLine("2. Laberinto predefinido (backtracking)");
                Console.WriteLine("3. Torres de Hanoi (simulación visual)");
                Console.WriteLine("0. Salir");
                Console.Write("Opción: ");
                string opcion = Console.ReadLine();
                Console.WriteLine();

                switch (opcion)
                {
                    case "1": LaberintoAleatorio(); break;
                    case "2": ResolverLaberinto(Laberinto.DesdeTexto(LaberintoFijo)); break;
                    case "3": Hanoi(); break;
                    case "0": salir = true; break;
                    default: Console.WriteLine("Opción no válida."); break;
                }
            }
        }

        private static void LaberintoAleatorio()
        {
            int filas = Entrada.LeerEntero("Filas (7 - 31): ", 7, 31);
            int columnas = Entrada.LeerEntero("Columnas (7 - 61): ", 7, 61);
            bool lazos = Entrada.LeerSiNo("¿Agregar caminos alternativos (el laberinto tendrá varias rutas)?");

            Laberinto lab = Laberinto.Generar(filas, columnas, new Random(), lazos ? 0.08 : 0.0);
            ResolverLaberinto(lab);
        }

        private static void ResolverLaberinto(Laberinto lab)
        {
            bool animar = Entrada.LeerSiNo("¿Animar la exploración paso a paso?");
            int retardo = animar ? Entrada.LeerEntero("Retardo por paso en ms (1 - 500): ", 1, 500) : 0;

            Console.WriteLine();
            ResolutorLaberinto resolutor = new ResolutorLaberinto(lab, animar, retardo);
            bool exito = resolutor.Resolver();

            Console.WriteLine("\nLeyenda: S = inicio | E = salida | · = ruta en prueba | x = callejón (retroceso) | * = camino final");
            Console.WriteLine($"Resultado             : {(exito ? "¡Camino encontrado!" : "No existe camino")}");
            Console.WriteLine($"Llamadas recursivas   : {resolutor.Llamadas}");
            Console.WriteLine($"Retrocesos (backtrack): {resolutor.Retrocesos}");
            Console.WriteLine($"Profundidad máxima    : {resolutor.ProfundidadMaxima}");
            if (exito)
                Console.WriteLine($"Longitud del camino   : {resolutor.LongitudCamino} celdas");
        }

        private static void Hanoi()
        {
            int discos = Entrada.LeerEntero("Número de discos (1 - 10): ", 1, 10);

            Console.WriteLine("Modo de visualización:");
            Console.WriteLine("  1. Animado");
            Console.WriteLine("  2. Paso a paso (Enter en cada movimiento)");
            Console.WriteLine("  3. Solo la lista de movimientos");
            int modo = Entrada.LeerEntero("Modo: ", 1, 3);

            ModoVisual visual = modo == 1 ? ModoVisual.Animado
                              : modo == 2 ? ModoVisual.PasoAPaso
                              : ModoVisual.Silencioso;

            int retardo = visual == ModoVisual.Animado
                ? Entrada.LeerEntero("Retardo entre movimientos en ms (50 - 2000): ", 50, 2000)
                : 0;

            SimuladorHanoi simulador = new SimuladorHanoi(discos, visual, retardo);
            simulador.Ejecutar();
        }
    }
}
