using Autofac;
using GarthToland.ListSites.Services;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GarthToland.ListSites
{
    class Program
    {
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
            var settingsJson = File.ReadAllText(Constants.SETTINGS_FILE);
            var settings = JsonConvert.DeserializeObject<Settings>(settingsJson);

            // Get path
            Console.WriteLine($"Enter webserver root path [{settings.LocalHostPath}]:");

            var input = Console.ReadLine();

            while (!Directory.Exists(input))
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine($"Localhost path remaining default");
                    break;
                }

                Console.WriteLine($"Incorrect path, try again");
                input = Console.ReadLine();
            }

            if (!string.IsNullOrWhiteSpace(input))
            {
                settings.LocalHostPath = input;
            }

            // Get URL
            Console.WriteLine($"Enter localhost URL and port [{settings.LocalhostUrl}]:");
            input = Console.ReadLine();

            while (!input.StartsWith("http") || !Uri.IsWellFormedUriString(input, UriKind.Absolute))
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine($"Localhost URL remaining default");
                    break;
                }

                Console.WriteLine($"Incorrect URL, try again");
                input = Console.ReadLine();
            }

            if (!string.IsNullOrWhiteSpace(input))
            {
                settings.LocalhostUrl = input;
            }

            File.WriteAllText(Constants.SETTINGS_FILE, JsonConvert.SerializeObject(settings, Formatting.Indented));

            if (!File.Exists(Constants.SETTINGS_FILE))
                return null;

            Console.WriteLine($"You can find the settings file at {Path.GetFullPath(Constants.SETTINGS_FILE)}.");

            settingsJson = File.ReadAllText(Constants.SETTINGS_FILE);

            return JsonConvert.DeserializeObject<Settings>(settingsJson);
        }
    }
}
