using System;
using System.Reflection;
using TarjetaSube;

namespace TrabajoTarjeta.Tests
{
    public static class TestUtilities
    {
        // Método para modificar campos privados (como 'viajesHoy' o 'cantidadViajes')
        public static void SetPrivateField(object obj, string fieldName, object value)
        {
            var type = obj.GetType();
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            // Si no lo encuentra en la clase actual, busca en la clase base
            while (field == null && type.BaseType != null)
            {
                type = type.BaseType;
                field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }

            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                throw new Exception($"No se encontró el campo '{fieldName}' en el objeto {obj.GetType().Name}");
            }
        }

        // Método específico para cambiar la fecha de retiro de la bici (que suele ser readonly/propiedad)
        public static void CambiarFechaRetiroBici(BoletoMiBiciTuBici boleto, DateTime nuevaFecha)
        {
            var field = typeof(BoletoMiBiciTuBici).GetField("<FechaRetiroBici>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(boleto, nuevaFecha);
            }
            else
            {
                // Fallback por si cambia la implementación
                var prop = typeof(BoletoMiBiciTuBici).GetProperty("FechaRetiroBici");
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(boleto, nuevaFecha);
                }
            }
        }
    }
}