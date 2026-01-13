# 🏔️ ASCENT

**Un joc de alpinism singleplayer provocator dezvoltat în Unity**

---

## 📋 Prezentare

Acest proiect reprezintă un joc provocator de escaladare dezvoltat cu **Unity 6.2 LTS**.

**ASCENT** este un joc single-player de escaladare unde trebuie să navighez diverse nivele periculoase. Fiecare greșeală te poate trimite la început. Ai determinarea să ajungi la vârful celor mai grele nivele?

---

## ✨ Caracteristici Principale

La această etapă, proiectul conține:

- **⛏️ Escaladare realistă** — Sistem de climbing cu management al staminei
- **⏱️ Provocări contra timp** — Bate-ți propriile recorduri și speedrun nivelurile
- **👤 Personalizare** — Customizează-ți personajul după preferințe
- **🎮 Gameplay 3D Level-Based** — Explorare liberă pentru a găsi drumul tău către victorie
- **🏃 Explorare și Speedrunning** — Găsește calea ta și bate recordurile

### 🎯 Jocuri Similare de Inspirație

- **PEAK** — Mecanici de escaladare
- **Getting Over It** — Platforming de precizie
- **Celeste** — Gameplay provocator de escaladare
- **A Difficult Game About Climbing** — Escaladare bazată pe fizică

---

## 🚀 Cum Se Instalează și Se Rulează

### Setup-ul proiectului

1. **Clonează repo-ul:**
   ```bash
   git clone https://github.com/dobri1408/ProiectUnity2025
   ```

2. **Deschide în Unity Hub** — Utilizează **Unity 6.2** sau mai nou

3. **Încarcă scena principală:**
   - Accesează `Assets/Scenes/Main.unity`
   - Poate dura puțin până se încarcă materialele și texturile

4. **Apasă Play** în Unity Editor pentru a testa jocul

---

## 🎮 Comenzi de Control

| **Mișcare orizontală** | `W` `A` `S` `D` |
| **Apucare pereți** (Climbing) | `Left Click` |

---

## � Sisteme de Joc

Proiectul include următoarele sisteme și mecanici implementate:

### ⛏️ Sistemul de Escaladare
- **Mâna interactivă** - Punct de ancorare pentru escaladare cu mișcare fluidă [`Assets/Scripts/Player/Hand.cs`]
- **Management al staminei** - Regenerare progresivă și consum în timp real [`Assets/Scripts/Player/Player.cs`]
- **Sistem de momentum** - Viteza de alergare se transferă în forța swingului [`Assets/Scripts/Player/Player.cs`]
- **Fizică realista** - Utilizează RigidBody și PhysicMaterial pentru interacțiuni naturale [`Assets/Scripts/Player/Player.physicMaterial`]

### 🎮 Obiecte și Mecanici de Nivel
- **Platforme Rotative** - Se rotesc constant, complicând traversarea [`Assets/Scripts/Objects/Spinner.cs`]
- **Platforme Mobile** - Se deplasează pe o cale predefinită cu pauze la fiecare punct [`Assets/Scripts/Objects/MultiPointPlatform.cs`]
- **Teleporturi** - Transportă jucătorul la puncte specifice, resetând viteza [`Assets/Scripts/Objects/Teleport.cs`]
- **Flag de Victorie** - Marchează finalizarea nivelului cu sistem de stele [`Assets/Scripts/Objects/WinFlag.cs`]

### 🔊 Sistem de Audio
- **Sunet de vânt dinamic** - Se adapteaza la viteza de mișcare a jucătorului [`Assets/Scripts/Player/Player.cs`]
- **Sunet de pași** - Se redă când jucătorul este pe teren [`Assets/Scripts/Player/Player.cs`]

### 💾 Sistem de Salvare și Progresie
- **Unlock de nivele** - Progresie liniară prin nivele [`Assets/Scripts/GameSaveManager.cs`]
- **Personal Best Tracking** - Registrează cel mai bun timp pe fiecare nivel [`Assets/Scripts/GameSaveManager.cs`]
- **Sistem de Stele** - 0-3 stele bazate pe timp de completare [`Assets/Scripts/GameSaveManager.cs`]
- **Salvare Setări** - Volum master și sensibilitate mouse persistent [`Assets/Scripts/GameSaveManager.cs`]

### 🖼️ UI și Meniuri
- **Main Menu** - Intrare în joc cu navigație fluidă [`Assets/Scripts/UI/MainMenu.cs`]
- **Level Select** - Selectare și încărcarea nivelurilor [`Assets/Scripts/UI/LevelSelectMenu.cs`]
- **Loading Screen** - Animații în timp ce se încarcă nivelul [`Assets/Scripts/UI/LoadingScreen.cs`]
- **Timer UI** - Afișare timp real în joc și pentru calcul stele [`Assets/Scripts/UI/TimerUI.cs`]
- **Stamina Bar** - Afișare vizuală a nivelului de oboseală [`Assets/Scripts/UI/StaminaUI.cs`]
- **Win Menu** - Rezultate și opțiuni după finalizare nivel [`Assets/Scripts/UI/WinMenu.cs`]

---

## �📞 Feedback și Review

Pentru orice sugestii, rapoarte de bug-uri sau feedback:

- **Email:** serbanandrei1338@gmail.com
- **Microsoft Teams:** andrei.serban7@s.unibuc.ro

---

