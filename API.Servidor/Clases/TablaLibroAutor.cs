﻿using System.Collections.Generic;

namespace API.Servidor.Clases {
    public class TablaLibroAutor : Tabla {
        public Tabla TablaLibro { get; set; }
        public Tabla TablaAutor { get; set; }

        public TablaLibroAutor(string nombre, string directorio, Tabla tablaLibro, Tabla tablaAutor) {
            Nombre = nombre;
            Directorio = directorio;
            Registros = new List<string>();
            TablaLibro = tablaLibro;
            TablaAutor = tablaAutor;
            ActualizarRegistrosLocales();
        }
        
        public void Relacionar(string nombreLibro, string nombreAutor) {
            var registro = nombreLibro + "|" + nombreAutor;
            
            if (TablaLibro.RegistroExiste(nombreLibro) && 
                TablaAutor.RegistroExiste(nombreAutor) &&
                !RegistroExiste(registro)) {
                AgregarRegistro(registro);
            }
        }

        public List<string> ObtenerLibrosDeAutor(string nombreAutor) {
            var lista = new List<string>();
            string libro;
            string autor;

            foreach (var registro in Registros) {
                var libroAutor = registro.Split('|');
                libro = libroAutor[0];
                autor = libroAutor[1];

                if (autor == nombreAutor) {
                    lista.Add(libro);
                }
            }

            return lista;
        }

        public List<string> ObtenerAutoresDeLibro(string nombreLibro) {
            var lista = new List<string>();
            string libro;
            string autor;

            foreach (var registro in Registros) {
                var libroAutor = registro.Split('|');
                libro = libroAutor[0];
                autor = libroAutor[1];

                if (libro == nombreLibro) {
                    lista.Add(autor);
                }
            }

            return lista;
        }

        public override string ToString() {
            var resultado = "Tabla: " + Nombre;

            resultado += "\nÍndice\tLibro\tAutor";
            for (var i = 0; i < Registros.Count; i++) {
                var registroActual = Registros[i].Split('|');
                var libro = registroActual[0];
                var autor = registroActual[1];

                resultado += string.Format("\n{0}\t{1}\t{2}",
                    i,
                    libro,
                    autor);
            }

            resultado += "\n";

            return resultado;
        }
    }
}
