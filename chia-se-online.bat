@echo off
chcp 65001 >nul
title PolyLearn AI - Lay Link Online (4G / Khac Mang)
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0chia-se-online.ps1"
pause
