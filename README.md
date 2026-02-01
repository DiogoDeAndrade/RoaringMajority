# THE ROARING MAJORITY

![Title](Screenshots/title.png)

Developed for [2026 Global Game Jam](https://globalgamejam.org/2026), with theme Mask, this project is a small experimental management/strategy game about crowds, protests, and identity. The core idea is to explore masks as both cosmetic and gameplay elements: anonymity, symbolism, protection, provocation.

The project is intentionally scoped to be solo‑dev friendly and fun to build within a tight jam timeframe, while still allowing expressive systemic play.

## Game

The player manages protests inside a city. Each person can wear a different mask (which symbolizes their role), affecting how they behave, how others react to them, and how situations escalate.

Gameplay alternates between:

- A macro view: deploying people to different locations in the city
- Close-up view: observing and influencing what happens on the ground (tension, clashes, negotiation, dispersal)
- HQ: control macro concepts like purchasing ads, or buying signage!

The challenge is to balance numbers, mask types, and intent, while reacting to police presence, counter-groups, and emergent chaos.

The current prototype only has one location/cause working (barely, needed a bunch of stuff still).

## Current

* Add journalist
  * Call press (+visibility, +tension)

## Todo

* Transfer local variables to global
* Map view
* HQ
  * Buy signs
  * Buy ads
  * Buy bots (internet campaign)
  * TV system
* Opposing crowd system
* Stance system
* Main Menu screen
  * Start
  * Credits
  * Select Cause
  * Exit
* Polish
  * Better tooltip font (spacing is crap)
  * Combat text on change variables
  * Spread protesters a bit
  * Show cooldown on recruit button
  * Show effects of actions on tooltip
  * Customize cursor/icons
* Design
  * Improve cooldown on recruit

## Art

Image process notes:
* Generate the image or get some stock art
* Reduce resolution (Background: 640x360 / Characters: avg. 160 height)
* Select all (Shift-Click on layer)
* Quantize color (archimedes 64 palette)
* Back to RGB
* (Characters) Extract background (contract selection 1 pixel)
* (Background) Desaturate -100, Lightness -25

- [Fist icons](https://www.flaticon.com/free-icons/fist) created by Freepik - [Flaticon], free to use with attribution.
- [Heart icons](https://www.flaticon.com/free-icons/heart) created by Chanut - [Flaticon], free to use with attribution.
- [Visibility icons](https://www.flaticon.com/free-icons/visibility) created by Andrean Prabowo - [Flaticon], free to use with attribution.
- [Dollar icons](https://www.flaticon.com/free-icons/dollar) created by Gregor Cresnar - [Flaticon], free to use with attribution.
- [Chanting icons](https://www.flaticon.com/free-icons/chanting) created by Freepik - [Flaticon], free to use with attribution.
- [Announcement icons](https://www.flaticon.com/free-icons/chanting) created by Dewi Sari - [Flaticon], free to use with attribution.
- [Brochure icons](https://www.flaticon.com/free-icons/brochure) created by Freepik - [Flaticon], free to use with attribution.
- [Song icons](https://www.flaticon.com/free-icons/song) created by Freepik - [Flaticon], free to use with attribution.
- [Unfair icons](https://www.flaticon.com/free-icons/unfair) created by Fusion5085 - [Flaticon], free to use with attribution.
- [Check out icons](https://www.flaticon.com/free-icons/check-out) created by Icongeek26 - [Flaticon], free to use with attribution.
- [Charity icons](https://www.flaticon.com/free-icons/charity) created by Freepik - [Flaticon], free to use with attribution.
- [Apple icons](https://www.flaticon.com/free-icons/apple) created by Freepik - [Flaticon], free to use with attribution.
- [Clap icons](https://www.flaticon.com/free-icons/clap) created by Freepik - [Flaticon], free to use with attribution.
- [Water icons](https://www.flaticon.com/free-icons/water) created by Good Ware - [Flaticon], free to use with attribution.
- Font [Boomer Tantrum](https://chequered.ink/product/boomer-tantrum/) by [Checkered Ink](https://chequered.ink/), purchased and under the [Checkered Ink License](https://chequered.ink/wp-content/uploads/2025/01/License-Agreement-All-Fonts-Pack.pdf)
- Font [Fake News](https://chequered.ink/product/fake-news/) by [Checkered Ink](https://chequered.ink/), purchased and under the [Checkered Ink License](https://chequered.ink/wp-content/uploads/2025/01/License-Agreement-All-Fonts-Pack.pdf)
- Font [Front Page News](https://chequered.ink/product/front-page-news/) by [Checkered Ink](https://chequered.ink/), purchased and under the [Checkered Ink License](https://chequered.ink/wp-content/uploads/2025/01/License-Agreement-All-Fonts-Pack.pdf)
- [Nanner 32 Palette](https://lospec.com/palette-list/nanner-32) by [Afterimage](https://lospec.com/afterimage)
- [archimedes 64 Palette](https://lospec.com/palette-list/archimedes-64) by [Pythagoras_314](https://lospec.com/pythagoras314)
- Most background and character art generated with Dall-E and processed by [Diogo de Andrade]
- Everything else done by [Diogo de Andrade], [CC0] license.

## Sound

- Everything else done by [Diogo de Andrade], licensed through the [CC0] license.

## Code

- Uses [Unity Common], [MIT] license.
- [NaughtyAttributes](https://github.com/dbrizov/NaughtyAttributes.git#upm) by Denis Rizov available through the [MIT] license.
- All remaining game source code by Diogo de Andrade is licensed under the [MIT] license.

## Screenshots

![Art Test](Screenshots/dev_art_test.png)
![Gameplay](Screenshots/gameplay01.png)

## Metadata

- Autor: [Diogo de Andrade]

[Diogo de Andrade]:https://github.com/DiogoDeAndrade
[CC0]:https://creativecommons.org/publicdomain/zero/1.0/
[CC-BY 3.0]:https://creativecommons.org/licenses/by/3.0/
[CC-BY-NC 3.0]:https://creativecommons.org/licenses/by-nc/3.0/
[CC-BY-SA 4.0]:http://creativecommons.org/licenses/by-sa/4.0/
[CC-BY 4.0]:https://creativecommons.org/licenses/by/4.0/
[CC-BY-NC 4.0]:https://creativecommons.org/licenses/by-nc/4.0/
[Unity Common]:https://github.com/DiogoDeAndrade/UnityCommon
[Flaticon]:https://www.flaticon.com
[MIT]:LICENSE
