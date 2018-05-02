using Google.Apis.Download;
using Middleware_WPF.Clases;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Utilidades;

namespace Middleware_WPF {
    public partial class MainWindow : Window {
        const string EnlaceArchivoDrive = @"https://drive.google.com/open?id=1hld-ETknmoM-wjXVhqjoVHFgixBsP6lb";
        const string NombreArchivoDrive = "Datos.txt";
        private MemoryStream Stream;
        string RutaLocal = AppDomain.CurrentDomain.BaseDirectory;
        string Texto = null;

        public MainWindow() {
            InitializeComponent();

            Notificar("Programa iniciado.");

            DescargarArchivoAsync();
        }

        private void Notificar(string mensaje) {
            EjecutarEnSegundoPlano(new Action(() =>
            txt_Notificaciones.Text = string.Format("\n[{0:H:mm:ss}] {1}\n{2}", DateTime.Now, mensaje, txt_Notificaciones.Text)
                        ));
        }

        private void Notificar(string formato, params string[] args) {
            var mensaje = string.Format(formato, args);
            EjecutarEnSegundoPlano(new Action(() =>
        txt_Notificaciones.Text = string.Format("\n[{0:H:mm:ss}] {1}\n{2}", DateTime.Now, mensaje, txt_Notificaciones.Text)
                    ));
        }

        public static void EjecutarEnSegundoPlano(Action accion) {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, accion);
        }

        public static void EjecutarEnSegundoPlano(Action[] acciones) {
            foreach (var accion in acciones) {
                EjecutarEnSegundoPlano(accion);
            }
        }

        internal void DescargarArchivoAsync() {
            Notificar("Descargando archivo desde Google Drive:\n{0}", EnlaceArchivoDrive);

            DescargarAsync(EnlaceArchivoDrive);
        }
        
        private async void DescargarAsync(string enlace) {
            var DriveService = new GoogleDriveService("client_secret_drive.json", "CD_Middleware");
            var servicio = DriveService.Service;

            // Inicializando el flujo de memoria.
            Stream = new MemoryStream();

            // Obteniendo ID del archivo.
            var fileID = "";
            var regex = new Regex(@"https://drive.google.com/(?:open\?id=|file(?:/u)?(?:/\d)?(?:/d)?/)(?<id>.*)");
            var match = regex.Match(enlace);
            if (match.Success) {
                fileID = match.Groups["id"].Value;
            }

            // Especificando el archivo por su ID.
            var request = servicio.Files.Get(fileID);

            // Estableciendo rutina de cambio de progreso.
            request.MediaDownloader.ProgressChanged += Descarga_ProgressChanged;

            // Estableciendo tamaño de las partes de descarga.
            request.MediaDownloader.ChunkSize = (int) (0.5 * Math.Pow(10, 6));

            // Iniciando descarga asíncrona.
            await request.DownloadAsync(Stream);
        }

        private void Descarga_ProgressChanged(IDownloadProgress progress) {
            switch (progress.Status) {
                case DownloadStatus.Downloading:
                    break;

                case DownloadStatus.Completed:
                    // Ruta final del archivo.
                    var ruta = Path.Combine(RutaLocal, NombreArchivoDrive);

                    // Guardando el archivo en disco.
                    GoogleDriveService.SaveStream(Stream, ruta);

                    Notificar("¡Archivo descargado!\n{0}", ruta);
                    LeerTexto();
                    EnviarDatos();
                    break;

                case DownloadStatus.Failed:
                    Notificar("Error al descargar el archivo:\n{0}\n{1}", progress.Exception.Message, progress.Exception.StackTrace);
                    break;
            }
        }

        private void LeerTexto() {
            Texto = File.ReadAllText(NombreArchivoDrive);

            Notificar("Datos:\n{0}", Texto);
        }

        private void EnviarDatos() {
            Notificar("Enviando datos a Watson...");
            
            var rutaPrograma = @"..\..\..\ToneAnalyzer\bin\Debug\netcoreapp2.0\ToneAnalyzer.dll";
            var args = Funciones.UnirArgumentosCMD(rutaPrograma, Texto);
            Notificar("Iniciando:\ndotnet {0}", args);
            Thread.Sleep(5000);
            Process.Start("dotnet", args);
        }
    }
}
