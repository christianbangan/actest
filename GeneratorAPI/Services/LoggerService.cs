using GeneratorAPI.Models.Common;
using GeneratorAPI.Services.Interfaces;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GeneratorAPI.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly IConfiguration _configuration;

        private const string PROJECT_NAME = "GeneratorAPI";

        private readonly IEnumerable<MaskLogsRegexModel> maskLogsRegex = new List<MaskLogsRegexModel>()
        {
            new()
            {
                Pattern = "\"token\":\"[a-zA-Z0-9_]*\"",
                Replacement = "\"token\":\"*****\""
            },
        };

        public LoggerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Logger
        /// </summary>
        /// <param name="gv">GlobalVariables Model</param>
        /// <param name="message">message</param>
        /// <param name="insertNewLine">Set to true if you want to insert new line. Default value is false</param>
        /// <returns></returns>
        public async Task Log(string message, bool insertNewLine = false)
        {
            message = maskLogsRegex.Aggregate(message, (current, item) =>
                Regex.Replace(current, item.Pattern ?? string.Empty, item.Replacement ?? string.Empty, RegexOptions.Multiline));

            var mainFolderName = PROJECT_NAME;

            var counter = 1;
            while (counter < 11)
            {
                try
                {
                    var fileName = DateTime.Now.ToString("MM-dd-yy") + "File no " + counter;

                    var dirName = $"{_configuration["Logger:LogUrlFolder"]}\\{mainFolderName}\\{DateTime.Now.Year}\\{DateTime.Now.ToString("MMMM")}\\{DateTime.Now.ToString("dddd, dd")}\\";
                    if (Directory.Exists(dirName))
                    {
                        var filePath = Path.Combine(dirName, fileName + ".txt");

                        using var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, bufferSize: 4096, useAsync: true);
                        using var sw = new StreamWriter(fs);
                        message = insertNewLine
                            ? Environment.NewLine + DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": " + message
                            : DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": " + message;
                        await sw.WriteLineAsync(message);
                    }
                    else
                    {
                        Directory.CreateDirectory(dirName);
                        var filePath = Path.Combine(dirName, fileName + ".txt");

                        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, bufferSize: 4096, useAsync: true);
                        using var sw = new StreamWriter(fs);
                        message = insertNewLine
                            ? Environment.NewLine + DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": " + message
                            : DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": " + message;
                        await sw.WriteLineAsync(message);
                    }
                    break;
                }
                catch
                {
                    counter++;
                    if (counter == 11)
                        return;
                }
            }
        }
    }
}