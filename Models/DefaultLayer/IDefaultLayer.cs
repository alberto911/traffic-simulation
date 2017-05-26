using System;
using System.Collections.Generic;
using System.Linq;
using LIFE.API.Environment.GridCommon;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Environments.GridEnvironment;
using ISteppedLayer = LIFE.API.Layer.ISteppedLayer;
using RegisterAgent = LIFE.API.Layer.RegisterAgent;
using UnregisterAgent = LIFE.API.Layer.UnregisterAgent;
using LIFE.Components.Services.AgentManagerService.Implementation;

namespace Mars.NagelSchreckenberg.Models.DefaultLayer
{
    public interface IDefaultLayer : ISteppedActiveLayer
    {
        int DimensionX { get; }
        int DimensionY { get; }
        List<int> FreeCells { get; }
    }
}