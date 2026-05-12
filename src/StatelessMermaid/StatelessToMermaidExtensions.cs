using Stateless;
using StatelessMermaid.Internal;

namespace StatelessMermaid;

/// <summary>
/// Extension method entry point
/// </summary>
public static class StatelessToMermaidExtensions
{
    /// <param name="machine">The State Machine</param>
    extension<TState, TTrigger>(StateMachine<TState, TTrigger> machine)
    {
        /// <summary>
        /// Generates a Mermaid state diagram for the state machine using default rendering options.
        /// </summary>
        /// <returns>A Mermaid diagram string, wrapped in a fenced code block by default.</returns>
        public string ToMermaid() => machine.ToMermaid(MermaidOptions.Default);

        /// <summary>
        /// Generates a Mermaid state diagram for the state machine using the specified options.
        /// </summary>
        /// <param name="options">Controls direction, title, syntax version, and code-block wrapping.</param>
        /// <returns>A Mermaid diagram string.</returns>
        /// <remarks>
        /// Composite states, choice nodes, entry/exit action notes, ignored triggers, terminal state
        /// markers, and parameterized trigger type annotations are all derived automatically from the
        /// state machine's configuration. States and triggers decorated with
        /// <see cref="System.ComponentModel.DescriptionAttribute"/> use that description as their
        /// display label; PascalCase names without a description are split into words automatically.
        /// </remarks>
        public string ToMermaid(MermaidOptions options) =>
            new MermaidDiagramBuilder(options, machine.ExtractTriggerParameters())
                .Build(machine.GetInfo());
    }
}