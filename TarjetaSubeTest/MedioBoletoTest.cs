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
        public void Setup() => colectivo = new ColectivoUrbano("101");

        [Test]
        public void Pagar_DosBoletosMedios_ConEspera()
        {
            VerificarHorario();
            MedioBoletoEstudiantil tarjeta = new MedioBoletoEstudiantil();
            tarjeta.CargarTarjeta(4000);

            // Viaje 1
            TestUtilities.SetPrivateField(tarjeta, "fechaUltimoViaje", DateTime.Now.AddMinutes(-20)); // Reset tiempo
            colectivo.PagarCon(tarjeta);
            double saldoTrasPrimerViaje = tarjeta.Saldo;

            // Simular espera de 6 minutos para habilitar el segundo medio boleto
            TestUtilities.SetPrivateField(tarjeta, "fechaUltimoViaje", DateTime.Now.AddMinutes(-6));

            // Viaje 2
            colectivo.PagarCon(tarjeta);

            // Verificamos que se cobraron dos medios boletos (790 c/u)
            Assert.AreEqual(4000 - 790, saldoTrasPrimerViaje);
            Assert.AreEqual(4000 - (790 * 2), tarjeta.Saldo);
        }

        [Test]
        public void Pagar_MedioBoleto_ConSaldoNegativo()
        {
            VerificarHorario();
            // Caso: saldo negativo también permite pagar medio boleto.
            MedioBoletoEstudiantil tarjeta = new MedioBoletoEstudiantil();
            // Saldo -100. Margen es -1200. Tiene disponible 1100. Medio boleto es 790.
            // Debería poder pagar.
            tarjeta.Saldo = -100;
            TestUtilities.SetPrivateField(tarjeta, "fechaUltimoViaje", DateTime.Now.AddMinutes(-20));

            var boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto, "Debería poder pagar con saldo negativo dentro del margen");
            Assert.AreEqual(-100 - 790, tarjeta.Saldo);
        }

        [Test]
        public void Prioridad_Transbordo_Sobre_MedioBoleto()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.Hour < 7 || DateTime.Now.Hour > 22)
                Assert.Ignore("Horario no válido para transbordo");

            // Caso: Verificar que viaje con transbordo si se cumplen los requisitos.
            MedioBoletoEstudiantil tarjeta = new MedioBoletoEstudiantil();
            tarjeta.CargarTarjeta(2000);

            // 1. Primer viaje (Medio Boleto)
            TestUtilities.SetPrivateField(tarjeta, "fechaUltimoViaje", DateTime.Now.AddMinutes(-20));
            colectivo.PagarCon(tarjeta); // Línea 101
            double saldoIntermedio = tarjeta.Saldo;

            // 2. Segundo viaje en OTRA línea dentro de la hora (Transbordo)
            // Simulamos que pasaron 10 min
            TestUtilities.SetPrivateField(tarjeta, "fechaUltimoViaje", DateTime.Now.AddMinutes(-10));

            ColectivoInterurbano otraLinea = new ColectivoInterurbano("35/9");
            var boleto = otraLinea.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            // El saldo NO debe cambiar, porque el transbordo es GRATIS y tiene prioridad sobre el cobro de medio boleto
            Assert.AreEqual(saldoIntermedio, tarjeta.Saldo, "Debería ser gratis por transbordo, ignorando el medio boleto.");
        }

        [Test]
        public void LimiteDiario_TercerViaje_CobraCompleto()
        {
            VerificarHorario();
            MedioBoletoEstudiantil tarjeta = new MedioBoletoEstudiantil();
            tarjeta.CargarTarjeta(5000);

            // Simulamos que ya viajó 2 veces hoy
            TestUtilities.SetPrivateField(tarjeta, "viajesHoy", 2);
            TestUtilities.SetPrivateField(tarjeta, "fechaUltimoViaje", DateTime.Now.AddMinutes(-10));

            // Tercer viaje
            colectivo.PagarCon(tarjeta);

            // Debe cobrar tarifa completa (1580) en lugar de media (790)
            Assert.AreEqual(5000 - 1580, tarjeta.Saldo);
        }

        private void VerificarHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
                Assert.Ignore("Test ignorado por horario nocturno.");
        }
    }
}
