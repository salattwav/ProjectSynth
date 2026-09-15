@echo on

set "TargetPath=%~1"
set "ConfigFile=deploy_path.txt"
if not exist "%ConfigFile%" (
    echo ERROR: %ConfigFile% not found.
    exit /b 1
)

for /f "usebackq delims=" %%A in ("%ConfigFile%") do set "DeployPath=%%A"

copy "%TargetPath%" "..\Build\plugins"
if %ERRORLEVEL% EQU 0 (echo COPY DLL: successful) else (echo COPY DLL: failed)
echo:

if exist "..\ProjectSynth_Unity\AssetBundles\projectsynth_bundle" (
    copy "..\ProjectSynth_Unity\AssetBundles\projectsynth_bundle" "..\Build\plugins\AssetBundles\"
    if %ERRORLEVEL% EQU 0 (echo ASSETBUNDLE COPY: successful) else (echo ASSETBUNDLE COPY: failed)
echo:
)

if exist "..\ProjectSynth_WWISE\GeneratedSoundBanks\Windows\Init.bnk" (
    copy "..\ProjectSynth_WWISE\GeneratedSoundBanks\Windows\Init.bnk" "..\Build\plugins\SoundBanks\CustomMusic\SynthDJ_Init.bnk"
    if %ERRORLEVEL% EQU 0 (echo INIT_BANK COPY: successful) else (echo INIT_BANK COPY: failed)
echo:
)

if exist "..\ProjectSynth_WWISE\GeneratedSoundBanks\Windows\SynthDJ.bnk" (
    copy "..\ProjectSynth_WWISE\GeneratedSoundBanks\Windows\SynthDJ.bnk" "..\Build\plugins\SoundBanks\CustomMusic\SynthDJ.bnk"
    if %ERRORLEVEL% EQU 0 (echo SYNTHDJ COPY: successful) else (echo SYNTHDJ COPY: failed)
echo:
)

if exist "..\ProjectSynth_WWISE\GeneratedSoundBanks\Windows\SynthSounds.bnk" (
    copy "..\ProjectSynth_WWISE\GeneratedSoundBanks\Windows\SynthSounds.bnk" "..\Build\plugins\SoundBanks\SynthSounds.sound"
    if %ERRORLEVEL% EQU 0 (echo SYNTHSOUNDS COPY: successful) else (echo SYNTHSOUNDS COPY: failed)
echo:
)

if exist "..\ProjectSynth_WWISE\GeneratedSoundBanks\Windows\Media\" (
    Xcopy /E /I /Y "..\ProjectSynth_WWISE\GeneratedSoundBanks\Windows\Media" "..\Build\plugins\SoundBanks\CustomMusic\Media"
    if %ERRORLEVEL% EQU 0 (echo MEDIA XCOPY: successful) else (echo MEDIA XCOPY: failed)
echo:
)

Xcopy /E /I /Y "..\Build\plugins" "%DeployPath%\TeamSynth-ProjectSynth\"
if %ERRORLEVEL% EQU 0 (echo FINAL XCOPY: successful) else (echo FINAL XCOPY: failed)
echo:

exit /b %ERRORLEVEL%
