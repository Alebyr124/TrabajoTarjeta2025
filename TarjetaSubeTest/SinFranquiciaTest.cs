using NUnit.Framework;
using TarjetaSube;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class SinFranquiciaTest
    {
        private ColectivoUrbano colectivo;

        [SetUp]
        public void Setup() => colectivo = new ColectivoUrbano("101");

        [Test]
        public void Descuento_Viaje30_Aplica20Porciento()
        {
            SinFranquicia tarjeta = new SinFranquicia();
            tarjeta.CargarTarjeta(30000);

            // Simulamos que ya hizo 30 viajes (entró en la franja de descuento)
            TestUtilities.SetPrivateField(tarjeta, "cantidadViajes", 30);

            colectivo.PagarCon(tarjeta);

            // Tarifa 1580 * 0.8 = 1264
            // Saldo esperado: 30000 - 1264 = 28736
            Assert.AreEqual(28736, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void Descuento_Viaje81_VuelveATarifaPlena()
        {
            SinFranquicia tarjeta = new SinFranquicia();
            tarjeta.CargarTarjeta(30000);

            // Simulamos 81 viajes (se acabaron los descuentos)
            TestUtilities.SetPrivateField(tarjeta, "cantidadViajes", 81);

            colectivo.PagarCon(tarjeta);

            // Tarifa plena 1580
            Assert.AreEqual(30000 - 1580, tarjeta.Saldo, 0.01);
        }
    }
}