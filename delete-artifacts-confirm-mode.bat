@echo Deleting all artifacts ...
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S "%%G"
del /s /p /f *.user
@echo.
@echo Artifacts successfully deleted. Press any key to continue.