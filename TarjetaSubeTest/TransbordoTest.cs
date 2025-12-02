using System;
using NUnit.Framework;
using TarjetaSube;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class TransbordoTest
    {
        [Test]
        public void Transbordo_Valido_EsGratis()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.Hour < 7 || DateTime.Now.Hour > 22)
                Assert.Ignore("Horario no válido para transbordo");

            Tarjeta tarjeta = new SinFranquicia();
            tarjeta.CargarTarjeta(2000);
            Colectivo linea1 = new ColectivoUrbano("101");
            Colectivo linea2 = new ColectivoUrbano("144");

            // Viaje 1: Hace 20 minutos
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-20);
            // Seteamos la línea manualmente porque no podemos llamar a Pagar sin resetear la fecha a Now
            // Ojo: PagarCon llama a Pagar, y Pagar setea la fecha a Now.
            // Para testear transbordo real llamamos a PagarCon 1 vez.
            linea1.PagarCon(tarjeta);
            double saldoIntermedio = tarjeta.Saldo;

            // Hack: Retrocedemos el reloj interno de la tarjeta para simular que el viaje 1 fue hace 10 min
            // y no "recién" (aunque para el transbordo "recién" también vale si es < 60 min).
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-10);

            // Viaje 2: Transbordo
            linea2.PagarCon(tarjeta);

            Assert.AreEqual(saldoIntermedio, tarjeta.Saldo, "El saldo no debió bajar en el segundo viaje.");
        }
    }
}