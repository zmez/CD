using API.Servidor.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
            Notificar("Iniciando API en modo de servicio...");

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
        
        private static void Notificar(string mensaje) {
            Console.WriteLine(string.Format("\n[{0:H:mm:ss.fff}] {1}", DateTime.Now, mensaje));
        }

        private static void Notificar(string formato, params string[] args) {
            var mensaje = string.Format(formato, args);
            Console.WriteLine(string.Format("\n[{0:H:mm:ss.fff}] {1}", DateTime.Now, mensaje));
        }

        private static void IniciarServidor() {
            IniciarServidor(IP, Constantes.Puerto, ManejarMensajeCliente);
        }

        public static void IniciarServidor(string ipServidor, int puerto, Func<string, string> funcion) {
            Notificar("Iniciando servidor en: {0}:{1}", ipServidor, puerto.ToString());

            // Escuchando IP en puerto indicado.
            var IPLocal = IPAddress.Parse(ipServidor);
            var escuchando = new TcpListener(IPLocal, puerto);
            Notificar("Escuchando...");
            escuchando.Start();

            // Aceptando conexión entrante.
            var cliente = escuchando.AcceptTcpClient();

            // Obteniendo datos entrantes.
            var stream = cliente.GetStream();
            var buffer = new byte[cliente.ReceiveBufferSize];

            // Leyendo datos.
            var bytesLeidos = stream.Read(buffer, 0, cliente.ReceiveBufferSize);

            Console.Clear();

            // Convirtiendo datos.
            var datosRecibidos = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);
            Notificar("Recibido:\n" + GenerarJson(datosRecibidos));

            // Respondiendo al cliente.
            var respuesta = funcion(datosRecibidos);
            Notificar("Respondiendo:\n\n" + respuesta);
            var bytesRespuesta = Encoding.UTF8.GetBytes(respuesta);
            stream.Write(bytesRespuesta, 0, bytesRespuesta.Length);

            // Cerrando servidor.
            cliente.Close();
            escuchando.Stop();
            Notificar("Servidor detenido.");
        }

        private static string ManejarMensajeCliente(string mensaje) {
            var partes = mensaje.Split(',');
            if (partes.Length == 2) {
                var usuarioClave = partes[0];
                var funcionArgs = partes[1].Split('|');
                var funcion = funcionArgs[0];
                var args = funcionArgs[1].Split(';');

                Notificar("Validando usuario y clave...");
                Notificar("Tabla actual:\n{0}", TablaUsuarios.ToString());

                if (usuarioClave.Contains("user1") &&
                    funcion.ContainsAny(new string[] { "Agregar", "Eliminar", "Actualizar"})) {
                    var _args = funcionArgs[1].Replace(';', ',');
                    var resultado2 = string.Format("El usuario no posee permisos para modificar la base de datos, por lo que no puede ejecutar:\n{0}({1})", funcion, _args);
                    Notificar(resultado2);
                    return GenerarMensajeJson(resultado2);
                }

                if (TablaUsuarios.RegistroExiste(usuarioClave)) {
                    Notificar("El usuario '{0}' es válido.", usuarioClave.Split('|')[0]);

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
                Notificar(resultado);
                return GenerarMensajeJson(resultado);
            }
            
            return GenerarMensajeJson("Argumentos no válidos.");
        }

        private static string GenerarJson(string mensaje) {
            var partes = mensaje.Split(',');
            var usuarioClave = partes[0].Split('|');
            var usuario = usuarioClave[0];
            var clave = usuarioClave[1];
            var funcionArgs = partes[1].Split('|');
            var funcion = funcionArgs[0];
            var args = funcionArgs[1].Split(';').ToList();

            var _usuario = string.Format("{0}: {1}",
                Funciones.EntrecomillarString("usuario"),
                Funciones.EntrecomillarString(usuario));

            var _clave = string.Format("{0}: {1}",
                Funciones.EntrecomillarString("clave"),
                Funciones.EntrecomillarString(clave));

            var _funcion = string.Format("{0}: {1}",
                Funciones.EntrecomillarString("funcion"),
                Funciones.EntrecomillarString(funcion));

            var _args = ListToArrayJson(args, "args");

            var mensajeJson = string.Format("{{\n\t{0},\n\t{1},\n\t{2},\n\t{3}\n}}", _usuario, _clave, _funcion, _args);

            return mensajeJson;
        }

        private static string GenerarMensajeJson(string contenido) {
            return string.Format("{{\n\t{0}: {1}\n}}",
                Funciones.EntrecomillarString("mensaje"),
                Funciones.EntrecomillarString(contenido));
        }

        private static string GenerarMensajeJson(string cabecera, string contenido) {
            return string.Format("{{\n\t{0}: {1}\n}}",
                Funciones.EntrecomillarString(cabecera),
                Funciones.EntrecomillarString(contenido));
        }

        private static string ListToArrayJson<T>(List<T> lista, string nombreArray) {
            var respuesta = "";

            for (var i = 0; i < lista.Count; i++) {
                var elementoActual = lista[i].ToString();
                if (i == 0) {
                    respuesta = "\t" + Funciones.EntrecomillarString(elementoActual);
                } else {
                    respuesta += ",\n\t" + Funciones.EntrecomillarString(elementoActual);
                }
            }
            
            respuesta = string.Format("{0}: [\n{1}\n\t]",
                Funciones.EntrecomillarString(nombreArray),
                respuesta);

            return respuesta;
        }

        private static string AgregarLibro(string nombre) {
            Notificar("Agregando libro '{0}'...", nombre);
            Notificar("Tabla actual:\n{0}", TablaLibros.ToString());
            TablaLibros.AgregarRegistro(nombre);
            Notificar("Tabla actualizada:\n{0}", TablaLibros.ToString());
            var mensaje = string.Format("Libro '{0}' agregado.", nombre);
            return GenerarMensajeJson(mensaje);
        }

        private static string AgregarAutor(string nombre) {
            Notificar("Agregando autor '{0}'...", nombre);
            Notificar("Tabla actual:\n{0}", TablaAutores.ToString());
            TablaAutores.AgregarRegistro(nombre);
            Notificar("Tabla actualizada:\n{0}", TablaAutores.ToString());
            var mensaje = string.Format("Autor '{0}' agregado.", nombre);
            return GenerarMensajeJson(mensaje);
        }

        private static string ObtenerLibro(int indice) {
            Notificar("Obteniendo libro de índice '{0}'...", indice.ToString());
            Notificar("Tabla actual:\n{0}", TablaLibros.ToString());
            if (TablaLibros.Registros.Count >= indice) {
                return GenerarMensajeJson("libro", TablaLibros.Registros[indice]);
            } else {
                return GenerarMensajeJson(String.Format("No existe el libro de índice '{0}' en la base de datos.", indice));
            }
        }

        private static string ObtenerAutor(int indice) {
            Notificar("Obteniendo autor de índice '{0}'...", indice.ToString());
            Notificar("Tabla actual:\n{0}", TablaAutores.ToString());
            if (TablaAutores.Registros.Count >= indice) {
                return GenerarMensajeJson("autor", TablaAutores.Registros[indice]);
            } else {
                return GenerarMensajeJson(String.Format("No existe el autor de índice '{0}' en la base de datos.", indice));
            }
        }

        private static string EliminarLibro(string nombre) {
            Notificar("Eliminando libro '{0}'...", nombre);
            Notificar("Tabla actual:\n{0}", TablaLibros.ToString());
            try {
                TablaLibros.EliminarRegistro(nombre);
                Notificar("Tabla actualizada:\n{0}", TablaLibros.ToString());
                return GenerarMensajeJson(string.Format("Libro '{0}' eliminado.", nombre));
            } catch (NullReferenceException) {
                return GenerarMensajeJson(string.Format("El libro '{0}' no existe.", nombre));
            }
        }

        private static string EliminarAutor(string nombre) {
            Notificar("Eliminando autor '{0}'...", nombre);
            try {
                Notificar("Tabla actual:\n{0}", TablaAutores.ToString());
                TablaAutores.EliminarRegistro(nombre);
                Notificar("Tabla actualizada:\n{0}", TablaAutores.ToString());
                return GenerarMensajeJson(string.Format("Autor '{0}' eliminado.", nombre));
            } catch (NullReferenceException) {
                return GenerarMensajeJson(String.Format("El autor '{0}' no existe.", nombre));
            }
        }

        private static string ActualizarLibro(string nombreActual, string nombreNuevo) {
            Notificar("Actualizando libro '{0}' a '{1}'...", nombreActual, nombreNuevo);
            Notificar("Tabla actual:\n{0}", TablaLibros.ToString());
            try {
                TablaLibros.ActualizarRegistro(nombreActual, nombreNuevo);
                Notificar("Tabla actualizada:\n{0}", TablaLibros.ToString());
                var mensaje = string.Format("Libro '{0}' ahora se llama '{1}'.", nombreActual, nombreNuevo);
                return GenerarMensajeJson(mensaje);
            } catch (NullReferenceException) {
                return GenerarMensajeJson(string.Format("El libro '{0}' no existe.", nombreActual));
            }
        }

        private static string ActualizarAutor(string nombreActual, string nombreNuevo) {
            Notificar("Actualizando autor '{0}' a '{1}'...", nombreActual, nombreNuevo);
            Notificar("Tabla actual:\n{0}", TablaAutores.ToString());
            try {
                TablaAutores.ActualizarRegistro(nombreActual, nombreNuevo);
                Notificar("Tabla actualizada:\n{0}", TablaAutores.ToString());
                var mensaje = string.Format("Autor '{0}' ahora se llama '{1}'.", nombreActual, nombreNuevo);
                return GenerarMensajeJson(mensaje);
            } catch (NullReferenceException) {
                return GenerarMensajeJson(String.Format("El autor '{0}' no existe.", nombreActual));
            }
        }

        private static string ObtenerLibrosDeAutor(string nombreAutor) {
            Notificar("Obteniendo libros del autor '{0}'...", nombreAutor);
            Notificar("Tabla actual:\n{0}", TablaLibroAutor.ToString());
            try {
                var libros = TablaLibroAutor.ObtenerLibrosDeAutor(nombreAutor);
                
                return string.Format("{{\n\t{0}\n}}", ListToArrayJson(libros, "libros"));
            } catch (NullReferenceException) {
                return GenerarMensajeJson(String.Format("El autor '{0}' no existe.", nombreAutor));
            }
        }

        private static string ObtenerAutoresDeLibro(string nombreLibro) {
            Notificar("Obteniendo autores del libro '{0}'...", nombreLibro);
            Notificar("Tabla actual:\n{0}", TablaLibroAutor.ToString());
            try {
                var autores = TablaLibroAutor.ObtenerAutoresDeLibro(nombreLibro);
                return string.Format("{{\n\t{0}\n}}", ListToArrayJson(autores, "autores"));
            } catch (NullReferenceException) {
                return GenerarMensajeJson(string.Format("El libro '{0}' no existe.", nombreLibro));
            }
        }
    }

    public static class ExtensionMethods {
        public static bool ContainsAny(this string value, string[] stringArray) {
            foreach (var s in stringArray) {
                if (value.Contains(s)) {
                    return true;
                }
            }
            return false;
        }
    }
}
