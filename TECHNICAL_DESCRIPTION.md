# Descriere Tehnica - ASCENT

## Overview

ASCENT este un joc 3D single-player de tip adventure/platformer dezvoltat in Unity, cu focus pe mecanici de escalada realista si survival. Jocul combina elemente de physics-based movement cu un sistem de resource management.

---

## Arhitectura Tehnica

### 1. Character Controller

**Sistem custom de miscare si escalada:**
- Detectie de suprafete urcabile (climbing surfaces)
- Sistem de grip points pentru pozitionarea mainilor/picioarelor
- Fizica bazata pe rigidbody pentru realism
- Interpolation pentru animatii smooth

**Componente principale:**
```
PlayerController.cs
├── MovementController.cs - Miscare la sol
├── ClimbingController.cs - Logica de escalada
├── StaminaManager.cs - Gestionare energie
└── InputHandler.cs - Procesare input
```

### 2. Stamina System

**Mecanism de energie:**
- Stamina se consuma la escalada, alergare, saritura
- Regenerare graduala cand playerul se odihneste
- Visual feedback prin UI bar
- Efecte pe viteza de miscare cand stamina e scazuta

**Factori care afecteaza stamina:**
- Greutatea echipamentului
- Nivelul de damage al playerului
- Conditii de mediu (frig, caldura)
- Consumabile (mancare, bauturi energizante)

### 3. Physics-Based Climbing

**Implementare:**
- Raycast detection pentru identificare grip points
- Inverse Kinematics (IK) pentru pozitionarea mainilor/picioarelor
- Collision detection pentru prevenirea clipului prin pereti
- Force-based movement pentru momentum realistic

**Gameplay mechanics:**
- Click & hold pentru a prinde suprafete
- WASD pentru directie de urcat
- Space pentru jump intre puncte de prindere
- Cresterea dificultatii prin distanta intre grip points

### 4. Level Design

**Structura:**
- 4 biome distincte cu dificultate crescatoare:
  1. **Forest Base** - Tutorial zone, difficulty: Easy
  2. **Rocky Cliffs** - Medium difficulty, obstacole mobile
  3. **Snowy Peaks** - Hard difficulty, stamina drain crescut
  4. **Summit Zone** - Expert, combinare toate mecanicile

**Generare procedurala:**
- Algoritm de generare bazat pe noise (Perlin/Simplex)
- Placement logic pentru grip points
- Spawn points pentru items si obstacole
- Seed-based generation pentru consistency

### 5. Inventory System

**Componente:**
- Grid-based inventory (4x6 slots)
- Item categories:
  - Consumabile (mancare, bauturi)
  - Echipament (carabiniere, franghii)
  - Tools (pickaxe, hammer)
  - Collectibles (pentru achievements)

**Sistem de crafting basic:**
- Combinare items pentru echipament mai bun
- Upgrade la stamina max
- Repair tools pentru echipament uzat

### 6. AI & Obstacles

**Sisteme dinamice:**
- Roci care cad (trigger zones + physics)
- Platforme mobile
- Conditii meteo (vant, zapada)
- Wildlife (pasari, animale mici) - ambient only

**Pathfinding:**
- NavMesh pentru animale
- Waypoint system pentru platforme mobile
- Trigger system pentru eventi scriptati

### 7. Visual Effects

**Shader-uri custom:**
- Wind shader pentru vegetatie
- Snow accumulation shader
- Weathering effects pe stanci
- Fog/distance shader pentru atmosfera

**Particle systems:**
- Praf/pietre cand escaladezi
- Efecte de vant
- Zapada/ploaie
- Breathe effects la altitudine

### 8. Audio System

**Implementare:**
- Audio mixer cu multiple channels
- 3D spatial audio pentru ambient sounds
- Footstep system cu multiple surfaces
- Dynamic music based on tension/altitude

**Audio categories:**
- Muzica (adaptive soundtrack)
- SFX (climbing, footsteps, weather)
- Ambient (vant, pasari, apa)
- UI sounds

### 9. Save System

**Persistent data:**
- Player progress (checkpoints, biome unlocks)
- Inventory state
- Statistics (timp, morti, achievements)
- Settings (audio, graphics, controls)

**Implementare:**
- JSON serialization pentru save data
- Auto-save la checkpoints
- Multiple save slots
- Cloud save optional (PlayerPrefs/local files)

### 10. UI/UX

**Interfata:**
- Minimal HUD (stamina bar, health, quick items)
- Pause menu cu optiuni complete
- Inventory screen (drag & drop)
- Map/progress screen
- Achievement notifications

**Accessibility:**
- Rebindable controls
- Multiple difficulty settings
- Colorblind modes
- Scalable UI

---

## Performance Optimization

### Rendering
- LOD (Level of Detail) pentru modele 3D
- Occlusion culling pentru optimizare
- Texture streaming
- Baked lighting unde e posibil

### Physics
- Collision layers pentru optimizare
- Trigger zones instead of continuous checks
- Object pooling pentru projectile/particles
- Physics update optimization (FixedUpdate usage)

### Memory
- Asset bundling
- Texture compression
- Mesh optimization
- Audio compression (OGG format)

---

## Tools & Technologies

| Category | Technology |
|----------|-----------|
| Engine | Unity 2022.3 LTS |
| Language | C# (.NET Standard 2.1) |
| Version Control | Git + GitHub |
| IDE | Visual Studio / Rider |
| 3D Modeling | Blender (optional) |
| Audio | Audacity / Unity Audio Mixer |
| Animations | Unity Animator + Timeline |

---

## Development Phases

### Phase 1: Core Mechanics (Week 1-2)
- [x] Character controller setup
- [x] Basic climbing mechanics
- [ ] Stamina system
- [ ] Camera controller

### Phase 2: Level Design (Week 3-4)
- [ ] Biome 1 - Forest
- [ ] Biome 2 - Cliffs
- [ ] Obstacle system
- [ ] Checkpoint system

### Phase 3: Systems (Week 5-6)
- [ ] Inventory implementation
- [ ] Save/Load system
- [ ] UI/UX polish
- [ ] Audio integration

### Phase 4: Polish (Week 7-8)
- [ ] Visual effects
- [ ] Performance optimization
- [ ] Bug fixing
- [ ] Playtesting & balancing

---

## Technical Challenges

### Challenge 1: Climbing Physics
**Problem:** Realistic climbing cu physics poate fi instabil
**Solution:** Combinare raycast detection cu force-based movement, limitare physic updates

### Challenge 2: Procedural Generation
**Problem:** Seed-based generation trebuie sa fie consistent
**Solution:** Fixed seed per level, validation pass pentru playability

### Challenge 3: Performance
**Problem:** Multiple systems active simultan
**Solution:** Object pooling, LOD system, optimized collision detection

### Challenge 4: Stamina Balance
**Problem:** Gameplay prea usor sau prea greu
**Solution:** Playtesting iterations, adjustable difficulty settings

---

## Code Structure

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── ClimbingController.cs
│   │   ├── StaminaManager.cs
│   │   └── InputHandler.cs
│   ├── Managers/
│   │   ├── GameManager.cs
│   │   ├── LevelManager.cs
│   │   ├── SaveManager.cs
│   │   └── AudioManager.cs
│   ├── Systems/
│   │   ├── InventorySystem.cs
│   │   ├── AchievementSystem.cs
│   │   └── CheckpointSystem.cs
│   ├── Environment/
│   │   ├── ProceduralGenerator.cs
│   │   ├── ObstacleController.cs
│   │   └── WeatherSystem.cs
│   └── UI/
│       ├── HUDController.cs
│       ├── MenuManager.cs
│       └── InventoryUI.cs
├── Prefabs/
├── Scenes/
├── Materials/
├── Models/
└── Audio/
```

---

## Testing Strategy

### Unit Testing
- Character controller movement
- Stamina calculations
- Inventory operations
- Save/Load functionality

### Integration Testing
- Player-environment interactions
- Physics system stability
- UI-system communication

### Playtesting
- Difficulty balancing
- Bug identification
- UX feedback
- Performance testing

---

## Future Enhancements (Optional)

- Multiple playable characters cu stats diferite
- Weather system dinamic
- Leaderboard pentru speedruns
- Photo mode
- VR support (experimental)

---

*Document creat pentru proiect academic - ASCENT*
*Ultima actualizare: [Data]*
