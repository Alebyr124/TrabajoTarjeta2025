using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoTarjeta;

namespace TarjetaSube
{
    public class FranquiciaCompleta : Tarjeta
    {
        public FranquiciaCompleta()
        {
            TipoTarjeta = "Franquicia Completa";
        }

        public override double? Pagar(double tarifa, string lineaActual)
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Console.WriteLine("La franquicia completa solo es válida entre las 6:00 y las 22:00 horas.");
                return base.Pagar(tarifa, lineaActual);
            }
            Console.WriteLine("Viaje gratuito por franquicia completa.");
            return base.Pagar(0, lineaActual);
        }
    }
}
