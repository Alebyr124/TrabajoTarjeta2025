namespace TrabajoTarjeta
{
    public class Colectivo
    {
        protected string linea;

        public Colectivo(string linea)
        {
            this.linea = linea;
        }

        public string Linea => linea;
        public virtual double Tarifa { get; }

        public Boleto PagarCon(Tarjeta tarjeta)
        {
            double? montoPagado = tarjeta.Pagar(Tarifa, Linea);

            if (montoPagado.HasValue)
            {
                return new Boleto(Linea, tarjeta);
            }

            return null;
        }
    }
}