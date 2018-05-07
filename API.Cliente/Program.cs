using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Utilidades;

namespace API.Cliente {
    class Program {
        static string Usuario = "Juana";
        static string Clave = "laloca";
        static string Funcion;
        static string FuncionArgs;
        static string MensajeEnviando;
        static string IP;
        static int Puerto;

        static void Main(string[] args) {
            Notificar("Programa cliente iniciado.");

            IP = Funciones.SeleccionarIPLocal();
            Puerto = Constantes.Puerto;

            PruebaAgregarLibro();
            PruebaAgregarAutor();

            PruebaObtenerLibro();
            PruebaObtenerAutor();

            PruebaActualizarLibro();
            PruebaActualizarAutor();

            PruebaEliminarLibro();
            PruebaEliminarAutor();

            PruebaObtenerLibrosDeAutor();
            PruebaObtenerAutoresDeLibro();

            Console.WriteLine("\nPresione una tecla para finalizar...");
            Console.ReadKey();
        }

        private static void Notificar(string mensaje) {
            Console.WriteLine(string.Format("\n[{0:H:mm:ss.fff}] {1}", DateTime.Now, mensaje));
        }

        private static void Notificar(string formato, params string[] args) {
            var mensaje = string.Format(formato, args);
            Console.WriteLine(string.Format("\n[{0:H:mm:ss.fff}] {1}", DateTime.Now, mensaje));
        }

        private static string EnviarMensaje(string mensajeEnviando, string ip, int puerto) {
            Notificar("Iniciando cliente hacia: {0}:{1}", ip, puerto.ToString());

            // Datos a enviar.
            var texto = mensajeEnviando;

            // Cliente TCP en IP y puerto indicados.
            var cliente = new TcpClient(ip, puerto);
            var stream = cliente.GetStream();
            var bytesEnviando = Encoding.UTF8.GetBytes(texto);

            // Enviando datos.
            Notificar("Enviando mensaje...");
            stream.Write(bytesEnviando, 0, bytesEnviando.Length);
            Notificar("¡Mensaje enviado!");

            // Leyendo respuesta del servidor.
            var bytesLeyendo = new byte[cliente.ReceiveBufferSize];
            var bytesLeidos = stream.Read(bytesLeyendo, 0, cliente.ReceiveBufferSize);
            var mensajeRecibido = Encoding.UTF8.GetString(bytesLeyendo, 0, bytesLeidos);

            // Cerrando cliente.
            cliente.Close();
            Notificar("Cliente detenido.");

            return mensajeRecibido;
        }

        private static void PruebaAgregarLibro() {
            Notificar("Prueba: Agregar libro.");
            Funcion = "AgregarLibro";
            FuncionArgs = "LibroTest";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);
            Notificar("Mensaje a enviar:\n{0}", GenerarJson(MensajeEnviando));

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Notificar("Respuesta:\n\n{0}", respuesta);

            EsperarTecla();
        }

        private static void PruebaAgregarAutor() {
            Notificar("Prueba: Agregar autor.");
            Funcion = "AgregarAutor";
            FuncionArgs = "AutorTest";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);
            Notificar("Mensaje a enviar:\n{0}", GenerarJson(MensajeEnviando));

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Notificar("Respuesta:\n\n{0}", respuesta);

            EsperarTecla();
        }

        private static void PruebaObtenerLibro() {
            Notificar("Prueba: Obtener libro.");
            Funcion = "ObtenerLibro";
            FuncionArgs = "0";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);
            Notificar("Mensaje a enviar:\n{0}", GenerarJson(MensajeEnviando));

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Notificar("Respuesta:\n\n{0}", respuesta);

            EsperarTecla();
        }

        private static void PruebaObtenerAutor() {
            Notificar("Prueba: Obtener autor.");
            Funcion = "ObtenerAutor";
            FuncionArgs = "0";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);
            Notificar("Mensaje a enviar:\n{0}", GenerarJson(MensajeEnviando));

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Notificar("Respuesta:\n\n{0}", respuesta);

            EsperarTecla();
        }

        private static void PruebaEliminarLibro() {
            Notificar("Prueba: Eliminar libro.");
            Funcion = "EliminarLibro";
            FuncionArgs = "LibroTestActualizado";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);
            Notificar("Mensaje a enviar:\n{0}", GenerarJson(MensajeEnviando));

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Notificar("Respuesta:\n\n{0}", respuesta);

            EsperarTecla();
        }

        private static void PruebaEliminarAutor() {
            Notificar("Prueba: Eliminar autor.");
            Funcion = "EliminarAutor";
            FuncionArgs = "AutorTestActualizado";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);
            Notificar("Mensaje a enviar:\n{0}", GenerarJson(MensajeEnviando));

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Notificar("Respuesta:\n\n{0}", respuesta);

            EsperarTecla();
        }

        private static void PruebaActualizarLibro() {
            Notificar("Prueba: Actualizar libro.");
            Funcion = "ActualizarLibro";
            FuncionArgs = "LibroTest;LibroTestActualizado";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);
            Notificar("Mensaje a enviar:\n{0}", GenerarJson(MensajeEnviando));

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Notificar("Respuesta:\n\n{0}", respuesta);

            EsperarTecla();
        }

        private static void PruebaActualizarAutor() {
            Notificar("Prueba: Actualizar autor.");
            Funcion = "ActualizarAutor";
            FuncionArgs = "AutorTest;AutorTestActualizado";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);
            Notificar("Mensaje a enviar:\n{0}", GenerarJson(MensajeEnviando));

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Notificar("Respuesta:\n\n{0}", respuesta);

            EsperarTecla();
        }

        private static void PruebaObtenerLibrosDeAutor() {
            Notificar("Prueba: Obtener libros de autor.");
            Funcion = "ObtenerLibrosDeAutor";
            FuncionArgs = "autor1";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);
            Notificar("Mensaje a enviar:\n{0}", GenerarJson(MensajeEnviando));

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Notificar("Respuesta:\n\n{0}", respuesta);

            EsperarTecla();
        }

        private static void PruebaObtenerAutoresDeLibro() {
            Notificar("Prueba: Obtener autores de libro.");
            Funcion = "ObtenerAutoresDeLibro";
            FuncionArgs = "libro2";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);
            Notificar("Mensaje a enviar:\n{0}", GenerarJson(MensajeEnviando));

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Notificar("Respuesta:\n\n{0}", respuesta);

            EsperarTecla();
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

        private static void EsperarTecla() {
            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
