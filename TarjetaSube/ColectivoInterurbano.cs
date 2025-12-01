namespace TrabajoTarjeta
{
    public class ColectivoInterurbano : Colectivo
    {
        public ColectivoInterurbano(string linea) : base(linea) { }

        public override double Tarifa => 3000;
    }
}