using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace Middleware_WPF.Clases {
    public class GoogleDriveService {
        private string[] Scopes = { DriveService.Scope.DriveMetadataReadonly };
        private string ApplicationName;
        private string JsonPath;

        /// <summary>
        /// Servicio utilizando la API v3 de Google Drive.
        /// </summary>
        public DriveService Service { get; set; }

        /// <summary>
        /// Inicializa un servicio con Google Drive.
        /// </summary>
        public GoogleDriveService(string jsonPath, string applicationName) {
            ApplicationName = applicationName;
            JsonPath = jsonPath;

            Service = new DriveService(new BaseClientService.Initializer() {
                HttpClientInitializer = GetCredentials(),
                ApplicationName = ApplicationName,
            });
        }

        private UserCredential GetCredentials() {
            UserCredential credential;

            using (var stream = new FileStream(JsonPath, FileMode.Open, FileAccess.Read)) {
                var credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            return credential;
        }

        /// <summary>
        /// Crea un archivo con los datos almacenados en memoria.
        /// </summary>
        /// <param name="stream">Flujo de datos almacenados en memoria.</param>
        /// <param name="destinyPath">Ruta del archivo de destino.</param>
        public static void SaveStream(MemoryStream stream, string destinyPath) {
            using (var file = new FileStream(destinyPath, FileMode.Create, FileAccess.Write)) {
                stream.WriteTo(file);
            }
        }
    }
}
