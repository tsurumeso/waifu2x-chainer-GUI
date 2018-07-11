@echo off
set UA="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36"

cd tools
nvcc -V >nul 2>&1 || goto Check_if_python_is_installed
nvcc -V|find "Cuda compilation tools, release 8.0" >nul&&set cuda_ver=80
nvcc -V|find "Cuda compilation tools, release 9.0" >nul&&set cuda_ver=90
nvcc -V|find "Cuda compilation tools, release 9.1" >nul&&set cuda_ver=91
nvcc -V|find "Cuda compilation tools, release 9.2" >nul&&set cuda_ver=92

:Check_if_python_is_installed
python -h >nul 2>&1||goto install_python

:not_install_python
python -m pip install -U pip
pip install chainer
if defined cuda_ver pip install cupy-cuda%cuda_ver%
pip install wand
pip install pillow
goto install_waifu2x-chainer

:install_python
if "%PROCESSOR_ARCHITECTURE%" EQU "x86" (
   curl -H %UA% -s "https://www.anaconda.com/download/#windows" -o "%TEMP%\anaconda_download.txt" >nul 2>&1
   mfind /W /M "/.*\x22(https:\/\/repo\.anaconda\.com\/archive\/Anaconda3[^\/]*?Windows-x86.exe)\x22.*/$1/" "%TEMP%\anaconda_download.txt" >nul 2>&1 ||goto error_end
   set /p conda_URL=<"%TEMP%\anaconda_download.txt"
)
if "%PROCESSOR_ARCHITECTURE%" EQU "AMD64" (
   curl -H %UA% -s "https://www.anaconda.com/download/#windows" -o "%TEMP%\anaconda_download.txt" >nul 2>&1
   mfind /W /M "/.*\x22(https:\/\/repo\.anaconda\.com\/archive\/Anaconda3[^\/]*?Windows-x86_64.exe)\x22.*/$1/" "%TEMP%\anaconda_download.txt" >nul 2>&1 ||goto error_end
   set /p conda_URL=<"%TEMP%\anaconda_download.txt"
)
del "%TEMP%\anaconda_download.txt"
echo "%conda_URL%"|findstr /X ".https://repo\.anaconda\.com/archive/Anaconda3[^/]*Windows-[^/]*.exe." >nul ||goto error_end
echo Download Anaconda

echo.
curl -H %UA% --retry 5 -o "%TEMP%\Anaconda_Windows-setup.exe" "%conda_URL%"
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
"%UserProfile%\Anaconda3\Scripts\pip.exe" install chainer
if defined cuda_ver "%UserProfile%\Anaconda3\Scripts\pip.exe" install cupy-cuda%cuda_ver%
"%UserProfile%\Anaconda3\Scripts\pip.exe" install wand
"%UserProfile%\Anaconda3\Scripts\pip.exe" install pillow
goto install_waifu2x-chainer

:install_waifu2x-chainer
echo.
echo Download waifu2x-chainer
echo.
curl -H %UA% --retry 5 -o "%TEMP%\waifu2x-chainer.zip" -L "https://github.com/tsurumeso/waifu2x-chainer/archive/master.zip"
7za.exe x -y -o"%TEMP%\" "%TEMP%\waifu2x-chainer.zip"
del "%TEMP%\waifu2x-chainer.zip"
xcopy /E /I /H /y "%TEMP%\waifu2x-chainer-master" "C:\waifu2x-chainer"
rd "%TEMP%\waifu2x-chainer-master"
:end
cls
echo successful Installation.
pause

:error_end
echo URL acquisition failed.
pause
exit
