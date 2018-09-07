using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace GarthToland.ListSites.Services
{
    public interface IListSitesService
    {
        void GenerateSiteList();
    }

    public class ListSitesService : IListSitesService
    {
        private readonly IMessageService _messageService;
        private readonly ISettings _settings;

        public ListSitesService(IMessageService messageService, ISettings settings)
        {
            _messageService = messageService;
            _settings = settings;
        }

        /// <summary>
        /// Generates a list of sites using the path from the settings file.
        /// </summary>
        public void GenerateSiteList()
        {
            _messageService.DisplayMessage($"Searching for sites at {_settings.LocalHostPath}");

            var directories = GetDirectories();
            var html = BuildHtml(directories);

            using (StreamWriter w = File.CreateText(_settings.LocalHostPath + "index.html"))
            {
                w.WriteLine(html);
                w.Flush();
            }

            System.Diagnostics.Process.Start(_settings.LocalhostUrl);
        }


        /// <summary>
        /// Builds a HTML document, including the directories provided.
        /// </summary>
        /// <param name="directories"></param>
        /// <returns></returns>
        private string BuildHtml(List<XElement> directories)
        {

            var html = new XElement("html",
                new XElement("head",
                    new XElement("title", "Local sites"),
                    new XElement("link",
                        new XAttribute("rel", "stylesheet"),
                        new XAttribute("href", "https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css")
                    )
                ),
                new XElement("body",
                    new XElement("div",
                        new XAttribute("class", "container mt-3"),
                        new XElement("h2", 
                            "Local sites",
                            new XElement("small",
                            new XAttribute("class", "text-muted"),
                                " - by Garth Toland"
                            )
                        ),
                        new XElement("div",
                            new XAttribute("class", "row"),
                            new XElement("div",
                                new XAttribute("class", "col"),
                                new XElement("div",
                                    new XAttribute("class", "list-group"),
                                    directories
                                )
                            )
                        )
                    )
                )
            );

            return $"<!DOCTYPE html>{html.ToString()}";
        }


        /// <summary>
        /// Gets a list of directories as buttons.
        /// </summary>
        /// <returns></returns>
        private List<XElement> GetDirectories()
        {
            var directories = new List<XElement>();
            string[] directoryIterator = null;

            try
            {
                directoryIterator = Directory.GetDirectories(_settings.LocalHostPath);
            }
            catch (DirectoryNotFoundException)
            {
                _messageService.DisplayMessage("Invalid path. (Try C:\\xampp\\htdocs\\)", ConsoleColor.Red);
            }

            if (directoryIterator != null)
            {
                foreach (var directory in directoryIterator)
                {
                    var shortDirectoryName = directory.Replace(_settings.LocalHostPath, "");

                    if (!shortDirectoryName.Contains("."))
                        continue;

                    var listItem = new XElement("a",
                        new XAttribute("class", "list-group-item list-group-item-action fade show"),
                        new XAttribute("href", $"http://localhost/{shortDirectoryName}"),
                        new XAttribute("target", "_blank"),
                        shortDirectoryName
                    );

                    directories.Add(listItem);

                    _messageService.DisplayMessage($"Found {shortDirectoryName}", ConsoleColor.Green);
                }

                if (directories.Count == 0)
                    _messageService.DisplayMessage("Site folder names must contain a '.', e.g. 'example.com'.", ConsoleColor.Red);
            }

            return directories;
        }
    }
}
