@echo Deleting all artifacts ...
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"
del /s /q /f *.user
@echo.
@echo Artifacts successfully deleted. Press any key to continue.