# Welcome to TMT!
TMT stands for **T**emperature **M**easurment **T**ool. 
The idea behind it is to create a tool for doctor offices to save temperature values in an Excel File. 
This [pdf](https://praxissiegel.de/cms/upload/PDFs/Leitfaden_Zertifizierung_HA_vs10.pdf) provided by praxissiegel.de contains the requirements for licensed individual or group practices in germany: 
 **Zertifizierungskriterium C5 says:** 
> "Der Kühlschrank der Praxis, in dem Arzneimittel aufbewahrt werden, ist mit einem funktionstüchtigen MinimumMaximum-Thermometer ausgestattet. Die Temperatur im Kühlschrank liegt im Normbereich zwischen 2 und 8 Grad Celsius und wird werktäglich dokumentiert. Die Zuständigkeiten für die Dokumentation sind geregelt."
>  
## Features
 - Supports XLSX with the Library **[EPPlus](https://github.com/JanKallman/EPPlus)** 
 - It also notes observes local holidays with the Library **[Nager.Date](https://github.com/tinohager/Nager.Date)**
 - Insert vacation days 
 - Supports personal settings
	 - Managing assigend employees
	 - Determine temperature limits
 - Converts Excel Worksheets into PDF's  by using **[Microsofts Interop](https://docs.microsoft.com/de-de/dotnet/csharp/programming-guide/interop/how-to-access-office-onterop-objects)** Library
 - Start print process by using **[Microsofts Interop](https://docs.microsoft.com/de-de/dotnet/csharp/programming-guide/interop/how-to-access-office-onterop-objects)** Library
 - Mockup-Function to create Excel File with mockup (*"fake"*) data 
 - ...

## List of useful changes to make
 - For making that programm international useable...
	 - ...the country code needs to be in the settings (*It hardcoded Germany*) 
	 - ...find out Solution for the NumberDecimalSeparator  (*its hardcoded the german format*)
 - ...
## List of features
 - [ ] **Missing Entries Dialog** 
When the programm starts to look if any entries from the last couple of days are missing. By clicking on the button a dialog opens for quickly revise all missing entries
 
