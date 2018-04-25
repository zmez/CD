using API.Servidor.Clases;
using System;
using Utilidades;

namespace API.Servidor {
    class Program {
        const string Directorio = "BD";
        static Tabla TablaLibros = new Tabla("Libros", Directorio);
        static Tabla TablaAutores = new Tabla("Autores", Directorio);
        static TablaLibroAutor TablaLibroAutor = new TablaLibroAutor("LibroAutor", Directorio, TablaLibros, TablaAutores);
        static TablaUsuarios TablaUsuarios = new TablaUsuarios("Usuarios", Directorio);

        static string IP;

        static void Main(string[] args) {
            Console.WriteLine("Iniciando API en modo de servicio...");

            try {
                IP = Funciones.SeleccionarIPLocal();

                while (true) {
                    IniciarServidor();
                }
            } catch (Exception e) {
                Console.WriteLine(Funciones.ObtenerMensajesDeExcepcion(e));

                throw e;
            }
        }
        
        private static void IniciarServidor() {
            Funciones.IniciarServidor(IP, Constantes.Puerto, ManejarMensajeCliente);
        }

        private static string ManejarMensajeCliente(string mensaje) {
            var partes = mensaje.Split(',');
            if (partes.Length == 2) {
                var usuarioClave = partes[0];
                var funcionArgs = partes[1].Split('|');
                var funcion = funcionArgs[0];
                var args = funcionArgs[1].Split(';');

                Console.WriteLine("Validando usuario y clave: {0}", usuarioClave);
                if (TablaUsuarios.RegistroExiste(usuarioClave)) {
                    Console.WriteLine("El usuario \"{0}\" es válido.", usuarioClave.Split('|')[0]);

                    switch (funcion) {
                        case "AgregarLibro":
                            return AgregarLibro(args[0]);
                        case "AgregarAutor":
                            return AgregarAutor(args[0]);
                        case "EliminarLibro":
                            return EliminarLibro(args[0]);
                        case "EliminarAutor":
                            return EliminarAutor(args[0]);
                        case "ActualizarLibro":
                            return ActualizarLibro(args[0], args[1]);
                        case "ActualizarAutor":
                            return ActualizarAutor(args[0], args[1]);
                        case "ObtenerLibro":
                            return ObtenerLibro(int.Parse(args[0]));
                        case "ObtenerAutor":
                            return ObtenerAutor(int.Parse(args[0]));
                        case "ObtenerLibrosDeAutor":
                            return ObtenerLibrosDeAutor(args[0]);
                        case "ObtenerAutoresDeLibro":
                            return ObtenerAutoresDeLibro(args[0]);
                    }
                }

                var resultado = "El usuario o clave no son válidos.";
                Console.WriteLine(resultado);
                return resultado;
            }
            
            return "Argumentos no válidos.";
        }

        private static string AgregarLibro(string nombre) {
            TablaLibros.AgregarRegistro(nombre);
            return string.Format("Libro \"{0}\" agregado.", nombre);
        }

        private static string AgregarAutor(string nombre) {
            TablaAutores.AgregarRegistro(nombre);
            return string.Format("Autor \"{0}\" agregado.", nombre);
        }

        private static string ObtenerLibro(int indice) {
            if (TablaLibros.Registros.Count >= indice) {
                return TablaLibros.Registros[indice];
            } else {
                return "El libro no existe.";
            }
        }

        private static string ObtenerAutor(int indice) {
            if (TablaAutores.Registros.Count >= indice) {
                return TablaAutores.Registros[indice];
            } else {
                return "El autor no existe.";
            }
        }

        private static string EliminarLibro(string nombre) {
            try {
                TablaLibros.EliminarRegistro(nombre);
                return string.Format("Libro \"{0}\" eliminado.", nombre);
            } catch (NullReferenceException) {
                return "El libro no existe.";
            }
        }

        private static string EliminarAutor(string nombre) {
            try {
                TablaAutores.EliminarRegistro(nombre);
                return string.Format("Autor \"{0}\" eliminado.", nombre);
            } catch (NullReferenceException) {
                return "El autor no existe.";
            }
        }

        private static string ActualizarLibro(string nombreActual, string nombreNuevo) {
            try {
                TablaLibros.ActualizarRegistro(nombreActual, nombreNuevo);
                return string.Format("Libro \"{0}\" ahora se llama \"{1}\".", nombreActual, nombreNuevo);
            } catch (NullReferenceException) {
                return "El libro no existe.";
            }
        }

        private static string ActualizarAutor(string nombreActual, string nombreNuevo) {
            try {
                TablaAutores.ActualizarRegistro(nombreActual, nombreNuevo);
                return string.Format("Autor \"{0}\" ahora se llama \"{1}\".", nombreActual, nombreNuevo);
            } catch (NullReferenceException) {
                return "El autor no existe.";
            }
        }

        private static string ObtenerLibrosDeAutor(string nombreAutor) {
            try {
                var libros = TablaLibroAutor.ObtenerLibrosDeAutor(nombreAutor);
                return Funciones.ListToSeparatedString(libros, ',');
            } catch (NullReferenceException) {
                return "El autor no existe.";
            }
        }

        private static string ObtenerAutoresDeLibro(string nombreLibro) {
            try {
                var autores = TablaLibroAutor.ObtenerAutoresDeLibro(nombreLibro);
                return Funciones.ListToSeparatedString(autores, ',');
            } catch (NullReferenceException) {
                return "El libro no existe.";
            }
        }
    }
}
