# Roll-a-Ball

A small 3D game built in Unity 6 as an introductory project for a university game development course. The goal is to collect enough coins to win before the warden catches you, or before you fall off the map.

## Features

- **Player movement**: physics-based ball controller using Unity's Input System.
- **Coin collection**: coins award score on contact and are respawned each session.
- **Warden AI**: enemy that chases the player using NavMesh pathfinding.
- **Dynamic obstacles**: physics boxes that are spawned and cleared each session.
- **Win / Lose conditions**: reach the score target to win; get caught or fall off to lose.
- **Game state management**: singleton GameManager handles session flow, UI transitions, and entity spawning.
- **WebGL support**: playable in browser via GitHub Pages.

## Assets

This project makes use of free third-party assets sourced from the Unity Asset Store and other providers. All respective assets remain under their original licences.

## Licence

This licence applies to the source code in this repository only (C# scripts).
Third-party assets remain under their respective licences.

The source code in this repository is released under the [MIT Licence](LICENSE).  
For third-party assets refer to [3rdP Notices](ThirdPartyNotices.md).