using System;
using NUnit.Framework;
using TarjetaSube;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class FranquiciaTest
    {
        private ColectivoUrbano colectivo;

        [SetUp]
        public void Setup() => colectivo = new ColectivoUrbano("101");

        [Test]
        public void FranquiciaCompleta_SiempreGratis()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22) Assert.Ignore("Horario nocturno");

            FranquiciaCompleta tarjeta = new FranquiciaCompleta();
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuito_DosViajesGratis_TerceroPaga()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22) Assert.Ignore("Horario nocturno");

            BoletoGratuitoEstudiantil tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.CargarTarjeta(2000);

            // Viaje 1 y 2 (Gratis)
            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(2000, tarjeta.Saldo);

            // Viaje 3 (Paga)
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(2000 - 1580, tarjeta.Saldo);
        }
    }
}