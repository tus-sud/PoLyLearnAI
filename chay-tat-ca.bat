@echo off
chcp 65001 >nul
title PolyLearn AI - Khoi dong Web Server + Chia se Online
cls
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0chia-se-online.ps1"
pause
