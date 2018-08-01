using System;

namespace GarthToland.ListSites
{
    public interface IMessageService
    {
        void DisplayMessage(string output);
        void DisplayMessage(string output, ConsoleColor textColor);
        void DisplayMessage(string output, ConsoleColor textColor, ConsoleColor backgroundColor);
    }

    public class MessageService : IMessageService
    {
        public void DisplayMessage(string output)
        {
            Console.WriteLine(output);
        }

        public void DisplayMessage(string output, ConsoleColor textColor)
        {
            Console.ForegroundColor = textColor;
            Console.WriteLine(output);
            Console.ResetColor();
        }

        public void DisplayMessage(string output, ConsoleColor textColor, ConsoleColor backgroundColor)
        {
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = textColor;
            Console.WriteLine(output);
            Console.ResetColor();
        }
    }
}
