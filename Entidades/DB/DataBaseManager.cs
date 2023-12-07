using System.Data.SqlClient;
using Entidades.Excepciones;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;

namespace Entidades.DataBase
{
    public static class DataBaseManager
    {
        private static SqlConnection connection;
        private static string stringConnection;

        static DataBaseManager() 
        {
            DataBaseManager.stringConnection = "Server = DESKTOP-8A3M14H; Database = 20230622SP; Trusted_Connection = True";
        }
        public static string GetImagenComida(string tipo)
        {
            try
            {
                using (connection = new SqlConnection(stringConnection))
                {
                    string query = $"SELECT * FROM comidas WHERE tipo_comida = @tipo";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@tipo", tipo);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new ComidaInvalidaExeption("No se encontro comida");
                    }
                    if (reader.Read())

                        return reader.GetString(2);
                }
                throw new ComidaInvalidaExeption("No se encontro comida");

            }
            catch (ComidaInvalidaExeption ex)
            {
                FileManager.Guardar(ex.Message, "logs.txt", false);
                throw ex;
            }
            //catch (Exception)
            //{
            //    string mensajeError = "Error al leer de la base de datos";
            //    FileManager.Guardar(mensajeError, "logs.txt", false);
            //    throw new DataBaseManagerException(mensajeError);
            //}
            //try
            //{
            //    using (connection = new SqlConnection(stringConnection))
            //    {
            //        string query = $"SELECT * FROM comidas WHERE tipo_comida = @tipo";

            //        SqlCommand command = new SqlCommand(query, connection);
            //        command.Parameters.AddWithValue("@tipo", tipo);
            //        connection.Open();
            //        SqlDataReader reader = command.ExecuteReader();
            //        //columna 2
            //        return reader.GetString(2);
            //    }
            //}
            //catch(Exception ex)
            //{
            //    throw new ComidaInvalidaExeption("msgcomidaZexcept");
            //}
        }

        public static bool GuardarTicket<T>(string nombreEmpleado, T comida) where T : IComestible, new() 
        {
            try
            {
                using (connection = new SqlConnection(stringConnection))
                {
                    string query = $"INSERT INTO tickets (empleado,ticket) VALUES (@empleado,@ticket)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@empleado", nombreEmpleado);
                    command.Parameters.AddWithValue("@ticket", comida.Ticket);
                    connection.Open(); // Abrir la conexión a la base de datos
                    command.ExecuteNonQuery(); // Ejecutar el comando SQL para insertar el ticket
                    
                    return false; // Devolver true si la operación fue exitosa
                }
                
            }
            catch (Exception ex)
            {
                string mensajeOperacion = "escribir";
                
                if (ex is SqlException)
                {
                    mensajeOperacion = "leer";
                }

                string mensajeError = $"Error al intentar {mensajeOperacion} el ticket: {ex.Message}";
                throw new DataBaseManagerException(mensajeError, ex);
            }
        }
        


    }
}
