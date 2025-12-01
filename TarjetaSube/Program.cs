using System;
using System.Collections.Generic;
using TrabajoTarjeta;
using TarjetaSube;

namespace TrabajoTarjeta
{
    class Program
    {
        // Lista de colectivos disponibles para simular distintas líneas
        static List<Colectivo> colectivos = new List<Colectivo>();
        static Tarjeta miTarjeta;

        static void Main(string[] args)
        {
            InicializarColectivos();
            ConfigurarTarjeta();
            MenuPrincipal();
        }

        static void InicializarColectivos()
        {
            colectivos.Add(new ColectivoUrbano("101 (Urbano - $1580)"));
            colectivos.Add(new ColectivoUrbano("144 (Urbano - $1580)")); // Otra línea urbana para probar trasbordo
            colectivos.Add(new ColectivoInterurbano("35/9 (Interurbano - $3000)"));
        }

        static void ConfigurarTarjeta()
        {
            Console.Clear();
            Console.WriteLine("=== BIENVENIDO AL SISTEMA SUBE ===");
            Console.WriteLine("Por favor, seleccione el tipo de tarjeta para esta sesión:");
            Console.WriteLine("1. Tarjeta Común (Sin Franquicia)");
            Console.WriteLine("2. Medio Boleto Estudiantil");
            Console.WriteLine("3. Boleto Gratuito Estudiantil");
            Console.WriteLine("4. Franquicia Completa (Jubilados/Policía)");
            Console.Write("\nOpción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    miTarjeta = new SinFranquicia();
                    break;
                case "2":
                    miTarjeta = new MedioBoletoEstudiantil();
                    break;
                case "3":
                    miTarjeta = new BoletoGratuitoEstudiantil();
                    break;
                case "4":
                    miTarjeta = new FranquiciaCompleta();
                    break;
                default:
                    Console.WriteLine("Opción inválida. Se usará Tarjeta Común por defecto.");
                    miTarjeta = new SinFranquicia();
                    break;
            }

            Console.WriteLine($"\nTarjeta creada: {miTarjeta.TipoTarjeta}");
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void MenuPrincipal()
        {
            bool continuar = true;

            while (continuar)
            {
                Console.Clear();
                MostrarEstadoTarjeta();

                Console.WriteLine("\n--- MENÚ PRINCIPAL ---");
                Console.WriteLine("1. Cargar Saldo");
                Console.WriteLine("2. Viajar (Elegir Colectivo)");
                Console.WriteLine("3. Cambiar de Tarjeta (Reiniciar)");
                Console.WriteLine("4. Salir");
                Console.Write("\nSeleccione una opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        MenuCarga();
                        break;
                    case "2":
                        MenuViaje();
                        break;
                    case "3":
                        ConfigurarTarjeta(); // Reinicia la tarjeta
                        break;
                    case "4":
                        continuar = false;
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
        }

        static void MostrarEstadoTarjeta()
        {
            Console.WriteLine("========================================");
            Console.WriteLine($" TARJETA: {miTarjeta.TipoTarjeta}");
            Console.WriteLine($" ID:      {miTarjeta.Id}");
            Console.WriteLine($" SALDO:   ${miTarjeta.Saldo:F2}");
            if (miTarjeta.SaldoPendiente > 0)
            {
                Console.WriteLine($" PENDIENTE DE ACREDITACIÓN: ${miTarjeta.SaldoPendiente:F2}");
            }
            Console.WriteLine("========================================");
        }

        static void MenuCarga()
        {
            Console.Clear();
            Console.WriteLine("--- CARGAR SALDO ---");
            Console.WriteLine("Montos aceptados: 2000, 3000, 4000, 5000, 10000, 15000, 20000, 25000, 30000");
            Console.Write("Ingrese monto a cargar: ");

            if (double.TryParse(Console.ReadLine(), out double monto))
            {
                bool resultado = miTarjeta.CargarTarjeta(monto);
                if (resultado)
                {
                    Console.WriteLine($"¡Carga exitosa! Se sumaron ${monto} (o quedaron pendientes si superó el límite).");
                }
                else
                {
                    Console.WriteLine("Error: Monto no permitido por el sistema.");
                }
            }
            else
            {
                Console.WriteLine("Entrada inválida.");
            }
            Console.WriteLine("\nPresione cualquier tecla para volver...");
            Console.ReadKey();
        }

        static void MenuViaje()
        {
            Console.Clear();
            Console.WriteLine("--- SELECCIONE EL COLECTIVO ---");
            for (int i = 0; i < colectivos.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {colectivos[i].Linea} (Tarifa base: ${colectivos[i].Tarifa})");
            }
            Console.WriteLine("0. Cancelar");
            Console.Write("\nOpción: ");

            if (int.TryParse(Console.ReadLine(), out int opcion) && opcion > 0 && opcion <= colectivos.Count)
            {
                Colectivo colectivoElegido = colectivos[opcion - 1];

                // Intentar pagar
                Console.WriteLine("\nProcesando pago...");
                Boleto boleto = colectivoElegido.PagarCon(miTarjeta);

                if (boleto != null)
                {
                    Console.WriteLine("\n¡VIAJE ACEPTADO!");
                    Console.WriteLine("--------------------------------");
                    boleto.Imprimir();
                    Console.WriteLine("--------------------------------");
                }
                else
                {
                    Console.WriteLine("\nX VIAJE RECHAZADO X");
                    Console.WriteLine("Posibles causas: Saldo insuficiente, horario no permitido o restricción de tiempo.");
                }
            }
            else if (opcion != 0)
            {
                Console.WriteLine("Colectivo no válido.");
            }

            Console.WriteLine("\nPresione cualquier tecla para volver...");
            Console.ReadKey();
        }
    }
}