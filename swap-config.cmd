@ECHO OFF
IF "%1"=="local" (
    IF EXIST "config.cfg.local" (
        del /q config.cfg
        copy config.cfg.local config.cfg
        ECHO Switched to local config
    ) ELSE (
        ECHO Local config file missing
    )
) ELSE IF "%1"=="live" (
    IF EXIST "config.cfg.live" (
        del /q config.cfg
        copy config.cfg.live config.cfg
        ECHO Switched to live config
    ) ELSE (
        ECHO Live config file missing
    )
) ELSE (
    ECHO Specify 'live' or 'local'
)