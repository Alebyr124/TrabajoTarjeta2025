using NUnit.Framework;
using TrabajoTarjeta;
using TarjetaSube;
using System;
using System.Reflection;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class TestIntegrationNUnit
    {
        private MiBiciTuBici estacion;
        private Colectivo colectivoUrbano;
        private Colectivo colectivoInterurbano;

        [SetUp]
        public void Setup()
        {
            estacion = new MiBiciTuBici();
            colectivoUrbano = new ColectivoUrbano("101");
            colectivoInterurbano = new ColectivoInterurbano("35/9");
        }

        [Test]
        public void Test_Bici_RetiroExitoso_SaldoCorrecto()
        {
            Tarjeta tarjeta = new SinFranquicia();
            tarjeta.CargarTarjeta(4000);
            double saldoInicial = tarjeta.Saldo;

            BoletoMiBiciTuBici boleto = estacion.RetirarBici(tarjeta);

            Assert.IsNotNull(boleto, "El boleto no debería ser null si hay saldo suficiente.");
            Assert.AreEqual(saldoInicial - MiBiciTuBici.TARIFA, tarjeta.Saldo, 0.01, "El saldo descontado no coincide con la tarifa.");
        }

        [Test]
        public void Test_Bici_Rechazo_SaldoInsuficiente()
        {
            Tarjeta tarjeta = new SinFranquicia();
            tarjeta.CargarTarjeta(2000);
            tarjeta.Saldo = 100;
            double saldoInicial = tarjeta.Saldo;

            BoletoMiBiciTuBici boleto = estacion.RetirarBici(tarjeta);

            Assert.IsNull(boleto, "El sistema debería rechazar el retiro por saldo insuficiente.");
            Assert.AreEqual(saldoInicial, tarjeta.Saldo, "El saldo no debería cambiar tras un rechazo.");
        }

        [Test]
        public void Test_Bici_DevolucionConMulta()
        {
            Tarjeta tarjeta = new SinFranquicia();
            tarjeta.CargarTarjeta(10000);
            double saldoInicial = tarjeta.Saldo;

            BoletoMiBiciTuBici boleto = estacion.RetirarBici(tarjeta);
            Assert.IsNotNull(boleto, "Retiro inicial fallido.");

            Assert.AreEqual(saldoInicial - MiBiciTuBici.TARIFA, tarjeta.Saldo, 0.01);

            CambiarFechaRetiroBici(boleto, DateTime.Now.AddHours(-3));

            double saldoAntesDevolucion = tarjeta.Saldo;
            estacion.DevolverBici(tarjeta, boleto);

            double multaEsperada = 2000;

            Assert.AreEqual(saldoAntesDevolucion - multaEsperada, tarjeta.Saldo, 0.01, "No se cobró la multa correctamente al devolver.");
        }

        [Test]
        public void Test_MedioBoleto_DosViajesConEspera()
        {
            MedioBoletoEstudiantil tarjeta = new MedioBoletoEstudiantil();
            tarjeta.CargarTarjeta(4000);
            double tarifaMedia = colectivoUrbano.Tarifa / 2;

            CambiarFechaUltimoViaje(tarjeta, DateTime.Now.AddMinutes(-20));

            colectivoUrbano.PagarCon(tarjeta);
            Assert.AreEqual(4000 - tarifaMedia, tarjeta.Saldo, 0.01, "El primer viaje no cobró medio boleto.");

            CambiarFechaUltimoViaje(tarjeta, DateTime.Now.AddMinutes(-6));

            colectivoUrbano.PagarCon(tarjeta);
            double saldoEsperadoFinal = 4000 - (tarifaMedia * 2);
            Assert.AreEqual(saldoEsperadoFinal, tarjeta.Saldo, 0.01, "El segundo viaje no cobró medio boleto.");
        }

        [Test]
        public void Test_MedioBoleto_LimiteDiario_TercerViaje()
        {
            MedioBoletoEstudiantil tarjeta = new MedioBoletoEstudiantil();
            tarjeta.CargarTarjeta(10000);
            double tarifaMedia = colectivoUrbano.Tarifa / 2;
            double tarifaPlena = colectivoUrbano.Tarifa;

            CambiarFechaUltimoViaje(tarjeta, DateTime.Now.AddMinutes(-30));
            colectivoUrbano.PagarCon(tarjeta);

            CambiarFechaUltimoViaje(tarjeta, DateTime.Now.AddMinutes(-10));
            colectivoUrbano.PagarCon(tarjeta);

            double saldoAntesTercero = tarjeta.Saldo;

            CambiarFechaUltimoViaje(tarjeta, DateTime.Now.AddMinutes(-10));

            Boleto boleto = colectivoUrbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto, "El tercer viaje debería emitirse.");
            Assert.AreEqual(saldoAntesTercero - tarifaPlena, tarjeta.Saldo, 0.01, "El tercer viaje debería cobrarse a tarifa plena.");
        }

        [Test]
        public void Test_MedioBoleto_PrioridadTransbordo()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Ignore("Test de transbordo ignorado por horario/día no hábil del sistema.");
            }

            MedioBoletoEstudiantil tarjeta = new MedioBoletoEstudiantil();
            tarjeta.CargarTarjeta(5000);

            CambiarFechaUltimoViaje(tarjeta, DateTime.Now.AddHours(-2));
            colectivoUrbano.PagarCon(tarjeta);
            double saldoDespuesPrimerViaje = tarjeta.Saldo;

            CambiarFechaUltimoViaje(tarjeta, DateTime.Now.AddMinutes(-15));

            Colectivo otraLinea = new ColectivoInterurbano("35/9");
            Boleto boletoTransbordo = otraLinea.PagarCon(tarjeta);

            Assert.IsNotNull(boletoTransbordo, "El transbordo debería emitirse.");

            Assert.AreEqual(saldoDespuesPrimerViaje, tarjeta.Saldo, 0.01,
                "El saldo no debería cambiar. El Transbordo (Gratis) tiene prioridad sobre el Medio Boleto.");
        }

        private void CambiarFechaRetiroBici(BoletoMiBiciTuBici boleto, DateTime nuevaFecha)
        {
            var field = typeof(BoletoMiBiciTuBici).GetField("<FechaRetiroBici>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(boleto, nuevaFecha);
            }
            else
            {
                var prop = typeof(BoletoMiBiciTuBici).GetProperty("FechaRetiroBici");
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(boleto, nuevaFecha);
                }
            }
        }

        private void CambiarFechaUltimoViaje(Tarjeta tarjeta, DateTime nuevaFecha)
        {
            tarjeta.fechaUltimoViaje = nuevaFecha;
        }
    }
}