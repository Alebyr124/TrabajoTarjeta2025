using System;
using System.Linq;
using System.Threading;

namespace TrabajoTarjeta
{
    public class Tarjeta
    {
        public double Saldo { get; set; }
        public string TipoTarjeta { get; set; }
        public string Id { get; set; }
        public DateTime fechaUltimoViaje = DateTime.MinValue;
        public string ultimaLinea;
        public const double SALDO_NEGATIVO = 1200;

        public const double SALDO_MAXIMO = 56000;
        public double SaldoPendiente { get; set; }


        private readonly double[] cargasAceptadas = { 2000, 3000, 4000, 5000, 10000, 15000, 20000, 25000, 30000 };

        public Tarjeta()
        {
            Saldo = 0;
            TipoTarjeta = "Sin Franquicia";
            Id = $"SUBE-{Guid.NewGuid()}";
            SaldoPendiente = 0;
        }

        public bool CargarTarjeta(double monto)
        {

            if (!cargasAceptadas.Contains(monto))
            {
                return false;
            }


            if (monto + Saldo + SaldoPendiente > SALDO_MAXIMO)
            {
                double espacioDisponible = SALDO_MAXIMO - Saldo - SaldoPendiente;
                if (espacioDisponible > 0)
                {
                    SaldoPendiente += monto - espacioDisponible;
                    Saldo += espacioDisponible;
                }
                else
                {
                    SaldoPendiente += monto;
                }
            }
            else
            {
                Saldo += monto;
            }

            return true;
        }

        public void AcreditarCarga()
        {
            if (SaldoPendiente > 0)
            {
                double espacioDisponible = SALDO_MAXIMO - Saldo;

                if (espacioDisponible > 0)
                {
                    double montoAAcreditar = Math.Min(SaldoPendiente, espacioDisponible);
                    Saldo += montoAAcreditar;
                    SaldoPendiente -= montoAAcreditar;
                }
            }
        }


        public virtual double? Pagar(double tarifa, string lineaActual)
        {
            bool esTrasbordo = false;

            if (Saldo + SALDO_NEGATIVO >= tarifa)
            {
                if ((DateTime.Now - fechaUltimoViaje).TotalHours < 1
                    && DateTime.Now.DayOfWeek != DayOfWeek.Sunday
                    && DateTime.Now.Hour >= 7 && DateTime.Now.Hour < 22
                    && ultimaLinea != null
                    && ultimaLinea != lineaActual)
                {
                    esTrasbordo = true;
                }

                if (!esTrasbordo)
                {
                    Saldo -= tarifa;
                }

                fechaUltimoViaje = DateTime.Now;
                ultimaLinea = lineaActual;

                AcreditarCarga();

                return tarifa;
            }
            return null;
        }

    }
}