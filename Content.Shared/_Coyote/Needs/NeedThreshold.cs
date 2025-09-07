namespace Content.Shared._Coyote.Needs;

/// <summary>
/// The threshold of a need, all need to be filled out
/// </summary>
public enum NeedThreshold : byte
{
    /// <summary>
    /// The need is in the best possible state
    /// </summary>
    ExtraSatisfied,

    /// <summary>
    /// The need is satisfied
    /// </summary>
    Satisfied,

    /// <summary>
    /// The need is low
    /// </summary>
    Low,

    /// <summary>
    /// The need is critical, threshold will be set to the minimum value automagestically
    /// </summary>
    Critical,
}
