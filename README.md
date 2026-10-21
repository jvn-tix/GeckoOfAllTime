## Developer & Contributions
**Dicco** (Game Artist & Programmer) <br>
**Jevon** (Programmer) <br>
**Angel** (Programmer)

## About
Gecko Of All Time is a 2D roguelike bullet-heaven in the style of Vampire Survivors. Play as a gecko in an endless arena, aim your shots at swarming enemies, collect EXP orbs to level up, and pick power-ups to build a stronger run. Stay alive until the timer runs out to win.

<br>

## Key Features

**Aimed Shooting**: Aim with the mouse or touch to fire bullets at the waves of enemies closing in from all sides.

**Level Up & Power-Ups**: Defeated enemies drop EXP orbs. Fill the EXP bar to level up and choose a power-up, including turrets that add extra firepower.

**Survive the Clock**: A run lasts until the game timer ends. Enemy spawns ramp up as time passes, so keep moving and keep shooting.

**Health Orbs**: Health orbs spawn during the run to heal you, so a risky pickup can keep you alive a little longer.

**Endless Arena**: An infinitely tiling map follows the player, so the arena never runs out.

<table>
<tr>
<td align="center" width="50%">
<strong>Bullet-Heaven Combat</strong><br>
Aim and shoot as enemies swarm the gecko
<img width="100%" alt="gif1" src="docs/gifs/combat.gif">
</td>
<td align="center" width="50%">
<strong>Level-Up Power-Ups</strong><br>
Collect EXP and pick a power-up on each level
<img width="100%" alt="gif2" src="docs/gifs/levelup.gif">
</td>
</tr>
</table>

## Scene Flow
```mermaid
flowchart LR
  mm[Main Menu]
  gp[Gameplay]
  pause[Pause Menu]
  over[Game Over]
  win[Victory]

  mm -- "Play" --> gp
  gp -- "Pause" --> pause
  pause -- "Resume" --> gp
  pause -- "Quit" --> mm
  gp -- "Health Reaches 0" --> over
  gp -- "Timer Ends" --> win
  over -- "Retry" --> gp
  over -- "Menu" --> mm
  win -- "Menu" --> mm
```

## Layer / Module Design
```mermaid
---
config:
  theme: neutral
  look: neo
---
graph TD
    subgraph "Core Systems"
        GT[GameTimer]
        PM[PauseManager]
        GO[GameOver]
        AM[AudioManager]
        SM[SceneMusic]
    end

    subgraph "Player Systems"
        PMove[PlayerMovement]
        PHealth[PlayerHealth]
        PExp[PlayerExp]
        Cam[CameraTarget]
        Turret[TurretController]
        Bullet[Bullet]
    end

    subgraph "Enemy Systems"
        ES[EnemySpawner]
        EC[EnemyController]
        EM[EnemyMovement]
        Hp[Health]
    end

    subgraph "Pickups & Progression"
        Exp[ExpOrb]
        HO[HealthOrb]
        HOS[HealthOrbSpawner]
        LU[LevelUpManager]
        PU[PowerUp Controller & Data]
    end

    subgraph "UI Systems"
        MM[MainMenu]
        ExpUI[ExpBarUI]
        Pop[DamagePopup]
        Btn[ButtonClick]
    end

    subgraph "World"
        Tile[InfiniteTile]
    end

    PMove --> Cam
    PMove --> Tile
    Turret --> Bullet
    Bullet --> Hp
    ES --> EC
    EC --> EM
    EC --> Hp
    EC --> PHealth
    Hp --> Pop
    Hp --> Exp
    Exp --> PExp
    PExp --> ExpUI
    PExp --> LU
    LU --> PU
    PU --> Turret
    HOS --> HO
    HO --> PHealth
    PHealth --> GO
    GT --> ES
    PM --> GT
    AM --> SM
    MM --> Btn

    classDef coreStyle fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    classDef playerStyle fill:#e8f5e8,stroke:#1b5e20,stroke-width:2px
    classDef enemyStyle fill:#ffebee,stroke:#b71c1c,stroke-width:2px
    classDef pickupStyle fill:#fff3e0,stroke:#e65100,stroke-width:2px
    classDef uiStyle fill:#f3e5f5,stroke:#4a148c,stroke-width:2px

    class GT,PM,GO,AM,SM coreStyle
    class PMove,PHealth,PExp,Cam,Turret,Bullet playerStyle
    class ES,EC,EM,Hp enemyStyle
    class Exp,HO,HOS,LU,PU pickupStyle
    class MM,ExpUI,Pop,Btn uiStyle
```

## Modules and Features

The bullet-heaven gameplay, enemy waves, EXP progression and power-up system is powered by a Unity C# scripting system.

| 📂 Name | 🎬 Scene | 📋 Responsibility |
|---------|----------|-------------------|
| **PlayerMovement** | **Gameplay** | - Move the gecko around the arena |
| **PlayerHealth** | **Gameplay** | - Track player health<br/>- Take damage from enemies and heal from health orbs<br/>- Trigger game over on death |
| **PlayerExp** | **Gameplay** | - Collect EXP and track the current level<br/>- Update the EXP bar<br/>- Trigger the level-up screen |
| **CameraTarget** | **Gameplay** | - Make the camera follow the player |
| **TurretController** | **Gameplay** | - Control turrets that shoot at enemies |
| **Bullet** | **Gameplay** | - Move projectiles and apply damage on hit |
| **EnemySpawner** | **Gameplay** | - Spawn enemies around the player over time |
| **EnemyController** | **Gameplay** | - Handle enemy behavior, contact damage and death<br/>- Drop EXP orbs |
| **EnemyMovement** | **Gameplay** | - Move enemies toward the player |
| **Health** | **Gameplay** | - Shared health component for damageable objects |
| **DamagePopup** | **Gameplay** | - Show floating damage numbers |
| **ExpOrb** | **Gameplay** | - Dropped by enemies, collected by the player for EXP |
| **HealthOrb / HealthOrbSpawner** | **Gameplay** | - Spawn healing pickups during the run<br/>- Heal the player on pickup |
| **LevelUpManager** | **Gameplay** | - Pause the game on level up<br/>- Offer power-up choices |
| **PowerUp Controller & Data** | **Gameplay** | - Define power-up data<br/>- Apply the chosen power-up effects |
| **InfiniteTile** | **Gameplay** | - Tile the background endlessly around the player |
| **GameTimer** | **Gameplay** | - Count the run time and end the run when the timer finishes |
| **PauseManager** | **Gameplay** | - Pause and resume the game |
| **GameOver** | **Gameplay** | - Show the game over screen and retry options |
| **ExpBarUI** | **Gameplay** | - Display EXP progress and level |
| **MainMenu** | **Main Menu** | - Play and Quit buttons |
| **ButtonClick** | **All Scenes** | - Play click sound effects on buttons |
| **AudioManager / SceneMusic** | **All Scenes** | - Manage music and sound effects per scene |

<br>

## Game Flow Chart
```mermaid
---
config:
  theme: redux
  look: neo
---
flowchart TD
  start([Run Start])
  start --> timerStart[Game Timer Starts]
  timerStart --> spawnEnemies[Enemies Spawn Over Time]
  spawnEnemies --> playerAct{Player Input}

  playerAct -->|Move| move[Move Gecko]
  playerAct -->|Aim and Shoot| shoot[Fire Bullet]

  shoot --> hitCheck{Bullet Hits Enemy?}
  hitCheck -->|Yes| dmg[Enemy Takes Damage]
  hitCheck -->|No| miss[Bullet Disappears]

  dmg --> enemyDead{Enemy Health = 0?}
  enemyDead -->|Yes| dropExp[Drop EXP Orb]
  enemyDead -->|No| spawnEnemies

  dropExp --> collect[Player Collects EXP]
  collect --> levelCheck{Level Up?}
  levelCheck -->|Yes| pick[Choose Power-Up]
  levelCheck -->|No| spawnEnemies
  pick --> spawnEnemies

  move --> contact{Enemy Touches Player?}
  contact -->|Yes| hurt[Player Takes Damage]
  contact -->|No| spawnEnemies

  hurt --> hpCheck{Health > 0?}
  hpCheck -->|Yes| healOrb{Health Orb Picked?}
  hpCheck -->|No| over[Game Over]
  healOrb -->|Yes| heal[Heal Player]
  healOrb -->|No| spawnEnemies
  heal --> spawnEnemies

  timerStart --> timeUp{Timer Ended?}
  timeUp -->|Yes| victory[Victory]
```

<br>

## Play The Game
<a href="https://YOUR-ITCH-LINK-HERE">Play Now on itch.io</a>

<br>

## Installation & Setup
1. Clone this repository
2. Open the project in Unity
3. Open the main menu scene
4. Press Play to start testing

## Controls
- **Mouse / Touch** - Aim and shoot
- **WASD / Virtual Joystick** - Move *(update to match your bindings)*
- **Escape** - Pause Menu

## Credits
Game developed as a team project for the Game Development Capstone.

- Engine: Unity
- Pixel art: Aseprite
