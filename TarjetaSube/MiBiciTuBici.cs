using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoTarjeta;

namespace TarjetaSube
{
    public class MiBiciTuBici
    {
        public const double TARIFA = 1777.50;
        public const double MULTA = 1000;
        public const int TIEMPO_MAXIMO_HORAS = 1;

        public BoletoMiBiciTuBici RetirarBici(Tarjeta tarjeta)
        {
            if (tarjeta == null || tarjeta.Saldo + Tarjeta.SALDO_NEGATIVO < TARIFA)
            {
                Console.WriteLine("Saldo insuficiente para retirar una bici.");
                return null;
            }
            tarjeta.Saldo -= TARIFA;

            return new BoletoMiBiciTuBici();
        }

        public void DevolverBici(Tarjeta tarjeta, BoletoMiBiciTuBici boletoBici)
        {
            DateTime ahora = DateTime.Now;
            double CantidadMultas = boletoBici.CalcularMultasAcumuladas(ahora);
            if (CantidadMultas > 0)
            {
                Console.WriteLine($"Se han acumulado {CantidadMultas} multas por exceder el tiempo máximo de uso de la bici.");
            }
            else
            {
                Console.WriteLine("Bici devuelta a tiempo. No se han acumulado multas.");
            }

            tarjeta.Saldo -= CantidadMultas * MULTA;
            

        }
    }
}