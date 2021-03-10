# CTM's AA Speedrun Tool
###### "The Swiss Army Knife of Minecraft Speedrun Tools"

![](preview_overlay.gif)
![](preview_main.gif)

## What's New?
This is a complete rewrite of the [Advancements Tracker I created back in November of 2020](https://github.com/DarwinBaker/AdvancementsTracker). I ended up being extremely limited by the performance of the framework that the original program was built on, and couldn't add many of the great features people have been requesting. This new version has no such limitations, so the sky is the limit! These massive performance improvements are thanks to the new version being built on DirectX, meaning it's rendered exclusively on the GPU. The old tracker was rendered on the CPU using GDI+, which while simpler to code, is *much* slower. Since the GPU is barely utilized at all in vanilla Minecraft anyway, having the tracker open should translate to zero impact on the game's performance. This could potentially lead to chunks generating perceptibly faster for people on very slow machines when switching from the old tracker.

In addition to being lightning fast (even on very old PCs) this new version also includes the totally customizable stream overlay that you have likely seen if you've watched [Elysaku's Stream](https://www.twitch.tv/elysaku). (Big thanks to him for beta testing!) [For a guide on how to set this up yourself, click here!](info/obs.md)

This tool supports all game versions 1.12+, and will eventually support multiple languages and have a built-in automatic update system.

## How It Works

Due to its unintrusive nature, the tracker only refreshes whenever Minecraft saves. This can be when it autosaves every 5 minutes, but **a save can also be manually triggered by simply pausing and unpausing the game**. (Think of *Escape* like a split key for the tracker). When Minecraft saves it writes all of your completed advancements and partial completion criteria to a JSON file. This program reads that file at configurable intervals. The only privilege given to the tracker is `FileAccess.Read`, so there should be absolutely no possibility of file corruption. Also, `FileShare.ReadWrite` is used while parsing the file so as not to inturrupt Minecraft if it happens to save during a refresh.

## Speedrun Legality

This tool was designed from the beginning to strictly adhere to the category rules and requirements. As stated on the tracker itself, this program is in accordance with the Speedrun.com Minecraft: Java Edition ruleset as of its public release on 1/25/2021. The guidelines state that you are allowed to read from the advancements and statistics files during a run. All this program does is read from the advancements and statistics files of the most recently accessed save. No data is written anywhere aside from the program's own settings files, and absolutely nothing is read from the game's memory. That being said, if the mods have any questions I am more than happy to answer!

## Installation

If you're thinking about taking on the All Advancements category and would like to take advantage of this tool, simply download and extract the zip file from the [Releases](https://github.com/DarwinBaker/AATool/releases) page. The tracker is currently only tailored for 1.16, but it should still work as far back as 1.12 (although this is untested) 
It's a standalone executable, so just run the exe inside and you're all set, no installation required!  

By default the program will look in your `AppData\Roaming\.minecraft\saves` folder, but if you're using a custom launcher or have moved your save directory, custom save paths are supported. This software is provided "as is", without warranty of any kind. 

You will likely run into a Windows SmartScreen pop up stating "Windows protected your PC" when you first run the program. You can safely ignore this and click "Run Anyway" and it shouldn't ask again. DO NOT download/run any version of this software not downloaded directly from this repository to avoid malware. I am not responsible if you run something that looks like this that didn't come from here.

#### Find Me On These Platforms:
- [Patreon](https://www.patreon.com/_ctm)
- [Twitch](https://www.twitch.tv/ctm_256)
- [YouTube](https://www.youtube.com/channel/UCdJ1FnTvTpna4VGkEyJ9_NA)
- [Reddit](https://www.reddit.com/user/_CTM_)
- [Speedrun.com](https://www.speedrun.com/user/CTM)

###### Creating this tool took many hours and many energy drinks. If you find it helpful or enjoy watching someone else who does and you want to show your support, consider [donating](https://www.paypal.com/donate?hosted_button_id=EN29468P8CY24) or supporting me on [Patreon](https://www.patreon.com/_ctm)! This program is and always will be completely free for everyone's use. :)
