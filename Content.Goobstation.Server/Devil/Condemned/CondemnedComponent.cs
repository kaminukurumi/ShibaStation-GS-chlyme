// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 SolsticeOfTheWinter <solsticeofthewinter@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Goobstation.Server.Devil.Condemned;

/// <summary>
/// Marks an entity as having sold their soul.
/// When you die, do NOT collect 200, do NOT pass go. Go directly to hell
/// </summary>
[RegisterComponent]
public sealed partial class CondemnedComponent : Component
{
    /// <summary>
    /// The current phase of the condemnation animation.
    /// </summary>
    [DataField]
    public CondemnedSystem.CondemnedPhase CurrentPhase = CondemnedSystem.CondemnedPhase.Waiting;

    /// <summary>
    /// Who owns this entities soul
    /// </summary>
    [DataField]
    public EntityUid SoulOwner;

    /// <summary>
    /// The elapsed time of the phase.
    /// </summary>
    [DataField]
    public float PhaseTimer;

    /// <summary>
    /// How long the hand effect will last
    /// </summary>
    [DataField]
    public float HandDuration;

    /// <summary>
    /// Should the examine message show when examining someone with this component?
    /// </summary>
    [DataField("showExamine")]
    public bool ShowExamineMessage = true;

    /// <summary>
    /// Is this entities soul owned, but not by a devil?
    /// </summary>
    [DataField]
    public bool SoulOwnedNotDevil = false;

    /// <summary>
    /// Should this entity be sent to hell on death?
    /// </summary>
    [DataField]
    public bool CondemnOnDeath = true;

    /// <summary>
    /// Was this target already weak to holy before becoming condemned?
    /// </summary>
    [DataField]
    public bool WasWeakToHoly;

    /// <summary>
    /// Should movement be locked during the animation?
    /// </summary>
    [DataField]
    public bool FreezeDuringCondemnation;

    /// <summary>
    /// Should this entity be banished (sent to limbo for several minutes) or should they just be deleted?
    /// </summary>
    [DataField]
    public CondemnedSystem.CondemnedBehavior CondemnedBehavior = CondemnedSystem.CondemnedBehavior.Delete;
}
