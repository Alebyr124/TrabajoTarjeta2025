using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoTarjeta;

namespace TarjetaSube
{
    public class BoletoGratuitoEstudiantil : Tarjeta
    {
        private int viajesHoy = 0;

        public BoletoGratuitoEstudiantil()
        {
            TipoTarjeta = "Boleto Gratuito Estudiantil";
        }

        public override double? Pagar(double tarifa, string lineaActual)
        {
            double tarifaActual = Trasbordo(tarifa, lineaActual);
            if (tarifaActual == 0)
                return 0;

            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Console.WriteLine("El boleto gratuito estudiantil solo es válido entre las 6:00 y las 22:00 horas.");
                return base.Pagar(tarifa, lineaActual);
            }

            DateTime hoy = DateTime.Today;

            if (fechaUltimoViaje.Date != hoy)
            {
                viajesHoy = 0;
            }

            if (viajesHoy < 2)
            {
                viajesHoy++;
                Console.WriteLine($"Viaje gratuito #{viajesHoy} del día ({hoy.ToShortDateString()})");
                return base.Pagar(0, lineaActual);
            }
            else
            {
                Console.WriteLine("Ya utilizaste los 2 boletos gratuitos del día. Este viaje se cobra completo.");
                return base.Pagar(tarifa, lineaActual);
            }
        }
    }
}
