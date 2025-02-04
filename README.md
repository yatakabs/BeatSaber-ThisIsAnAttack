# ThisIsAnAttack
Beat Saber plugin for match modelation and management, with no use of multiplayer environment.

## Features

### (to be Available)
1. Real-time score submission on match for some kind of competitions.

### (probably) Planned
1. Match modelation and management.
1. Match history management.
1. Syncronization of matches between players.
1. etc...

## How to Use

### Prerequisites
- Beat Saber with the BSIPA mod installed (following versions):
  - `v1.29.1` (Steam)
  - `v1.39.1` (Steam)

### Installation
2. Download the latest release from the [Release page](https://github.com/yatakabs/BeatSaber-ThisIsAnAttack/releases/latest).
3. Extract the zip file to your Beat Saber directory.
4. Run the game.

### File Structure after installation
```
Beat Saber
├── UserData
│   └── ThisIsAnAttack.json
│   └── ThisIsAnAttack
│        └── ... (some configuration files, resources, etc...)
│
├── Plugins
│   └── ThisIsAnAttack.dll
│
└── Libs
	└── ... (some dependency libraries)
```


## Configuration
Basic configuration is stored in `UserData/ThisIsAnAttack.json` file.
Content of the file is TBD.


## Usage (for the initial development stage, as of Feb 2024, NOT FIXED YET)

### Preparation (by the host)
1. Downlaod the plugin, extract it to some working directory.
2. Open `UserData/ThisIsAnAttack.json` file in the extracted directory.
	1. For each player, fill the following properties in:
	2. `player` object
      	1. `scoreSaberPlayerId` (string): ScoreSaber player ID of the player. (e.g. `76561198005985428`)
	3. Add a new `Match` object to the `matches` array:
      	1. `serverUri` (string): URI of the server to submit the score. (e.g. `https://example.match.server.com/`)
      	2. `matchId` (string): ID of the match. (e.g. `00000000-0000-0000-0000-000000000000`)
      	3. [Optional] matchKey : Key to access the match. (e.g. `123456`)
3. Save the files, and send the configuration files to the players.

### Deployment and use (by the players)
1. Download the plugin, extract it to the Beat Saber directory.
2. Get the configuration files from the host, which is already configured for the match and the players.
3. Copy the configuration files to the `UserData` directory in the Beat Saber directory, to be `UserData/ThisIsAnAttack.json`.
4. Run the game.
5. Play the game, and the plugin will submit the score to the server automatically.
	- The host can check the match status on the server.
	- If not, the host needs to troubleshoot the issue, and the players need to re-check the configuration files.

## Configuration
Basic configuration is stored in `UserData/ThisIsAnAttack.json` file.
Content of the file is TBD.

## Development

### Prerequisites
- Beat Saber with the BSIPA mod installed (following versions):
  - `v1.29.1` (Steam)
  - `v1.39.1` (Steam)
- Visual Studio 2022 (Community, Professional, or Enterprise)
  - C# / .NET development workload
  - .NET Framework 4.8 SDK (included in Visual Studio 2022 installation)
  - Extension(s):
    - [BeatSaberModdingTools](https://github.com/Zingabopp/BeatSaberModdingTools/releases)

### Setup for Development
1. Clone the repository.
2. Open the project in Visual Studio.
3. Build the project in `DebugInGame` configuration.
   - `DebugInGame` configuration copies the built files to the Beat Saber directory (Plugins folder) automatically.
   - `Debug` configuration does **NOT** copy the built files to the Beat Saber directory.
   - `Release` configuration for the final debug before release
4. Run the game.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
