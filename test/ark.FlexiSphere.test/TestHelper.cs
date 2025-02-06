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
// FileName: TestHelper.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.102Z
//
// --------------------------------------------------------------------------------------

#endregion

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using NLog;

namespace ark.FlexiSphere.test;

public static class TestHelper
{
    private static Dictionary<string, int> _conditionCounters = new();

    private static Lock _lock = new();

    /// <summary>
    /// Gets the value from the dictionary or adds it if it does not exist
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        using (var scope = _lock.EnterScope())
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }

            return dict[key];
        }
    }

    /// <summary>
    /// Delays the execution of the action while the condition is true
    /// </summary>
    /// <param name="millisecondsDelay"></param>
    /// <param name="condition"></param>
    /// <param name="peekingPeriod"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="caller"></param>
    /// <returns></returns>
    public static async Task<bool> DelayWhileAsync(int millisecondsDelay, Expression<Func<bool>> condition, int peekingPeriod = 250, CancellationToken cancellationToken = default,
      [ConstantExpected][CallerMemberName] string caller = "") =>
        await DelayWhileAsync(TimeSpan.FromMilliseconds(millisecondsDelay), condition, peekingPeriod, cancellationToken, caller);

    /// <summary>
    /// Delays the execution of the action while the condition is true
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="action"></param>
    /// <param name="peekingPeriod"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="caller"></param>
    /// <returns></returns>
    public static async Task<bool> DelayWhileAsync(TimeSpan timeout, Expression<Func<bool>> action, int peekingPeriod = 250, CancellationToken cancellationToken = default,
        [ConstantExpected][CallerMemberName] string caller = "")
    {
        var logger = LogManager.GetCurrentClassLogger();
        var sw = Stopwatch.StartNew();

        if (Debugger.IsAttached)
        {
            timeout = TimeSpan.FromSeconds(30);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            logger.Warn("[{0}]: Cancellation requested", caller);
            return false;
        }

        logger.Info("[{0}]: Delayed while condition, timeout: [{1}ms]", caller, timeout.TotalMilliseconds);
        var strAction = action.ToString();
        logger.Debug("Condition: [{0}]", strAction);

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(peekingPeriod));
        bool result = false;

        int uid = _conditionCounters.GetOrAdd(caller, 0) + 1;
        _conditionCounters[caller] = uid;

        var compiledAction = action.Compile();

        DateTime endTimeUtc = DateTime.UtcNow.Add(timeout);
        while (DateTime.UtcNow < endTimeUtc &&
            await timer.WaitForNextTickAsync(cancellationToken))
        {
            result = !compiledAction();

            if (result)
            {
                logger.Debug("[{0}|{1}] While condition is true in {2}ms", caller, uid, sw.ElapsedMilliseconds);
                return result;
            }
        }

        logger.Debug("[{0}|{1}] Timeout {2}ms: While condition is false", caller, uid, timeout.TotalMilliseconds);
        return result;
    }
}
