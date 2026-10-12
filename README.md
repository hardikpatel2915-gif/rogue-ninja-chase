# Rogue Ninja Chase

A short stealth game. You sneak into a warehouse, take a dossier out of a chest, and hand it over at the dock. A ninja is guarding the place. If he catches you, the run is over.

Open the project in Unity 6000.3.15f1. The play scene is Assets/Scenes/Game.unity. Press Play. Click the game view so the mouse locks.

## Controls

WASD to move. Hold Shift to sprint, C to crouch, Space to jump. The mouse looks around.

V switches the camera. The close view is the one you aim in. The far view follows behind you.

In the close view, Q, F, or the right mouse button throws a smoke bomb. The bomb makes a noise and can pull the ninja off his route. E opens the chest when you are standing in its trigger, and E is also how you interact in general. Escape frees the cursor. Click locks it again.

## What is in the build

Movement is a Rigidbody, not a CharacterController. The body moves from the camera direction. Crouch beats sprint and blocks the jump.

The two cameras do not move the body the same way. In the close camera the player keeps facing the crosshair. Side input plays strafe clips, left or right, walk or sprint. In the far camera there is no strafe. The body turns to face the stick direction and then walks or runs forward. Idle keeps the facing it already has.

The ninja sees with a cone written by hand. The half-angle cosine is cached once. Each check is a dot product against that cosine, then a distance check. There is no Acos on the hot path. A wall is a separate ray from the eye to the player. The math does not cast that ray. The view does, and the model only hears yes or no. Crouching shortens the range. The colored wedge on the floor is only a picture of the same cone. It is rebuilt from a few spoke rays so it stops at walls. Green is patrol, yellow is investigate, red is chase.

He patrols a route, walks over to investigate a noise or the last place he saw you, and chases while he has you. If he loses you he goes to that last spot and waits. He only catches you during the chase.

Smoke bombs and the smoke puffs are taken from a fixed pool. At startup the game makes four bombs and four puffs and then leaves them alone. A small free list of indexes tracks which ones are spare. Throwing turns one on and sets its position. When it hits, or when its time runs out, it is turned off and the index goes back. Spawn never calls Instantiate, and release never calls Destroy. The raycast buffers used by vision and steering are reused the same way, so the hot path does not allocate.

The mission is three steps. Find the chest and take the dossier. Leave the building and hand it over at the dock. Getting caught fails the run. The screen tells you which step you are on.

## How the code is split

The game is model, view, presenter. Scripts live under Assets/Scripts, and the folder name is the namespace.

A model holds the rules and does not call physics. Vision math, the ninja brain, the steering choice, the motor, and the mission steps are models. A view only reads a sensor or writes a result: the body, the animator, the cone mesh, or the UI. A presenter is the only piece that ticks. It asks the view, calls the model, and writes the view back. The MonoBehaviours on the prefabs just build those pieces so the Edit Mode tests can construct a presenter without a scene. (Tests are not included in the source file)

That split is also how the SOLID rules show up here. Each class does one job. The brain does not know about raycasts. The motor does not know which clip is playing. New behaviour is added by a new model or a new clip, not by growing one big update. Callers depend on a small interface where a concrete type would lock them together: the chest is an interactable, the ninja is a noise listener, and the mission is a mission. Names such as animator states and input actions, are fields on the component so a prefab can change them without a code edit.

## Art

The warehouse set is the free Warehouse Pack from the Unity Asset Store: https://assetstore.unity.com/packages/3d/environments/warehouse-pack-free-low-poly-warehouse-essentials-407032

The ninja model is Bigs2 Ninja, game ready and low poly: https://sketchfab.com/3d-models/bigs2-ninja-game-ready-low-poly-547bdf45338d47b89edac73ad061c6dc

The player model is the textured, rigged human: https://sketchfab.com/3d-models/human-torchtexturedrigged-e6cf0881e62e4cb799c41dfcbff507fe

The chest is Medieval Treasure Chest, animated, by Theo Kain on Sketchfab, under Creative Commons Attribution: https://sketchfab.com/3d-models/medieval-treasure-chest-animated-719d7c1639a64cdebb878fdf1ce26dcf

Ninja and player animation clips are from Mixamo.

## WebGL

Build for Web. Development Build should be off. The player settings already use IL2CPP, a size-focused build, exceptions limited to ones the code throws, and Brotli.

The size that matters is the compressed download: the .data.br, .framework.js.br, .wasm.br, and the loader. Do not add the unpacked .data and .wasm on top of those.

In the browser, click the game once before you play. The mouse will not lock until you do.

Warehouse props are marked Batching Static and do not cast shadows. Only the player and the ninja cast shadows. That is what keeps the draw count down.