# NOS TRAICIONARON Y ENCERRARON EN LA CAFETERÍA DE INGENIERÍA POR 100 AÑOS!!

Videojuego para el curso Introducción al Desarrollo de Videojuegos de la Universidad de los Andes.

Grupo 3:
- Gianfranco Bobadilla
- Borja Manterola
- Matias Pedemonte
- Vicente Rey
- Lucas Reyes

# Índice
- [NOS TRAICIONARON Y ENCERRARON EN LA CAFETERÍA DE INGENIERÍA POR 100 AÑOS!!](#nos-traicionaron-y-encerraron-en-la-cafetería-de-ingeniería-por-100-años)
- [Índice](#índice)
  - [1. Idea general del proyecto](#1-idea-general-del-proyecto)
    - [1.1 Mecánicas principales](#11-mecánicas-principales)
    - [1.2 Temática](#12-temática)
    - [1.3 Flujo del juego en detalle](#13-flujo-del-juego-en-detalle)
    - [1.4 Estructura del proyecto](#14-estructura-del-proyecto)
    - [1.5 Instalación y ejecución del proyecto](#15-instalación-y-ejecución-del-proyecto)
  - [2. Entrega 2](#2-entrega-2)
    - [2.1 Avance parcial 1](#21-avance-parcial-1)
      - [Gianfranco Bobadilla](#gianfranco-bobadilla)
      - [Borja Manterola](#borja-manterola)
      - [Matias Pedemonte](#matias-pedemonte)
      - [Vicente Rey y Lucas Reyes](#vicente-rey-y-lucas-reyes)
    - [2.2 Flujo esperado](#22-flujo-esperado)

## 1. Idea general del proyecto

El videojuego será 2D. Este se centrará en combates por turnos para su avance, hasta derrotar al jefe final. En el recorrido, el jugador deberá tomar distintas decisiones que afectarán sus estadísticas y habilidades. Dichos cambios se verán reflejados en los combates, haciéndolos más fáciles o difíciles. Está inspirado en Slay the Spire.

El objetivo será lograr derrotar al jefe final con las habilidades y estadísticas que el jugador elija durante el juego. El juego terminaría si se logra derrotar al jefe final (victoria) o si el jugador muere o se rinde (derrota).

**Genero:** *JRPG* y *Roguelike*.

### 1.1 Mecánicas principales

1. Combate por turnos entre personaje y enemigos
2. La vida de los personajes y enemigos es modificada por ataques
3. Cada ataque hace un daño específico
4. El jugador puede mejorar sus estadísticas y elegir habilidades al final de un combate
5. Enemigos y jefes dependen del mapa
6. Cada personaje tiene habilidades y estadísticas distintas
7. Elección de distintos caminos
8. Distintos mapas

### 1.2 Temática

La temática es un juego ubicado en la Universidad de los Andes con la premisa de alumnos pensando “¿por qué me metí a esta carrera?”. Los alumnos protagonistas son los personajes controlables por el jugador, es un un juego con toque humorístico pero jugabilidad sólida y desafiante.

**Narrativa:**
Los alumnos protagonistas fueron traicionados y encerrados en la cafetería del edificio de ingeniería de la universidad por 100 años dado que debían realizar un videojuego para el curso de Introducción al Desarrollo de Videojuegos. Para escapar antes de ese tiempo, deben enfrentar a mechones, alumnos de electivos, pregrado, postgrado y profesores para llegar a tiempo a la clase del curso y enfrentarse al responsable.

**personajes:**
Los desarrolladores serían los personajes controlables por el jugador, con una estética a lo mortal kombat. En cambio, los enemigos serían mechones o alumnos de la carrera de ingeniería y los jefes, profesores de la facultad. 


**Setting:**
El escenario es el edificio de ingeniería. 
- El primer piso es la cafetería con múltiples mechones pidiendo ayuda con Cálculo I y pasillos donde no faltaría el grupo haciendo trabajos a última hora. Estos alumnos se ponen en el camino de los protagonistas para intentar llegar a la sala del curso de Introducción al Desarrollo de Videojuegos.
- A medida que se avanza, se llega al primer piso de la facultad, con los laboratorios de las distintas especialidades (electrónica, obras civiles, computación) y salas con trampas ingeniosas. Por ej., en la sala de electrónica, los alumnos enemigos son de tipo eléctrico o metal y tienen armas como cables y protoboards; en el laboratorio de ingenieros ambientales, los alumnos son de tipo planta; etc.
- Finalmente, la sala final objetivo es la sala del curso de videojuegos, ubicada en el segundo piso. Esta se encuentra custodiada por el profesor, quien sería el jefe final.

### 1.3 Flujo del juego en detalle

La vista es lateral, el personaje aparece en una sala y debe elegir con el mouse hacia donde ir.

A cada sitio al que va, encuentra enemigos que aparecen, cada sala tiene 1 set de enemigos. Dependiendo la sala, puede ser un jefe o un enemigo normal.

El objetivo es avanzar en el camino que toma el jugador hasta llegar a la sala final donde está el jefe.

El combate funcionaría así:
- A la izquierda el jugador con su personaje, arriba una barra de vida y otra de "intelecto" (IQ, magia).
- A la derecha el enemigo con una barra de vida y su nombre Tanto el personaje del jugador como el enemigo tienen un icono de ojo que permite ver sus estadísticas como ataque, habilidad base (cada personaje tiene un elemento), defensa, vida.
- Por encima de ese fondo, hay un cuadrado que posee 4 opciones. Atacar (ataque físico), Habilidad, Objetos, Conversar.
  - Si se acciona atacar (con enter o espacio, o clickeando), se realiza un ataque sobre el enemigo, luego el enemigo ataca al jugador ya sea con una habilidad o un ataque físico.
  - Si se acciona habilidad, aparece un pequeño pop-up con las habilidades que el jugador tienen, cada habilidad consume Intelecto y tiene un elemento. El jugador siempre parte con 1 habilidad base, y a medida que avanza la partida puede ir obteniendo más.
  - Si se acciona Objeto, tambien aparece un pequeño pop-up pero con objetos que el jugador ha ido adqueriendo, estos pueden mejorar una estadística para la batalla actual, recuperar vida, recuperar intelecto o disuadir al enemigo.
  - Finalmente, conversar es una opción donde el enemigo te plantea un problema de intelecto, tu tienes un set de alternativas, si respondes correctamente la batalla termina, y si respondes incorrecto, el enemigo obtiene una mejora en sus estadísticas.

Al derrotar al enemigo, aleatoriamente salen 3 recompensas del conjunto que el jugador puede elegir: Experiencia (subir de nivel sube todas tus estadísticas un poco); Habilidad (nueva habilidad); Objeto (nuevo objeto); Mejora de alguna estadística (solo mejora 1 estadistica).

Tras elegir la recompensa que el jugador desee, esta se aplica y luego se le vuelve a permitir al jugador elegir a donde ir a continuación. Y así hasta el final.

En el caso de los jefes, la mecanica de conversar no termina la batalla, solo debilita al jefe, y si está incorrecta la respuesta, lo potencia.
No hay un mapa para ver durante la partida, uno va a ciegas explorando. Pero todas las partidas el mapa y orden de las salas es el mismo, lo que cambia es el camino y mejoras que el jugador elije.

### 1.4 Estructura del proyecto

```
cafeteria_de_ingenieria/Assets/
  ├─ Scripts/
  │    ├─ Core/                // Controladores y sistemas generales
  │    │    ├─ GameManager.cs
  │    │    ├─ UIManager.cs
  │    │    ├─ SceneLoader.cs
  │    │    └─ AudioManager.cs
  │    ├─ Battle/
  │    │    ├─ BattleManager.cs
  │    │    ├─ PlayerController.cs
  │    │    ├─ EnemyController.cs
  │    │    ├─ SkillSystem.cs
  │    │    └─ DialogueSystem.cs
  │    ├─ Rooms/
  │    │    ├─ RoomManager.cs
  │    │    ├─ RoomData.cs
  │    │    └─ PathManager.cs
  │    └─ DataModels/          // Clases para enemigos, items, etc.
  │         ├─ EnemyData.cs
  │         ├─ PlayerData.cs
  │         ├─ SkillData.cs
  │         ├─ ItemData.cs
  │         └─ RoomLayoutData.cs
  ├─ Prefabs/
  │    ├─ UI/
  │    │    ├─ BattleHUD.prefab
  │    │    ├─ SkillPopup.prefab
  │    │    └─ ItemPopup.prefab
  │    ├─ Enemies/
  │    │    ├─ Slime.prefab
  │    │    ├─ Goblin.prefab
  │    │    └─ BossDragon.prefab
  │    ├─ Rooms/
  │    │    ├─ BasicRoom.prefab
  │    │    ├─ BossRoom.prefab
  │    │    └─ TreasureRoom.prefab
  │    └─ Player.prefab
  ├─ Sprites/
  │    ├─ Player/
  │    ├─ Enemies/
  │    ├─ UI/
  │    └─ Backgrounds/
  ├─ Audio/
  │    ├─ Music/
  │    └─ SFX/
  └─ ScriptableObjects/
       ├─ Enemies/
       ├─ Skills/
       ├─ Items/
       └─ Rooms/
```

### 1.5 Instalación y ejecución del proyecto

1. Clonar el repositorio **en la rama actual**: `git clone --branch entregaX --single-branch https://github.com/UandesGIAN/videojuegos_cafeteria_de_ingenieria`
2. Abrir Unity Hub y seleccionar **Add project**, apuntando a la carpeta raíz.
3. Abrir la escena principal (por defecto): `Assets/Scenes/SampleScene.unity`.
4. Verificar que los paquetes necesarios estén instalados:
   - TextMeshPro
   - Input System
5. Presionar Play en Unity para probar el juego.
6. Para compilar, ir a **File → Build Settings**, seleccionar la plataforma deseada y luego **Build**.

## 2. Entrega 2

Aquí se describe y deja constancia de los avances realizados para la entrega 2 del proyecto.

### 2.1 Avance parcial 1

#### Gianfranco Bobadilla

Encargado del HUD básico y estructura del proyecto inicial.

Labores:
- Mostrar el `PlayerPanel` y `EnemyPanel` con sprites, nombres, HP e IQ.
- Mostrar el `ActionMenu` y que se resalté la opción elegida.
- Activar/desactivar los popups (`SkillPopup`, `ItemPopup`).
- Dejar disponible el `MessageLog` con acciones de batalla.

Scripts involucrados:
- `BattleManager.cs` (control central de UI y flujo de combate)

#### Borja Manterola

Encargado del Menú de Habilidades y Objetos (Inventario)

Labores:
- `SkillSystem.cs` → Mostrar habilidades del jugador, activar los botones del popup, queda pendiente el consumo de IQ cuando el jugador usa alguna.
- `ItemSystem.cs` → Mostrar items del jugador, dejar las funciones para aplicar los efectos al jugador (curar, recuperar IQ, mejorar stats).

Scripts involucrados:
- `SkillSystem.cs`
- `ItemSystem.cs`
- `BattleManager.cs` (para activar popups y pasar datos al sistema correspondiente).

#### Matias Pedemonte

Encargado de las stats de los enemigos y el jugador.

Labores:
- Definir los atributos base del jugador y enemigos: Vida (HP), Intelecto/Magia (IQ), Ataque, Defensa, Habilidad elemental, Experiencia, Nivel, etc.
- Proponer las clases básicas de las entidades del juego.

Scripts involucrados:
- `PlayerController.cs` (Manejo del jugador)
- `EnemyController.cs` (Manejo de los enemigos)
- `PlayerData.cs` (Datos del jugador)
- `EnemyData.cs` (Datos de los enemigos)

#### Vicente Rey y Lucas Reyes

Encargados del sistema de combate y turnos.

Labores:
- Cuando el jugador ataca:
  - Calcular daño usando stats de jugador y enemigo.
  - Actualizar `EnemyData.HP` y el HUD.
- Cuando el enemigo ataca:
  - Calcular daño usando stats de enemigo y jugador.
  - Actualizar `PlayerData.HP` y el HUD.
  - habilidades especiales propuesto.
- Gestionar el turno: jugador → enemigo → jugador.

Scripts involucrados:
- `BattleManager.cs` (para coordinar turnos y llamar a los métodos de atacar).
- `PlayerController.cs` (acciones del jugador).
- `EnemyController.cs` (lógica de ataques enemigos).


### 2.2 Flujo esperado

Player elige opción → BattleManager → (Attack / Skill / Item / Talk)

 ↓

PlayerController o EnemyController ejecuta acción
 ↓

BattleManager actualiza HUD (HP, IQ, log)

 ↓
Turno enemigo → EnemyController ataca → HUD actualizado

 ↓

Repitir hasta que jugador o enemigo mueran