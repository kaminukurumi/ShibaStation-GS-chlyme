# SPDX-FileCopyrightText: 2022 Aru Moon <anton17082003@gmail.com>
# SPDX-FileCopyrightText: 2022 Julian Giebel <juliangiebel@live.de>
# SPDX-FileCopyrightText: 2023 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
# SPDX-FileCopyrightText: 2023 MishaUnity <81403616+MishaUnity@users.noreply.github.com>
# SPDX-FileCopyrightText: 2023 Phill101 <28949487+Phill101@users.noreply.github.com>
# SPDX-FileCopyrightText: 2023 Phill101 <holypics4@gmail.com>
# SPDX-FileCopyrightText: 2024 ArchRBX <5040911+ArchRBX@users.noreply.github.com>
# SPDX-FileCopyrightText: 2024 Kot <1192090+koteq@users.noreply.github.com>
# SPDX-FileCopyrightText: 2024 lapatison <100279397+lapatison@users.noreply.github.com>
# SPDX-FileCopyrightText: 2024 Эдуард <36124833+Ertanic@users.noreply.github.com>
# SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
# SPDX-FileCopyrightText: 2025 Aiden <aiden@djkraz.com>
# SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
#
# SPDX-License-Identifier: AGPL-3.0-or-later

device-pda-slot-component-slot-name-cartridge = Cartridge

default-program-name = Program
notekeeper-program-name = Notekeeper
news-read-program-name = Station news

crew-manifest-program-name = Crew manifest
crew-manifest-cartridge-loading = Loading ...

net-probe-program-name = NetProbe
net-probe-scan = Scanned {$device}!
net-probe-label-name = Name
net-probe-label-address = Address
net-probe-label-frequency = Frequency
net-probe-label-network = Network

log-probe-program-name = LogProbe
log-probe-scan = Downloaded logs from {$device}!
log-probe-label-time = Time
log-probe-label-accessor = Accessed by
log-probe-label-number = #
log-probe-print-button = Print Logs
log-probe-printout-device = Scanned Device: {$name}
log-probe-printout-header = Latest logs:
log-probe-printout-entry = #{$number} / {$time} / {$accessor}

astro-nav-program-name = AstroNav

med-tek-program-name = MedTek

# Wanted list cartridge
wanted-list-program-name = Wanted list
wanted-list-label-no-records = It's all right, cowboy
wanted-list-search-placeholder = Search by name and status

wanted-list-age-label = [color=darkgray]Age:[/color] [color=white]{$age}[/color]
wanted-list-job-label = [color=darkgray]Job:[/color] [color=white]{$job}[/color]
wanted-list-species-label = [color=darkgray]Species:[/color] [color=white]{$species}[/color]
wanted-list-gender-label = [color=darkgray]Gender:[/color] [color=white]{$gender}[/color]

wanted-list-reason-label = [color=darkgray]Reason:[/color] [color=white]{$reason}[/color]
wanted-list-unknown-reason-label = unknown reason

wanted-list-initiator-label = [color=darkgray]Initiator:[/color] [color=white]{$initiator}[/color]
wanted-list-unknown-initiator-label = unknown initiator

wanted-list-status-label = [color=darkgray]status:[/color] {$status ->
        [suspected] [color=yellow]suspected[/color]
        [wanted] [color=red]wanted[/color]
        [detained] [color=#b18644]detained[/color]
        [paroled] [color=green]paroled[/color]
        [discharged] [color=green]discharged[/color]
        *[other] none
    }

wanted-list-history-table-time-col = Time
wanted-list-history-table-reason-col = Crime
wanted-list-history-table-initiator-col = Initiator
