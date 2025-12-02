using System;
using NUnit.Framework;
using TarjetaSube;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class MedioBoletoTest
    {
        private ColectivoUrbano colectivo;

        [SetUp]
        public void Setup()
        {
            colectivo = new ColectivoUrbano("101");
        }

        [Test]
        public void Pagar_CobraMitadDePrecio()
        {
            VerificarHorario();
            MedioBoletoEstudiantil tarjeta = new MedioBoletoEstudiantil();
            tarjeta.CargarTarjeta(2000);

            // Aseguramos que no haya espera pendiente
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-10);

            colectivo.PagarCon(tarjeta);

            // 2000 - (1580 / 2) = 1210
            Assert.AreEqual(1210, tarjeta.Saldo);
        }

        [Test]
        public void Restriccion_5Minutos_CobraCompleto()
        {
            VerificarHorario();
            MedioBoletoEstudiantil tarjeta = new MedioBoletoEstudiantil();
            tarjeta.CargarTarjeta(4000);

            // Viaje 1
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-10);
            colectivo.PagarCon(tarjeta);

            // Viaje 2 INMEDIATO (Simulamos que fue hace 0 mins)
            tarjeta.fechaUltimoViaje = DateTime.Now;

            colectivo.PagarCon(tarjeta);

            // El segundo viaje debe cobrarse a tarifa PLENA (1580)
            // Saldo esperado: 4000 - 790 - 1580 = 1630
            Assert.AreEqual(1630, tarjeta.Saldo);
        }

        [Test]
        public void LimiteDiario_TercerViaje_CobraCompleto()
        {
            VerificarHorario();
            MedioBoletoEstudiantil tarjeta = new MedioBoletoEstudiantil();
            tarjeta.CargarTarjeta(10000);

            // Simulamos que ya viajó 2 veces hoy usando Reflection
            TestUtilities.SetPrivateField(tarjeta, "viajesHoy", 2);
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-10); // Evitar restricción 5 min

            colectivo.PagarCon(tarjeta);

            // Debe cobrar tarifa completa (1580)
            Assert.AreEqual(10000 - 1580, tarjeta.Saldo);
        }

        private void VerificarHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
                Assert.Ignore("Test ignorado por horario nocturno.");
        }
    }
}