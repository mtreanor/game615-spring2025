# Playing with Simulated People

## NPC Dialogue Prototype

<img src="./images/terranigma.png" width="500" align="right">

Create small narrative experience that centers around speaking with non-player characters with dialogue created using [ADialogueSystem and a spreadsheet](https://github.com/mtreanor/ADialogueSystem). 

The prototype requirements are:

- There should be at least 4 NPCs with unique dialogue
- The player should have a goal that when met progresses the `quest state` (a "lock/key puzzle")
- The two `quest states` should have different dialogue for each character
- At least 2 of the characters should use the conditions to have dialogue that is dependent on the blackboard (see below)
- **BONUS:** Have your characters be 3D models with animations!

### A Dialogue System

"A Dialogue System" is a character dialogue toolset for Unity that allows authors to create conditional character dialogue using google sheets. The system allows authors to dynamically create state data, and dynamically select dialogue based on that state. The state can also be accessed and modified from outside the spreadsheet as well. Go [here to download A Dialogue System](https://github.com/mtreanor/ADialogueSystem/blob/main/ADialogueSystem.unitypackage) and see its [GitHub page for setup instructions](https://github.com/mtreanor/ADialogueSystem) for further instructions.

In this system, dialogue is structured around a "quest state" that defines the broad chapter or level in which it applies. Each dialogue entry is associated with a character and consists of two conditions and an effect. Authors can read from and write to a shared "blackboard," a state storage system that tracks labeled data. Conditions act as queries to the blackboard, determining whether a dialogue line is triggered, while effects allow authors to update the blackboard, modifying the game's state dynamically.

At the approprite time for your game, you can modify the `DialogueManager.scc.questState` variable to move on to the next set of character dialogue options.

### Visual Asset Resources

- [Mixamo](https://www.mixamo.com/)
- [City People FREE Samples](https://assetstore.unity.com/packages/3d/characters/city-people-free-samples-260446)
- [Kenney Animated Characters](https://kenney.nl/assets/animated-characters-1)
