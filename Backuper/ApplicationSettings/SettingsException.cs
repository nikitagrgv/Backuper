using System;

namespace Backuper.ApplicationSettings
{
    internal class SettingsException : ApplicationException
    {
        private readonly string _fileName;

        public SettingsException(string message, string fileName)
            : base(message)
        {
            _fileName = fileName;
        }

        public string GetFileName()
        {
            return _fileName;
        }
    }

    internal class BadJsonFileException : SettingsException
    {
        public BadJsonFileException(string fileName)
            : base("Bad settings json file", fileName)
        {
        }
    }

    internal class SettingsFileNotFoundException : SettingsException
    {
        public SettingsFileNotFoundException(string fileName)
            : base("Settings file not found", fileName)
        {
        }
    }
}