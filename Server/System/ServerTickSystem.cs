using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Server.Context;
using Server.Log;

namespace Server.System
{
    public class ServerTickSystem
    {
        private static readonly ServerTickSystem Instance = new ServerTickSystem();
        public static ServerTickSystem InstanceProperty => Instance;

        private const int TargetTickRate = 100; // 100 ticks per second = 10ms interval
        private static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(1000.0 / TargetTickRate);

        private long _tickCount;
        private long _lastTickTimestamp;
        private long _tickOverruns;
        private long _totalTickLatencyMs;

        public long TickCount => Interlocked.Read(ref _tickCount);
        public long TickOverruns => Interlocked.Read(ref _tickOverruns);
        public double AverageTickLatencyMs => (double)Interlocked.Read(ref _totalTickLatencyMs) / Math.Max(1, TickCount);

        public long ServerTick => Interlocked.Read(ref _tickCount);

        public event Action OnTick;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginTick()
        {
            Interlocked.Increment(ref _tickCount);
            _lastTickTimestamp = ServerContext.ServerClock.ElapsedMilliseconds;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EndTick()
        {
            var elapsed = ServerContext.ServerClock.ElapsedMilliseconds - _lastTickTimestamp;
            var targetMs = (int)TickInterval.TotalMilliseconds;
            var overrun = elapsed - targetMs;

            if (overrun > 0)
            {
                Interlocked.Increment(ref _tickOverruns);
                Interlocked.Add(ref _totalTickLatencyMs, overrun);
            }

            var remaining = targetMs - (int)elapsed;
            if (remaining > 0)
            {
                Thread.Sleep(remaining);
            }
        }

        public void RunTickLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                BeginTick();
                try
                {
                    OnTick?.Invoke();
                }
                catch (Exception ex)
                {
                    LunaLog.Error($"Tick error: {ex.Message}");
                }
                EndTick();
            }
        }

        public void Reset()
        {
            _tickCount = 0;
            _lastTickTimestamp = 0;
            _tickOverruns = 0;
            _totalTickLatencyMs = 0;
        }
    }
}