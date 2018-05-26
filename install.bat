@echo off
cd tools
nvcc -V >nul 2>&1 || goto check_install_python
nvcc -V|find "Cuda compilation tools, release 8.0" >nul&&set cuda_ver=80
nvcc -V|find "Cuda compilation tools, release 9.0" >nul&&set cuda_ver=90
nvcc -V|find "Cuda compilation tools, release 9.1" >nul&&set cuda_ver=91

:check_install_python
python -h >nul 2>&1||goto install_python

:not_install_python
python -m pip install -U pip
if defined cuda_ver pip install cupy-cuda%cuda_ver%
pip install chainer
pip install wand
pip install pillow
goto install_waifu2x-chainer

:install_python
if "%PROCESSOR_ARCHITECTURE%" EQU "x86" FOR /f delims^=^"^ tokens^=4 %%A IN ('curl -s "https://www.anaconda.com/download/#windows" ^| findstr "https://repo\.anaconda\.com/archive/Anaconda3[^/]*Windows-x86.exe"') DO SET "conda_URL=%%A"
if "%PROCESSOR_ARCHITECTURE%" EQU "AMD64" FOR /f delims^=^"^ tokens^=4 %%A IN ('curl -s "https://www.anaconda.com/download/#windows" ^| findstr "https://repo\.anaconda\.com/archive/Anaconda3[^/]*Windows-x86_64.exe"') DO SET "conda_URL=%%A"
echo "%conda_URL%"|findstr /X ".https://repo\.anaconda\.com/archive/Anaconda[^/]*Windows-[^/]*.exe." >nul ||echo URL acquisition failed.&&pause&&exit
echo Download Anaconda
echo.
curl --retry 5 -o "%TEMP%\Anaconda_Windows-setup.exe" "%conda_URL%"
echo.
echo Install Anaconda
echo.

echo start /wait "" "%TEMP%\Anaconda_Windows-setup.exe" /InstallationType=JustMe /RegisterPython=1 /AddToPath=1 /S "/D=%UserProfile%\Anaconda3">"%TEMP%\Anaconda_Windows-setup.bat"
echo exit /b>>"%TEMP%\Anaconda_Windows-setup.bat"
powershell Start-Process "%TEMP%\Anaconda_Windows-setup.bat" -Wait -Verb runas
del "%TEMP%\Anaconda_Windows-setup.bat"
del "%TEMP%\Anaconda_Windows-setup.exe"

"%UserProfile%\Anaconda3\Scripts\conda.exe" update conda -y
"%UserProfile%\Anaconda3\Scripts\conda.exe" update --all -y
if defined cuda_ver "%UserProfile%\Anaconda3\Scripts\pip.exe" install cupy-cuda%cuda_ver%
"%UserProfile%\Anaconda3\Scripts\pip.exe" install chainer
"%UserProfile%\Anaconda3\Scripts\pip.exe" install wand
"%UserProfile%\Anaconda3\Scripts\pip.exe" install pillow
goto install_waifu2x-chainer

:install_waifu2x-chainer
echo.
echo Download waifu2x-chainer
echo.
curl --retry 5 -o "%TEMP%\waifu2x-chainer.zip" -L "https://github.com/tsurumeso/waifu2x-chainer/archive/master.zip"
7za.exe x -y -o"C:\" "%TEMP%\waifu2x-chainer.zip"
del "%TEMP%\waifu2x-chainer.zip"
move /y "C:\waifu2x-chainer-master" "C:\waifu2x-chainer"
:end
cls
echo successful Installation.
pause
