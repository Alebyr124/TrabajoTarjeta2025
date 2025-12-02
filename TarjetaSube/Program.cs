/*using System;
using System.Collections.Generic;
using TrabajoTarjeta;
using TarjetaSube;

namespace TrabajoTarjeta
{
    class Program
    {
        // Lista de colectivos disponibles
        static List<Colectivo> colectivos = new List<Colectivo>();
        static Tarjeta miTarjeta;

        // --- NUEVO: Variables para el sistema de Bicis ---
        static MiBiciTuBici estacionBicis = new MiBiciTuBici();
        static BoletoMiBiciTuBici boletoBiciActual = null; // Guarda el ticket si tienes una bici
        // -------------------------------------------------

        static void Main(string[] args)
        {
            InicializarColectivos();
            ConfigurarTarjeta();
            MenuPrincipal();
        }

        static void InicializarColectivos()
        {
            colectivos.Add(new ColectivoUrbano("101 (Urbano - $1580)"));
            colectivos.Add(new ColectivoUrbano("144 (Urbano - $1580)"));
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
                case "1": miTarjeta = new SinFranquicia(); break;
                case "2": miTarjeta = new MedioBoletoEstudiantil(); break;
                case "3": miTarjeta = new BoletoGratuitoEstudiantil(); break;
                case "4": miTarjeta = new FranquiciaCompleta(); break;
                default:
                    Console.WriteLine("Opción inválida. Se usará Tarjeta Común por defecto.");
                    miTarjeta = new SinFranquicia();
                    break;
            }

            // Reiniciamos el estado de la bici al cambiar tarjeta
            boletoBiciActual = null;

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

                // Aviso visual si hay bici en uso
                if (boletoBiciActual != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(">> ATENCIÓN: TIENES UNA BICICLETA EN ALQUILER <<");
                    Console.ResetColor();
                }

                Console.WriteLine("\n--- MENÚ PRINCIPAL ---");
                Console.WriteLine("1. Cargar Saldo");
                Console.WriteLine("2. Viajar (Elegir Colectivo)");
                Console.WriteLine("3. Mi Bici Tu Bici (Estación)"); // Nueva opción
                Console.WriteLine("4. Cambiar de Tarjeta (Reiniciar)");
                Console.WriteLine("5. Salir");
                Console.Write("\nSeleccione una opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1": MenuCarga(); break;
                    case "2": MenuViaje(); break;
                    case "3": MenuBicis(); break; // Llamada al nuevo menú
                    case "4": ConfigurarTarjeta(); break;
                    case "5": continuar = false; break;
                    default: Console.WriteLine("Opción no válida."); break;
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
                if (miTarjeta.CargarTarjeta(monto))
                    Console.WriteLine($"¡Carga exitosa! Se sumaron ${monto}.");
                else
                    Console.WriteLine("Error: Monto no permitido por el sistema.");
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
                Console.WriteLine("\nProcesando pago...");
                Boleto boleto = colectivoElegido.PagarCon(miTarjeta);

                if (boleto != null)
                {
                    Console.WriteLine("\n¡VIAJE ACEPTADO!");
                    boleto.Imprimir();
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

        // --- NUEVO MÉTODO: Lógica de Bicis ---
        static void MenuBicis()
        {
            Console.Clear();
            Console.WriteLine("--- ESTACIÓN MI BICI TU BICI ---");
            Console.WriteLine($"Tarifa: ${MiBiciTuBici.TARIFA}");
            Console.WriteLine($"Tiempo Límite: {MiBiciTuBici.TIEMPO_MAXIMO_HORAS} hora(s)");
            Console.WriteLine($"Multa por hora excedida: ${MiBiciTuBici.MULTA}");
            Console.WriteLine("--------------------------------");

            if (boletoBiciActual == null)
            {
                // Si NO tiene bici, mostramos opción de retirar
                Console.WriteLine("1. Retirar Bicicleta");
                Console.WriteLine("2. Volver");

                string op = Console.ReadLine();
                if (op == "1")
                {
                    boletoBiciActual = estacionBicis.RetirarBici(miTarjeta);
                    if (boletoBiciActual != null)
                        Console.WriteLine("¡Disfruta el viaje! Recuerda devolverla antes de 1 hora.");
                }
            }
            else
            {
                // Si YA tiene bici, mostramos opción de devolver
                TimeSpan tiempoUso = DateTime.Now - boletoBiciActual.FechaRetiroBici;
                Console.WriteLine($"Estado: BICI EN USO. Tiempo transcurrido: {tiempoUso.TotalMinutes:F0} min.");
                Console.WriteLine("1. Devolver Bicicleta");
                Console.WriteLine("2. Volver");

                string op = Console.ReadLine();
                if (op == "1")
                {
                    estacionBicis.DevolverBici(miTarjeta, boletoBiciActual);
                    boletoBiciActual = null; // Limpiamos la variable porque ya la devolvió
                }
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}*/