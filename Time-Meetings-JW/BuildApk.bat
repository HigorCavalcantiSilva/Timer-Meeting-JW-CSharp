@echo off
echo ========================================
echo Publicando APK para android...
echo ========================================

dotnet publish -f:net9.0-android -c:Release -p:AndroidPackageFormat=apk

echo ----------------------------------------
echo Publicação finalizada!
echo APK gerado em:
echo bin\Release\net9.0-android\publish\
echo ----------------------------------------
pause