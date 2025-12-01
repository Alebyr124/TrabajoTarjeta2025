using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoTarjeta;

namespace TarjetaSube
{
    public class SinFranquicia : Tarjeta
    {
        int cantidadViajes = 0;
        public float UsoFrecuente()
        {

            float descuento = 1;

            if (DateTime.Now.Day == 1)
            {
                cantidadViajes = 0;
            }
            if (cantidadViajes <= 29)
                descuento = 1f;
            else if (cantidadViajes <= 59)
                descuento = 0.8f;
            else if (cantidadViajes <= 80)
                descuento = 0.75f;
            else
                descuento = 1f;

            return descuento;
        }
        public override double? Pagar(double tarifa, string lineaActual)
        {
            double? pagoExitoso = base.Pagar(tarifa * UsoFrecuente(), lineaActual);
            if (pagoExitoso != null)
            {
                cantidadViajes++;
            }
            return pagoExitoso;
        }
    }
}
