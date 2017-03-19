# QRK Registrierkasse als Serverdienst für alte Kassenlösungen
Die Software *[QRK Registrierkasse](http://www.ckvsoft.at/)* ist eine einfache Kasse, die nach Angaben der Projekthomepage der österreichischen Registrierkassensicherheitsverordnung entspricht.
Neben der typischen Kassenfunktion nimmt die Kasse im Server Modus Rechnungsdaten per JSON-Datei entgegen. Mit dieser Funktion können Rechnungen aus anderen Programmen heraus erstellt werden.
Hier wird beschrieben, wie Bons aus POS-Software der Codebasis POSper, Chromis POS, uniCenta POS,... mit Hilfe von QRK erstellt und signiert werden können.

Die Beschreibung wurde unter Windows XP und 7 mit der POS-Software POSper getestet. Rückmeldung zu anderen Kassenlösungen arbeite ich gerne ein!

**Haftungsausschluss:** Der Einsatz der genannten Softwareprodukte erfolgt auf eigenes Risiko!

## QRK
Siehe Knowledgebase auf der Projekthomepage. 

Einstellungen:
- Importpfad für den Server-Modus: `C:\CkvSoft\import`
- Server-Modus muss aktiviert sein.
- Test: Kopiere die JSON-Testdatei **TODO** in den Importpfad.

## Installation des MFILEMON-Druckeranschlusses
Die Software *[Multi File Port Monitor](https://sourceforge.net/projects/mfilemon/)* legt einen speziellen Druckeranschluss an und erlaubt die Manipulation von Druckaufträgen. 
- Installiere die Software
- Lege einen neuen Drucker an, wähle *Einen lokalen Drucker hinzufügen* und erstelle einen neuen Anschluss vom Typ *Multi File Port Monitor*.
-- Name des Ports: *kasse*
-- Output Path: `C:\CkvSoft\import`
-- Filename Pattern: `bon-%0000i.txt`
-- Overwrite existing files: aktiviert
- Druckertreiber: HP LaserJet 4100 PCL6
- Name des Druckers: *kasse*

## Anpassung des Bon-Layouts in der POS-Software
Das Format der JSON-Datei wurde im [Forum](http://www.ckvsoft.at/forum/qrk-fragen-und-antworten/anbindung-an-boniersystem/#post-648) veröffentlicht. Die Anpassung der Bons der genannten Kassensysteme erfolgt über die Template-Engine *Apache Velocity*. Die Vorlage [Printer.Ticket](Printer.Ticket) erstellt ein JSON-Objekt für QRK.

## Einstellung des POS-Druckers
Die POS-Software muss nun dazu gebracht werden, den Bon über den eben erstellten Drucker auszugeben. **TODO**

### Gescheiterte Versuche
- Nutzung des file-Druckertreibers der POS-Software: Die Ausgabedatei wird während der Laufzeit der POS-Software offen gehalten und neue Ausgaben werden am Ende angehängt. Ein Abgriff der einzelnen JSON-Daten wäre wieder nur mit Zusatzsoftware möglich.
- Verwendung des Windows-Standarddruckers mit dem Druckertreiber "Generic / Text only" und Umgehung der Steuerzeichen-Bereinigung: Die Ausgabedatei enthält nur ein einzelnes Zeichen. Stellt man den Druckertreiber auf z.B. XPS oder PDF (per PDFCreator) um, wird der JSON-Code kopierbar ausgegeben.
- Die genannten Software-Produkte sind freie Software und können ja frei verändert werden: Dazu fehlt die Zeit zur Einarbeitung in den Code und vermutlich reichen auch meine Programmierkenntnisse nicht aus - eine Einreichung des Features *JSON-Export pro Bon* kann an die Entwickler/Community weitergereicht werden.