using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Utilidades;
using System.Linq;
using System.Threading;

namespace Abuson.Clases {
    public class Nodo {
        public Direccion DireccionLocal { get; set; }
        public Direccion DireccionSiguiente { get; set; }

        public List<Direccion> Direcciones { get; set; }
        public Direccion DireccionCoordinador { get; set; }
        public Direccion DireccionCoordinadorAnterior { get; set; }
        private Acciones SiguienteAccion { get; set; }
        
        public Nodo() {
            DireccionLocal = new Direccion();
            DireccionSiguiente = new Direccion();
            Direcciones = new List<Direccion>();
            DireccionCoordinador = new Direccion();
        }

        public Nodo(
            Direccion direccionLocal,
            Direccion direccionSiguiente,
            List<Direccion> direcciones,
            Direccion direccionCoordinador
            ) {
            DireccionLocal = direccionLocal;
            DireccionSiguiente = direccionSiguiente;
            Direcciones = direcciones;
            DireccionCoordinador = direccionCoordinador;
    }

        public bool SoyCoordinador() {
            return DireccionCoordinador.ToString() == DireccionLocal.ToString();
        }

        public void Iniciar() {
            if (SoyCoordinador()) {
                Console.WriteLine("\nPresione una tecla para iniciar...");
                Console.ReadKey();
                
                while (true) {
                    ContactarNodoSiguiente();
                    Escuchar();
                    MostrarEstadoCorto();
                }
            } else {
                while (true) {
                    Escuchar();
                    MostrarEstadoCorto();
                    ContactarNodoSiguiente();
                }
            }
        }
        
        private void Escuchar() {
            Funciones.IniciarServidor(DireccionLocal.IP, DireccionLocal.Puerto, ManejarRespuesta);
        }
        
        private string ManejarRespuesta(string mensajeCliente) {
            if (mensajeCliente.Contains("Saludos desde")) {
                // El nodo anterior saluda.
                SiguienteAccion = Acciones.ContactarSiguienteNodo;
                return string.Format("¡Saludos devueltos desde {0}!", DireccionLocal.ToString());
            } else if (mensajeCliente.Contains("proclamo")) {
                // Alguien dice que hay un nuevo coordinador.
                var args = mensajeCliente.Split(',');
                DireccionCoordinador = new Direccion(args[1]);
                Console.WriteLine("El nuevo coordinador es: {0}", DireccionCoordinador.ToString());
                SiguienteAccion = Acciones.Escuchar;
                return "OK";
            } else if (mensajeCliente.Contains("caído")) {
                // Alguien dice que el coordinador ha caído.
                SiguienteAccion = Acciones.Escuchar;
                return "OK";
            } else if (mensajeCliente.Equals("Eres el nuevo coordinador.")) {
                NotificarNuevoCoordinador();
                SiguienteAccion = Acciones.Escuchar;
            }

            return "Mensaje del cliente no entendido.";
        }
        
        private List<Direccion> ObtenerDireccionesMayores() {
            var listaDireccionesMayores = new List<Direccion>();
            Console.WriteLine("Direcciones disponibles: {0}", Funciones.ListToSeparatedString(Direcciones, ','));
            foreach (var dir in Direcciones) {
                if (dir.Puerto > DireccionLocal.Puerto && dir.ToString() != DireccionCoordinador.ToString()) {
                    listaDireccionesMayores.Add(dir);
                    Console.WriteLine("Dirección mayor: {0}", dir.ToString());
                }
            }

            return listaDireccionesMayores;
        }
        
        public string IniciarClienteParcial(string mensajeEnviando, string ip, int puerto) {
            MostrarHora();
            Console.WriteLine("Iniciando cliente hacia: {0}:{1}", ip, puerto);

            // Datos a enviar.
            var texto = mensajeEnviando;

            // Cliente TCP en IP y puerto indicados.
            var cliente = new TcpClient(ip, puerto);
            var stream = cliente.GetStream();
            var bytesEnviando = Encoding.UTF8.GetBytes(texto);

            // Enviando datos.
            Console.WriteLine("Enviando: " + texto);
            stream.Write(bytesEnviando, 0, bytesEnviando.Length);

            // Leyendo respuesta del servidor.
            var bytesLeyendo = new byte[cliente.ReceiveBufferSize];
            var bytesLeidos = stream.Read(bytesLeyendo, 0, cliente.ReceiveBufferSize);
            var mensajeRecibido = Encoding.UTF8.GetString(bytesLeyendo, 0, bytesLeidos);
            Console.WriteLine("Recibido: " + mensajeRecibido);
            
            // Cerrando cliente.
            cliente.Close();
            Console.WriteLine("Cliente detenido.");

            return mensajeRecibido;
        }
        
        private void ContactarNodoSiguiente() {
            MostrarHora();
            Console.WriteLine("Contactando siguiente nodo...");
            Esperar();
            var mensaje = string.Format("¡Saludos desde {0}!", DireccionLocal.ToString());
            try {
                var respuesta = IniciarClienteParcial(mensaje, DireccionSiguiente.IP, DireccionSiguiente.Puerto);
            } catch (Exception) {
                Console.WriteLine("El nodo está caído.");
                Direcciones.Remove(DireccionSiguiente);
                /*
                var stackTrace = Funciones.ObtenerMensajesDeExcepcion(e);
                var mensajeError = string.Format("Error al contactar siguiente nodo. Detalles:\n{0}", stackTrace);
                Console.WriteLine(mensajeError);
                */

                if (DireccionSiguiente.ToString() == DireccionCoordinador.ToString()) {
                    ElegirCoordinador();
                } else {
                    var direccionCaida = DireccionSiguiente;
                    ActualizarSiguienteNodo();

                    if (direccionCaida.ToString() == DireccionSiguiente.ToString()) {
                        Console.WriteLine("Ups, no quedan más nodos disponibles.");
                        DireccionCoordinadorAnterior = DireccionCoordinador;
                        DireccionCoordinador = DireccionLocal;
                        MostrarEstadoCorto();
                        Console.WriteLine("\nPresione una tecla para finalizar...");
                        Console.ReadKey();
                        Environment.Exit(0);
                    }

                    ContactarNodoSiguiente();
                }
            }
            Console.WriteLine("Contacto finalizado.");
        }

        private void ElegirCoordinador() {
            MostrarHora();
            Console.WriteLine("Eligiendo coordinador...");
            // Obtener direcciones mayores al nodo actual.
            var direccionesNodosMayores = ObtenerDireccionesMayores();
            var direccionesDisponibles = new List<Direccion>();

            Console.WriteLine("\nDirecciones mayores a la local:\n", Funciones.ListToSeparatedString(direccionesNodosMayores, ','));

            if (direccionesNodosMayores.Count == 0) {
                Console.WriteLine("Soy coordinador.");
                DireccionCoordinadorAnterior = DireccionCoordinador;
                DireccionCoordinador = DireccionLocal;
                NotificarNuevoCoordinador();
                return;
            }

            // Comunicar una a una que el coordinador cayó y que deben reportar su estado.
            foreach (var direccionNodoMayor in direccionesNodosMayores) {
                Console.WriteLine("\nContactando a {0}...", direccionNodoMayor.ToString());
                var mensaje = string.Format("El coordinador, {0}, ha caído.", DireccionCoordinador);
                var estadoNodoMayor = IniciarClienteParcial(mensaje, direccionNodoMayor.IP, direccionNodoMayor.Puerto);

                if (estadoNodoMayor == "OK") {
                    Console.WriteLine("Nodo disponible.");
                    direccionesDisponibles.Add(direccionNodoMayor);
                }

                var DireccionMayorDisponible = ObtenerDireccionMayor(direccionesDisponibles);
                DireccionCoordinadorAnterior = DireccionCoordinador;
                DireccionCoordinador = DireccionMayorDisponible;
                Console.WriteLine("El nuevo coordinador es: {0}", DireccionCoordinador.ToString());

                IniciarClienteParcial("Eres el nuevo coordinador.", DireccionCoordinador.IP, DireccionCoordinador.Puerto);
            }
        }

        private Direccion ObtenerDireccionMayor(List<Direccion> lista) {
            var mayor = lista.First();

            foreach (var dir in lista) {
                if (dir.Puerto > mayor.Puerto) {
                    mayor = dir;
                }
            }

            return mayor;
        }

        private void NotificarNuevoCoordinador() {
            MostrarHora();
            Console.WriteLine("Notificando que soy el nuevo coordinador supremo...");

            foreach (var dir in Direcciones) {
                if (dir.ToString() != DireccionLocal.ToString() && dir.ToString() != DireccionCoordinadorAnterior.ToString()) {
                    var mensaje = string.Format("Yo, {0}, me proclamo coordinador.", DireccionLocal);
                    IniciarClienteParcial(mensaje, dir.IP, dir.Puerto);
                }
            }
        }

        private void ActualizarSiguienteNodo() {
            MostrarHora();
            Console.WriteLine("Actualizando siguiente nodo...");
            Console.WriteLine("Direcciones disponibles: {0}", Funciones.ListToSeparatedString(Direcciones, ','));
            for (var i = 0; i < Direcciones.Count; i++) {
                if (Direcciones[i].ToString() == DireccionSiguiente.ToString()) {
                    if (Direcciones[i].ToString() == Direcciones.Last().ToString()) {
                        DireccionSiguiente = Direcciones.First();
                        goto Fin;
                    } else {
                        DireccionSiguiente = Direcciones[i + 1];
                        goto Fin;
                    }
                }
            }

            Fin:
            Console.WriteLine("Siguiente dirección actualizada: {0}", DireccionSiguiente.ToString());
        }

        private void EnviarMensaje(string mensajeEnviando, string ip, int puerto) {
            MostrarHora();
            Console.WriteLine("Iniciando cliente hacia: {0}:{1}", ip, puerto);

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

            // Cerrando cliente.
            cliente.Close();
            Console.WriteLine("Cliente detenido.");
        }

        public void MostrarEstado() {
            Console.WriteLine(ToString());
        }

        public void MostrarEstadoCorto() {
            var separador = "---------------";
            var estado = string.Format("\nEstado:\n{0}\nSoy coordinador: {1}\nDireccion coordinador: {2}\n{3}", separador, SoyCoordinador(), DireccionCoordinador.ToString(), separador);
            Console.WriteLine(estado);
        }

        public override string ToString() {
            var direcciones = "";
            var orden = "";

            for (var i = 0; i < Direcciones.Count; i++) {
                var direccionActual = Direcciones[i];

                if (i == 0) {
                    orden = direccionActual.Puerto.ToString();
                } else {
                    orden += " -> " + direccionActual.Puerto;
                }

                if (direccionActual != DireccionLocal) {
                    if (i == 0) {
                        direcciones = direccionActual.ToString();
                    } else {
                        direcciones += " | " + direccionActual.ToString();
                    }
                }
            }

            var separador = "---------------";
            var estado = string.Format("\nEstado:\n{0}\nSoy coordinador: {1}\nDireccion coordinador: {2}\nDireccion local: {3}\nDemás direcciones: {4}\nOrden: {5}\n{6}", separador, SoyCoordinador(), DireccionCoordinador.ToString(), DireccionLocal.ToString(), direcciones, orden, separador);

            return estado;
        }

        public void Esperar() {
            var segundos = 3;
            Console.WriteLine("Esperando {0} segundos...", segundos);
            Thread.Sleep(1000 * segundos);
        }

        public void MostrarHora() {
            Console.WriteLine("\n[{0}]", DateTime.Now.ToLongTimeString());
        }
    }

    public enum Acciones {
        ContactarSiguienteNodo,
        Escuchar
    }
}