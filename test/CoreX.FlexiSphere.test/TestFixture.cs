#region Header

// --------------------------------------------------------------------------------------
// Powered by:
// 
//     __________.__                  .___    ___________                             
//     \______   \__| ____   ____   __| _/____\__    ___/___   ____       ____  __ __ 
//      |     ___/  |/    \_/ __ \ / __ |\__  \ |    |_/ __ \_/ ___\    _/ __ \|  |  \
//      |    |   |  |   |  \  ___// /_/ | / __ \|    |\  ___/\  \___    \  ___/|  |  /
//      |____|   |__|___|  /\___  >____ |(____  /____| \___  >\___  > /\ \___  >____/ 
//                   \/     \/     \/     \/           \/     \/  \/     \/
// 
// 
// FileName: TestFixture.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-06T16:38:41.758Z
//
// --------------------------------------------------------------------------------------

#endregion

using Microsoft.Extensions.Configuration;

using NLog;
using NLog.Extensions.Logging;

namespace CoreX.FlexiSphere.test;

public class TestFixture : IDisposable
{
    private bool _disposed = false;

    static TestFixture()
    {
        ResetDirectory("app.logs");
    }

    public TestFixture()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
            .AddJsonFile("test.config.json", optional: false, reloadOnChange: true)
            .Build();

        LogManager.Configuration = new NLogLoggingConfiguration(config.GetRequiredSection("NLog"));

        TestFixture.UpdateLogFileName("${basedir}/app.logs/test." + this.GetType().Name + ".${date:format=yyyy.MM.dd}.log");
    }

    private static void UpdateLogFileName(string newFileName)
    {
        var target = LogManager.Configuration.FindTargetByName<NLog.Targets.FileTarget>("logfile");
        if (target != null)
        {
            target.FileName = newFileName;
            LogManager.ReconfigExistingLoggers();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources here.
            }

            // Dispose unmanaged resources here.

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected static void ResetDirectory(string directoryPattern = "test*")
    {
        var directories = Directory.GetDirectories(".", directoryPattern, SearchOption.AllDirectories);
        foreach (var directory in directories)
        {
            try
            {
                Directory.Delete(directory, true);
            }
            catch
            {
                // Nothing to do
            }
        }
    }
}
