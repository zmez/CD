namespace Abuson.Clases {
    public class Direccion {
        public string IP;
        public int Puerto;

        public Direccion() {
            IP = string.Empty;
            Puerto = 0;
        }

        public Direccion(string ip, int puerto) {
            IP = ip;
            Puerto = puerto;
        }

        public Direccion(string dir) {
            var aux = dir.Split(':');
            IP = aux[0];
            Puerto = int.Parse(aux[1]);
        }

        public override string ToString() {
            return string.Format("{0}:{1}", IP, Puerto);
        }

        public override bool Equals(object obj) {
            var dir = obj as Direccion;

            if (dir == null) {
                return false;
            }
            
            return ToString() == dir.ToString();
        }
    }
}
