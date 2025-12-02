using System;
using TarjetaSube;
namespace TrabajoTarjeta
{
    public class BoletoMiBiciTuBici
    {
        public DateTime FechaRetiroBici { get; } = DateTime.Now;
        public double MultasAcumuladas;


        public BoletoMiBiciTuBici()
        {
            FechaRetiroBici = DateTime.Now;
            MultasAcumuladas = 0;
        }

        public double CalcularMultasAcumuladas(DateTime FechaDevolverBici) 
        {
            TimeSpan tiempoUso = FechaDevolverBici - FechaRetiroBici;
            MultasAcumuladas = Math.Ceiling(tiempoUso.TotalHours - MiBiciTuBici.TIEMPO_MAXIMO_HORAS);

            //double TarifaMultas = MultasAcumuladas * MiBiciTuBici.MULTA;

            return MultasAcumuladas;
        }

    }
}
