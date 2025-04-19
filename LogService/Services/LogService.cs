using LogService.Interfaces;

namespace LogService.Services
{
    public class LogService:ILogService
    {
        private readonly string logFile;
        private IConfiguration _configuration;
        public LogService(IConfiguration configuration)
        {
            this._configuration = configuration;
            logFile = _configuration.GetSection("LogFilePath").Value; ;
        }

        public void LogInfo(string message)
        {
            Log("INFO", message);
        }

        public void LogWarning(string message)
        {
            Log("WARNING", message);
        }

        public void LogError(string message)
        {
            Log("ERROR", message);
        }

        private void Log(string level, string message)
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
            File.AppendAllText(logFile, logMessage);
        }
    }
}
