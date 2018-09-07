using Autofac;
using GarthToland.ListSites.Services;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GarthToland.ListSites
{
    class Program
    {
        private static readonly string SETTINGS_FILE = "settings.json";
        private static string _localhostPath = "C:/xampp/htdocs/";
        private static string _localhostUrl = "http://localhost/";

        private static void Main(string[] args)
        {
            var settings = LoadSettings();
            IContainer container = null;

            if (settings == null)
                return;

            container = InversionOfControl.BuildContainer(settings);

            using (var scope = container.BeginLifetimeScope())
            {
                scope.Resolve<IListSitesService>().GenerateSiteList();
            }

            Console.ReadLine();
        }

        private static Settings LoadSettings()
        {
            // Get path
            Console.WriteLine($"Enter path of htdocs [{_localhostPath}]:");

            var input = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(input) || !Directory.Exists(input))
            {
                Console.WriteLine($"Incorrect path, try again");
                input = Console.ReadLine();
            }

            _localhostPath = input;

            // Get URL
            Console.WriteLine($"Enter localhost URL [{_localhostUrl}]:");
            input = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(input) || !input.StartsWith("http") || !Uri.IsWellFormedUriString(input, UriKind.Absolute))
            {
                Console.WriteLine($"Incorrect URL, try again");
                input = Console.ReadLine();
            }

            _localhostUrl = input;

            var settings = new Settings { LocalHostPath = _localhostPath, LocalhostUrl = _localhostUrl };

            File.WriteAllText(SETTINGS_FILE, JsonConvert.SerializeObject(settings, Formatting.Indented));

            if (!File.Exists(SETTINGS_FILE))
                return null;

            Console.WriteLine($"You can find the settings file at {Path.GetFullPath(SETTINGS_FILE)}.");

            var settingsJson = File.ReadAllText(SETTINGS_FILE);

            return JsonConvert.DeserializeObject<Settings>(settingsJson);
        }
    }
}
