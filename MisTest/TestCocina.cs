using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Modelos;

namespace MisTest
{
    [TestClass]
    public class TestCocina
    {
        [TestMethod]
        [ExpectedException(typeof(FileManagerException))]
        public void AlGuardarUnArchivo_ConNombreInvalido_TengoUnaExcepcion()
        {

            string text = "texto";
            string nombreText = ".QQQQQQQQQ°||'¿";
            bool flag = false;


            FileManager.Guardar(text, nombreText, flag);

            // Assert
            

        }

        [TestMethod]

        public void AlInstanciarUnCocinero_SeEspera_PedidosCero()
        {
            //arrange
            Cocinero<Hamburguesa> masterChef = new Cocinero<Hamburguesa>("QQQ");

            //act

            Assert.AreEqual(0, masterChef.CantPedidosFinalizados);
            //assert
        }
    }
}