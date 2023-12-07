using Entidades.Exceptions;
using Entidades.Interfaces;
using Entidades.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entidades.Files
{
    
    public static class FileManager
    {
        private static string path;
        
        static FileManager()
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);//toma la carpeta en escritorio
            path = Path.Combine(path, "carpetdondeseguarda");

            ValidarExistenciaDeDirectorio();
        }
        
        public static void Guardar(string data, string nombreArchivo, bool append)
        {
            try
            {
                string rutaCompleta = Path.Combine(path, nombreArchivo);
                // Comprobamos si se debe agregar información o sobrescribir el archivo
                if (append)
                {
                   
                    // Si se debe agregar información, abrimos el archivo en modo Append
                    using (StreamWriter file = new StreamWriter(rutaCompleta, true))
                    {
                        file.WriteLine(data);
                        
                    }
                }
                else
                {
                    // Si se debe sobrescribir el archivo, escribimos la información
                    // Sobrescribiendo el archivo si ya existe
                    File.WriteAllText(rutaCompleta, data);
                }

                //Console.WriteLine("¡Datos guardados correctamente!");
            }
            catch (Exception ex)
            {
                string msgError = "Error al guardar los datos: ";
                FileManager.Guardar(msgError, "logs.txt", true);
                Console.WriteLine(msgError + ex.ToString());
                throw new FileManagerException(msgError, ex);
            }
            
        }

        public static bool Serializar<T>(T elemento, string nombreArchivo) where T : class
        {
            try
            {
                string pathCompleto = Path.Combine(path, nombreArchivo);

                string json = JsonSerializer.Serialize(elemento);
                
                File.WriteAllText(pathCompleto, json);
                
            }
            catch (Exception ex)
            {
                string msgError = "Error al serializar los datos: ";
                FileManager.Guardar(msgError, "logs.txt", true);
                Console.WriteLine(msgError + ex.ToString());
            }
            return true;
        }
        private static void ValidarExistenciaDeDirectorio()
        {
            
            //si no existe directorio en path se creará
            if(!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch(FileManagerException ex)
                {
                    string msgError = "Error al crear directorio ";
                    FileManager.Guardar(msgError, "logs.txt", true);
                    throw new FileManagerException(msgError, ex);

                }
                
            }
            
            
        }

    }
}
