@echo off
if "%1"=="" (
    echo Commands: env, server, swap-config
) else (
    call tools\%1.cmd %2
)