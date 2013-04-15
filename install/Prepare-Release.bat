@echo off
setlocal enableextensions

mkdir FixVBKeywords.7.1 2> NUL
copy /y ..\src\resharper-fix-vb-keyword-completion\bin\Release\*.* FixVBKeywords.7.1\
