Building
========

> This build guide assumes that you already have **Risk of Rain 2** with **BepInEx 5.4.21+** set up and working.
> Unless it's true, good place to start is [**RoR2 Modding Wiki Getting Started**](https://risk-of-thunder.github.io/R2Wiki/Playing/Getting-Started/) guide.

### Prerequisites

- [Git](https://git-scm.com/downloads) (obviously)
- [.NET 8 SDK](https://dotnet.microsoft.com/download) or newer.
- [Visual Studio 2022 17.8+](https://visualstudio.microsoft.com/downloads/) with the _**".NET desktop development"**_ workload
(or any other IDE of your choice for that matter)

### Setup

1. Clone the repo
	
	```sh
	git clone https://github.com/salattwav/ProjectSynth.git
	```

2. Navigate to **ProjectSynth_VS** folder, and run next command to create a `deploy_path.txt` file.
Replace `[PATH_TO_YOUR_MODS_FOLDER]` with an actual path to your mods folder.

	Your path should look something like this:

	`D:\[YOUR_R2MODMAN_FOLDER]\r2profiles\RiskOfRain2\profiles\[YOUR_PROFILE_NAME]\BepInEx\plugins`
	
	```sh
	echo "[PATH_TO_YOUR_MODS_FOLDER]" > .\deploy_path.txt
	```

	Contents of `deploy_path.txt` will be fetched by `PostBuild.bat`, which will collect
	all the necessary files into one folder, thus creating a mod folder,
	and put this folder where your mods are located.

3. Open **ProjectSynth.sln** file.

### Build

- **Ctrl + Shift + B** or go to navigation bar Build > Build Solution

or if you are a CLI person:

```sh
dotnet build -c Debug -tl:off
```

###### _*you are not necessarily required to use `-tl:off`, I just like it._


License
=======

This repository is licensed under MIT - see [LICENSE](LICENSE.md).

<!--Original assets (sprites, audio, models, etc.) are NOT covered by the code license - see [ASSETS_NOTICE](ASSETS_NOTICE.md).-->

_License terms may be revisited as the project matures - current version reflects our intent as of September 13th, 2026._

Contributing
============

See [CONTRIBUTING](CONTRIBUTING.md) for details on how to get involved.
