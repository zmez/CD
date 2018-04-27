using Abuson.Clases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Utilidades;

namespace Abuson {
    class Program {
        static List<Nodo> ListaNodos;
        static Nodo NodoLocal;

        static void Main(string[] args) {
            if (!ManejarArgumentos(args)) {
                Console.WriteLine("Iniciando en modo automático...");
                Automatizar();
            } else {
                NodoLocal.Iniciar();
            }

            /*
            Console.WriteLine("\nPresione una tecla para finalizar...");
            Console.ReadKey();
            */
        }

        private static void Automatizar() {
            GenerarNodos(3);
            LevantarNodos();
        }

        private static void GenerarNodos(int cantidad) {
            Console.WriteLine("\nGenerando nodos...");

            var listaDirecciones = new List<Direccion>();
            var listaNodos = new List<Nodo>();
            var ipDirecciones = Funciones.ObtenerIPDesdeInterfaz(NetworkInterfaceType.Loopback);
            var puertoInicial = 8081;

            for (var i = 0; i < cantidad; i++) {
                var directorio = new Direccion(ipDirecciones, puertoInicial + i);
                listaDirecciones.Add(directorio);
            }

            for (var i = 0; i < cantidad; i++) {
                var dirActual = listaDirecciones[i];
                Direccion dirSiguiente = null;

                if (i + 1 == cantidad) {
                    dirSiguiente = listaDirecciones[0];
                } else {
                    dirSiguiente = listaDirecciones[i + 1];
                }

                var nodo = new Nodo() {
                    ID = i + 1,
                    DireccionLocal = dirActual,
                    DireccionSiguiente = dirSiguiente,
                    DireccionCoordinador = listaDirecciones[0],
                    Direcciones = listaDirecciones
                };

                listaNodos.Add(nodo);
            }
            
            ListaNodos = listaNodos;
            Console.WriteLine("{0} nodos generados.", ListaNodos.Count);
        }
        
        private static void LevantarNodos() {
            Console.WriteLine("\nLenvantando {0} nodos...", ListaNodos.Count);
            
            var args = string.Empty;
            
            for (var i = 0; i < ListaNodos.Count; i++) {
                Console.WriteLine("\nLevantando nodo {0}...", i + 1);

                var nodo = ListaNodos[i];

                args = Funciones.UnirArgumentosCMD(
                    nodo.ID.ToString(),
                    nodo.DireccionLocal.ToString(),
                    nodo.DireccionSiguiente.ToString(),
                    nodo.DireccionCoordinador.ToString(),
                    Funciones.ListToSeparatedString(nodo.Direcciones, '|')
                );
                Process.Start("Abuson.exe", args);

                Console.WriteLine("Nodo levantado.");
            }

            Console.WriteLine("Nodos listos.");
        }

        private static bool ManejarArgumentos(string[] args) {
            if (args.Length == 5) {
                Console.WriteLine("Argumentos recibidos.\nIniciando como nodo local...");

                Console.WriteLine("\nMostrando argumentos");
                for (var i = 0; i < args.Length; i++) {
                    Console.WriteLine("{0}> {1}", i, args[i]);
                }
                Console.WriteLine();

                var direccionesString = args[4].Split('|');
                var listaDirecciones = new List<Direccion>();

                foreach (var dirString in direccionesString) {
                    var dir = new Direccion(dirString);
                    listaDirecciones.Add(dir);
                }

                NodoLocal = new Nodo {
                    ID = int.Parse(args[0]),
                    DireccionLocal = new Direccion(args[1]),
                    DireccionSiguiente = new Direccion(args[2]),
                    DireccionCoordinador = new Direccion(args[3]),
                    Direcciones = listaDirecciones
                };

                Console.WriteLine("Nodo local creado:\n" + NodoLocal);
                return true;
            }

            Console.WriteLine("\nArgumentos incorrectos:");
            for (var i = 0; i < args.Length; i++) {
                Console.WriteLine("{0}> {1}", i, args[i]);
            }

            Console.WriteLine("\nPresione una tecla para finalizar...");
            Console.ReadKey();
            Environment.Exit(0);

            return false;
        }
    }
}
