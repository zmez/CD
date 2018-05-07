using System;
using System.Collections.Generic;
using System.IO;
using Utilidades;

namespace API.Servidor.Clases {
    public class Tabla {
        public string Nombre { get; set; }
        public string Directorio { get; set; }
        public string Ruta { get => Path.Combine(Directorio, Nombre + ".txt"); }
        public virtual List<string> Registros { get; set; }

        public Tabla() {

        }

        public Tabla(string nombre, string directorio) {
            Nombre = nombre;
            Directorio = directorio;
            Registros = new List<string>();
            ActualizarRegistrosLocales();
        }

        public void ActualizarRegistrosLocales() {
            if (File.Exists(Ruta)) {
                Registros = Funciones.StringToList(File.ReadAllText(Ruta), ',');
            } else {
                File.Create(Ruta);
            }
        }

        public void ActualizarRegistrosFisicos() {
            var texto = Funciones.ListToSeparatedString(Registros, ',');
            File.WriteAllText(Ruta, texto);
        }

        public void AgregarRegistro(string registro) {
            if (!string.IsNullOrEmpty(registro) && !string.IsNullOrWhiteSpace(registro)) {
                if (Registros.Contains(registro)) {
                    goto Fin;
                }

                Registros.Add(registro);
            } else {
                throw new InvalidOperationException(
                    string.Format("El valor del registro no puede ser vacío, blanco ni nulo ni estar en los registros.\nRegistro: {0}\nRegistros: {1}", 
                    registro, 
                    Funciones.ListToSeparatedString(Registros, ',')));
            }
            Fin:
            ActualizarRegistrosFisicos();
        }

        public void EliminarRegistro(string registro) {
            if (!string.IsNullOrEmpty(registro) && !string.IsNullOrWhiteSpace(registro) && Registros.Contains(registro)) {
                Registros.Remove(registro);
            } else {
                throw new NullReferenceException(
                    string.Format("El valor del registro no puede ser vacío, blanco ni nulo y debe estar en los registros.\nRegistro: {0}\nRegistros: {1}",
                    registro,
                    Funciones.ListToSeparatedString(Registros, ',')));
            }

            ActualizarRegistrosFisicos();
        }

        public bool RegistroExiste(string registro) {
            return Registros.Contains(registro);
        }

        public void ActualizarRegistro(string registroActual, string registroNuevo) {
            if (RegistroExiste(registroActual)) {
                EliminarRegistro(registroActual);
                AgregarRegistro(registroNuevo);
            } else {
                throw new NullReferenceException(string.Format("El registro \"{0}\" no existe en la tabla \"{1}\".", registroActual, Nombre));
            }
        }

        public override string ToString() {
            var resultado = "Tabla: " + Nombre;

            resultado += "\nÍndice\tRegistro";
            for (var i = 0; i < Registros.Count; i++) {
                var registroActual = Registros[i];
                resultado += string.Format("\n{0}\t{1}", i, registroActual);
            }

            resultado += "\n";

            return resultado;
        }
    }
}
