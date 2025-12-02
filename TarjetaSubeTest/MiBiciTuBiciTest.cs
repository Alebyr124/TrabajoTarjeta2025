using System;
using NUnit.Framework;
using TarjetaSube;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class MiBiciTuBiciTest
    {
        private MiBiciTuBici estacion;

        [SetUp]
        public void Setup() => estacion = new MiBiciTuBici();

        [Test]
        public void RetirarBici_SaldoSuficiente_Exito()
        {
            Tarjeta tarjeta = new SinFranquicia();
            tarjeta.CargarTarjeta(2000);

            var boleto = estacion.RetirarBici(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(2000 - MiBiciTuBici.TARIFA, tarjeta.Saldo);
        }

        [Test]
        public void DevolverBici_ConExceso_CobraMulta()
        {
            Tarjeta tarjeta = new SinFranquicia();
            tarjeta.CargarTarjeta(10000);

            var boleto = estacion.RetirarBici(tarjeta);
            double saldoTrasRetiro = tarjeta.Saldo;

            // Simular 2.5 horas de uso (1.5h exceso -> 2 multas)
            TestUtilities.CambiarFechaRetiroBici(boleto, DateTime.Now.AddHours(-2.5));

            estacion.DevolverBici(tarjeta, boleto);

            // Multa: 2000
            Assert.AreEqual(saldoTrasRetiro - 2000, tarjeta.Saldo);
        }
    }
}