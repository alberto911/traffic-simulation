using System;
using System.Collections.Generic;
using System.Linq;
using LIFE.API.Environment.GridCommon;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Environments.GridEnvironment;
using LIFE.Components.ESC.SpatialAPI.DataLayer;
using ISteppedLayer = LIFE.API.Layer.ISteppedLayer;
using RegisterAgent = LIFE.API.Layer.RegisterAgent;
using UnregisterAgent = LIFE.API.Layer.UnregisterAgent;
using LIFE.Components.Services.AgentManagerService.Implementation;

namespace Mars.NagelSchreckenberg.Models.DefaultLayer
{
    public class DefaultLayer : IDefaultLayer
    {
        private readonly IGridEnvironment<Car> _gridCars;
        private RegisterAgent _regFkt;
        private UnregisterAgent _unregFkt;
        private long _tick;
        public int DimensionX { get; }
        public int DimensionY { get; }
        public List<int> FreeCells { get; }

        public DefaultLayer()
        {
            DimensionX = 100;
            DimensionY = 1;
            FreeCells = new List<int>(Enumerable.Range(0, DimensionX));
            _gridCars = new GridEnvironment<Car>(DimensionX, DimensionY);
        }

        public bool InitLayer(TInitData layerInitData,
            RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            _regFkt = registerAgentHandle;
            _unregFkt = unregisterAgentHandle;

            var cars = AgentManager.GetAgentsByAgentInitConfig<Car>(
                layerInitData.AgentInitConfigs.FirstOrDefault(),
                _regFkt, _unregFkt, new List<ILayer>() {this}, _gridCars);
            Console.WriteLine("[EnvironmentLayer] Cars spawned (" + cars.Count + ").");

            return true;
        }

        public long GetCurrentTick()
        {
            return _tick;
        }

        public void SetCurrentTick(long currentTick)
        {
            _tick = currentTick;
        }

        public void Tick()
        {
            /*var cars = _gridCars.Explore(0, 0);
            foreach (var car in cars)
            {
                Console.WriteLine($"Car {car.ID} -> Speed: {car.Speed}, Position: {car.X}, {car.Y}");
            }*/
        }

        public void PreTick()
        {
            //Console.WriteLine("PreTick");
        }

        public void PostTick()
        {
            // Console.WriteLine("PostTick");
        }
    }
}