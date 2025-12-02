using NUnit.Framework;
using TarjetaSube;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class ColectivoTest
    {
        private ColectivoUrbano urbano;
        private ColectivoInterurbano interurbano;

        [SetUp]
        public void Setup()
        {
            urbano = new ColectivoUrbano("101");
            interurbano = new ColectivoInterurbano("35/9");
        }

        [Test]
        public void Tarifas_SonCorrectas()
        {
            Assert.AreEqual(1580, urbano.Tarifa);
            Assert.AreEqual(3000, interurbano.Tarifa);
        }

        [Test]
        public void Pagar_SaldoSuficiente_DevuelveBoleto()
        {
            Tarjeta tarjeta = new SinFranquicia();
            tarjeta.CargarTarjeta(2000);

            Boleto boleto = urbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual("101", boleto.Linea);
            Assert.AreEqual(2000 - 1580, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_SaldoInsuficiente_DevuelveNull()
        {
            Tarjeta tarjeta = new SinFranquicia();
            // Saldo 100 + Margen 1200 = 1300 < 1580 -> Insuficiente
            tarjeta.Saldo = 100;

            Boleto boleto = urbano.PagarCon(tarjeta);

            Assert.IsNull(boleto);
            Assert.AreEqual(100, tarjeta.Saldo);
        }
    }
}