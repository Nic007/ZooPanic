using Assets.Scripts.Components;
using JetBrains.Annotations;

namespace Assets.Scripts.Actions
{
    public interface IAction
    {
        AgentComponent Agent { get; set; }

        bool Completed { get; }

        void Update();
    }
}
