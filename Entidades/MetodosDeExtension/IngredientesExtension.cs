using Entidades.Enumerados;


namespace Entidades.MetodosDeExtension
{
    public static class IngredientesExtension
    {
        public static double CalcularCostoIngrediente(this List<EIngrediente> ingredientes, int costoInicial)
        {
            double costo = costoInicial;
            foreach (EIngrediente eIngrediente in ingredientes)
            {
                costo += (int)eIngrediente;
            }
            return costo;
        }
        public static List<EIngrediente> IngredientesAleatorios(this Random rand)
        {
            List<EIngrediente> ingredientes = new List<EIngrediente>()
            {
                EIngrediente.QUESO,
                EIngrediente.PANCETA,
                EIngrediente.ADHERESO,
                EIngrediente.HUEVO,
                EIngrediente.JAMON
            };

            int cantidadIngredientes = rand.Next(1, ingredientes.Count + 1);

            return ingredientes.Take(cantidadIngredientes).ToList();

        }







    }
}
