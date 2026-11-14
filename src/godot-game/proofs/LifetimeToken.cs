using System;
using System.Collections.Generic;
using System.Threading;

namespace KingdomZombies.Proofs;
/// <summary>Retains independent observed values after its scene generation has been freed.</summary>
internal sealed class LifetimeToken
{
    /// <summary>Gets whether queued results may still enter this scene generation.</summary>
    internal bool IsOpen { get; private set; } = true;
    /// <summary>Gets the observed engine callback order for the scene generation.</summary>
    internal List<string> Events { get; } = [];
    /// <summary>Gets or sets the real parameterless constructor count.</summary>
    internal int Constructions { get; set; }
    /// <summary>Gets or sets the real parent process callback count.</summary>
    internal int Processes { get; set; }
    /// <summary>Gets or sets the real emitted-signal handler count.</summary>
    internal int Handlers { get; set; }
    /// <summary>Gets or sets the scene value changed only through main-thread work.</summary>
    internal int Value { get; set; }
    /// <summary>Gets or sets the number of successfully checked serialized-value uses.</summary>
    internal int Uses { get; set; }
    /// <summary>Gets or sets genuine native resource construction observations.</summary>
    internal int Resources { get; set; }
    /// <summary>Gets or sets the actual engine identity of this generation's root node.</summary>
    internal ulong Instance { get; set; }
    /// <summary>Gets the cancellation source independently of the generation acceptance fence.</summary>
    internal CancellationTokenSource Cancellation { get; } = new();

    /// <summary>Closes result acceptance before cancellation and signal disconnection.</summary>
    internal void Close()
    {
        IsOpen = false;
        Cancellation.Cancel();
    }
}
