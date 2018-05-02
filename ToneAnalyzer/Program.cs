using IBM.WatsonDeveloperCloud.ToneAnalyzer.v3;
using IBM.WatsonDeveloperCloud.ToneAnalyzer.v3.Model;
using System;
using System.IO;

namespace ToneAnalyser {
    public class Program {
        static string Texto = null;

        public static void Main(string[] args) {
            if (args.Length == 1) {
                Texto = args[0];
                Iniciar();
            }
            
            Console.WriteLine("\nPresione una tecla para finalizar...");
            Console.ReadKey();
        }
        
        private static void Notificar(string mensaje) {
            Console.WriteLine(string.Format("\n[{0:H:mm:ss}] {1}", DateTime.Now, mensaje));
        }

        private static void Notificar(string formato, params string[] args) {
            var mensaje = string.Format(formato, args);
            Console.WriteLine(string.Format("\n[{0:H:mm:ss}] {1}", DateTime.Now, mensaje));
        }

        private static Tuple<string, string> ObtenerCredenciales() {
            var credenciales = File.ReadAllText("Credenciales.txt").Split(',');
            return new Tuple<string, string>(credenciales[0], credenciales[1]);
        }

        private static void Iniciar() {
            Notificar("Tone Analyzer iniciado.");
            Analizar();
            Notificar("Tone Analyzer finalizado.");
        }

        private static void Analizar() {
            #region SetUp
            var credentials = string.Empty;
            const string _url = "https://gateway.watsonplatform.net/tone-analyzer/api";

            var usernameAndPassword = ObtenerCredenciales();
            var _username = usernameAndPassword.Item1;
            var _password = usernameAndPassword.Item2;

            const string versionDate = "2016-05-19";

            var _toneAnalyzer = new ToneAnalyzerService(_username, _password, versionDate) {
                Endpoint = _url
            };

            Notificar("Servicio iniciado.");
            #endregion

            #region PostTone
            // Test PostTone
            var toneInput = new ToneInput() {
                Text = Texto
            };

            Notificar("Enviando texto. Esperando respuesta de Watson...");
            var postToneResult = _toneAnalyzer.Tone(toneInput, "application/json", null);
            
            Notificar("Respuesta recibida. Procediendo a mostrar el análisis...");
            MostrarAnalisis(postToneResult);
            #endregion
        }

        private static void MostrarAnalisis(ToneAnalysis toneAnalysis) {
            Notificar("Analizando respuesta.");

            Console.WriteLine("Texto: {0}", Texto);
            var documento = toneAnalysis.DocumentTone;

            Console.WriteLine("Categorías:");
            for (var i = 0; i < documento.ToneCategories.Count; i++) {
                var categoria = documento.ToneCategories[i];
                Console.WriteLine("\n{0}. {1}", i + 1, categoria.CategoryName);

                Console.WriteLine("\tTonos:");
                for (var i2 = 0; i2 < categoria.Tones.Count; i2++) {
                    var tono = categoria.Tones[i2];
                    Console.WriteLine("\t{0}. {1}: {2} puntos", i2 + 1, tono.ToneName, tono.Score);
                }
            }
        }
    }
}
