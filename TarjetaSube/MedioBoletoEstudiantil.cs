using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoTarjeta;

namespace TarjetaSube
{
    public class MedioBoletoEstudiantil : Tarjeta
    {
        private int viajesHoy = 0;

        public MedioBoletoEstudiantil()
        {
            TipoTarjeta = "Medio Boleto Estudiantil";
        }

        public override double? Pagar(double tarifa, string lineaActual)
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Console.WriteLine("El medio boleto estudiantil solo es válido entre las 6:00 y las 22:00 horas.");
                return base.Pagar(tarifa, lineaActual);
            }

            DateTime ahora = DateTime.Now;
            DateTime hoy = DateTime.Today;
            double montoAPagar;
            bool beneficioAplicado = false;


            if (fechaUltimoViaje.Date != hoy)
            {
                viajesHoy = 0;
            }

            if ((ahora - fechaUltimoViaje).TotalMinutes < 5)
            {
                montoAPagar = tarifa;
                Console.WriteLine("Debe esperar al menos 5 minutos antes de usar nuevamente el beneficio de la tarjeta. Este viaje se cobra completo.");
            }

            else if (viajesHoy < 2)
            {
                montoAPagar = tarifa * 0.5;
                viajesHoy++;
                beneficioAplicado = true;
                Console.WriteLine($"Viaje con descuento #{viajesHoy} del día.");

            }
            else
            {
                montoAPagar = tarifa;
                Console.WriteLine("Ya utilizaste los 2 medios boletos del día. Este viaje se cobra completo.");
            }

            double? pagoExitoso = base.Pagar(montoAPagar, lineaActual);
            if (pagoExitoso == null && beneficioAplicado)
            {
                viajesHoy--;
            }

            return pagoExitoso;
        }
    }
}
