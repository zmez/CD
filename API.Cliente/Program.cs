using System;
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
            Console.WriteLine("Programa cliente iniciado.");

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

            Console.WriteLine("Presione una tecla para finalizar...");
            Console.ReadKey();
        }

        private static string EnviarMensaje(string mensajeEnviando, string ip, int puerto) {
            Console.WriteLine("\nIniciando cliente hacia: {0}:{1}", ip, puerto);

            // Datos a enviar.
            var texto = mensajeEnviando;

            // Cliente TCP en IP y puerto indicados.
            var cliente = new TcpClient(ip, puerto);
            var stream = cliente.GetStream();
            var bytesEnviando = Encoding.UTF8.GetBytes(texto);

            // Enviando datos.
            Console.WriteLine("Enviando: " + texto);
            stream.Write(bytesEnviando, 0, bytesEnviando.Length);
            Console.WriteLine("¡Mensaje enviado!");

            // Leyendo respuesta del servidor.
            var bytesLeyendo = new byte[cliente.ReceiveBufferSize];
            var bytesLeidos = stream.Read(bytesLeyendo, 0, cliente.ReceiveBufferSize);
            var mensajeRecibido = Encoding.UTF8.GetString(bytesLeyendo, 0, bytesLeidos);

            // Cerrando cliente.
            cliente.Close();
            Console.WriteLine("Cliente detenido.");

            return mensajeRecibido;
        }

        private static void PruebaAgregarLibro() {
            Console.WriteLine("\n> Prueba: Agregar libro.");
            Funcion = "AgregarLibro";
            FuncionArgs = "LibroTest";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Console.WriteLine("Respuesta: {0}", respuesta);

            Console.WriteLine("-- Fin --");
        }

        private static void PruebaAgregarAutor() {
            Console.WriteLine("\n> Prueba: Agregar autor.");
            Funcion = "AgregarAutor";
            FuncionArgs = "AutorTest";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Console.WriteLine("Respuesta: {0}", respuesta);

            Console.WriteLine("-- Fin --");
        }
        
        private static void PruebaObtenerLibro() {
            Console.WriteLine("\n> Prueba: Obtener libro.");
            Funcion = "ObtenerLibro";
            FuncionArgs = "0";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Console.WriteLine("Respuesta: {0}", respuesta);
            Console.WriteLine("-- Fin --");
        }

        private static void PruebaObtenerAutor() {
            Console.WriteLine("\n> Prueba: Obtener autor.");
            Funcion = "ObtenerAutor";
            FuncionArgs = "0";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Console.WriteLine("Respuesta: {0}", respuesta);
            Console.WriteLine("-- Fin --");
        }

        private static void PruebaEliminarLibro() {
            Console.WriteLine("\n> Prueba: Eliminar libro.");
            Funcion = "EliminarLibro";
            FuncionArgs = "LibroTestActualizado";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Console.WriteLine("Respuesta: {0}", respuesta);

            Console.WriteLine("-- Fin --");
        }

        private static void PruebaEliminarAutor() {
            Console.WriteLine("\n> Prueba: Eliminar autor.");
            Funcion = "EliminarAutor";
            FuncionArgs = "AutorTestActualizado";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Console.WriteLine("Respuesta: {0}", respuesta);

            Console.WriteLine("-- Fin --");
        }

        private static void PruebaActualizarLibro() {
            Console.WriteLine("\n> Prueba: Actualizar libro.");
            Funcion = "ActualizarLibro";
            FuncionArgs = "LibroTest;LibroTestActualizado";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Console.WriteLine("Respuesta: {0}", respuesta);

            Console.WriteLine("-- Fin --");
        }

        private static void PruebaActualizarAutor() {
            Console.WriteLine("\n> Prueba: Actualizar autor.");
            Funcion = "ActualizarAutor";
            FuncionArgs = "AutorTest;AutorTestActualizado";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Console.WriteLine("Respuesta: {0}", respuesta);

            Console.WriteLine("-- Fin --");
        }
        
        private static void PruebaObtenerLibrosDeAutor() {
            Console.WriteLine("\n> Prueba: Obtener libros de autor.");
            Funcion = "ObtenerLibrosDeAutor";
            FuncionArgs = "autor1";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Console.WriteLine("Respuesta: {0}", respuesta);
            Console.WriteLine("-- Fin --");
        }

        private static void PruebaObtenerAutoresDeLibro() {
            Console.WriteLine("\n> Prueba: Obtener autores de libro.");
            Funcion = "ObtenerAutoresDeLibro";
            FuncionArgs = "libro2";

            MensajeEnviando = string.Format("{0}|{1},{2}|{3}", Usuario, Clave, Funcion, FuncionArgs);

            var respuesta = EnviarMensaje(MensajeEnviando, IP, Puerto);
            Console.WriteLine("Respuesta: {0}", respuesta);
            Console.WriteLine("-- Fin --");
        }
    }
}
