# 2D QuickPlay - Mini-game Collection Framework 🎮

**A scalable 2D game framework designed to manage multiple mini-games within a unified Core Architecture.**

## 📖 About The Project
This project is not just a collection of games; it is a study in **Scalable Game Architecture**. The goal was to build a robust **Core Framework** that handles common systems (Audio, Scene Loading, Data) efficiently, allowing for rapid development of new mini-games without code duplication.

**Current Mini-games:** 2 (and counting).

## 🚀 Key Technical Highlights

### 1. Core Architecture (Singleton Pattern)
- Built a unified **Core Framework** using the Singleton pattern to manage global systems across all scenes.
- **Modules:** Audio Manager, Scene Loader, Data Manager.

### 2. Scalable Logic with MVC
- **Model-View-Controller (MVC):** strictly applied to separate Data (Stats, Score) from View (UI, Sprites).
- **Partial Classes:** Used to break down complex Skill Logic into manageable, readable chunks.
- **Decoupling:** Ensuring gameplay logic can be expanded without breaking existing features.

### 3. Persistent Global UI
- Implemented a **Global UI System (Persistent Canvas)** that survives scene transitions.
- Handles Loading Screens, Toast Notifications, and Popups seamlessly.
- **Mobile-First:** Auto-adapts to **Safe Area** (Notch design) and uses **DoTween** for smooth UI animations.

### 4. Data & Optimization
- **JSON Serialization:** Centralized save system for High Scores and User Settings.
- **Event-Driven:** Replaced traditional `Update()` polling with **C# Events (Unity Actions)** for better performance.
- **Object Pooling:** Heavily used to minimize Garbage Collection (GC) allocation on mobile devices.
- **Input:** Integrated **New Input System** with Haptic Feedback support.

## 🛠 Tech Stack
* **Engine:** Unity 2022.3.62f3
* **Language:** C#
* **Patterns:** Singleton, MVC, Object Pooling, Observer.
* **Tools:** DoTween, New Input System.

## 📸 Screenshots
<table>
  <tr>
    <td width="33%">
      <img width="505" height="912" alt="Screenshot 2026-01-22 135808" src="https://github.com/user-attachments/assets/cf133ded-da3f-4870-9ae2-6c8c41be662f" />
    </td>
    <td width="33%">
      <img width="510" height="910" alt="Screenshot 2026-01-22 135940" src="https://github.com/user-attachments/assets/552e9b30-ece1-44e1-add8-deba2413cf5c" />
    </td>
    <td width="33%">
      <img width="522" height="918" alt="image" src="https://github.com/user-attachments/assets/838a1294-b5ac-4f68-8774-4f5923b5979b" />
    </td>
  </tr>
</table>

## 📦 How to Run
1.  Clone the repo: `git clone https://github.com/Maz2605/2D_QuickPlay.git`
2.  Open in Unity Hub.
3.  Open scene: `Scenes/Loading`.
4.  Press **Play**.

---
*Developed by [Maz](https://github.com/Maz2605)*
