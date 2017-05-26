using System;
using System.ComponentModel;
using System.Collections.Generic;
using LIFE.API.Agent;
using LIFE.API.Layer;
using LIFE.API.LIFECapabilities;
using LIFE.API.Results;
using LIFE.API.Environment.GridCommon;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.Environments.GridEnvironment;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;

namespace Mars.NagelSchreckenberg.Models.DefaultLayer
{
    public class Car : IAgent, IGridCoordinate, ISimResult
    {
        #region fields

        private int _speed;
        private readonly int _speedlimit;
        private readonly float _brakeprobability;
        private readonly UnregisterAgent _unregFkt;

        private Random Random { get; set; }

        #endregion

        #region agent state

        public Guid ID { get; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public GridDirection GridDirection { get; }

        public int Speed
        {
            get => _speed;
            private set
            {
                if (Speed == value) return;
                _speed = value;
                OnPropertyChanged("Speed");
            }
        }

        #endregion

        public IGridEnvironment<Car> Environment { get; set; }

        public IDefaultLayer Layer { get; set; }


        public Car AgentReference => this;

        [PublishForMappingInMars]
        public Car(IDefaultLayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt,
            IGridEnvironment<Car> env, Guid id)
        {
            ID = id;
            Random = new Random(ID.GetHashCode());
            Layer = layer;
            Environment = env;
            _unregFkt = unregFkt;

            regFkt(Layer, this);
            Y = Random.Next(Layer.DimensionY);

            var xIndex = Random.Next(Layer.FreeCells.Count);
            X = Layer.FreeCells[xIndex];
            Console.WriteLine(X);
            Layer.FreeCells.RemoveAt(xIndex);
            env.Insert(this);

            _speedlimit = 5;
            _brakeprobability = 0.3f;
            Speed = Random.Next(_speedlimit + 1);
        }


        public void Tick()
        {
            // Accelerate
            if (Speed < _speedlimit)
                Speed = Speed + 1;

            // Brake to avoid collision
            var nearest = Environment.GetNearest(X, Y, Speed, c => c.X > X);
            if (nearest != null)
                Speed = nearest.X - X - 1;
            else if (X + Speed >= Layer.DimensionX)
            {
                var speedModulo = (X + Speed) % Layer.DimensionX;
                nearest = Environment.GetNearest(0, Y, speedModulo, c => c.X <= speedModulo);
                if (nearest != null)
                    Speed = Layer.DimensionX - X + nearest.X - 1;
            }

            // Random decceleration
            if (Speed > 0 && Random.NextDouble() < _brakeprobability)
            {
                Console.WriteLine("Random brake");
                Speed = Speed - 1;
            }

            // Move
            IInteraction interaction = new MovementAction(() =>
            {
                var result = Environment.MoveToPosition(this, (X + Speed) % Layer.DimensionX, Y);
                X = result.X;
                Y = result.Y;
            });

            interaction.Execute();
            // Console.WriteLine($"Car {ID} -> Speed: {Speed}, Position: {X}, {Y}");
        }

        public bool Equals(IGridCoordinate other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public AgentSimResult GetResultData()
        {
            return new AgentSimResult
            {
                AgentId = ID.ToString(),
                AgentType = GetType().Name,
                Layer = Layer.GetType().Name,
                Tick = Layer.GetCurrentTick(),
                Position = new[] {X, Y},
                AgentData = new Dictionary<string, object>()
                {
                    {"Speed", Speed},
                    {"SpeedLimit", _speedlimit},
                    {"BrakeProbability", _brakeprobability}
                }
            };
        }

        #region property changed handling

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
            => PropertyChanged?.Invoke(this, e);

        protected void OnPropertyChanged(string propertyName)
            => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}