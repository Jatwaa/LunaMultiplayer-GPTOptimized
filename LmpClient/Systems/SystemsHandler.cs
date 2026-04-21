using LmpClient.Base.Interface;
using LmpClient.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Profiling;

// ReSharper disable ForCanBeConvertedToForeach

namespace LmpClient.Systems
{
    /// <summary>
    /// This class contains all the systems. Here they are called from the MainSystem so they are run in the update, fixed update or late update calls
    /// </summary>
    public static class SystemsHandler
    {
        private static ISystem[] _systems = new ISystem[0];

        /// <summary>
        /// Here we pick all the classes that inherit from ISystem and we put them in the systems array
        /// </summary>
        public static void FillUpSystemsList()
        {
            var systemsList = new List<ISystem>();

            var systems = Assembly.GetExecutingAssembly().GetLoadableTypes().Where(t => t.IsClass && typeof(ISystem).IsAssignableFrom(t) && !t.IsAbstract).ToArray();
            foreach (var system in systems)
            {
                try
                {
                    if (system.GetProperty("Singleton", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetValue(null, null) is ISystem systemImplementation)
                        systemsList.Add(systemImplementation);
                }

                catch (Exception ex)
                {
                    LunaLog.LogError($"Exception loading system type {system.FullName}: {ex.Message}");
                }
            }

            _systems = systemsList.OrderBy(s => s.ExecutionOrder).ToArray();
        }

        /// <summary>
        /// Call all the fixed updates of the systems
        /// </summary>
        public static void FixedUpdate()
        {
            for (var i = 0; i < _systems.Length; i++)
            {
                var sys = _systems[i];
                try
                {
                    Profiler.BeginSample(sys.SystemName);
                    sys.FixedUpdate();
                    Profiler.EndSample();
                }
                catch (Exception e)
                {
                    MainSystem.Singleton.HandleException(e, "SystemHandler-FixedUpdate", sys.SystemName);
                }
            }
        }

        /// <summary>
        /// Call all the updates of the systems
        /// </summary>
        public static void Update()
        {
            for (var i = 0; i < _systems.Length; i++)
            {
                var sys = _systems[i];
                try
                {
                    Profiler.BeginSample(sys.SystemName);
                    sys.Update();
                    Profiler.EndSample();
                }
                catch (Exception e)
                {
                    MainSystem.Singleton.HandleException(e, "SystemHandler-Update", sys.SystemName);
                }
            }
        }

        /// <summary>
        /// Call all the late-updates of the systems
        /// </summary>
        public static void LateUpdate()
        {
            for (var i = 0; i < _systems.Length; i++)
            {
                var sys = _systems[i];
                try
                {
                    Profiler.BeginSample(sys.SystemName);
                    sys.LateUpdate();
                    Profiler.EndSample();
                }
                catch (Exception e)
                {
                    // Was incorrectly labelled "SystemHandler-Update" — now correctly "SystemHandler-LateUpdate"
                    MainSystem.Singleton.HandleException(e, "SystemHandler-LateUpdate", sys.SystemName);
                }
            }
        }
    }
}
