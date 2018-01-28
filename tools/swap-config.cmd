@echo off
if "%1"=="local" (
    if exist "config.cfg.local" (
        del /q config.cfg
        copy config.cfg.local config.cfg
        echo Switched to local config
    ) else (
        echo Local config file missing
    )
) else if "%1"=="live" (
    if exist "config.cfg.live" (
        del /q config.cfg
        copy config.cfg.live config.cfg
        echo Switched to live config
    ) else (
        echo Live config file missing
    )
) else (
    echo Specify 'live' or 'local'
)