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
            // Caso: Se paga la tarifa y se retira una bici exitosamente.
            Tarjeta tarjeta = new SinFranquicia();
            tarjeta.CargarTarjeta(4000); // Saldo suficiente

            var boleto = estacion.RetirarBici(tarjeta);

            Assert.IsNotNull(boleto, "Debería entregar boleto");
            // Verificando que la tarifa cobrada sea la correcta
            Assert.AreEqual(4000 - MiBiciTuBici.TARIFA, tarjeta.Saldo);
        }

        [Test]
        public void RetirarBici_SaldoInsuficiente_Falla()
        {
            // Caso: No se puede retirar la bici por saldo insuficiente.
            Tarjeta tarjeta = new SinFranquicia();
            // Saldo 100 + Margen 1200 = 1300 < 1777.50 -> Debe fallar
            tarjeta.Saldo = 100;

            var boleto = estacion.RetirarBici(tarjeta);

            Assert.IsNull(boleto, "No debería entregar boleto con saldo insuficiente");
            Assert.AreEqual(100, tarjeta.Saldo, "El saldo no debe cambiar si falla el retiro");
        }

        [Test]
        public void Flujo_Retiro_Con_Multas()
        {
            // Caso: Se retira una bici con una o varias multas acumuladas (simulación de flujo).
            Tarjeta tarjeta = new SinFranquicia();
            tarjeta.CargarTarjeta(10000);

            // 1. Primer retiro
            var boleto1 = estacion.RetirarBici(tarjeta);

            // 2. Devolución tarde (genera multa)
            // Simulamos 2.5 horas de uso (1.5h exceso -> 2 multas de 1000 = 2000)
            TestUtilities.CambiarFechaRetiroBici(boleto1, DateTime.Now.AddHours(-2.5));
            estacion.DevolverBici(tarjeta, boleto1);

            double saldoDespuesMulta = tarjeta.Saldo;

            // 3. Intentar retirar de nuevo (debe permitirlo si le queda saldo tras la multa)
            var boleto2 = estacion.RetirarBici(tarjeta);

            Assert.IsNotNull(boleto2, "Debería permitir retirar nuevamente tras pagar la multa implícita.");
            Assert.AreEqual(saldoDespuesMulta - MiBiciTuBici.TARIFA, tarjeta.Saldo);
        }
    }
}