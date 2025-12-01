namespace TrabajoTarjeta
{
    public class ColectivoUrbano : Colectivo
    {
        public ColectivoUrbano(string linea) : base(linea) { }

        public override double Tarifa => 1580;
    }
}