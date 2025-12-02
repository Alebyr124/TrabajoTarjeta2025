using NUnit.Framework;
using TarjetaSube;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class TarjetaTest
    {
        [Test]
        public void CargarSaldo_MontoValido_SumaSaldo()
        {
            Tarjeta tarjeta = new SinFranquicia();
            bool resultado = tarjeta.CargarTarjeta(4000);

            Assert.IsTrue(resultado);
            Assert.AreEqual(4000, tarjeta.Saldo);
        }

        [Test]
        public void CargarSaldo_MontoInvalido_RechazaCarga()
        {
            Tarjeta tarjeta = new SinFranquicia();
            bool resultado = tarjeta.CargarTarjeta(500); // Monto no permitido

            Assert.IsFalse(resultado);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void SaldoPendiente_ExcesoLimite_GuardaExcedente()
        {
            Tarjeta tarjeta = new SinFranquicia();
            tarjeta.Saldo = 50000;

            // Carga 10000 -> 6000 van a saldo (tope 56000), 4000 a pendiente
            tarjeta.CargarTarjeta(10000);

            Assert.AreEqual(56000, tarjeta.Saldo);
            Assert.AreEqual(4000, tarjeta.SaldoPendiente);
        }

        [Test]
        public void AcreditarSaldo_AlGastar_RecuperaPendiente()
        {
            Tarjeta tarjeta = new SinFranquicia();
            tarjeta.Saldo = 56000;
            tarjeta.SaldoPendiente = 2000;

            // Simulamos gasto manual
            tarjeta.Saldo -= 2000;

            // Forzamos acreditación (esto suele llamarse dentro de Pagar)
            tarjeta.AcreditarCarga();

            Assert.AreEqual(56000, tarjeta.Saldo);
            Assert.AreEqual(0, tarjeta.SaldoPendiente);
        }
    }
}