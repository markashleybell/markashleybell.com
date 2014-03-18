@ECHO OFF
IF "%1"=="local" (
    IF EXIST "config.cfg.local" (
        ren config.cfg config.tmp
        ren config.cfg.local config.cfg
        ren config.tmp config.cfg.live
        ECHO Switched to local config
    ) ELSE (
        ECHO Already using local config
    )
) ELSE IF "%1"=="live" (
    IF EXIST "config.cfg.live" (
        ren config.cfg config.tmp
        ren config.cfg.live config.cfg
        ren config.tmp config.cfg.local
        ECHO Switched to live config
    ) ELSE (
        ECHO Already using live config
    )
) ELSE (
    ECHO Specify 'live' or 'local'
)