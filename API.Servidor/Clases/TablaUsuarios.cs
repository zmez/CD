using System.Collections.Generic;

namespace API.Servidor.Clases {
    public class TablaUsuarios : Tabla {
        public TablaUsuarios(string nombre, string directorio) {
            Nombre = nombre;
            Directorio = directorio;
            Registros = new List<string>();
            ActualizarRegistrosLocales();
        }
        
        public void AgregarUsuario(string nombre, string clave) {
            if (!UsuarioExiste(nombre)) {
                AgregarRegistro(nombre + "|" + clave);
            }
        }
        
        public void EliminarUsuario(string nombre) {
            var registro = "";

            foreach (var r in Registros) {
                if (r.Contains(nombre)) {
                    registro = r;
                }
            }
            
            EliminarRegistro(registro);
        }

        public bool UsuarioExiste(string nombre) {
            foreach (var registro in Registros) {
                if (registro.Substring(0, nombre.Length) == nombre) {
                    return true;
                }
            }

            return false;
        }

        public bool UsuarioClaveValidos(string usuario, string clave) {
            var registro = usuario + "," + clave;
            if (RegistroExiste(registro)) {
                return true;
            }

            return false;
        }
    }
}
