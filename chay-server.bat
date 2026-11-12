@echo off
title PolyLearn AI - Server Local (Port 5008)
cls
echo ================================================================
echo           POLYLEARN AI - WEB SERVER (PORT 5008)
echo ================================================================
echo.
echo * May tinh nay vao web tai:       http://localhost:5008
echo * Dien thoai / May khac cung WiFi: http://192.168.0.195:5008
echo.
echo Dang khoi dong may chu... Vui long giu cua so nay de hoc!
echo ================================================================
echo.
dotnet run --launch-profile http
pause
