// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2023 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2025 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 DrSmugleaf <drsmugleaf@gmail.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 Ichaie <167008606+Ichaie@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Ilya246 <57039557+Ilya246@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 JORJ949 <159719201+JORJ949@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 MortalBaguette <169563638+MortalBaguette@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Panela <107573283+AgentePanela@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Poips <Hanakohashbrown@gmail.com>
// SPDX-FileCopyrightText: 2025 PuroSlavKing <103608145+PuroSlavKing@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Solstice <solsticeofthewinter@gmail.com>
// SPDX-FileCopyrightText: 2025 Whisper <121047731+QuietlyWhisper@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 blobadoodle <me@bloba.dev>
// SPDX-FileCopyrightText: 2025 coderabbitai[bot] <136622811+coderabbitai[bot]@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2025 github-actions[bot] <41898282+github-actions[bot]@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 kamkoi <poiiiple1@gmail.com>
// SPDX-FileCopyrightText: 2025 shibe <95730644+shibechef@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 tetra <169831122+Foralemes@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.LinkAccount;

public sealed class RMCChangeGhostColorMsg : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public Color Color;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        Color = buffer.ReadColor();
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(Color);
    }
}