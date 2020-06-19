# SmartObservations - London Studios
**SmartObservations** is a **FiveM** resource coded in **C#** which enhances the experience of taking and working with **medical observations** (eg, Heart Rate, Blood Pressure). Not only that, but also ranking observations based on risk, and playing the sound accordingly to the paramedic/medical team on scene.

This resource provides the ability to easily set your observations, ensuring they are in the allowed values and notifying you if they are **high** or **medium** risk - the plugin will also know if you are in a cardiac arrest.

Medical teams on scene will receive updates if your observations change, allowing for realistic roleplay!

Join our **Discord** server [here](https://discord.gg/WqJdMR).

![SmartObservations](https://i.imgur.com/n3NPoSX.png)

If you enjoy my resources, you can buy me a coffee [here](https://www.buymeacoffee.com/londonstudios).

## Usage
**/setobs** - This opens up the menu, allowing you to set your observations.
**/obs** - This takes the observations of the nearest player (they must have set their obs for it to work), this will then notify you if their observations change, unless you disable notifications.
**/obsupdates** - This will enable or disable observation updates from all players to yourself. If you want to disable just one player, open up their observations menu and press "Disable Notifications". This will stop tracking.

## Installation

 1.  Create a new **resource folder** on your server.
 2.  Add the contents of **"resource"** inside it. This includes:
"Client.net.dll", "Server.net.dll", "NativeUI.dll", "fxmanifest.lua", "html", "stream".
3. In **server.cfg**, "ensure" SmartObservations, to make it load with your server startup.

## Source Code
Please find the source code in the **"src"** folder. Please ensure you follow the licence in **"LICENCE.md"**.

## Feedback
We appreciate feedback, bugs and suggestions related to SmartObservations and future plugins. We hope you enjoy using the resource and look forward to hearing from people!