// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using Content.Goobstation.Shared.Redial;
using Robust.Client;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Goobstation.Client.Redial;

public sealed class RedialManager : SharedRedialManager
{
    public override void Initialize()
    {
        _netManager.RegisterNetMessage<MsgRedial>(RedialOnMessage);
    }

    private void RedialOnMessage(MsgRedial message)
        => IoCManager.Resolve<IGameController>().Redial(message.Address);
}